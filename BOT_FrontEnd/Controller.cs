using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;

namespace BOT_FrontEnd
{
    //Wrapping class for a DirectX input device (ie. gamepad)
    public class Controller
    {
        private DirectInput directInput;

        private Joystick currentDevice;     //current device
        private JoystickState deviceState;  //joystickstate of the gamepad
        private IntPtr hWnd;                //handle to the parent of this device instance (ie. the main program window)
        private Capabilities devCaps;       //device capabilities 
        private long full_scale;            //size of joystick axes
        public string Name { get; private set; }
        public Guid DeviceGuid { get; private set; }

        private int[] CHANNEL;
        public ControllerProperty[] ChannelMapping { get; private set; }
        public bool[] ChannelInverted { get; private set; }

        public Controller(IntPtr Owner)
        {
            hWnd = Owner;
            currentDevice = null;
            Name = "NA";
            full_scale = 100;
            ChannelMapping = null;

            CHANNEL = new int[(int)ChannelNumber.NUM_CHANNELS];

            directInput = new DirectInput();
        }

        /********************************************************************************
         * FUNCTION:            SetCurrent
         * Description:         Sets the current input device instance based on passed GUID
         * Parameters:  
         *      device_guid -   The GUID of the input device
         ********************************************************************************/
        public void SetCurrent(Guid device_guid)
        {
            DeviceInstance di;

            if (device_guid == DeviceGuid) { return; }
            if (currentDevice != null) 
            {
                currentDevice.Unacquire();
                currentDevice.Dispose(); 
            }

            currentDevice = new Joystick(directInput, device_guid);
            Name = currentDevice.Properties.InstanceName;
            DeviceGuid = device_guid;

            currentDevice.SetCooperativeLevel(hWnd, CooperativeLevel.Background | CooperativeLevel.Nonexclusive);
            currentDevice.Acquire();

            devCaps = currentDevice.Capabilities;

            this.Poll();
            full_scale = 2 * deviceState.X;

            di = getCurrentDeviceInstance();
            if (di.Type == DeviceType.Joystick)
            {
                ChannelMapping = Helpers.DefaultJoystick;
            }
            else
            {
                ChannelMapping = Helpers.DefaultGamepad;
            }
            ChannelInverted = Enumerable.Repeat<bool>(false, (int)ChannelNumber.NUM_CHANNELS).ToArray();
        }

        /********************************************************************************
         * FUNCTION:    Poll
         * Description: Acquires current device input into the instance class
         * Parameters:  N/A
         ********************************************************************************/
        public void Poll()
        {
            if(currentDevice != null)
            {
                currentDevice.Poll();
                deviceState = currentDevice.GetCurrentState();

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

        public DeviceInstance getCurrentDeviceInstance()
        {
            DeviceInstance curDevice = null;
            IList<DeviceInstance> ControllerList = directInput.GetDevices();

            foreach(DeviceInstance di in ControllerList)
            {
                if(di.InstanceGuid == this.DeviceGuid)
                {
                    curDevice = di;
                    break;
                }
            }

            return curDevice;
        }

        public void SetChannelMapping(ControllerProperty[] map, bool[] inversion)
        {
            ChannelMapping = map;
            ChannelInverted = inversion;
        }
        
    }
}
