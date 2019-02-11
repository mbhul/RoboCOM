using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using SlimDX.DirectInput;
using System.Runtime.InteropServices;
using System.Xml;

namespace BOT_FrontEnd
{
    public partial class Form1 : Form
    {
        //const float SPEED = 25; //mm/s
        //Maximum number of command lines to maintain in the rich text box for history. 
        const int MAX_CMD_LINES = 10000;

        private Controller controller;

        private List<Guid> connected_controllers;
        private bool sent_stop;
        private bool PauseTransfer;
        private bool z_persist;
        private bool z_accumulating;
        private double z_value;
        private string command_prev;

        private DirectInput DI;
        private XmlDocument configXML;
        private Config ctlConfig;

        public Form1()
        {
            try
            {
                //Initialize and set defaults
                InitializeComponent();
                bold_font = new Font(InComTxt.Font, FontStyle.Bold);
                BaudSelect.SelectedIndex = 2;
                sent_stop = true;
                PauseTransfer = false;
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

                configXML = new XmlDocument();
                configXML.Load("Config.xml");
                z_persist = Boolean.Parse(configXML.SelectNodes("//Channel[@param='CH1']").Item(0).Attributes["persist"].Value);
                z_accumulating = Boolean.Parse(configXML.SelectNodes("//Channel[@param='CH1']").Item(0).Attributes["accum"].Value);
                command_prev = "";

                ctlConfig = new Config("Config.xml");

                DI = new DirectInput();
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
        }

        private void Form_Load(object sender, EventArgs e)
        {
            
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
        }

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
                SendTimer.Enabled = false;
                ReadTimer.Enabled = false;
                MyVCOM.Close();
                SendBtn.Enabled = false;
                ConnReset.Text = "Open Connection";
                ConnStatusLbl.Text = "Not Connected";
                ConnStatusLbl.ForeColor = Color.Red;
            }
            else //else open the COM port
            {
                MyVCOM.PortName = "COM" + PortNumber.SelectedItem.ToString();
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
                { }

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

            //Store configuration values for later use
            z_fs = Double.Parse(configXML.SelectNodes("//Channel[@param='CH1']").Item(0).Attributes["max"].Value) -
                Double.Parse(configXML.SelectNodes("//Channel[@param='CH1']").Item(0).Attributes["min"].Value);

            x_fs = Double.Parse(configXML.SelectNodes("//Channel[@param='CH2']").Item(0).Attributes["max"].Value) -
                        Double.Parse(configXML.SelectNodes("//Channel[@param='CH2']").Item(0).Attributes["min"].Value);

            y_fs = Double.Parse(configXML.SelectNodes("//Channel[@param='CH3']").Item(0).Attributes["max"].Value) -
                Double.Parse(configXML.SelectNodes("//Channel[@param='CH3']").Item(0).Attributes["min"].Value);

            a_fs = Double.Parse(configXML.SelectNodes("//Channel[@param='CH4']").Item(0).Attributes["max"].Value) -
                Double.Parse(configXML.SelectNodes("//Channel[@param='CH4']").Item(0).Attributes["min"].Value);

            b_fs = Double.Parse(configXML.SelectNodes("//Channel[@param='CH5']").Item(0).Attributes["max"].Value) -
                Double.Parse(configXML.SelectNodes("//Channel[@param='CH5']").Item(0).Attributes["min"].Value);

            z_def = Double.Parse(configXML.SelectNodes("//Channel[@param='CH1']").Item(0).Attributes["default"].Value);
            x_def = Double.Parse(configXML.SelectNodes("//Channel[@param='CH2']").Item(0).Attributes["default"].Value);
            y_def = Double.Parse(configXML.SelectNodes("//Channel[@param='CH3']").Item(0).Attributes["default"].Value);
            a_def = Double.Parse(configXML.SelectNodes("//Channel[@param='CH4']").Item(0).Attributes["default"].Value);
            b_def = Double.Parse(configXML.SelectNodes("//Channel[@param='CH5']").Item(0).Attributes["default"].Value);

            z_gain = Double.Parse(configXML.SelectNodes("//Channel[@param='CH1']").Item(0).Attributes["gain"].Value);

            //If the Z-input is self-centering then the default value will be somewhere in the middle of the Z full-scale
            // Otherwise, the Z-input will be near the Z-min. This condition checks if the Z-default is less than 1% of Z full-scale
            z_centered = !((z_def - Double.Parse(configXML.SelectNodes("//Channel[@param='CH1']").Item(0).Attributes["min"].Value)) < (z_fs * 0.01));

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

                #region Keyboard Input Selected
                //if 'Keyboard'(arrow keys) is selected as the current method of input
                if ((String)ControllerSelect.SelectedItem == "Keyboard")
                {
                    double key_increment = this.controller.getFS() / 2;

                    //**** Get key states *****//
                    x += KeyboardInfo.GetKeyState(Keys.Left).IsPressed ? -0.5 : 0;
                    x += KeyboardInfo.GetKeyState(Keys.Right).IsPressed ? 0.5 : 0;
                    y += KeyboardInfo.GetKeyState(Keys.Up).IsPressed ? -0.5 : 0;
                    y += KeyboardInfo.GetKeyState(Keys.Down).IsPressed ? 0.5 : 0;
                    z += KeyboardInfo.GetKeyState(Keys.W).IsPressed ? 0.05 : 0;
                    z += KeyboardInfo.GetKeyState(Keys.S).IsPressed ? -0.05 : 0;

                    //**** Scale key values for drawing *****//
                    x = (x + 1) * key_increment;
                    y = (y + 1) * key_increment;
                    z *= key_increment;

                    if(z_persist)
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
                    formatstring = "\r\n" + configXML.SelectNodes("//StartOfFrame[@type='relative']").Item(0).InnerXml;
                    formatstring += configXML.SelectNodes("//Channel[@param='CH1']").Item(0).InnerXml + "{0:";
                    formatstring += configXML.SelectNodes("//Channel[@param='CH1']").Item(0).Attributes["precision"].Value + "}";
                    formatstring += configXML.SelectNodes("//ChannelSeparator").Item(0).InnerXml;

                    formatstring += configXML.SelectNodes("//Channel[@param='CH2']").Item(0).InnerXml + "{1:";
                    formatstring += configXML.SelectNodes("//Channel[@param='CH2']").Item(0).Attributes["precision"].Value + "}";
                    formatstring += configXML.SelectNodes("//ChannelSeparator").Item(0).InnerXml;

                    formatstring += configXML.SelectNodes("//Channel[@param='CH3']").Item(0).InnerXml + "{2:";
                    formatstring += configXML.SelectNodes("//Channel[@param='CH3']").Item(0).Attributes["precision"].Value + "}";
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
                            formatstring = "\r\n" + configXML.SelectNodes("//StopCommand[@enable='true']").Item(0).InnerXml;
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
                    bool[] buttons = state.GetButtons();
                    int[] pov = state.GetPointOfViewControllers();
                    float fs = (float)controller.getFS();
                    bool z_condition = false;

                    //**** Scale controller values for drawing *****//
                    x = limitControllerScaleValue(controller.CH2, 0.0, fs);
                    y = limitControllerScaleValue(controller.CH3, 0.0, fs);

                    DrawXY((int)x, (int)y);

                    z = controller.CH1;
                    if (z_persist)
                    {
                        if (z_accumulating)
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
                    double angle_A_B = (pov[0] == -1) ? -1 : (pov[0] / 100) * (Math.PI / 180);

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

                    cmd = "\r\n" + configXML.SelectNodes("//StartOfFrame[@type='relative']").Item(0).InnerXml;

                    //**** Convert the values to the configured command scale (in Config.xml) *****//
                    if (!z_centered)
                    {
                        z_condition = (Math.Abs(z) > (z_def + (0.05 * z_fs)) && (z_value != z));
                    }
                    else
                    {
                        z_condition = (Math.Abs(z - z_def) > (0.05 * z_fs) && (z_value != z));
                    }

                    formatstring = configXML.SelectNodes("//Channel[@param='CH1']").Item(0).InnerXml + "{0:";
                    formatstring += configXML.SelectNodes("//Channel[@param='CH1']").Item(0).Attributes["precision"].Value + "}";
                    formatstring += configXML.SelectNodes("//ChannelSeparator").Item(0).InnerXml;
                    cmd = String.Format(cmd + formatstring, z);

                    formatstring = configXML.SelectNodes("//Channel[@param='CH2']").Item(0).InnerXml + "{0:";
                    formatstring += configXML.SelectNodes("//Channel[@param='CH2']").Item(0).Attributes["precision"].Value + "}";
                    formatstring += configXML.SelectNodes("//ChannelSeparator").Item(0).InnerXml;
                    cmd = String.Format(cmd + formatstring, x);

                    formatstring = configXML.SelectNodes("//Channel[@param='CH3']").Item(0).InnerXml + "{0:";
                    formatstring += configXML.SelectNodes("//Channel[@param='CH3']").Item(0).Attributes["precision"].Value + "}";
                    formatstring += configXML.SelectNodes("//ChannelSeparator").Item(0).InnerXml;
                    cmd = String.Format(cmd + formatstring, y);

                    formatstring = configXML.SelectNodes("//Channel[@param='CH4']").Item(0).InnerXml + "{0:";
                    formatstring += configXML.SelectNodes("//Channel[@param='CH4']").Item(0).Attributes["precision"].Value + "}";
                    formatstring += configXML.SelectNodes("//ChannelSeparator").Item(0).InnerXml;
                    cmd = String.Format(cmd + formatstring, a);

                    formatstring = configXML.SelectNodes("//Channel[@param='CH5']").Item(0).InnerXml + "{0:";
                    formatstring += configXML.SelectNodes("//Channel[@param='CH5']").Item(0).Attributes["precision"].Value + "}";
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
                            formatstring = "\r\n" + configXML.SelectNodes("//StopCommand[@enable='true']").Item(0).InnerXml;
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
                ScrollToEnd(InComTxt);

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
                    temp.Replace(";", ";\n");

                    if (temp.Contains("G00"))
                        PauseTransfer = true;
                    else if (temp.Contains("G01"))
                        PauseTransfer = false;

                    System.Action a = new System.Action(() =>
                    {
                        OutComTxt.Text += temp;
                        ScrollToEnd(OutComTxt);
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
            if ((String)ControllerSelect.SelectedItem == "Keyboard")
            {
                ctlConfig.GetDefaultConfig();
                btnCtlSettings.Enabled = false;
            }
            else
            {
                ctlConfig.GetConfig(connected_controllers[ControllerSelect.SelectedIndex]);
                controller.SetCurrent(connected_controllers[ControllerSelect.SelectedIndex]);
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
            while (true)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                controller.Poll();
                System.Threading.Thread.Sleep(10);
            }
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
        }

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
            if(InComTxt.Lines.Count() > MAX_CMD_LINES)
            {
                InComTxt.Select(0, InComTxt.GetFirstCharIndexFromLine(1));
                InComTxt.SelectedText = "";
            }
        }

        /********************************************************************************
         * EVENT HANDLER:   OutComTxt_TextChanged
         * Description:     This handler limits the number of lines displayed on the command line
         *                  text box by effectively scrolling the old lines off 
         ********************************************************************************/
        private void OutComTxt_TextChanged(object sender, EventArgs e)
        {
            if (OutComTxt.Lines.Count() > MAX_CMD_LINES)
            {
                OutComTxt.Select(0, OutComTxt.GetFirstCharIndexFromLine(1));
                OutComTxt.SelectedText = "";
            }
        }

        private void btnCtlSettings_Click(object sender, EventArgs e)
        {
            ConfigForm configPanel = new ConfigForm(ref this.controller);
            StopControllerInput();

            var result = configPanel.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                controller.SetChannelMapping(configPanel.ChannelMapping, configPanel.ChannelInverted);
            }

            StartControllerInput();
        }

    }
}
