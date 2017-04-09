using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;

namespace BOT_FrontEnd
{
    //Wrapping class for a DirectX input device (ie. gamepad)
    class Controller
    {
        private DirectInput directInput;

        private Joystick currentDevice;     //current device
        private Guid deviceGuid;            //GUID of the current device
        private JoystickState deviceState;  //joystickstate of the gamepad
        private IntPtr hWnd;                //handle to the parent of this device instance (ie. the main program window)
        private Capabilities devCaps;       //device capabilities 
        private long full_scale;            //size of joystick axes
        public string Name { get; private set; }

        public Controller(IntPtr Owner)
        {
            hWnd = Owner;
            currentDevice = null;
            Name = "NA";
            full_scale = 100;

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
            if (device_guid == deviceGuid) { return; }
            if (currentDevice != null) 
            {
                currentDevice.Unacquire();
                currentDevice.Dispose(); 
            }

            currentDevice = new Joystick(directInput, device_guid);
            Name = currentDevice.Properties.InstanceName;
            deviceGuid = device_guid;

            currentDevice.SetCooperativeLevel(hWnd, CooperativeLevel.Background | CooperativeLevel.Nonexclusive);
            currentDevice.Acquire();

            devCaps = currentDevice.Capabilities;
            Poll();
            full_scale = 2 * deviceState.X;
        }

        /********************************************************************************
         * FUNCTION:    Poll
         * Description: Acquires current device input into the instance class
         * Parameters:  N/A
         ********************************************************************************/
        public void Poll()
        {
            currentDevice.Poll();
            deviceState = currentDevice.GetCurrentState();
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
    }
}
