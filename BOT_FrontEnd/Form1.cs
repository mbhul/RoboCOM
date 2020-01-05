using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using X11;

#if _WINDOWS
using SlimDX.DirectInput;
#endif

namespace BOT_FrontEnd
{
    public partial class Form1 : Form
    {
        
        //Maximum number of command lines to maintain in the rich text box for history. 
        const int MAX_CMD_LINES = 15;
        private Controller controller;

#if _WINDOWS
        private DirectInput DI;
        private List<Guid> connected_controllers;
#else
        private List<string> connected_controllers;
#endif

        private bool sent_stop;
        private bool PauseTransfer;
        private bool cancelBackgroundWorker;
        private double z_value;
        private string command_prev;

        //private Tuple<Keys, bool>[6];
        private Dictionary<Keys, bool> InputKeys;
        private Config ctlConfig;
        private bool isDocked = false;

        ProcessStartInfo vidLinkPy = new ProcessStartInfo();
        Process vidLinkProc;

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        public Form1(ref SplashScreen sc)
        {
            try
            {
                //Initialize and set defaults
                InitializeComponent();
                bold_font = new Font(InComTxt.Font, FontStyle.Bold);
                BaudSelect.SelectedIndex = 0;
                sent_stop = true;
                PauseTransfer = false;
                cancelBackgroundWorker = false;
                populatePortDropDown();
            
                ttTimer = new ToolTip();
                ttTimer.SetToolTip(TimerLabel, "The interval (in milliseconds) between successive lines being written to the COM port.");
                TimerIntSelect.Value = SendTimer.Interval;

                //Make sure COM port is closed
                MyVCOM.Close();
                ConnStatusLbl.Text = "Not Connected";
                ConnStatusLbl.ForeColor = Color.Red;

                InComTxt_default_position = InComTxt.Top;
                InComLbl_default_position = InComLbl.Top;

                ctlConfig = new Config("Config.xml");
                command_prev = "";

                //Define the valid Keyboard input keys for 'Controller' input
                InputKeys = new Dictionary<Keys, bool>();
                InputKeys.Add(Keys.Up, false);
                InputKeys.Add(Keys.Down, false);
                InputKeys.Add(Keys.Left, false);
                InputKeys.Add(Keys.Right, false);
                InputKeys.Add(Keys.W, false);
                InputKeys.Add(Keys.S, false);

#if _WINDOWS
                DI = new DirectInput();
#endif

                //Add the PreviewKeyDown event handler to every control on the form
                Control[] ctls = GetAll(this).ToArray<Control>();
                foreach (Control ctl in ctls)
                {
                    ctl.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form_PreviewKeyDown);
                }
            }
            catch (FileLoadException e)
            {
                //Console.Write(e.StackTrace);
                using (StreamWriter sw = File.CreateText("err.txt"))
                {
                    sw.WriteLine(e.Message);
                    sw.WriteLine(e.StackTrace);
                }
            }

            System.Threading.Thread.Sleep(2000);
            sc.Close();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            
        }


        //Method to get all controls on the form, including nested ones
        public IEnumerable<Control> GetAll(Control control)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl))
                                      .Concat(controls);
        }

        /********************************************************************************
         * FUNCTION:    populatePortDropDown
         * Description: Get list of COM ports and populate the port selection drop-down
         * Parameters:  None
         ********************************************************************************/
        private void populatePortDropDown()
        {
            string[] port_names = System.IO.Ports.SerialPort.GetPortNames();
            this.PortNumber.Items.Clear();
            
            foreach (string port in port_names)
            {
                PortNumber.Items.Add(port.Replace("COM", ""));
            }

            if (PortNumber.Items.Count > 0)
            {
                PortNumber.SelectedIndex = 0;
            }
        }

        /********************************************************************************
         * FUNCTION:    ProcessCmdKey
         * Description: Empty override for the arrow keys to prevent them from switching control focus
         * Parameters: 
         ********************************************************************************/
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (radioPad.Checked &&
                ((keyData == Keys.Right) || (keyData == Keys.Left) ||
                (keyData == Keys.Up) || (keyData == Keys.Down)))
            {
                //DO NOTHING
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        /********************************************************************************
         * FUNCTION:    getConnectedControllers
         * Description: Find all connected DirectX compatible input devices and populate the dropdown list
         * Parameters:  N/A
         ********************************************************************************/
#if _WINDOWS
        private void getConnectedControllers()
        {
            ControllerSelect.Items.Clear(); 
            controller = new Controller(this.Handle);
            connected_controllers = new List<Guid>();

            //Create the worker thread that will poll the selected controller for its current state
            ControllerPoller.WorkerSupportsCancellation = true;
            ControllerPoller.DoWork += new DoWorkEventHandler(ControllerPoller_DoWork);
            ControllerPoller.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ControllerPoller_RunWorkerCompleted);

            //Get the list of connected controllers (we want to use the gamepad so specify GameControl device type)
            IList<DeviceInstance> ControllerList = DI.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly);

            foreach (DeviceInstance dev in DI.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AttachedOnly))
            {
                ControllerList.Add(dev);
            }

            if (ControllerList.Count > 0)
            {
                foreach (DeviceInstance deviceInstance in ControllerList)
                {
                    ControllerSelect.Items.Add(deviceInstance.InstanceName);
                    connected_controllers.Add(deviceInstance.InstanceGuid);
                }
            }

            //add the keyboard at the end
            ControllerSelect.Items.Add("Keyboard");
            connected_controllers.Add(new Guid());
        }
