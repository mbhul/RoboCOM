using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using X11;

namespace BOT_FrontEnd
{
    static class Helpers
    {
        public static ControllerProperty[] DefaultGamepad = { ControllerProperty.Y, ControllerProperty.Z, ControllerProperty.RotationZ,
                                                              ControllerProperty.RotationX, ControllerProperty.RotationY};

        public static ControllerProperty[] DefaultJoystick = { ControllerProperty.Z, ControllerProperty.X, ControllerProperty.Y,
                                                              ControllerProperty.Button_0, ControllerProperty.Button_1};

        //****************************************************************************
        //See: https://stackoverflow.com/questions/42449050/cant-get-a-window-handle
        //****************************************************************************
        private static IntPtr[] FindChildWindows(IntPtr display, IntPtr window,
                                                 string title, ref List<IntPtr> windows)
        {
            IntPtr rootWindow;
            IntPtr parentWindow;

            IntPtr[] childWindows = new IntPtr[0];

            int childWindowsLength;

            X11lib.XQueryTree(display, window,
                                  out rootWindow, out parentWindow,
                                  out childWindows, out childWindowsLength);

            childWindows = new IntPtr[childWindowsLength];

            X11lib.XQueryTree(display, window,
                                  out rootWindow, out parentWindow,
                                  out childWindows, out childWindowsLength);

            string windowFetchedTitle;

            X11lib.XFetchName(display, window, out windowFetchedTitle);

            if (title == windowFetchedTitle &&
               !windows.Contains(window))
            {
                windows.Add(window);
            }

            for (int childWindowsIndexer = 0;
                childWindowsIndexer < childWindows.Length;
                childWindowsIndexer++)
            {
                IntPtr childWindow = childWindows[childWindowsIndexer];

                string childWindowFetchedTitle;

                X11lib.XFetchName(display, childWindow,
                                      out childWindowFetchedTitle);

                if (title == childWindowFetchedTitle &&
                   !windows.Contains(childWindow))
                {
                    windows.Add(childWindow);
                }

                FindChildWindows(display, childWindow, title, ref windows);
            }

            windows.TrimExcess();

            return windows.ToArray();
        }

        public static IntPtr[] FindWindows(IntPtr display, string title)
        {
            List<IntPtr> windows = new List<IntPtr>();

            return FindChildWindows(display,
                                    X11lib.XDefaultRootWindow(display),
                                    title,
                                    ref windows);
        }
    }

    public enum ControllerProperty
    {
        [StringValue("Button0")]
        Button_0 = 0,
        [StringValue("Button1")]
        Button_1 = 1,
        [StringValue("Button2")]
        Button_2 = 2,
        [StringValue("Button3")]
        Button_3 = 3,
        [StringValue("Button4")]
        Button_4 = 4,
        NUM_BUTTONS = 5,
        [StringValue("X")]
        X = 6,
        [StringValue("Y")]
        Y = 7,
        [StringValue("Z")]
        Z = 8,
        [StringValue("RotationX")]
        RotationX = 9,
        [StringValue("RotationY")]
        RotationY = 10,
        [StringValue("RotationZ")]
        RotationZ = 11
    }

    public enum ChannelNumber
    {
        [StringValue("CH1")]
        CH1 = 0,
        [StringValue("CH2")]
        CH2 = 1,
        [StringValue("CH3")]
        CH3 = 2,
        [StringValue("CH4")]
        CH4 = 3,
        [StringValue("CH5")]
        CH5 = 4,
        NUM_CHANNELS = 5
    }

    public class StringValue : System.Attribute
    {
        private string _value;

        public StringValue(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }

    public static class StringEnum
    {
        public static string GetStringValue(Enum value)
        {
            string output = null;
            Type type = value.GetType();

            FieldInfo fi = type.GetField(value.ToString());
            StringValue[] attrs =
               fi.GetCustomAttributes(typeof(StringValue),
                                       false) as StringValue[];
            if (attrs.Length > 0)
            {
                output = attrs[0].Value;
            }

            return output;
        }
    }

    public class ChannelConfigValues
    {
        public double MIN;
        public double MAX;
        public double CENTER;

        public ChannelConfigValues()
        {
            MIN = 0.0;
            MAX = 0.0;
            CENTER = 0.0;
        }
    }
}
