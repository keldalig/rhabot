using System;
using System.Text;
using System.Windows.Forms;

namespace Rhabot
{
    public partial class uscLog : UserControl
    {
        readonly StringBuilder sb = new StringBuilder();
        private delegate void LogUpdate();

        public uscLog()
        {
            InitializeComponent();

            // hook the log events
            ISXBotHelper.clsEvent.LogItemReceived += clsEvent_LogItemReceived;
        }

        void clsEvent_LogItemReceived(string LogMessage)
        {
            string LogLineBreak = "\r\n----------\r\n";

            // add the message to the stringbuilder
            sb.AppendFormat("{0}{1}", LogMessage, LogLineBreak);

            // update the log window
            if (this.txtLog.InvokeRequired)
                this.txtLog.Invoke(new LogUpdate(UpdateLogWindow));
            else
                UpdateLogWindow();
        }

        private void UpdateLogWindow()
        {
            // update the text with the stringbuilder
            this.txtLog.Text = sb.ToString();

            // scroll
            this.txtLog.Select(sb.Length - 1, 0);
            this.txtLog.ScrollToCaret();

            // resize the stringbuilder
            if (sb.Length > 1000000)
                sb.Remove(0, 500000);
        }

        /// <summary>
        /// Change verbose logging
        /// </summary>
        private void chkVerbose_CheckedChanged(object sender, EventArgs e)
        {
            ISXBotHelper.clsSettings.VerboseLogging = this.chkVerbose.Checked;
        }
    }
}
