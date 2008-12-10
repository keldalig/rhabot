using System;
using System.IO;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;
using ISXBotHelper.Settings.Settings;

namespace Rhabot
{
    public partial class uscSettings : UserControl
    {
        public uscSettings()
        {
            InitializeComponent();
        }

        private void uscSettings_Load(object sender, EventArgs e)
        {
            // exit if in designer
            if (DesignMode)
                return;

            LoadSettings();

            // hook the settings load event
            clsGlobals.SettingsLoaded += clsGlobals_SettingsLoaded;

            // hook the stop event
            clsEvent.ScriptStop += clsEvent_ScriptStop;

            // load last chatter file
            this.txtChatterPath.Text = clsGlobals.LastChatterFile;

            // disable non-full items
            this.chkHumanCheck.Enabled = clsSettings.IsFullVersion;
            this.grpChatter.Enabled = clsSettings.IsFullVersion;
#if Rhabot
            if ((clsSettings.IsFullVersion) && (clsSettings.HasLogin))
                this.chkCharStatus.Visible = true;
#endif
        }

        /// <summary>
        /// Raised when the script is stopped
        /// </summary>
        void clsEvent_ScriptStop()
        {
            this.chkHumanCheck.Checked = false;
        }

        void clsGlobals_SettingsLoaded(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            // load settings
            this.txtDurability.Value = clsGlobals.SetNumericValue(clsSettings.gclsLevelSettings.DurabilityPercent, this.txtDurability);
            this.txtLowAttack.Value = clsGlobals.SetNumericValue(clsSettings.gclsLevelSettings.LowLevelAttack, this.txtLowAttack);
            this.txtHighAttack.Value = clsGlobals.SetNumericValue(clsSettings.gclsLevelSettings.HighLevelAttack, this.txtHighAttack);
            this.txtSearchRange.Value = clsGlobals.SetNumericValue(clsSettings.gclsLevelSettings.SearchRange, this.txtSearchRange);
            this.txtTargetRange.Value = clsGlobals.SetNumericValue(clsSettings.gclsLevelSettings.TargetRange, this.txtTargetRange);
            this.txtStuckTimeout.Value = clsGlobals.SetNumericValue(clsSettings.gclsGlobalSettings.StuckTimeout, this.txtStuckTimeout);
            this.chkSkinner.Checked = clsSettings.gclsLevelSettings.IsSkinner;
            this.chkMiner.Checked = clsSettings.gclsLevelSettings.IsMiner;
            this.chkFlower.Checked = clsSettings.gclsLevelSettings.IsFlowerPicker;
            this.chkRogue.Checked = clsSettings.gclsLevelSettings.IsRogue;
            this.chkSearchChest.Checked = clsSettings.gclsLevelSettings.Search_Chest;
            this.chkTargetElites.Checked = clsSettings.gclsLevelSettings.TargetElites;
            this.chkLogoutStuck.Checked = clsSettings.gclsGlobalSettings.LogoutOnStuck;
            this.chkDeclineGuild.Checked = clsSettings.gclsGlobalSettings.DeclineGuildInvite;
            this.chkDeclineGroup.Checked = clsSettings.gclsGlobalSettings.DeclineGroupInvite;
            this.chkDeclineDuel.Checked = clsSettings.gclsGlobalSettings.DeclineDuelInvite;
            
            // non event items
            this.chkUnattended.Checked = clsSettings.IsUnattended;

            if (clsGlobals.SettingsLevel > 0)
            {
                this.txtStartLevel.Value = clsGlobals.SettingsLevel;
                this.txtEndLevel.Value = clsGlobals.SettingsLevel;
            }
        }

        #region Variables

        private int LastLogoutTime = 0;

        // Variables
        #endregion

        #region Click Events

        private void chkUnattended_CheckedChanged(object sender, EventArgs e)
        {
            // set unattended state
            clsSettings.IsUnattended = this.chkUnattended.Checked;
        }

