using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BOT_FrontEnd
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SplashScreen splash = new SplashScreen();
            splash.Show();
            Application.DoEvents();
            Application.Run(new Form1(ref splash));
            splash.Close();
        }
    }
}
