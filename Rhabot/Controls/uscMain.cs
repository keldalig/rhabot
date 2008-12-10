using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;
using Rhabot.BotThreads;
using rs;
using ISXRhabotGlobal;
using System.Linq;
using ISXBotHelper.Settings.Settings;

namespace Rhabot
{
    public partial class uscMain : UserControl
    {
        #region Variables

        /// <summary>
        /// The type of bot to run
        /// </summary>
        private enum eBotTYpe
        {
            None = 0,
            WithPaths,
            CombatOnly,
            HealBot,
            AutoNavCombat,
            SushiBot,
            IndividualPath
        }

        // bot threads
        private readonly clsRhabot MyRhabot = new clsRhabot();
        private readonly clsCombatOnly CombatOnly = new clsCombatOnly();
        private readonly clsHealBot HealBot = new clsHealBot();
        private readonly clsSushiBot SushiBot = new clsSushiBot();
        private readonly clsIndividPath IndividPath = new clsIndividPath();
        private readonly clsAutoNavBot AutoNavBot = new clsAutoNavBot();

        private ISXFloat frmFloat = null;
        private eBotTYpe BotType = eBotTYpe.None;

        // Variables
        #endregion

        #region Load

        public uscMain()
        {
            InitializeComponent();
        }

        private void uscMain_Load(object sender, EventArgs e)
        {
            // exit if in designer
            if (DesignMode)
                return;

            // load the last used settings file if we have it
            if (!clsGlobals.LastSettingsFolder.Contains(":\\"))
                this.cmbSettingsName.Text = clsGlobals.LastSettingsFolder;

            // set default level
            int currLevel = clsCharacter.CurrentLevel;
            if (currLevel < 1)
                this.txtLevelToLoad.Value = 1;
            else
                this.txtLevelToLoad.Value = currLevel;

            clsGlobals.SettingsLevel = (int)this.txtLevelToLoad.Value;

#if Rhabot
            // display ads
            if (!clsSettings.IsFullVersion) //&& (!string.IsNullOrEmpty(ISXBotHelper.Settings.clsLogin.MerlinAd)))
            {
                // hook ad event and open ad in new window
                clsEvent.AdsReceived += clsEvent_AdsReceived;
                clsGlobals.ExecuteFile("http://www.rhabot.com/rhabotad.aspx");
            }
#endif

            // pop the bot types combo
            this.cmbBotType.Items.Insert(0, Resources.BotType_RhabotStandard);

            // load the other bots in the list if this is a full version of Rhabot
            if (clsSettings.IsFullVersion)
            {
                this.cmbBotType.Items.Insert(1, Resources.BotType_RhabotCombatOnly);
                this.cmbBotType.Items.Insert(2, Resources.BotType_HealBot);
                this.cmbBotType.Items.Insert(3, Resources.BotType_SushiBot);
                this.cmbBotType.Items.Insert(4, Resources.BotType_IndividPath);

#if Rhabot
                this.cmbBotType.Items.Insert(5, "AutoNav Combat Bot - US Clients - enEN");
#endif
            }

            // pop the group path list
            PopGroupPathList();

            // pop the general path list
            PopGeneralPathList();

            this.cmbBotType.SelectedIndex = 0;

            // disable buttons
            Disable_Stop();
            Disable_Start();

            // start the main info thread
            this.uscMainInfo1.Start();

            // pop the settings list
            PopSettingsList();

            // set the mpq location
            this.txtMPQLocation.Text = clsSettings.MPQPath;

            // start the timer
            this.timer1.Start();
        }

        void clsEvent_AdsReceived(string NewAd)
        {
            clsGlobals.ExecuteFile("http://www.rhabot.com/rhabotad.aspx");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // stop the timer
            this.timer1.Stop();
        }

        // Load
        #endregion

        #region Settings Files

        /// <summary>
        /// Refresh the settings list
        /// </summary>
        private void PopSettingsList()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // add the settings to the combo
                this.cmbSettingsName.Items.Clear();
                List<rs.PathService.clsGroupName> SettingsList = new clsRS().GetUserSettingsList(clsSettings.LoginInfo.UserID, clsSettings.UpdateText, clsSettings.IsDCd);

                foreach (rs.PathService.clsGroupName GroupItem in SettingsList)
                {
                    // skip if NONE
                    if (GroupItem.GroupName.ToLower() == "none")
                        continue;

                    // add this item to the dropdown
                    this.cmbSettingsName.Items.Add(new clsPathGroupItem()
                    {
                        GroupName = GroupItem.GroupName,
                        GroupNote = clsSerialize.Deserialize_FromByteA(GroupItem.GroupNote, typeof(string)) as string
                    });
                }

