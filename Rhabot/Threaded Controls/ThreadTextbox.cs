using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rhabot.Threaded_Controls
{
    public partial class ThreadTextbox : TextBox
    {
        private delegate void dText();
        private string newText = string.Empty;

        public ThreadTextbox()
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
            base.Text = (string)newText.Clone();
            newText = string.Empty;
        }

        /// <summary>
        /// Updates text and scrolls to end of box
        /// </summary>
        /// <param name="text">the text to set</param>
        public void TextAndScroll(string text)
        {
            newText = text;
            if (base.InvokeRequired)
                base.Invoke(new dText(TandS));
            else
                TandS();
        }

        public void InsertAndScroll(string TextToInsert)
        {
            newText = TextToInsert;
            if (base.InvokeRequired)
                base.Invoke(new dText(InsertScroll));
            else
                InsertScroll();
        }

        /// <summary>
        /// update text and scroll, invoked
        /// </summary>
        private void TandS()
        {
            // set text
            base.Text = (string)newText.Clone();

            // scroll
            if (base.Text.Length > 0)
            {
                base.Select(newText.Length - 1, 0);
                base.ScrollToCaret();
            }

            // clear param
            newText = string.Empty;
        }

        private void InsertScroll()
        {
            if (base.Text.Length > 0)
                base.Text = base.Text.Insert(base.Text.Length - 1, newText);
            else
                base.Text = (string)newText.Clone();

            // scroll
            if (base.Text.Length > 0)
            {
                base.Select(newText.Length - 1, 0);
                base.ScrollToCaret();
            }

            // clear param
            newText = string.Empty;
        }
    }
}
