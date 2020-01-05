using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOT_FrontEnd
{

#if !_WINDOWS
    //Wrapping class
    public class Controller
    {
        private JoystickLx currentDevice;     //current device         
        private IntPtr hWnd;                //handle to the parent of this device instance (ie. the main program window)
        private long full_scale;            //size of joystick axes
        public string Name { get; private set; }
        public Guid DeviceGuid { get; private set; }
        public String InputFile { get; private set; }

        private int[] CHANNEL;
        public ControllerProperty[] ChannelMapping { get; private set; }
        public bool[] ChannelInverted { get; private set; }

        private FileStream JFS;
        private byte[] buff = new byte[8];
        private JoystickState deviceState = new JoystickState();

        public Controller(IntPtr Owner)
        {
            hWnd = Owner;
            currentDevice = null;
            Name = "NA";
            full_scale = 100;
            ChannelMapping = null;

            CHANNEL = new int[(int)ChannelNumber.NUM_CHANNELS];
        }

        /********************************************************************************
         * FUNCTION:            SetCurrent
         * Description:         Sets the current input device file
         * Parameters:  
         *      device_guid -   The GUID of the input device
         ********************************************************************************/
        public void SetCurrent(String inputFileName)
        {
            if (inputFileName == InputFile || inputFileName == "Keyboard") { return; }

            if(JFS != null)
            {
                JFS.Close();
            }
            JFS = new FileStream(inputFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            currentDevice = new JoystickLx();
            buff = new byte[8];

            Name = inputFileName;
            InputFile = inputFileName;

            this.Poll();
            full_scale = 65535;

            ChannelMapping = Helpers.DefaultJoystick;
            ChannelInverted = Enumerable.Repeat<bool>(false, (int)ChannelNumber.NUM_CHANNELS).ToArray();
        }

        public void RestartController()
        {
            if(InputFile != "")
            {
                if (JFS != null)
                {
                    JFS.Close();
                }
                JFS = new FileStream(InputFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            } 
        }

        /********************************************************************************
         * FUNCTION:    Poll
         * Description: Acquires current device input into the instance class
         * Parameters:  N/A
         ********************************************************************************/
        public void Poll()
        {
            // Read 8 bytes from file and analyze.
            if(JFS != null && currentDevice != null)
            {
                int bytesRead = JFS.Read(buff, 0, 8);
                //int bytesRead = await JFS.ReadAsync(buff, 0, 8);

                if(bytesRead >= 8)
                {
                    currentDevice.DetectChange(buff);
                    this.getState();

                    if (ChannelMapping != null)
                    {
                        for (int i = 0; i < (int)ChannelNumber.NUM_CHANNELS; i++)
                        {
                            switch (ChannelMapping[i])
                            {
                                case ControllerProperty.X:
                                    CHANNEL[i] = deviceState.X;
                                    break;
                                case ControllerProperty.Y:
                                    CHANNEL[i] = deviceState.Y;
                                    break;
                                case ControllerProperty.Z:
                                    CHANNEL[i] = deviceState.Z;
                                    break;
                                case ControllerProperty.RotationX:
                                    CHANNEL[i] = deviceState.RotationX;
                                    break;
                                case ControllerProperty.RotationY:
                                    CHANNEL[i] = deviceState.RotationY;
                                    break;
                                case ControllerProperty.RotationZ:
                                    CHANNEL[i] = deviceState.RotationZ;
                                    break;
                                default:
                                    break;
                            }

                            if (ChannelInverted[i])
                            {
                                CHANNEL[i] = (int)full_scale - CHANNEL[i];
                            }
                        }
                    }
                }
                
            } 
        }

        //Public GET properties
        public int CH1
        {
            get { return CHANNEL[0]; }
        }

        public int CH2
        {
            get { return CHANNEL[1]; }
        }

        public int CH3
        {
            get { return CHANNEL[2]; }
        }

        public int CH4
        {
            get { return CHANNEL[3]; }
        }

        public int CH5
        {
            get { return CHANNEL[4]; }
        }

        /********************************************************************************
         * FUNCTION:    getState
         * Description: Returns the current device state
         * Parameters:  N/A
         ********************************************************************************/
        public JoystickState getState()
        {
            for(int count = 0; count < currentDevice.Axis.Count; count++)
            {
                switch(count)
                {
                    case 0:
                        deviceState.X = currentDevice.Axis[0] + ((int)full_scale / 2);
                        break;
                    case 1:
                        deviceState.Y = currentDevice.Axis[1] + ((int)full_scale / 2);
                        break;
                    case 2:
                        deviceState.Z = currentDevice.Axis[2] + ((int)full_scale / 2);
                        break;
                    case 3:
                        deviceState.RotationX = currentDevice.Axis[3] + ((int)full_scale / 2);
                        break;
                    case 4:
                        deviceState.RotationY = currentDevice.Axis[4] + ((int)full_scale / 2);
                        break;
                    case 5:
                        deviceState.RotationZ = currentDevice.Axis[5] + ((int)full_scale / 2);
                        break;
                    default:
                        break;
                }
            }

            for (int count = 0; count < currentDevice.Button.Count; count++)
            {
                if (count == deviceState.buttons.Count)
                {
                    deviceState.buttons.Add(currentDevice.Button[(byte)count]);
                }
                else
                {
                    deviceState.buttons[count] = currentDevice.Button[(byte)count];
                }
            }

            return deviceState;
        }

        /********************************************************************************
         * FUNCTION:    getFS
         * Description: Returns the current device's full scale
         * Parameters:  N/A
         ********************************************************************************/
        public long getFS()
        {
            return full_scale;
        }

        public void SetChannelMapping(ControllerProperty[] map, bool[] inversion)
        {
            ChannelMapping = map;
            ChannelInverted = inversion;
        }
        
    }
#endif

    public class JoystickLx
    {
        enum STATE : byte { PRESSED = 0x01, RELEASED = 0x00 }
        enum TYPE : byte { AXIS = 0x02, BUTTON = 0x01 }
        enum MODE : byte { CONFIGURATION = 0x80, VALUE = 0x00 }

        /// <summary>
        /// Buttons collection, key: address, bool: value
        /// </summary>
        public Dictionary<byte, bool> Button;

        /// <summary>
        /// Axis collection, key: address, short: value
        /// </summary>
        public Dictionary<byte, short> Axis;

        public JoystickLx()
        {
            Button = new Dictionary<byte, bool>();
            Axis = new Dictionary<byte, short>();
        }

        /// <summary>
        /// Function recognizes flags in buffer and modifies value of button, axis or configuration.
        /// Every new buffer changes only one value of one button/axis. Joystick object have to remember all previous values.
        /// </summary>
        public void DetectChange(byte[] buff)
        {
            // If configuration
            if (checkBit(buff[6], (byte)MODE.CONFIGURATION))
            {
                if (checkBit(buff[6], (byte)TYPE.AXIS))
                {
                    // Axis configuration, read address and register axis
                    byte key = (byte)buff[7];
                    if (!Axis.ContainsKey(key))
                    {
                        Axis.Add(key, 0);
                        return;
                    }
                }
                else if (checkBit(buff[6], (byte)TYPE.BUTTON))
                {
                    // Button configuration, read address and register button
                    byte key = (byte)buff[7];
                    if (!Button.ContainsKey(key))
                    {
                        Button.Add((byte)buff[7], false);
                        return;
                    }
                }
            }

            // If new button/axis value
            if (checkBit(buff[6], (byte)TYPE.AXIS))
            {
                // Axis value, decode U2 and save to Axis dictionary.
                short value = BitConverter.ToInt16(new byte[2] { buff[4], buff[5] }, 0);
                Axis[(byte)buff[7]] = value;
                return;
            }
            else if (checkBit(buff[6], (byte)TYPE.BUTTON))
            {
                // Bytton value, decode value and save to Button dictionary.
                Button[(byte)buff[7]] = buff[4] == (byte)STATE.PRESSED;
                return;
            }
        }

        /// <summary>
        /// Checks if bits that are set in flag are set in value.
        /// </summary>
        bool checkBit(byte value, byte flag)
        {
            byte c = (byte)(value & flag);
            return c == (byte)flag;
        }
    }

#if !_WINDOWS
    public class JoystickState
    {
        public int X;
        public int Y;
        public int Z;
        public int RotationX;
        public int RotationY;
        public int RotationZ;
        public List<bool> buttons;

        public JoystickState()
        {
            buttons = new List<bool>();
        }
        public List<bool> GetButtons()
        {
            return buttons;
        }
    }
#endif
}
