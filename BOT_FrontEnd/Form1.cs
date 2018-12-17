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

namespace BOT_FrontEnd
{
    public partial class Form1 : Form
    {
        const float SPEED = 25; //mm/s

        private Controller controller;
        private List<Guid> connected_controllers;
        private bool sent_stop;
        private bool PauseTransfer;

        private DirectInput DI;

        public Form1()
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

            DI = new DirectInput();
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

                //if 'Keyboard'(arrow keys) is selected as the current method of input
                if ((String)ControllerSelect.SelectedItem == "Keyboard")
                {
                    double x = 0;
                    double y = 0;
                    double z = 0;
                    double key_increment = this.controller.getFS() / 2;             //half of full scale (unsigned)
                    x += KeyboardInfo.GetKeyState(Keys.Left).IsPressed ? -0.5 : 0;
                    x += KeyboardInfo.GetKeyState(Keys.Right).IsPressed ? 0.5 : 0;
                    y += KeyboardInfo.GetKeyState(Keys.Up).IsPressed ? -0.5 : 0;
                    y += KeyboardInfo.GetKeyState(Keys.Down).IsPressed ? 0.5 : 0;

                    x = (x + 1) * key_increment;
                    y = (y + 1) * key_increment;

                    DrawXY((int)x, (int)y);

                    x = ((x / key_increment) - 1) * (SendTimer.Interval / 250.0);
                    y = -1 * ((y / key_increment) - 1) * (SendTimer.Interval / 250.0);

                    cmd = String.Format("\r\nG91X{0:0.00}Y{1:0.00}Z{2:0.00};", x, y, z);
                    if (Math.Abs(x) > 0.05 || Math.Abs(y) > 0.05 || Math.Abs(z) > 0.05)
                    {
                        InComTxt.AppendText(cmd);
                        if (MyVCOM.IsOpen) { MyVCOM.Write(cmd); }
                        sent_stop = false;
                    }
                    else if (sent_stop == false)
                    {
                        InComTxt.AppendText("\r\nG91X0Y0Z0;");
                        if (MyVCOM.IsOpen) { MyVCOM.Write("\r\nG91X0Y0Z0;"); }
                        sent_stop = true;
                    }
                }
                else
                {
                    JoystickState state = controller.getState();
                    bool[] buttons = state.GetButtons();
                    int[] pov = state.GetPointOfViewControllers();
                    DrawXY(state.Z, state.RotationZ);
                    DrawZ(state.Y);

                    double angle_A_B = (pov[0] == -1) ? -1 : (pov[0] / 100) * (Math.PI / 180);

                    float fs = (float)controller.getFS();
                    float x = (((float)(state.Z - fs / 2) / fs) * SPEED * (float)(SendTimer.Interval / 1000.0));
                    float y = -1 * (((float)(state.RotationZ - fs / 2) / fs) * SPEED * (float)(SendTimer.Interval / 1000.0));
                    float z = (((float)(state.Y - fs / 2) / fs) * SPEED * (float)(SendTimer.Interval / 1000.0));

                    double b = (angle_A_B == -1) ? 0 : -0.1 * Math.Sin(angle_A_B);
                    double a = (angle_A_B == -1) ? 0 : 0.1 * Math.Cos(angle_A_B);                    

                    cmd = "\r\nG91";
                    if (Math.Abs(x) > 0.05) { cmd = String.Format(cmd + "Y{0:0.0000}", x); }
                    if (Math.Abs(y) > 0.05) { cmd = String.Format(cmd + "X{0:0.0000}", -y); }
                    if (Math.Abs(z) > 0.05) { cmd = String.Format(cmd + "Z{0:0.0000}", z); }
                    if (Math.Abs(a) > 0.05) { cmd = String.Format(cmd + "B{0:0.0000}", a); }
                    if (Math.Abs(b) > 0.05) { cmd = String.Format(cmd + "A{0:0.0000}", -b); }
                    cmd += ";";

                    if (Math.Abs(x) > 0.05 || Math.Abs(y) > 0.05 || Math.Abs(z) > 0.05 || Math.Abs(a) > 0.05 || Math.Abs(b) > 0.05)
                    {
                        InComTxt.AppendText(cmd);
                        if (MyVCOM.IsOpen) { MyVCOM.Write(cmd); }
                        sent_stop = false;
                    }
                    else if (sent_stop == false)
                    {
                        InComTxt.AppendText("\r\nG91X0Y0Z0;");
                        if (MyVCOM.IsOpen) { MyVCOM.Write("\r\nG91X0Y0Z0;"); }
                        sent_stop = true;
                    }
                }
                ScrollToEnd(InComTxt);
            }
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

                SendTimer.Interval = (int)TimerIntSelect.Value;
                SendTimer.Enabled = true;
                SendTimer.Start();

            }
            else
            {
                PadInPanel.SendToBack();
                PadXY_View.Visible = false;
                PadZ_View.Visible = false;

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
            float position_center = PadZ_View.Height * ((float)position / (float)controller.getFS());

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
        
    }
}