#else
        private void getConnectedControllers()
        {
            ControllerSelect.Items.Clear();
            controller = new Controller(this.Handle);
            connected_controllers = new List<string>();

            //Create the worker thread that will poll the selected controller for its current state
            ControllerPoller.WorkerSupportsCancellation = true;
            ControllerPoller.DoWork += new DoWorkEventHandler(ControllerPoller_DoWork);
            ControllerPoller.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ControllerPoller_RunWorkerCompleted);

            //Get the list of connected controllers
            try
            {
                string[] devFiles = Directory.GetFiles("/dev/input/").Select(Path.GetFileName).ToArray();

                if (devFiles.Count() > 0)
                {
                    foreach (string fname in devFiles)
                    {
                        if (Regex.Match(fname, "js[0-9]*").Success)
                        {
                            ControllerSelect.Items.Add(fname);
                            connected_controllers.Add("/dev/input/" + fname);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            //add the keyboard at the end
            ControllerSelect.Items.Add("Keyboard");
            connected_controllers.Add("Keyboard");
        }
#endif

        /********************************************************************************
         * EVENT HANDLER:   OpenFileBtn_Click
         * Description:     Open File Dialog
         ********************************************************************************/
        private void OpenFileBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDlg.ShowDialog();
            }
            catch (Exception ex)
            {
                //Do Nothing
            }
        }

        /********************************************************************************
         * EVENT HANDLER:   OpenFileDlg_FileOk
         * Description:     Executed when a file is opened using the OpenFileDlg dialog
         ********************************************************************************/
        private void OpenFileDlg_FileOk(object sender, CancelEventArgs e)
        {
            String filetext = File.ReadAllText(OpenFileDlg.FileName);
            InComTxt.Clear();

            //if the file is a vector image, process it accordingly
            if (OpenFileDlg.FileName.Contains(".svg"))
            {
                VectorImage vector_IM = new VectorImage(OpenFileDlg.FileName);
                vector_IM.setAxisOffset(new double[] { 0, 0 });
                filetext = vector_IM.getCode(new double[] { 4, 4 });
            }

            InComTxt.Text = filetext;
        }

        /********************************************************************************
         * EVENT HANDLER:   SaveFileBtn_Click
         * Description:     Save File button click handler
         ********************************************************************************/
        private void SaveFileBtn_Click(object sender, EventArgs e)
        {
            SaveFileDlg.FileName = OpenFileDlg.FileName;
            SaveFileDlg.ShowDialog();
        }

        /********************************************************************************
         * EVENT HANDLER:   SaveFileDlg_FileOk
         * Description:     Writes the contents of the COM Input text box to the selected file
         ********************************************************************************/
        private void SaveFileDlg_FileOk(object sender, CancelEventArgs e)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            File.WriteAllText(SaveFileDlg.FileName, InComTxt.Text.Replace("\n", "\r\n"), enc);
        }

        /********************************************************************************
         * EVENT HANDLER:   SendBtn_Click
         * Description:     Send button click handler.
         *                  Begins the timer event that will send the contents of the COM Input 
         *                  text box line-by-line
         ********************************************************************************/
        private void SendBtn_Click(object sender, EventArgs e)
        {
            instructions = InComTxt.Lines;
            instruction_count = 0;
            select_index = 0;

            //make sure the COM port is open before starting SendTimer
            if (MyVCOM.IsOpen)  
            {
                SendTimer.Interval = (int)TimerIntSelect.Value;
                SendTimer.Enabled = true;
            }
        }

        /********************************************************************************
         * EVENT HANDLER:   ConnReset_Click
         * Description:     Establish a new COM port connection, or close the existing connection
         *                  Open/Close Port button click handler
         ********************************************************************************/
        private void ConnReset_Click(object sender, EventArgs e)
        {
            //if the COM port is currently open, close it
            if (MyVCOM.IsOpen)
            {
                if(!radioPad.Checked)
                {
                    SendTimer.Enabled = false;
                }

                ReadTimer.Enabled = false;
                MyVCOM.Close();
                SendBtn.Enabled = false;
                ConnReset.Text = "Open Connection";
                ConnStatusLbl.Text = "Not Connected";
                ConnStatusLbl.ForeColor = Color.Red;
            }
            else //else open the COM port
            {
                if (IsLinux)
                {
                    MyVCOM.PortName = PortNumber.SelectedItem.ToString();
                }
                else
                {
                    MyVCOM.PortName = "COM" + PortNumber.SelectedItem.ToString();
                }
                
                MyVCOM.BaudRate = Convert.ToInt32(BaudSelect.Text);
                MyVCOM.DtrEnable = true;

                try
                {
                    MyVCOM.Open();
                    SendBtn.Enabled = true;
                    ConnReset.Text = "Close Connection";
                    ConnStatusLbl.Text = "Connected";
                    ConnStatusLbl.ForeColor = Color.Green;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }

                if (MyVCOM.IsOpen)
                {
                    ReadTimer.Enabled = true;
                }
            }
        }

        /********************************************************************************
         * EVENT HANDLER:   SendTimer_Tick
         * Description:     SendTimer tick handler.
         *                  Each time this event is fired, we will write one command line to the COM port
         ********************************************************************************/
        private void SendTimer_Tick(object sender, EventArgs e)
        {
            string formatstring = "";
            double x = 0;
            double y = 0;
            double z = 0;
            double a = 0;
            double b = 0;
            double x_fs, y_fs, z_fs, a_fs, b_fs;
            double x_def, y_def, z_def, a_def, b_def;
            double z_gain;
            bool z_centered = true;

            //If controller selection drop-down is active, then temporarily disable the function until
            // the selected controller is confirmed
            if (ControllerSelect.DroppedDown)
            {
                return;
            }

            //Store configuration values for later use
            z_fs = (double)ctlConfig.channel_config[(int)ChannelNumber.CH1].MAX -
                    (double)ctlConfig.channel_config[(int)ChannelNumber.CH1].MIN;

            x_fs = (double)ctlConfig.channel_config[(int)ChannelNumber.CH2].MAX -
                    (double)ctlConfig.channel_config[(int)ChannelNumber.CH2].MIN;

            y_fs = (double)ctlConfig.channel_config[(int)ChannelNumber.CH3].MAX -
                    (double)ctlConfig.channel_config[(int)ChannelNumber.CH3].MIN;

            a_fs = (double)ctlConfig.channel_config[(int)ChannelNumber.CH4].MAX -
                    (double)ctlConfig.channel_config[(int)ChannelNumber.CH4].MIN;

            b_fs = (double)ctlConfig.channel_config[(int)ChannelNumber.CH5].MAX -
                    (double)ctlConfig.channel_config[(int)ChannelNumber.CH5].MIN;

            z_def = (double)ctlConfig.channel_config[(int)ChannelNumber.CH1].CENTER;
            x_def = (double)ctlConfig.channel_config[(int)ChannelNumber.CH2].CENTER;
            y_def = (double)ctlConfig.channel_config[(int)ChannelNumber.CH3].CENTER;
            a_def = (double)ctlConfig.channel_config[(int)ChannelNumber.CH4].CENTER;
            b_def = (double)ctlConfig.channel_config[(int)ChannelNumber.CH5].CENTER;

            z_gain = ctlConfig.channelGain[(int)ChannelNumber.CH1];

            //If the Z-input is self-centering then the default value will be somewhere in the middle of the Z full-scale
            // Otherwise, the Z-input will be near the Z-min. This condition checks if the Z-default is less than 1% of Z full-scale
            z_centered = !((z_def - ctlConfig.channel_config[(int)ChannelNumber.CH1].MIN) < (z_fs * 0.01));

            //If the 'Text' send option is checked and there are commands(lines) left to write:
            if (radioText.Checked && instruction_count < instructions.Length)
            {
                InComTxt.Select(select_index, instructions[instruction_count].Length);
                InComTxt.SelectionFont = bold_font;
                if (PauseTransfer == false)
                {
                    select_index += instructions[instruction_count].Length + 1;
                    if(!instructions[instruction_count].Contains("("))
                    {
                        MyVCOM.Write(instructions[instruction_count]);
                    }
                    instruction_count++;
                }    
            }

            //If the controller(gamepad/keyboard) send option is checked:
             
            if (radioPad.Checked)
            {
                string cmd = "";
                
                if(!this.Focused && !IsLinux && vidLinkProc == null)
                {
                    SetForegroundWindow(this.Handle);
                }

#region Keyboard Input Selected
                //if 'Keyboard'(arrow keys) is selected as the current method of input
                if ((String)ControllerSelect.SelectedItem == "Keyboard")
                {
                    double key_increment = this.controller.getFS() / 2;

                    //**** Get key states *****//
                    x += this.InputKeys[Keys.Left] ? -0.5 : 0; //KeyboardInfo.GetKeyState(Keys.Left).IsPressed ? -0.5 : 0;
                    x += this.InputKeys[Keys.Right] ? 0.5 : 0; //KeyboardInfo.GetKeyState(Keys.Right).IsPressed ? 0.5 : 0;
                    y += this.InputKeys[Keys.Up] ? -0.5 : 0; //KeyboardInfo.GetKeyState(Keys.Up).IsPressed ? -0.5 : 0;
                    y += this.InputKeys[Keys.Down] ? 0.5 : 0; //KeyboardInfo.GetKeyState(Keys.Down).IsPressed ? 0.5 : 0;
                    z += this.InputKeys[Keys.W] ? 0.05 : 0; //KeyboardInfo.GetKeyState(Keys.W).IsPressed ? 0.05 : 0;
                    z += this.InputKeys[Keys.S] ? -0.05 : 0; //KeyboardInfo.GetKeyState(Keys.S).IsPressed ? -0.05 : 0;

                    //**** Scale key values for drawing *****//
                    x = (x + 1) * key_increment;
                    y = (y + 1) * key_increment;
                    z *= key_increment;

                    if (ctlConfig.z_persist)
                    {
                        z += z_value;
                        z_value = z;
                    }

                    DrawXY((int)x, (int)y);
                    DrawZ_Persist((int)z);

                    //**** Convert the values to the configured command scale (in Config.xml) *****//
                    x = (((x / key_increment) - 1) * 0.5) * x_fs;
                    x += x_def;

                    y = -1 * (((y / key_increment) - 1) * 0.5) * y_fs;
                    y += y_def;

                    z = ((z / key_increment) * 0.5) * z_fs;
                    z += z_def;

                    //**** Build the output command format string *****//
                    formatstring = "\r\n" + ctlConfig.cmdPrefixRelative;
                    formatstring += ctlConfig.channelPrefix[(int)ChannelNumber.CH1] + "{0:";
                    formatstring += ctlConfig.precisionFormat[(int)ChannelNumber.CH1] + "}";
                    formatstring += ctlConfig.chanSeparator;

                    formatstring += ctlConfig.channelPrefix[(int)ChannelNumber.CH2] + "{1:";
                    formatstring += ctlConfig.precisionFormat[(int)ChannelNumber.CH2] + "}";
                    formatstring += ctlConfig.chanSeparator;

                    formatstring += ctlConfig.channelPrefix[(int)ChannelNumber.CH3] + "{2:";
                    formatstring += ctlConfig.precisionFormat[(int)ChannelNumber.CH3] + "}";
                    formatstring += ";";

                    cmd = String.Format(formatstring, z, x, y);
                    if (Math.Abs(x - x_def) > (0.05 * x_fs) ||
                        Math.Abs(y - y_def) > (0.05 * y_fs) ||
                        (Math.Abs(z) > (z_def + (0.05 * z_fs)) && (z_value != z)))
                    {
                        if (!(chkRepeat.Checked && cmd == command_prev))
                        {
                            InComTxt.AppendText(cmd);
                            if (MyVCOM.IsOpen) { MyVCOM.Write(cmd); }
                            sent_stop = false;
                        }
                    }
                    else if (sent_stop == false)
                    {
                        try
                        {
                            formatstring = "\r\n" + ctlConfig.stopCommand;
                            InComTxt.AppendText(formatstring);
                            if (MyVCOM.IsOpen) { MyVCOM.Write(formatstring); }
                        }
                        catch(Exception exc)
                        {
                            //Do nothing
                        }
                        sent_stop = true;
                    }
                }
#endregion
#region DirectX Controller Selected
                else
                {
                    JoystickState state = controller.getState();
#if _WINDOWS
					bool[] buttons = state.GetButtons();
					int[] pov = state.GetPointOfViewControllers();
					double angle_A_B = (pov[0] == -1) ? -1 : (pov[0] / 100) * (Math.PI / 180);
#else
                    bool[] buttons = state.GetButtons().ToArray();
					double angle_A_B = 0;
#endif

                    float fs = (float)controller.getFS();
                    bool z_condition = false;

                    //**** Scale controller values for drawing *****//
                    x = limitControllerScaleValue(controller.CH2, 0.0, fs);
                    y = limitControllerScaleValue(controller.CH3, 0.0, fs);

                    DrawXY((int)x, (int)y);

                    z = controller.CH1;
                    if (ctlConfig.z_persist)
                    {
                        if (ctlConfig.z_accumulating)
                        {
                            z = -1 * (controller.CH1 - (fs / 2)) * z_gain;
                            z += z_value;
                            z = limitControllerScaleValue(z, 0.0, fs);
                            z_value = z;
                        }
                        
                        DrawZ_Persist((int)z);
                    }
                    else
                    {
                        DrawZ((int)z);
                    }

                    //**** Convert the values to the configured command scale (in Config.xml) *****//
                    x = ((float)(x - fs / 2) / fs) * x_fs;
                    x += x_def;

                    y = -1 * ((float)(y - fs / 2) / fs) * y_fs;
                    y += y_def;

                    //Compute Z-value offset based on whether the input is set as self-centering
                    if (!z_centered)
                    {
                        z = ((float)z / fs) * z_fs;
                    }
                    else
                    {
                        z = ((float)(z - fs / 2) / fs) * z_fs;
                    }
                    z += z_def;

                    a = (angle_A_B == -1) ? 0 : -0.1 * Math.Cos(angle_A_B);
                    a += a_def;

                    b = (angle_A_B == -1) ? 0 : 0.1 * Math.Sin(angle_A_B);
                    b += b_def;

                    cmd = "\r\n" + ctlConfig.cmdPrefixRelative;

                    //**** Convert the values to the configured command scale (in Config.xml) *****//
                    if (!z_centered)
                    {
                        z_condition = (Math.Abs(z) > (z_def + (0.05 * z_fs)) && (z_value != z));
                    }
                    else
                    {
                        z_condition = (Math.Abs(z - z_def) > (0.05 * z_fs) && (z_value != z));
                    }

                    formatstring = ctlConfig.channelPrefix[(int)ChannelNumber.CH1] + "{0:";
                    formatstring += ctlConfig.precisionFormat[(int)ChannelNumber.CH1] + "}";
                    formatstring += ctlConfig.chanSeparator;
                    cmd = String.Format(cmd + formatstring, z);

                    formatstring = ctlConfig.channelPrefix[(int)ChannelNumber.CH2] + "{0:";
                    formatstring += ctlConfig.precisionFormat[(int)ChannelNumber.CH2] + "}";
                    formatstring += ctlConfig.chanSeparator;
                    cmd = String.Format(cmd + formatstring, x);

                    formatstring = ctlConfig.channelPrefix[(int)ChannelNumber.CH3] + "{0:";
                    formatstring += ctlConfig.precisionFormat[(int)ChannelNumber.CH3] + "}";
                    formatstring += ctlConfig.chanSeparator;
                    cmd = String.Format(cmd + formatstring, y);

                    formatstring = ctlConfig.channelPrefix[(int)ChannelNumber.CH4] + "{0:";
                    formatstring += ctlConfig.precisionFormat[(int)ChannelNumber.CH4] + "}";
                    formatstring += ctlConfig.chanSeparator;
                    cmd = String.Format(cmd + formatstring, a);

                    formatstring = ctlConfig.channelPrefix[(int)ChannelNumber.CH5] + "{0:";
                    formatstring += ctlConfig.precisionFormat[(int)ChannelNumber.CH5] + "}";
                    cmd = String.Format(cmd + formatstring, b); 

                    cmd += ";";

                    if (Math.Abs(x - x_def) > (0.05 * x_fs) ||
                        Math.Abs(y - y_def) > (0.05 * y_fs) ||
                        z_condition || 
                        Math.Abs(a) > 0.05 || Math.Abs(b) > 0.05)
                    {
                        if (!(chkRepeat.Checked && cmd == command_prev))
                        {
                            InComTxt.AppendText(cmd);
                            if (MyVCOM.IsOpen) { MyVCOM.Write(cmd); }
                            sent_stop = false;
                        }
                    }
                    else if (sent_stop == false)
                    {
                        try
                        {
                            formatstring = "\r\n" + ctlConfig.stopCommand;
                            InComTxt.AppendText(formatstring);
                            if (MyVCOM.IsOpen) { MyVCOM.Write(formatstring); }
                        }
                        catch (Exception exc)
                        {
                            //Do nothing
                        }
                        sent_stop = true;
                    }
                }
#endregion
                if(!IsLinux)
                {
                    ScrollToEnd(InComTxt);
                }

                command_prev = cmd;
            }
        }

        private double limitControllerScaleValue(double pInput, double pMin, double pMax)
        {
            double ret_val = pInput;

            if(pInput < pMin)
            {
                ret_val = pMin;
            }
            else if (pInput > pMax)
            {
                ret_val = pMax;
            }
            else { }

            return ret_val;
        }

        /********************************************************************************
         * EVENT HANDLER:   ReadTimer_Tick
         * Description:     Read Timer tick handler.
         *                  Polls the serial port for received data
         ********************************************************************************/
        private void ReadTimer_Tick(object sender, EventArgs e)
        {
            if (MyVCOM.BytesToRead > 0)
            {
                try
                {
                    string temp = MyVCOM.ReadLine();
                    temp.Replace(";", ";\r\n");

                    if (temp.Contains("G00"))
                        PauseTransfer = true;
                    else if (temp.Contains("G01"))
                        PauseTransfer = false;

                    System.Action a = new System.Action(() =>
                    {
                        OutComTxt.AppendText(temp);

                        if (!IsLinux)
                        {
                            ScrollToEnd(OutComTxt);
                        }

                    });
                    this.BeginInvoke(a);
                }
                catch (TimeoutException) { }
            }
        }

        /********************************************************************************
         * EVENT HANDLER:   ClearOutBtn_Click
         * Description:     Clear button click handler.
         *                  Clears the Send/Receive text boxes
         ********************************************************************************/
        private void ClearOutBtn_Click(object sender, EventArgs e)
        {
            OutComTxt.Clear();
            InComTxt.Clear();
        }

        /********************************************************************************
         * EVENT HANDLER:   radioPad_CheckedChanged
         * Description:     Check Changed handler for the 'Controller' radio button.
         *                  Used to toggle the UI settings for gamepad input
         ********************************************************************************/
        private void radioPad_CheckedChanged(object sender, EventArgs e)
        {
            if (radioPad.Checked == true)
            {
                radioText.Checked = false;
                PadInPanel.BringToFront();
                getConnectedControllers();

                ControllerSelect.SelectedIndex = 0;
                PadXY_View.Visible = true;
                PadZ_View.Visible = true;
                btnCtlSettings.Enabled = true;

                InComTxt.Height = InComTxt.Bottom - OutComTxt.Top;
                InComTxt.Top = OutComTxt.Top;
                InComLbl.Top = OutComLbl.Top;

                SendBtn.Text = "Begin Transfer";

                if (connected_controllers.Count > 0 && controller != null)
                {
                    StartControllerInput(); 
                }
                else
                {
                    //MessageBox.Show("No Controller Found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //radioText.PerformClick();
                }
            }
            else
            {
                PadInPanel.SendToBack();
                PadXY_View.Visible = false;
                PadZ_View.Visible = false;
                btnCtlSettings.Enabled = false;

                InComTxt.Height = InComTxt.Bottom - InComTxt_default_position;
                InComTxt.Top = InComTxt_default_position;
                InComLbl.Top = InComLbl_default_position;

                SendBtn.Text = "Send Input";
                StopControllerInput();
            }
        }

        /********************************************************************************
         * EVENT HANDLER:   ControllerSelect_SelectedIndexChanged
         * Description:     
         ********************************************************************************/
        private void ControllerSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(SendTimer.Enabled == false)
            {
                getControllerConfiguration();
            }
        }

        private void ControllerSelect_SelectionCommited(object sender, EventArgs e)
        {
            getControllerConfiguration();
        }

        private void getControllerConfiguration()
        {
            if ((String)ControllerSelect.SelectedItem == "Keyboard")
            {
                ctlConfig.GetDefaultConfig();
                btnCtlSettings.Enabled = false;
            }
            else
            {
                ctlConfig.GetConfig(connected_controllers[ControllerSelect.SelectedIndex]);
                controller.SetCurrent(connected_controllers[ControllerSelect.SelectedIndex]);
                controller.SetChannelMapping(ctlConfig.inputMapping, ctlConfig.inputInverted);
                btnCtlSettings.Enabled = true;
            }
        }

        /********************************************************************************
         * FUNCTION:        StartControllerInput
         * Description:     Starts the polling thread used to acquire gamepad input
         * Parameters:      N/A
         ********************************************************************************/
        private void StartControllerInput()
        {
            controller.SetCurrent(connected_controllers[ControllerSelect.SelectedIndex]);
            cancelBackgroundWorker = false;

            while (ControllerPoller.IsBusy)
            {
                ControllerPoller.CancelAsync();
                System.Threading.Thread.Sleep(100);
            }
            ControllerPoller.RunWorkerAsync();
            
            SendTimer.Interval = (int)TimerIntSelect.Value;
            SendTimer.Enabled = true;
            SendTimer.Start();

            System.Threading.Thread.Sleep(50);
        }

        /********************************************************************************
         * FUNCTION:        StopControllerInput
         * Description:     Terminates the gamepad polling thread
         * Parameters:      N/A
         ********************************************************************************/
        private void StopControllerInput()
        {
            SendTimer.Stop();
            SendTimer.Enabled = false;
            ControllerPoller.CancelAsync();
            cancelBackgroundWorker = true;
            Console.WriteLine("Cancellation Requested.");
            System.Threading.Thread.Sleep(100);
        }

        /********************************************************************************
         * FUNCTION:        DrawZ
         * Description:     
         * Parameters:      N/A
         ********************************************************************************/
        private void DrawZ(long position)
        {
            Bitmap bitmap = new Bitmap(PadZ_View.Width, PadZ_View.Height);
            float position_center = PadZ_View.Height * (1 - ((float)position / (float)controller.getFS()));

            for (int i = -5; i < 5; i++)
            {
                if (position_center+i > 0 && PadZ_View.Height - (position_center+i) > 0)
                {
                    bitmap.SetPixel(PadZ_View.Width / 2, (int)position_center + i, Color.Black);
                }

                if(position_center+i > 0 && position_center < PadZ_View.Height)
                {
                    bitmap.SetPixel((PadZ_View.Width / 2) + i, (int)position_center, Color.Black);
                }
            }

            PadZ_View.Image = (Image)bitmap;
        }

        private void DrawZ_Persist(long position)
        {
            Bitmap bitmap = new Bitmap(PadZ_View.Width, PadZ_View.Height);
            float position_center = PadZ_View.Height * ((float)position / (float)controller.getFS());

            using (Graphics gr = Graphics.FromImage(bitmap))
            {
                Rectangle rect = new Rectangle(0, PadZ_View.Height - (int)position_center, PadZ_View.Width, (int)position_center);
                gr.FillRectangle(Brushes.GreenYellow, rect);
            }

            PadZ_View.Image = (Image)bitmap;
        }

        /********************************************************************************
         * FUNCTION:        DrawXY
         * Description:     
         * Parameters:      N/A
         ********************************************************************************/
        private void DrawXY(long X, long Y)
        {
            Bitmap bitmap = new Bitmap(PadXY_View.Width, PadXY_View.Height);
            float fullscale = (float)controller.getFS();
            Point position_center = new Point( (int)(PadXY_View.Width * ((float)X / fullscale)),
                (int)(PadXY_View.Height * ((float)Y / fullscale)) );

            for (int i = -5; i < 5; i++)
            {
                if (position_center.Y+i > 0 && PadXY_View.Height - (position_center.Y+i) > 0)
                {
                    if(position_center.X > 0 && position_center.X < PadXY_View.Width)
                    {
                        bitmap.SetPixel(position_center.X, (int)position_center.Y + i, Color.Black);
                    }
                }

                if (position_center.X+i > 0 && position_center.X+i < PadXY_View.Width)
                {
                    if(position_center.Y > 0 && position_center.Y < PadXY_View.Height)
                    {
                        bitmap.SetPixel(position_center.X + i, (int)position_center.Y, Color.Black);
                    }
                }
            }
            PadXY_View.Image = (Image)bitmap;
        }

        /********************************************************************************
         * EVENT HANDLER:   ControllerPoller_DoWork
         * Description:     
         ********************************************************************************/
        private void ControllerPoller_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            //Console.WriteLine("Poll Start");
            //int count = 0;

            while (true)
            {
                if (cancelBackgroundWorker) //worker.CancellationPending == true || 
                {
                    Console.WriteLine("Cancellation Pending.");
                    e.Cancel = true;
                    break;
                }
                controller.Poll();
                System.Threading.Thread.Sleep(10);

                //count++;
                //Console.WriteLine("Poll Count: " + count.ToString());
            }
            //Console.WriteLine("Poll End");
        }

        /********************************************************************************
         * EVENT HANDLER:   ControllerPoller_RunWorkerCompleted
         * Description:     
         ********************************************************************************/
        private void ControllerPoller_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw (e.Error);
            }
            //Console.WriteLine("Worker Completed");
        }

        //Only works on Windows. 
        [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        const int WM_VSCROLL = 277;
        const int SB_BOTTOM = 7;

        private void ScrollToEnd(RichTextBox rtb)
        {
            IntPtr ptrWparam = new IntPtr(SB_BOTTOM);
            IntPtr ptrLparam = new IntPtr(0);
            SendMessage(rtb.Handle, WM_VSCROLL, ptrWparam, ptrLparam);
        }

        /********************************************************************************
         * EVENT HANDLER:   radioText_CheckedChanged
         * Description:     
         ********************************************************************************/
        private void radioText_CheckedChanged(object sender, EventArgs e)
        {

        }

        /********************************************************************************
         * EVENT HANDLER:   InComTxt_TextChanged
         * Description:     This handler limits the number of lines displayed on the command line
         *                  text box by effectively scrolling the old lines off 
         ********************************************************************************/
        private void InComTxt_TextChanged(object sender, EventArgs e)
        {
            if (InComTxt.Lines.Count() > MAX_CMD_LINES)
            {
                var lines = this.InComTxt.Lines;
                var newLines = lines.Skip(1);
                this.InComTxt.Lines = newLines.ToArray();
                InComTxt.SelectionStart = InComTxt.TextLength;
            }

            if (!InComTxt.Focused)
            {
                InComTxt.Focus();
            }

            this.InComTxt.ScrollToCaret();
        }

        /********************************************************************************
         * EVENT HANDLER:   OutComTxt_TextChanged
         * Description:     This handler limits the number of lines displayed on the command line
         *                  text box by effectively scrolling the old lines off 
         ********************************************************************************/
        private void OutComTxt_TextChanged(object sender, EventArgs e)
        {
            int linecount = OutComTxt.GetLineFromCharIndex(OutComTxt.TextLength);
            //Console.WriteLine("OutComTxt Lines: " + linecount);
            //linecount = OutComTxt.Lines.Count(); //doesn't work on RPi. Lines Array only contains 1 element.

            if (linecount > MAX_CMD_LINES)
            {
                var lines = this.OutComTxt.Lines;
                var newLines = lines.Skip(1);
                this.OutComTxt.Lines = newLines.ToArray();
                OutComTxt.SelectionStart = OutComTxt.TextLength;
            }

            this.OutComTxt.ScrollToCaret();
        }

        /********************************************************************************
         * EVENT HANDLER:   btnCtlSettings_Click
         * Description:     Button click handler for controller settings button                
         ********************************************************************************/
        private void btnCtlSettings_Click(object sender, EventArgs e)
        {
            ConfigForm configPanel = new ConfigForm(ref this.controller, ref this.ctlConfig);
            StopControllerInput();

            var result = configPanel.ShowDialog(this);

            if(result == System.Windows.Forms.DialogResult.OK)
            {
                controller.SetChannelMapping(configPanel.ChannelMapping, configPanel.ChannelInverted);
                ctlConfig = configPanel.activeConfig;
                ctlConfig.SaveToFile("Config.xml", controller);
            }

#if !_WINDOWS
            controller.RestartController();
#endif
            configPanel.Dispose();
            StartControllerInput();
        }

        /********************************************************************************
         * EVENT HANDLER:   Form_KeyDown
         * Description:     Keyboard KeyDown event handler               
         ********************************************************************************/
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            List<Keys> keysCopy = new List<Keys>(this.InputKeys.Keys);
            bool keyPressedIsInput = false;

            if (this.radioPad.Checked)
            {
                foreach (Keys entry in keysCopy)
                {
                    if(e.KeyCode == entry)
                    {
                        InputKeys[entry] = true;
                        keyPressedIsInput = true;
                    }
                }

                if(keyPressedIsInput)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
            }

            Console.WriteLine("Key {0} intercepted by Form", 'q');
        }

        /********************************************************************************
         * EVENT HANDLER:   Form_KeyUp
         * Description:     Keyboard KeyUp event handler               
         ********************************************************************************/
        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            List<Keys> keysCopy = new List<Keys>(this.InputKeys.Keys);
            bool keyPressedIsInput = false;

            if (this.radioPad.Checked)
            {
                foreach (Keys entry in keysCopy)
                {
                    if (e.KeyCode == entry)
                    {
                        InputKeys[entry] = false;
                        keyPressedIsInput = true;
                    }
                }

                if (keyPressedIsInput)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
            }
        }

        /********************************************************************************
         * EVENT HANDLER:   Form_PreviewKeyDown
         * Description:     Default handler for all controls' PreviewKeyDown event
         *                  Needed in order to register arrow key input with the form
         ********************************************************************************/
        private void Form_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    e.IsInputKey = true;
                    break;
                default:
                    break;
            }
        }

        /********************************************************************************
         * EVENT HANDLER:   BtnPlane_Click
         * Description:     Handler for the RC plane button. Essentially a built-in mode
         *                  for the RC ground station project.
         ********************************************************************************/
        private void BtnPlane_Click(object sender, EventArgs e)
        {
            Rectangle thisScreen;
            int x, y;
            String pyPath;
            bool closeVideo = false;

            thisScreen = Screen.FromControl(this).Bounds;

            //Dock the window at the bottom of the screen to leave room for the video downlink above
            if (isDocked)
            {
                this.TopMost = false;

                //If already docked, then reset the form to default size and position
                this.Height = 522;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.btnPlane.BackColor = SystemColors.Control;

                //Set the form in the centre of the screen
                this.StartPosition = FormStartPosition.CenterScreen;
                x = (thisScreen.Width - this.Width) / 2;
                y = (thisScreen.Height - this.Height) / 2;

                closeVideo = true;
            }
            else
            {
                //The way screen resolution is interpretted seems to be slightly different in Mono
                // the difference below was determined empirically on my Pi3 running Raspbian
                if(IsLinux)
                {
                    this.Height = 290;
                }
                else
                {
                    this.Height = 300;
                }
                //After re-sizing the form, set it at the bottom of the screen 
                // and make it not manually resizable
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.radioPad.Checked = true;
                this.btnPlane.BackColor = SystemColors.MenuHighlight;

                this.StartPosition = FormStartPosition.Manual;
                x = (thisScreen.Width - this.Width) / 2;
                y = thisScreen.Height - 300;

                //Run the video downlink python script
                pyPath = this.ctlConfig.SelectNode("//PythonPath");
                vidLinkPy.FileName = pyPath;
                vidLinkPy.Arguments = "RCVideoDownlink.py";
                vidLinkPy.UseShellExecute = true;
                vidLinkPy.RedirectStandardInput = false;

                vidLinkProc = Process.Start(vidLinkPy);

                //Keep this form on top of the video feed
                this.TopMost = true;
            }

            this.Location = new Point(x, y);

            isDocked = !isDocked;

            if(closeVideo)
            {
                //tempTimer.Enabled = true;
                //tempTimer.Start();
                CloseVideoDownlink();
            }
        }

        private void CloseVideoDownlink()
        {
            //If the downlink script is still active, then close it
            if (vidLinkProc != null)
            {
                if(!vidLinkProc.HasExited)
                {
                    IntPtr h = vidLinkProc.MainWindowHandle;

                    //Linux code doesn't work :(
                    if (IsLinux)
                    {
                        Console.WriteLine("isLinux == true");

                        if (h != IntPtr.Zero)
                        {
                            //X11lib.XRaiseWindow(_display, h);
                            //X11lib.XSetInputFocus(_display, h, X11lib.TRevertTo.RevertToParent, (TInt)0);

                            //Debug
                            Console.WriteLine("First window handle was not NULL.");
                        }
                        else
                        {
                            XSendKeystroke('q', "RCVideo");
                        }
                    }
                    else
                    {
                        SetForegroundWindow(h);
                    }

                    SendKeys.SendWait("q");
                }

                tempTimer.Interval = 1000;
                tempTimer.Enabled = true;
                tempTimer.Start();
            }
        }

        /********************************************************************************
         * EVENT HANDLER:   OnClosed
         * Description:     Main form closed event handler. Used to kill the video downlink
         *                  python process if it's still alive
         ********************************************************************************/
        protected override void OnClosed(EventArgs e)
        {
            CloseVideoDownlink();
        }

        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        private void XSendKeystroke(char key, string windowTitle)
        {
            IntPtr _display = X11lib.XOpenDisplay(String.Empty);
            IntPtr rootwin = X11lib.XDefaultRootWindow(_display);
            IntPtr ActiveWindowAtom = X11lib.XInternAtom(_display, "_NET_ACTIVE_WINDOW", false);

            int winIndex = 0;
            int tempreturn = 0;

            XEvent ev = new XEvent();
            ev.ClientMessageEvent.type = XEventName.ClientMessage;
            ev.ClientMessageEvent.message_type = ActiveWindowAtom;
            ev.ClientMessageEvent.format = 32;
            ev.ClientMessageEvent.ptr1 = (IntPtr)1U;
            ev.ClientMessageEvent.ptr2 = (IntPtr)1U;
            ev.ClientMessageEvent.ptr3 = (IntPtr)0U;
            ev.ClientMessageEvent.ptr4 = (IntPtr)0U;
            ev.ClientMessageEvent.ptr5 = (IntPtr)0U;

            XEvent kev = new XEvent();
            kev.KeyEvent.type = XEventName.KeyPress;
            kev.KeyEvent.display = _display;
            kev.KeyEvent.keycode = X11lib.XKeysymToKeycode(_display, 0x0071); //q
            kev.KeyEvent.root = rootwin;
            kev.KeyEvent.same_screen = true;

            //Find windows based on title
            IntPtr[] allWindows = Helpers.FindWindows(_display, windowTitle);
            if (allWindows.Count() > 0)
            {
                X11lib.XSelectInput(_display, allWindows[winIndex], EventMask.KeyPressMask | EventMask.KeyReleaseMask);

                //Raise window to top of stack - works!
                ev.ClientMessageEvent.window = allWindows[winIndex];
                tempreturn = (int)X11lib.XSendEvent(_display, rootwin, (TBoolean)0, (TLong)X11.EventMask.SubstructureRedirectMask, ref ev);
                X11lib.XMapRaised(_display, allWindows[winIndex]);

                //Send key press event - doesn't work :(
                kev.KeyEvent.window = allWindows[winIndex];
                tempreturn = (int)X11lib.XSendEvent(_display, allWindows[winIndex], (TBoolean)1, (TLong)X11.EventMask.KeyPressMask, ref kev);

                kev.KeyEvent.type = XEventName.KeyRelease;
                tempreturn = (int)X11lib.XSendEvent(_display, allWindows[winIndex], (TBoolean)1, (TLong)X11.EventMask.KeyReleaseMask, ref kev);
                X11lib.XSync(_display, false);
            }
            else
            {
                Console.WriteLine("No Window found!");
            }

            X11lib.XCloseDisplay(_display);
        }

        private void tempTimer_Tick(object sender, EventArgs e)
        {
            if(vidLinkProc.HasExited)
            {
                vidLinkProc.Close();
                vidLinkProc.Dispose();
                vidLinkProc = null;
                
                tempTimer.Stop();
                tempTimer.Enabled = false;
                Console.WriteLine("vidLinkProc has exited!");
            }
            else
            {
                XSendKeystroke('q', "RCVideo");
                SendKeys.SendWait("q");
            }
        }
    }
}
