using System;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;

namespace Rhabot
{
    public partial class ISXFloat : Form
    {
        public ISXFloat()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pause Rhabot
        /// </summary>
        private void cmdPause_Click(object sender, EventArgs e)
        {
            if (clsSettings.Pause)
                this.cmdPause.Text = Resources.Pause;
            else
                this.cmdPause.Text = Resources.Unpause;

            // pause the bot
            clsSettings.Pause = !clsSettings.Pause;
        }

        private delegate void dCloseWindow();
        private void cmdStop_Click(object sender, EventArgs e)
        {
            // raise the event
            clsGlobals.Raise_FloatStopped();

            // close the window
            CloseFloatWindow();
        }

        /// <summary>
        /// Used to close the float window
        /// </summary>
        internal void CloseFloatWindow()
        {
            // close this window
            if (this.InvokeRequired)
                this.Invoke(new dCloseWindow(CloseWindow));
            else
                CloseWindow();
        }
        
        /// <summary>
        /// Used to close the window via an invoke statemen
        /// </summary>
        private void CloseWindow()
        {
            this.Close();
        }

        private void ISXFloat_Load(object sender, EventArgs e)
        {
            // hook bad gupdate event
            clsGlobals.BadGUpdate += new EventHandler(clsGlobals_BadGUpdate);

            // hook log message receive event
            clsEvent.DebugLogItemReceived += clsEvent_DebugLogItemReceived;

            // change caption
            if (clsCharacter.CharacterIsValid)
                this.Text = clsGlobals.SetFormText(string.Format("{0} - {1} Control", clsCharacter.CharacterName, Resources.Rhabot));
        }

        /// <summary>
        /// Raised when a bad guid update is sent/received
        /// </summary>
        void clsGlobals_BadGUpdate(object sender, EventArgs e)
        {
            // first, disable me
            this.Enabled = false;

            // try to shut down
            cmdPause_Click(sender, e);
        }

        void clsEvent_DebugLogItemReceived(string LogMessage)
        {
            this.lblDebug.Text = LogMessage;

            // if stopped, then exit form
            if (clsSettings.Stop)
                cmdStop_Click(null, new EventArgs());
        }
    }
}