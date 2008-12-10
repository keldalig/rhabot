using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ISXBotHelper
{
    public partial class ISXLog : Form
    {
        public ISXLog()
        {
            InitializeComponent();
        }

        private void ISXLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}