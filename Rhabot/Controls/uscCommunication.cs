using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;

namespace Rhabot
{
    public partial class uscCommunication : UserControl
    {
        public uscCommunication()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Start/Stop MSN chat
        /// </summary>
        private void chkUseMSN_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                // stop chat if unchecked
                if (! this.chkUseMSN.Checked)
                {
                    clsSettings.StopMSN();
                    return;
                }

                // try to start chat

                // make sure we have username/password
                if ((string.IsNullOrEmpty(this.txtMSNPassword.Text.Trim())) || (string.IsNullOrEmpty(this.txtMSNUsername.Text.Trim())))
                {
                    // show an error, uncheck, then exit
                    clsError.ShowError(new Exception(Resources.MSNUsernameInvalid), Resources.PleaseEnterCorrectMSNUname, true, new StackFrame(0, true), false);
                    this.chkUseMSN.Checked = false;
                    return;
                }

                // start chat
                clsSettings.Logging.AddToLog(Resources.StartingMSNchat);
                clsSettings.StartMSN();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.StartStopMSN);
            }
        }

        private void uscCommunication_Load(object sender, EventArgs e)
        {
            // exit if in designer
            if (DesignMode)
                return;

            // hook settings loaded event
            clsGlobals.SettingsLoaded += clsGlobals_SettingsLoaded;
            LoadSettings();

            // disable for free version
            this.chkUseMSN.Enabled = clsSettings.IsFullVersion;
        }

        void clsGlobals_SettingsLoaded(object sender, EventArgs e)
        {
            LoadSettings();
        }

        /// <summary>
        /// Load the settings
        /// </summary>
        private void LoadSettings()
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                // pop the fields
                this.txtEmailPassword.Text = clsSettings.DecryptString(clsSettings.gclsGlobalSettings.Comm_Email_Password);
                this.txtEmailServer.Text = clsSettings.gclsGlobalSettings.Comm_Email_SMTPServer;
                this.txtEmailServerPort.Value = clsSettings.gclsGlobalSettings.Comm_Email_SMTPPort;
                this.txtEmailUsername.Text = clsSettings.gclsGlobalSettings.Comm_Email_Usename;
                this.txtMSNPassword.Text = clsSettings.DecryptString(clsSettings.gclsGlobalSettings.MSN_Password);
                this.txtMSNUsername.Text = clsSettings.gclsGlobalSettings.MSN_Username;

                // pop email addresses
                foreach (string email in clsSettings.gclsGlobalSettings.Comm_Email_List)
                    sb.AppendFormat("{0}\r\n", email);
                this.txtEmailList.Text = sb.ToString().Trim();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.LoadCommunicationSettings);
            }            
        }

        /// <summary>
        /// Save MSN / Email settings
        /// </summary>
        private void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                // set wait cursor
                this.Cursor = Cursors.WaitCursor;

                // save settings
                clsSettings.gclsGlobalSettings.Comm_Email_Password = clsSettings.EncryptString(this.txtEmailPassword.Text);
                clsSettings.gclsGlobalSettings.Comm_Email_SMTPServer = this.txtEmailServer.Text;
                clsSettings.gclsGlobalSettings.Comm_Email_SMTPPort = (int)this.txtEmailServerPort.Value;
                clsSettings.gclsGlobalSettings.Comm_Email_Usename = this.txtEmailUsername.Text;
                clsSettings.gclsGlobalSettings.MSN_Password = clsSettings.EncryptString(this.txtMSNPassword.Text);
                clsSettings.gclsGlobalSettings.MSN_Username = this.txtMSNUsername.Text;

                // add the emails to the list
                clsSettings.gclsGlobalSettings.Comm_Email_List.Clear();
                clsSettings.gclsGlobalSettings.Comm_Email_List.AddRange(this.txtEmailList.Lines);

                // save
                clsSettings.SaveGlobalSettings();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.SaveCommunicationSettings);
            }            
            finally
            {
                // set cursor
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// send an email test
        /// </summary>
        private void cmdTest_Click(object sender, EventArgs e)
        {
            ISXBotHelper.Communication.clsSend.SendToAll_Email(
                string.Format("{0} communications test was successful", Resources.Rhabot),
                string.Format("Message from {0}", Resources.Rhabot));
        }

    }
}
