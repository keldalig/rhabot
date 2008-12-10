using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rhabot.Threaded_Controls
{
    public partial class ThreadLabel : Label
    {
        private delegate void dText();
        private string newText = string.Empty;

        public ThreadLabel()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                newText = value;
                if (base.InvokeRequired)
                    base.Invoke(new dText(UpdateText));
                else
                    UpdateText();
            }
        }

        private void UpdateText()
        {
            base.Text = newText;
        }
    }
}