                // pop the default settings list
                clsCreateProfile.CreateDefaultProfiles();

                // add default settings to the list
                foreach (clsGlobalSettings gs in clsSettings.DefaultSettingsList.Keys)
                {
                    this.cmbSettingsName.Items.Add(new clsPathGroupItem()
                    {
                        GroupName = gs.MSN_Username,
                        GroupNote = ""
                    });
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscMain);
            }

            finally
            {
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Create the settings folder
        /// </summary>
        private void cmdCreate_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(Resources.CreateSettingsInstruct);

                this.cmbSettingsName.DropDownStyle = ComboBoxStyle.DropDown;
                this.cmbSettingsName.SelectedIndex = -1;
                this.cmbSettingsName.Text = string.Empty;
                this.cmbSettingsName.Focus();

                // change load button's text
                this.cmdLoadSettings.Text = "Save Profile";
                this.cmdLoadSettings.ForeColor = System.Drawing.Color.Red;
                this.cmdLoadSettings.Font = new System.Drawing.Font(this.cmdLoadSettings.Font, System.Drawing.FontStyle.Bold);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscMain, Resources.UnableToCreateSettingsFolder);
            }
        }

        /// <summary>
        /// Load the settings file
        /// </summary>
        private void cmdLoadSettings_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.cmbSettingsName.DropDownStyle != ComboBoxStyle.DropDown)
                {
                    string SettingsFolder = this.cmbSettingsName.Text;

                    // make sure we have a folder
                    if (string.IsNullOrEmpty(SettingsFolder))
                    {
                        MessageBox.Show(Resources.PleaseEnterSettingsFolder);
                        return;
                    }

                    Cursor = Cursors.WaitCursor;

                    // load the settings
                    clsSettings.CurrentSettingsName = SettingsFolder;
                    clsGlobals.LastSettingsFolder = this.cmbSettingsName.Text;
                    clsSettings.LoadSettings((int)this.txtLevelToLoad.Value);
                    clsSettings.LoadGlobalSettings();

                    // pop the group path list
                    PopGroupPathList();

                    // enable start buttons
                    Enable_Start();
                    this.grpProfileNotes.Enabled = true;
                    this.cmdSharedPaths.Enabled = true;
                    this.cmdSharedProfiles.Enabled = true;
                    ClearPathNotes();

                    // raise settings loaded event
                    clsGlobals.Raise_SettingsLoaded();

                    // notify of load
                    MessageBox.Show(Resources.SettingsFileLoaded);
                }
                else
                {
                    // create the settings folder name
                    if (string.IsNullOrEmpty(this.cmbSettingsName.Text))
                    {
                        MessageBox.Show("Please enter a settings name");
                        return;
                    }
                    clsSettings.CurrentSettingsName = this.cmbSettingsName.Text;
                    clsSettings.LastSettingsFolder = this.cmbSettingsName.Text;
                    clsSettings.gclsGlobalSettings = new ISXBotHelper.Settings.Settings.clsGlobalSettings();
                    clsSettings.gclsLevelSettings = new ISXBotHelper.Settings.Settings.clsLevelSettings();
                    clsSettings.SaveGlobalSettings();
                    this.cmbSettingsName.DropDownStyle = ComboBoxStyle.DropDownList;
                    this.cmbSettingsName.Items.Add(new clsPathGroupItem() { GroupName = this.cmbSettingsName.Text });
                    this.cmbSettingsName.Refresh();
                    this.cmbSettingsName.Text = string.Empty;

                    // reset load button text
                    this.cmdLoadSettings.Text = "Load Profile";
                    this.cmdLoadSettings.ForeColor = System.Drawing.Color.Black;
                    this.cmdLoadSettings.Font = new System.Drawing.Font(this.cmdLoadSettings.Font, System.Drawing.FontStyle.Regular);

                    PopSettingsList();
                    MessageBox.Show("Settings Saved");
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscMain, Resources.FailedToLoadSettings);
            }            

            finally
            {
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Refresh the settings list
        /// </summary>
        private void cmdRefresh_Click(object sender, EventArgs e)
        {
            PopSettingsList();
        }

        // Settings Files
        #endregion

        #region Bot Control Button Clicks

        /// <summary>
        /// Raised when the user changes the path group
        /// </summary>
        private void cmbPathGroupName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // exit if nothing selected
            int index = this.cmbPathGroupName.SelectedIndex;
            if (index < 0)
            {
                ClearPathNotes();
                return;
            }

            // pop path note
            clsPathGroupItem item = this.cmbPathGroupName.SelectedItem as clsPathGroupItem;
            this.txtPathNotes.Text = ((item != null) && (!string.IsNullOrEmpty(item.GroupNote))) ? item.GroupNote : "";
            this.txtPathNotes.Enabled = true;
            this.cmdSavePathNote.Enabled = true;
        }

        private void cmdStartBot_Click(object sender, EventArgs e)
        {
            // exit if nothing selected
            int index = this.cmbBotType.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show(Resources.PleaseSelectBoType);
                return;
            }

            // exit if no group path selected
            if ((index == 0) && (string.IsNullOrEmpty(this.cmbPathGroupName.Text)))
            {
                MessageBox.Show(Resources.PleaseSelectGroupPathName);
                return;
            }

            // reload settings
            clsSettings.LoadSettings(clsCharacter.CurrentLevel);

            // raise settings loaded event
            clsGlobals.Raise_SettingsLoaded();

            clsSettings.Stop = false;
            clsSettings.Pause = false;

            // disable buttons
            Disable_Start();
            Enable_Stop();

            // start the bot
            switch (index)
            {
                case 0: // standard
                    BotType = eBotTYpe.WithPaths;
                    MyRhabot.Start(this.cmbPathGroupName.Text);
                    break;
                
                case 1: // combat only
                    BotType = eBotTYpe.CombatOnly;
                    CombatOnly.Start();
                    break;
                    
                case 2: // healbot
                    BotType = eBotTYpe.HealBot;
                    HealBot.Start();
                    break;

                case 3: // Sushi Bot
                    BotType = eBotTYpe.SushiBot;
                    clsSettings.Start();
                    SushiBot.StartSushi();
                    break;

                case 4: // inidividual path
                    BotType = eBotTYpe.IndividualPath;
                    IndividPath.Start(this.cmbGeneralPaths.SelectedItem as clsPathGroupItem);
                    break;

                case 5: // auto nav combat bot
                    BotType = eBotTYpe.AutoNavCombat;
                    clsSettings.Start();
                    AutoNavBot.Start();
                    break;

                // TODO: other bot types
            }

            // update BotRan
            new clsRS().UpdateBotRun(clsSettings.LoginInfo.UserID, this.cmbBotType.Text, clsSettings.UpdateText, clsSettings.IsDCd);

            // open the floating window
            LoadFloat();
        }

        private void cmdPauseBot_Click(object sender, EventArgs e)
        {
            if (clsSettings.Pause)
                this.cmdPauseBot.Text = Resources.PauseBot;
            else
                this.cmdPauseBot.Text = Resources.UnpauseBot;

            // pause the bot
            clsSettings.Pause = !clsSettings.Pause;
        }

        private void cmdStopBot_Click(object sender, EventArgs e)
        {
            StopBot();
        }

        private void txtLevelToLoad_ValueChanged(object sender, EventArgs e)
        {
            // update global level
            clsGlobals.SettingsLevel = (int)this.txtLevelToLoad.Value;
        }

        /// <summary>
        /// Opens the floating control window
        /// </summary>
        private void LoadFloat()
        {
            // hook the event
            clsGlobals.FloatStopped += clsGlobals_FloatStopped;

            // minimize this form
            clsGlobals.frmMain.WindowState = FormWindowState.Minimized;

            // load the form
            if (frmFloat == null)
                frmFloat = new ISXFloat();
            if (! frmFloat.Visible)
                frmFloat.Show();
        }

        /// <summary>
        /// Raised when the user clicks stop on the floating window
        /// </summary>
        void clsGlobals_FloatStopped(object sender, EventArgs e)
        {
            // unhook the event
            clsGlobals.FloatStopped -= clsGlobals_FloatStopped;

            // close the form
            if ((frmFloat != null) && (frmFloat.Visible))
            {
                frmFloat.CloseFloatWindow();
                frmFloat = null;
            }

            // Stop the bot
            StopBot();
        }

        /// <summary>
        /// Stops the bot.
        /// </summary>
        private void StopBot()
        {
            // stop
            clsSettings.Stop = true;

            // close frmfloat
            clsSettings.Logging.DebugWrite(Resources.ClosingFloatForm);
            if ((frmFloat != null) && (frmFloat.Visible))
                frmFloat.CloseFloatWindow();
            if (frmFloat != null)
                frmFloat = null;

            Application.DoEvents();

            // reshow this form
            clsSettings.Logging.DebugWrite(Resources.ReshowingForm);
            if (clsGlobals.frmMain.WindowState == FormWindowState.Minimized)
                clsGlobals.frmMain.WindowStateNormal();

            Application.DoEvents();

            clsSettings.Logging.DebugWrite(Resources.FormShow);
            if (!clsGlobals.frmMain.Visible)
                clsGlobals.frmMain.ShowThisForm();

            Application.DoEvents();

            // TODO: when we add the other Start/Stop panels, we'll need to know
            // which process to stop
            // pause the bot
            clsSettings.Logging.AddToLog(Resources.uscMain, Resources.StoppingBot);
            switch (BotType)
            {
                case eBotTYpe.WithPaths:
                    MyRhabot.Stop();
                    break;
                case eBotTYpe.CombatOnly:
                    CombatOnly.Stop();
                    break;
                case eBotTYpe.HealBot:
                    HealBot.Stop();
                    break;
                case eBotTYpe.SushiBot:
                    clsSettings.Stop = true;
                    break;
                case eBotTYpe.IndividualPath:
                    IndividPath.Stop();
                    break;

                case eBotTYpe.AutoNavCombat:
                    AutoNavBot.Shutdown = true;
                    AutoNavBot.Stop();
                    break;

                // TODO: stop other bot types
            }
            Application.DoEvents();

            // stop the threads
            clsSettings.AbortThreads();

            // enable buttons
            clsSettings.Logging.DebugWrite(Resources.RhabotStopped);
            Enable_Start();
            Disable_Stop();
        }

        /// <summary>
        /// User wants to change the MPQ path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdChangeMPQ_Click(object sender, EventArgs e)
        {
            try
            {
                // show the dialog. Exit if not ok
                if (this.folderBrowserDialog1.ShowDialog(this) != DialogResult.OK)
                    return;

                // set the new path
                clsSettings.MPQPath = this.folderBrowserDialog1.SelectedPath;

                // update text box
                this.txtMPQLocation.Text = clsSettings.MPQPath;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Unable to set MPQ file location");
            }            
        }

        // Bot Control Button Clicks
        #endregion

        #region Enable / Disable

        private void ClearPathNotes()
        {
            this.txtPathNotes.Text = "";
            this.txtPathNotes.Enabled = false;
            this.cmdSavePathNote.Enabled = false;
        }

        /// <summary>
        /// Enables the start.
        /// </summary>
        private void Enable_Start()
        {
            this.cmdStartBot.Enabled = true;
            this.cmbBotType.Enabled = true;
            this.cmbPathGroupName.Enabled = true;
        }

        /// <summary>
        /// Enables the stop.
        /// </summary>
        private void Enable_Stop()
        {
            this.cmdStopBot.Enabled = true;
            this.cmdPauseBot.Enabled = true;
        }

        /// <summary>
        /// Disables the start.
        /// </summary>
        private void Disable_Start()
        {
            this.cmdStartBot.Enabled = false;
            this.cmbBotType.Enabled = false;
            this.cmbPathGroupName.Enabled = false;
        }

        /// <summary>
        /// Disables the stop.
        /// </summary>
        private void Disable_Stop()
        {
            this.cmdStopBot.Enabled = false;
            this.cmdPauseBot.Enabled = false;
        }

        // Enable / Disable
        #endregion

        #region Combat Bot

        private void cmbBotType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // show target checkbox for combat bot
            this.chkTarget.Visible = (this.cmbBotType.SelectedIndex == 1);

            // show/hide group paths name
            this.cmbPathGroupName.Visible = (this.cmbBotType.SelectedIndex == 0);
            this.cmdPathName.Visible = ((this.cmbBotType.SelectedIndex == 0) || (this.cmbBotType.SelectedIndex == 4));

            // show/hide general paths list
            this.cmbGeneralPaths.Visible = (this.cmbBotType.SelectedIndex == 4);
            if (this.cmbGeneralPaths.Visible)
                this.cmbPathGroupName.Visible = false;

            // clear path notes
            this.txtPathNotes.Enabled = this.cmbPathGroupName.Visible;
            this.cmdSavePathNote.Enabled = this.cmbPathGroupName.Visible;
        }

        private void chkTarget_CheckedChanged(object sender, EventArgs e)
        {
            clsSettings.CombatOnly_AttackOnTarget = this.chkTarget.Checked;
        }

        // Combat Bot
        #endregion

        #region Paths

        /// <summary>
        /// Pop the list of general paths
        /// </summary>
        private void PopGeneralPathList()
        {
            try
            {
                // clear the list
                this.cmbGeneralPaths.Items.Clear();

                // get the list
                List<rs.PathService.clsPathListInfo> pathList = new rs.clsRS().GetPathGroupList(clsSettings.LoginInfo.UserID, clsSettings.UpdateText, clsSettings.IsDCd);
                List<clsPathGroupItem> AddedList = new List<clsPathGroupItem>();

                // pop the combo
                foreach (rs.PathService.clsPathListInfo pInfo in pathList)
                {
                    // skip if not a general path
                    if ((pInfo.PathName == clsGlobals.Path_Graveyard1) ||
                        (pInfo.PathName == clsGlobals.Path_Graveyard2) ||
                        (pInfo.PathName == clsGlobals.Path_Hunting) ||
                        (pInfo.PathName == clsGlobals.Path_Mailbox) ||
                        (pInfo.PathName == clsGlobals.Path_StartHunt) ||
                        (pInfo.PathName == clsGlobals.Path_Vendor))
                            continue;

                    // skip if already in the list
                    if (AddedList.Any<clsPathGroupItem>(x => x.GroupName == pInfo.PathName))
                        continue;

                    // add to the list
                    clsPathGroupItem pgi = new clsPathGroupItem() { GroupName = pInfo.PathName, GroupNote = pInfo.PathID.ToString() };
                    this.cmbGeneralPaths.Items.Add(pgi);
                    AddedList.Add(pgi);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "PopGeneralPathList");
            }            
        }

        private void PopGroupPathList()
        {
            clsGlobals.PopGroupPathList(this.cmbPathGroupName, true);
        }

        /// <summary>
        /// User clicked to refresh the path group name list
        /// </summary>
        private void cmdPathName_Click(object sender, EventArgs e)
        {
            if (this.cmbPathGroupName.Visible)
                PopGroupPathList();
            else if (this.cmbGeneralPaths.Visible)
                PopGeneralPathList();
        }

        // Paths
        #endregion

        #region ProfileNotes

        /// <summary>
        /// Raised when the user changes the profile in the dropdown
        /// </summary>
        private void cmbSettingsName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // exit if nothing selected
            int index = this.cmbSettingsName.SelectedIndex;
            if (index < 0)
            {
                this.txtProfileNotes.Text = "";
                return;
            }

            // pop note
            this.txtProfileNotes.Text = ((clsPathGroupItem)this.cmbSettingsName.SelectedItem).GroupNote;
        }

        /// <summary>
        /// Save the profile note
        /// </summary>
        private void cmdSaveProfileNotes_Click(object sender, EventArgs e)
        {
            try
            {
                // set cursor
                this.Cursor = Cursors.WaitCursor;

                // exit if nothing selected
                int index = this.cmbSettingsName.SelectedIndex;
                if (index < 0)
                    return;

                // get the item in the dropdown
                clsPathGroupItem gItem = this.cmbSettingsName.SelectedItem as clsPathGroupItem;
                gItem.GroupNote = this.txtProfileNotes.Text;

                // save the note
                new rs.clsRS().SaveProfileNote(
                    clsSettings.LoginInfo.UserID,
                    gItem.GroupName,
                    clsSerialize.Serialize_ToByteA(gItem.GroupNote, typeof(string)),
                    clsSettings.UpdateText,
                    clsSettings.IsDCd);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Save Profile Note");
            }

            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void cmdSavePathNote_Click(object sender, EventArgs e)
        {
            try
            {
                // set cursor
                this.Cursor = Cursors.WaitCursor;

                // exit if nothing selected
                int index = this.cmbPathGroupName.SelectedIndex;
                if (index < 0)
                    return;

                // get the item in the dropdown
                clsPathGroupItem gItem = this.cmbPathGroupName.SelectedItem as clsPathGroupItem;
                gItem.GroupNote = this.txtPathNotes.Text;

                // save the note
                new rs.clsRS().SavePathGroupNote(
                    clsSettings.LoginInfo.UserID,
                    gItem.GroupName,
                    clsSerialize.Serialize_ToByteA(gItem.GroupNote, typeof(string)),
                    clsSettings.UpdateText,
                    clsSettings.IsDCd);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Save Path Note");
            }

            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // ProfileNotes
        #endregion

        #region Sharing

        /// <summary>
        /// Show the shared profiles tab
        /// </summary>
        private void cmdSharedProfiles_Click(object sender, EventArgs e)
        {
            Rhabot.clsGlobals.ShowSharedProfiles();
        }

        /// <summary>
        /// Show the shared paths tab
        /// </summary>
        private void cmdSharedPaths_Click(object sender, EventArgs e)
        {
            Rhabot.clsGlobals.ShowSharedPaths();
        }

        // Sharing
        #endregion
    }
}
