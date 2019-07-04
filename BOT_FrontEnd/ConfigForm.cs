using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BOT_FrontEnd
{
    public partial class ConfigForm : Form
    {
        public ControllerProperty[] ChannelMapping {get; private set;}
        public bool[] ChannelInverted { get; private set; }
        public Config activeConfig { get; private set; }
        
        private int prev_x;
        private int prev_y;
        private int prev_z;
        private int prev_rotation_x;
        private int prev_rotation_y;
        private int prev_rotation_z;
        private long full_scale;

        private Controller activeController;
        private ChannelNumber activeChannel;
        private ControllerProperty activeInput;
        private bool inputSelected = false;
        private bool inputInverted = false;

        public ConfigForm(ref Controller ctl, ref Config cfg)
        {            
            InitializeComponent();
            activeController = ctl;
            activeController.RestartController();
            activeConfig = cfg;
            activeController.Poll();
            full_scale = activeController.getFS();

            ChannelMapping = activeController.ChannelMapping;
            ChannelInverted = activeController.ChannelInverted;
            UpdateInputLabels();
            setDefaultSetupValues();

            ControllerPoller.WorkerSupportsCancellation = true;
            ControllerPoller.DoWork += new DoWorkEventHandler(ControllerPoller_DoWork);
            ControllerPoller.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ControllerPoller_RunWorkerCompleted);
        }

        /********************************************************************************
         * FUNCTION:        setDefaultSetupValues
         * Description:     Populate the text boxes with their initial values
         * Parameters:      N/A
         ********************************************************************************/
        private void setDefaultSetupValues()
        {
            if(activeConfig != null)
            {
                txt1_min.Text = activeConfig.channel_config[(int)ChannelNumber.CH1].MIN.ToString();
                txt1_max.Text = activeConfig.channel_config[(int)ChannelNumber.CH1].MAX.ToString();
                txt1_center.Text = activeConfig.channel_config[(int)ChannelNumber.CH1].CENTER.ToString();

                txt2_min.Text = activeConfig.channel_config[(int)ChannelNumber.CH2].MIN.ToString();
                txt2_max.Text = activeConfig.channel_config[(int)ChannelNumber.CH2].MAX.ToString();
                txt2_center.Text = activeConfig.channel_config[(int)ChannelNumber.CH2].CENTER.ToString();

                txt3_min.Text = activeConfig.channel_config[(int)ChannelNumber.CH3].MIN.ToString();
                txt3_max.Text = activeConfig.channel_config[(int)ChannelNumber.CH3].MAX.ToString();
                txt3_center.Text = activeConfig.channel_config[(int)ChannelNumber.CH3].CENTER.ToString();

                txt4_min.Text = activeConfig.channel_config[(int)ChannelNumber.CH4].MIN.ToString();
                txt4_max.Text = activeConfig.channel_config[(int)ChannelNumber.CH4].MAX.ToString();
                txt4_center.Text = activeConfig.channel_config[(int)ChannelNumber.CH4].CENTER.ToString();

                txt5_min.Text = activeConfig.channel_config[(int)ChannelNumber.CH5].MIN.ToString();
                txt5_max.Text = activeConfig.channel_config[(int)ChannelNumber.CH5].MAX.ToString();
                txt5_center.Text = activeConfig.channel_config[(int)ChannelNumber.CH5].CENTER.ToString();

                chkPersist.Checked = activeConfig.z_persist;
                chkAccum.Checked = activeConfig.z_accumulating;
            }
        }

        /********************************************************************************
         * EVENT:        btnCHx_Click
         * Description:     Event handlers for each of the input select buttons
         * Parameters:      N/A
         ********************************************************************************/
        private void btnCH1_Click(object sender, EventArgs e)
        {
            inputSelected = false;
            activeChannel = ChannelNumber.CH1;
            label1.BackColor = Color.Blue;
            UpdatePreviousState();
            StartControllerInput();
        }

        private void btnCH2_Click(object sender, EventArgs e)
        {
            inputSelected = false;
            activeChannel = ChannelNumber.CH2;
            label2.BackColor = Color.Blue;
            UpdatePreviousState();
            StartControllerInput();
        }

        private void btnCH3_Click(object sender, EventArgs e)
        {
            inputSelected = false;
            activeChannel = ChannelNumber.CH3;
            label3.BackColor = Color.Blue;
            UpdatePreviousState();
            StartControllerInput();
        }

        private void btnCH4_Click(object sender, EventArgs e)
        {
            inputSelected = false;
            activeChannel = ChannelNumber.CH4;
            label4.BackColor = Color.Blue;
            UpdatePreviousState();
            StartControllerInput();
        }

        private void btnCH5_Click(object sender, EventArgs e)
        {
            inputSelected = false;
            activeChannel = ChannelNumber.CH5;
            label5.BackColor = Color.Blue;
            UpdatePreviousState();
            StartControllerInput();
        }

        /********************************************************************************
         * FUNCTION:        StartControllerInput
         * Description:     Starts the polling thread used to acquire gamepad input
         * Parameters:      N/A
         ********************************************************************************/
        private void StartControllerInput()
        {
            while (ControllerPoller.IsBusy)
            {
                ControllerPoller.CancelAsync();
                System.Threading.Thread.Sleep(10);
            }
            ControllerPoller.RunWorkerAsync();
            System.Threading.Thread.Sleep(50);
        }

        /********************************************************************************
         * FUNCTION:        UpdateInputLabels
         * Description:     Update label text beside each input select button based on the 
         *                  current channel mapping
         * Parameters:      N/A
         ********************************************************************************/
        private void UpdateInputLabels()
        {
            int index = 0;
            Control[] labels = {label1, label2, label3, label4, label5};
            String temp_str = "";

            if (ChannelMapping != null)
            {
                foreach (ControllerProperty cp in ChannelMapping)
                {
                    switch (cp)
                    {
                        case ControllerProperty.X:
                            temp_str = StringEnum.GetStringValue(ControllerProperty.X);
                            break;
                        case ControllerProperty.Y:
                            temp_str = StringEnum.GetStringValue(ControllerProperty.Y);
                            break;
                        case ControllerProperty.Z:
                            temp_str = StringEnum.GetStringValue(ControllerProperty.Z);
                            break;
                        case ControllerProperty.RotationX:
                            temp_str = StringEnum.GetStringValue(ControllerProperty.RotationX);
                            break;
                        case ControllerProperty.RotationY:
                            temp_str = StringEnum.GetStringValue(ControllerProperty.RotationY);
                            break;
                        case ControllerProperty.RotationZ:
                            temp_str = StringEnum.GetStringValue(ControllerProperty.RotationZ);
                            break;
                        case ControllerProperty.Button_0:
                            temp_str = StringEnum.GetStringValue(ControllerProperty.Button_0);
                            break;
                        case ControllerProperty.Button_1:
                            temp_str = StringEnum.GetStringValue(ControllerProperty.Button_1);
                            break;
                        case ControllerProperty.Button_2:
                            temp_str = StringEnum.GetStringValue(ControllerProperty.Button_2);
                            break;
                        case ControllerProperty.Button_3:
                            temp_str = StringEnum.GetStringValue(ControllerProperty.Button_3);
                            break;
                        case ControllerProperty.Button_4:
                            temp_str = StringEnum.GetStringValue(ControllerProperty.Button_4);
                            break;
                        default:
                            break;
                    }

                    if (ChannelInverted[index] == true)
                    {
                        temp_str = "-" + temp_str;
                    }

                    labels[index].Text = temp_str;
                    labels[index].Font = new Font(labels[index].Font, FontStyle.Bold);
                    labels[index].BackColor = SystemColors.Control;
                    index++;
                }
            } 
        }

        /********************************************************************************
         * FUNCTION:        StopControllerInput
         * Description:     Terminates the gamepad polling thread
         * Parameters:      N/A
         ********************************************************************************/
        private void StopControllerInput()
        {
            ControllerPoller.CancelAsync();
            System.Threading.Thread.Sleep(50);
        }

        /********************************************************************************
         * FUNCTION:        UpdatePreviousState
         * Description:     Updates the 'previous' values used for detection of the current input channel
         * Parameters:      N/A
         ********************************************************************************/
        private void UpdatePreviousState()
        {
            JoystickState state;
            activeController.Poll();
            state = activeController.getState();
            prev_x = state.X;
            prev_y = state.Y;
            prev_z = state.Z;
            prev_rotation_x = state.RotationX;
            prev_rotation_y = state.RotationY;
            prev_rotation_z = state.RotationZ;
        }

        /********************************************************************************
         * EVENT HANDLER:   ControllerPoller_DoWork
         * Description:     
         ********************************************************************************/
        private void ControllerPoller_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            while (!inputSelected)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                activeController.Poll();
                this.checkActiveInput();

                if (inputSelected)
                {
                    ChannelMapping[(int)activeChannel] = activeInput;
                    ChannelInverted[(int)activeChannel] = inputInverted;
                }

                System.Threading.Thread.Sleep(10);
            }
        }

        /********************************************************************************
         * EVENT HANDLER:   ControllerPoller_RunWorkerCompleted
         * Description:     
         ********************************************************************************/
        private void ControllerPoller_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateInputLabels();

            if (e.Error != null)
            {
                throw (e.Error);
            }
        }

        /********************************************************************************
         * FUNCTION:        checkActiveInput
         * Description:     
         * Parameters:      N/A
         ********************************************************************************/
        private void checkActiveInput()
        {
            JoystickState state;
            double max_diff = 0.25;
            double temp_diff = 0.0;
            bool[] buttons;

            state = activeController.getState();
            buttons = state.GetButtons().ToArray();
            inputInverted = false;

            for (int i = 0; i < (int)ControllerProperty.NUM_BUTTONS; i++)
            {
                if(buttons[i] == true)
                {
                    activeInput = (ControllerProperty)(ControllerProperty.Button_1 + i);
                    inputSelected = true;
                    return;
                }
            }

            //Check JoystickState X axis
            temp_diff = (double)Math.Abs(state.X - prev_x) / (double)full_scale;
            if (temp_diff > max_diff)
            {
                max_diff = temp_diff;
                activeInput = ControllerProperty.X;
                
                if((state.X - prev_x) < 0)
                {
                    inputInverted = true;
                }

                inputSelected = true;
            }

            //Check JoystickState Y axis
            temp_diff = (double)Math.Abs(state.Y - prev_y) / (double)full_scale;
            if(temp_diff > max_diff)
            {
                max_diff = temp_diff;
                activeInput = ControllerProperty.Y;

                if ((state.Y - prev_y) < 0)
                {
                    inputInverted = true;
                }

                inputSelected = true;
            }

            //Check JoystickState Z axis
            temp_diff = (double)Math.Abs(state.Z - prev_z) / (double)full_scale;
            if (temp_diff > max_diff)
            {
                max_diff = temp_diff;
                activeInput = ControllerProperty.Z;

                if ((state.Z - prev_z) < 0)
                {
                    inputInverted = true;
                }

                inputSelected = true;
            }

            //Check JoystickState X rotation
            temp_diff = (double)Math.Abs(state.RotationX - prev_rotation_x) / (double)full_scale;
            if (temp_diff > max_diff)
            {
                max_diff = temp_diff;
                activeInput = ControllerProperty.RotationX;

                if ((state.RotationX - prev_rotation_x) < 0)
                {
                    inputInverted = true;
                }

                inputSelected = true;
            }

            //Check JoystickState Y rotation
            temp_diff = (double)Math.Abs(state.RotationY - prev_rotation_y) / (double)full_scale;
            if (temp_diff > max_diff)
            {
                max_diff = temp_diff;
                activeInput = ControllerProperty.RotationY;

                if ((state.RotationY - prev_rotation_y) < 0)
                {
                    inputInverted = true;
                }

                inputSelected = true;
            }

            //Check JoystickState Z rotation
            temp_diff = (double)Math.Abs(state.RotationZ - prev_rotation_z) / (double)full_scale;
            if (temp_diff > max_diff)
            {
                max_diff = temp_diff;
                activeInput = ControllerProperty.RotationZ;

                if ((state.RotationZ - prev_rotation_z) < 0)
                {
                    inputInverted = true;
                }

                inputSelected = true;
            }
        }

        private void UpdateConfig()
        {
            Control[,] configTxtBoxes = 
            {
                {txt1_min, txt1_max, txt1_center},
                {txt2_min, txt2_max, txt2_center},
                {txt3_min, txt3_max, txt3_center},
                {txt4_min, txt4_max, txt4_center},
                {txt5_min, txt5_max, txt5_center}
            };
            ChannelConfigValues[] configValues = new ChannelConfigValues[(int)ChannelNumber.NUM_CHANNELS];

            for(int i = 0; i < (int)ChannelNumber.NUM_CHANNELS; i++)
            {
                configValues[i] = new ChannelConfigValues();
                configValues[i].MIN = double.Parse(configTxtBoxes[i,0].Text);
                configValues[i].MAX = double.Parse(configTxtBoxes[i, 1].Text);
                configValues[i].CENTER = double.Parse(configTxtBoxes[i, 2].Text);
            }

            activeConfig.SetConfigParams(configValues, chkPersist.Checked, chkAccum.Checked);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateConfig();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}
