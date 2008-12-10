using System;
using System.Windows.Forms;
using System.Threading;
using ISXBotHelper;
using System.IO;
using ISXBotHelper.Properties;

namespace Rhabot
{
    public partial class ISXMain : Form
    {
        private ISXNewPathWiz frmPathWiz;

        public ISXMain()
        {
            InitializeComponent();
        }

        private void ISXMain_Load(object sender, EventArgs e)
        {
            try
            {
                // hook bad gupdate
                clsGlobals.BadGUpdate += clsGlobals_BadGUpdate;

                // set errors to display on screen
                clsSettings.IsUnattended = false;

                // hook the shutdown event (raised on extreme errors or stuck for too long)
                clsEvent.ForcingShutdown += clsEvent_ForcingShutdown;

                // show quick start nag
                clsSettings.ShowQuickStartNag();

                // disable the tabs
                EnableTabs(false);

                // start the caption thread
                if ((clsCharacter.CharacterIsValid) && (! string.IsNullOrEmpty(clsCharacter.CharacterName)))
                    Text = clsGlobals.SetFormText(string.Format("{0} - {1}", ISXBotHelper.Properties.Resources.Rhabot, clsCharacter.CharacterName));

                // hook settings loaded
                clsGlobals.SettingsLoaded += clsGlobals_SettingsLoaded;

                // delete old logs
                new Thread(DeleteOldLogs).Start();

                // set the expire menu
                if (string.IsNullOrEmpty(clsSettings.LoginInfo.ExpireInfo))
                    this.rhabotExpireToolStripMenuItem.Visible = false;
                else
                    this.rhabotExpireToolStripMenuItem.Text = clsSettings.LoginInfo.ExpireInfo;

#if Rhabot
                // show the subscribe menu
                this.mnuSubscribe.Visible = !clsSettings.HasLogin;
#endif
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, ISXBotHelper.Properties.Resources.Rhabot, "Loading");
            }

            finally
            {
                try
                {
                    // disable tabs if not full version
                    if (! clsSettings.IsFullVersion)
                    {
                        this.tabControl1.TabPages.Remove(this.tabTalents);
                        this.tabControl1.TabPages.Remove(this.tabSearch);
                        this.tabControl1.TabPages.Remove(this.tabHealBot);
                        this.tabControl1.TabPages.Remove(this.tabCommunication);
                        this.tabControl1.TabPages.Remove(this.tabChatter);
                        this.tabControl1.TabPages.Remove(this.tabAutoEquip);
                        this.tabControl1.TabPages.Remove(this.tabQuests);

                        // disable menus
                        this.mnuTalents.Enabled = false;
                        this.mnuHealBot.Enabled = false;
                        this.mnuCommunication.Enabled = false;
                        this.mnuChatter.Enabled = false;
                        this.mnuNewIndivPath.Enabled = false;
                        this.mnuAutoEquip.Enabled = false;
                    }
                }

                catch (Exception excep)
                {
                    clsError.ShowError(excep, "Error Removing Full Version Items");
                    clsSettings.Shutdown();
                }
                
            }
        }

        /// <summary>
        /// Raised when a bad guid update is sent/received
        /// </summary>
        void clsGlobals_BadGUpdate(object sender, EventArgs e)
        {
            // disable everything
            this.Enabled = false;
        }

        void clsGlobals_SettingsLoaded(object sender, EventArgs e)
        {
            // enable the tabs
            EnableTabs(true);
        }

        /// <summary>
        /// Enable/Disable the tabs
        /// </summary>
        private void EnableTabs(bool Enable)
        {
            this.uscQuests1.Enabled = Enable;
            this.uscAutoBuff1.Enabled = Enable;
            this.uscChatter1.Enabled = Enable;
            this.uscCombatSettings1.Enabled = Enable;
            this.uscCommunication1.Enabled = Enable;
            this.uscHealBot1.Enabled = Enable;
            this.uscItemMain1.Enabled = Enable;
            this.uscPaths1.Enabled = Enable;
            this.uscSettings1.Enabled = Enable;
            this.uscTalents1.Enabled = Enable;
            this.uscCombatRoutines1.Enabled = Enable;
            this.mnuNewIndivPath.Enabled = Enable;
            this.uscAutoUpgrade1.Enabled = Enable;
            this.uscSharePaths1.Enabled = Enable;
        }