        /// <summary>
        /// Turn on/off chatter
        /// </summary>
        private void chkChatter_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkChatter.Checked)
                {
                    // warn if we don't have a script
                    if ((string.IsNullOrEmpty(this.txtChatterPath.Text)) || (!File.Exists(this.txtChatterPath.Text)))
                    {
                        MessageBox.Show(Resources.enteravalidpathfortheChatterAIScript);
                        this.chkChatter.Checked = false;
                        return;
                    }

                    // disable the textboxes
                    this.txtChatterPath.Enabled = false;
                    this.cmdBrowseChatter.Enabled = false;

                    // start chatter
                    clsSettings.chatter.ChatterKBFile = this.txtChatterPath.Text;
                    clsSettings.chatter.StartChatter();
                }
                else
                {
                    // enable text
                    this.txtChatterPath.Enabled = true;
                    this.cmdBrowseChatter.Enabled = true;

                    // shutdown chatter
                    clsSettings.chatter.DoShutdown();
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.StartStopChatter);
            }
        }

        /// <summary>
        /// Browse for the Chatter file
        /// </summary>
        private void cmdBrowseChatter_Click(object sender, EventArgs e)
        {
            try
            {
                // set up the open dialog
                this.openFileDialog1.Title = Resources.BrowseforChatterAIScript;
                this.openFileDialog1.Filter = Resources.ChatterScriptExt;
                this.openFileDialog1.FilterIndex = 1;
                this.openFileDialog1.CheckFileExists = true;
                this.openFileDialog1.DereferenceLinks = true;
                
                // show the dialog
                if (this.openFileDialog1.ShowDialog() != DialogResult.OK)
                    return;

                // set the path
                this.txtChatterPath.Text = this.openFileDialog1.FileName;
                clsGlobals.LastChatterFile = this.txtChatterPath.Text;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.BrowseforChatterScript);
            }            
        }

        /// <summary>
        /// Turn on/off human check
        /// </summary>
        private void chkHumanCheck_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                // start or stop human check
                if (this.chkHumanCheck.Checked)
                    clsHumanCheck.Start();
                else
                    clsHumanCheck.DoShutdown();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.StartStopHumanCheck);
            }            
        }

        /// <summary>
        /// Applies all the settings and saves the settings file
        /// </summary>
        private void cmdApplySettings_Click(object sender, EventArgs e)
        {
            try
            {
                // if we have no level, then exit
                if (clsGlobals.SettingsLevel < 1)
                {
                    MessageBox.Show(Resources.Pleaseselectthesettingslevel);
                    return;
                }

                // set wait cursor
                this.Cursor = Cursors.WaitCursor;

                // save the settings files
                int j = (int)this.txtEndLevel.Value + 1;
                for (int i = (int)this.txtStartLevel.Value; i < j; i++)
                {
                    // load level settings
                    clsLevelSettings LevelSettings = clsSettings.LoadPanelSettings(i);

                    // pop the settings information
                    LevelSettings.DurabilityPercent = (int)this.txtDurability.Value;
                    LevelSettings.LowLevelAttack = (int)this.txtLowAttack.Value;
                    LevelSettings.HighLevelAttack = (int)this.txtHighAttack.Value;
                    LevelSettings.SearchRange = (int)this.txtSearchRange.Value;
                    LevelSettings.TargetRange = (int)this.txtTargetRange.Value;
                    LevelSettings.IsSkinner = this.chkSkinner.Checked;
                    LevelSettings.IsMiner = this.chkMiner.Checked;
                    LevelSettings.IsFlowerPicker = this.chkFlower.Checked;
                    LevelSettings.IsRogue = this.chkRogue.Checked;
                    LevelSettings.Search_Chest = this.chkSearchChest.Checked;
                    LevelSettings.TargetElites = this.chkTargetElites.Checked;

                    // save panel settings
                    clsSettings.SavePanelSettings(i, LevelSettings);
                }

                // save the global settings
                clsSettings.gclsGlobalSettings.StuckTimeout = (int)this.txtStuckTimeout.Value;
                clsSettings.gclsGlobalSettings.LogoutOnStuck = this.chkLogoutStuck.Checked;
                clsSettings.gclsGlobalSettings.DeclineGuildInvite = this.chkDeclineGuild.Checked;
                clsSettings.gclsGlobalSettings.DeclineGroupInvite = this.chkDeclineGroup.Checked;
                clsSettings.gclsGlobalSettings.DeclineDuelInvite = this.chkDeclineDuel.Checked;
                clsSettings.SaveGlobalSettings();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Apply Rhabot Settings");
            }            

            finally
            {
                // set cursor
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// User changed logout time
        /// </summary>
        private void txtLogoutTime_Leave(object sender, EventArgs e)
        {
            // skip if logout time not changed
            if (LastLogoutTime == (int)this.txtLogoutTime.Value)
                return;

            try
            {
                // set the last logout flag
                LastLogoutTime = (int)this.txtLogoutTime.Value;

                // reset if time set to 0
                if (LastLogoutTime == 0)
                {
                    clsSettings.LogoutAtTime(DateTime.MinValue);
                    clsSettings.Logging.AddToLog(Resources.Logoutattimereset);
                    this.lblLogoutTime.Text = string.Empty;
                    return;
                }

                // update logout time
                DateTime logTime = DateTime.Now.AddMinutes(LastLogoutTime);

                // log it
                clsSettings.Logging.AddToLogFormatted(Resources.LogouttimesettoX, logTime.ToString());

                // update label
                this.lblLogoutTime.Text = logTime.ToString();

                // update logout time
                clsSettings.LogoutAtTime(logTime);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Change logout time");
            }            
        }

        /// <summary>
        /// Disable / Enable Rendering
        /// </summary>
        private void chkDisableRendering_CheckedChanged(object sender, EventArgs e)
        {
            using (new clsFrameLock.LockBuffer())
            {
                if (this.chkDisableRendering.Checked)
                    clsSettings.isxwow.DisableRender();
                else
                    clsSettings.isxwow.EnableRender();
            }
        }

        /// <summary>
        /// Disable / Enable Background Rendering
        /// </summary>
        private void chkDisableBkgRender_CheckedChanged(object sender, EventArgs e)
        {
            using (new clsFrameLock.LockBuffer())
            {
                if (this.chkDisableBkgRender.Checked)
                    clsSettings.isxwow.DisableBackgroundRender();
                else
                    clsSettings.isxwow.EnableBackgroundRender();
            }
        }

        private void chkCharStatus_CheckedChanged(object sender, EventArgs e)
        {
#if Rhabot
            // start character update
            if ((clsSettings.IsFullVersion) && (clsSettings.HasLogin))
            {
                if ((this.chkCharStatus.Checked) && (!clsSettings.characterStatusMain.IsRunning))
                {
                    // start running
                    clsSettings.Logging.AddToLog(Resources.Settings, "Starting Character Status Updater");
                    clsSettings.characterStatusMain.Start();
                }
                else
                {
                    // stop running
                    clsSettings.characterStatusMain.Stop();
                }
            }
#endif
        }

        /// <summary>
        /// Send message on toon death
        /// </summary>
        private void chkSendOnDead_CheckedChanged(object sender, EventArgs e)
        {
            clsSettings.SendMsgOnDead = this.chkSendOnDead.Checked;
        }

        // Click Events
        #endregion
    }
}
