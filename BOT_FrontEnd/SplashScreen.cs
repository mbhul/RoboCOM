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
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
            this.label1.Parent = this.pictureBox1;
            this.label1.BackColor = Color.Transparent;
        }
    }
}