        private void ISXMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // shutdown the bot helper
                clsSettings.Shutdown();
                Application.DoEvents();
                Thread.Sleep(1000);
            }

            catch (Exception excep)
            {
#if DEBUG
                MessageBox.Show(clsError.BuildErrorInfoString(excep, new System.Diagnostics.StackFrame(0, true), "ISXMain Close Error"));
#endif
            }
            
            // force an application exit
            Application.Exit();
        }

        /// <summary>
        /// Raised on extreme errors or stuck too long
        /// </summary>
        void clsEvent_ForcingShutdown()
        {
            // force everything to stop
            clsSettings.Shutdown();

            // wait 5 seconds
            for (int i = 0; i < 5; i++)
            {
                Application.DoEvents();
                Thread.Sleep(1000);
            }

            // close the form
            Application.Exit();
            this.Close();
        }

        /// <summary>
        /// Go to the rhabot website
        /// </summary>
        private void mnuWebsite_Click(object sender, EventArgs e)
        {
            clsGlobals.ExecuteFile(string.Format(@"http://www.{0}.com/default.aspx", Resources.Rhabot));
        }

        private void rhabotDocumentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clsGlobals.ExecuteFile(string.Format(@"http://www.{0}.com/{0}.pdf", Resources.Rhabot));
        }

        private void rhabotQuickStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clsGlobals.ExecuteFile(string.Format(@"http://www.{0}.com/{0}_Quick_Start.pdf", Resources.Rhabot));
        }

        private void rhabotVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clsError.ShowError(new Exception(string.Format("{0} Version: {1}", Resources.Rhabot, clsSettings.BotVersion)), string.Format("{0} Version: {1}", Resources.Rhabot, clsSettings.BotVersion), false, new System.Diagnostics.StackFrame(0, true), false);
        }

        /// <summary>
        /// Delets old log files
        /// </summary>
        private static void DeleteOldLogs()
        {
            try
            {
                // last write date
                DateTime lastTime = DateTime.Now.AddDays(-15);

                // get all log files
                string[] logFiles = Directory.GetFiles(Path.GetDirectoryName(clsSettings.LogFilePath), "*.log", SearchOption.TopDirectoryOnly);

                // exit if no files
                if ((logFiles != null) && (logFiles.Length > 0))
                {
                    // loop through and delete old ones
                    foreach (string log in logFiles)
                    {
                        if (File.GetLastWriteTime(log) < lastTime)
                            File.Delete(log);
                    }
                }

                // delete old temp files
                logFiles = Directory.GetFiles(clsSettings.GetTempFolder(), "*.*", SearchOption.TopDirectoryOnly);
                
                // exit if no files
                if ((logFiles == null) || (logFiles.Length == 0))
                    return;

                // delete temp files
                foreach (string tempFile in logFiles)
                    File.Delete(tempFile);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Delete Old Logs", true, new System.Diagnostics.StackFrame(0, true), false);
            }            
        }

        #region Menus

        private void mnuMain_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabMain;
        }

        private void mnuLog_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabLog;
        }

        private void mnuChatLog_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabChatLog;
        }

        private void mnuSettings_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabSettigns;
        }

        private void mnuCombatSettings_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabCombatSettings;
        }

        private void mnuCombatClass_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabCombatClass;
        }

        private void mnuHealBot_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabHealBot;
        }

        private void mnuAutoBuff_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabAutoBuff;
        }

        private void mnuTalents_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabTalents;
        }

        private void mnuCommunication_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabCommunication;
        }

        private void mnuChatter_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabChatter;
        }

        private void mnuInventory_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabInventory;
        }

        private void mnuPathMng_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPath;
        }

        private void mnuNewIndivPath_Click(object sender, EventArgs e)
        {
            try
            {
                // exit if the form is open
                if ((frmPathWiz != null) && (frmPathWiz.Visible))
                    return;

                // get a new form instances
                frmPathWiz = new ISXNewPathWiz();

                // set save variable
                frmPathWiz.txtPathName.Text = clsGlobals.Path_IndividPath;
                frmPathWiz.txtPathName.Enabled = false;
                frmPathWiz.SaveToNewFile = true;

                // show the form
                frmPathWiz.Show(this);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "MnuNewIndivPath - ShowPathForm");
            }
        }

        private void mnuAutoEquip_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabAutoEquip;
        }

        private void mnuCharStatus_Click(object sender, EventArgs e)
        {
#if Rhabot
            clsGlobals.ExecuteFile("http://char.rhabot.com");
#endif
        }

        private void mnuUploadLastFiveLogs_Click(object sender, EventArgs e)
        {
            // show the log upload form
            ISXBotHelper.Uploads.clsUploadLog.UploadLogs();
        }

        private void mnuSharedProfiles_Click(object sender, EventArgs e)
        {
            clsGlobals.ShowSharedProfiles();
        }

        private void mnuSharedPaths_Click(object sender, EventArgs e)
        {
            clsGlobals.ShowSharedPaths();
        }

        private void mnuSubscribe_Click(object sender, EventArgs e)
        {
#if Rhabot
            clsGlobals.ExecuteFile("http://www.rhabot.com/purchase.aspx");
#endif
        }

        // Menus
        #endregion

        #region ThreadedFuncs

        private delegate void dWindowState();
        internal void WindowStateNormal()
        {
            if (this.InvokeRequired)
                this.Invoke(new dWindowState(p_WindowStateNormal));
            else
                p_WindowStateNormal();
        }
        private void p_WindowStateNormal()
        {
            this.WindowState = FormWindowState.Normal;
        }

        private delegate void dShowForm();
        internal void ShowThisForm()
        {
            if (this.InvokeRequired)
                this.Invoke(new dShowForm(p_ShowThisForm));
            else
                p_ShowThisForm();
        }
        private void p_ShowThisForm()
        {
            this.Show();
        }

        // ThreadedFuncs
        #endregion
    }
}