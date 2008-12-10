/*
 * This class handles loading/saving of settings to one or more settings files
 * The settings can be stored in separate files (such as one file for the food list, and one
 * file for general setting), or they can be in one large file. To have all the settings in
 * the same file, simple use the same file name for all load/save functions. Each function
 * creates a separate section in the xml file, so all groups can live in the same file.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ISXBotHelper.AutoNav;
using ISXBotHelper.CharacterStatus;
using ISXBotHelper.Chatter;
using ISXBotHelper.Communication;
using ISXBotHelper.MSNChat;
using ISXBotHelper.Properties;
using ISXBotHelper.Settings;
using ISXBotHelper.Talents;
using ISXBotHelper.Threading;
using ISXBotHelper.XProfile;
using ISXRhabotGlobal;
using MerlinEncrypt;
using rs;
using ISXBotHelper.Settings.Settings;

namespace ISXBotHelper
{
    /// <summary>
    /// Settings information for ISXBotHelper
    /// </summary>
    public static class clsSettings
    {
        #region Enums

        public enum EMobSearchType
        {
            /// <summary>
            /// Search for hostile mobs only
            /// </summary>
            Hostile = 0,

            /// <summary>
            /// Search for unfriendly and hostile mobs
            /// </summary>
            Unfriendly
        }

        // Enums
        #endregion

        #region Variables

        #region Settings.settings

        /// <summary>
        /// Global Settings
        /// </summary>
        internal static clsGlobalSettings gclsGlobalSettings = new clsGlobalSettings();

        /// <summary>
        /// Level Settings
        /// </summary>
        internal static clsLevelSettings gclsLevelSettings = new clsLevelSettings();

        // Settings.settings
        #endregion

        #region Shutdown

        private static int m_IsDCd; // 0 = false, 1 = true
        /// <summary>
        /// Set to true when we cannot connect to the GUpdate Service
        /// </summary>
        public static bool IsDCd
        {
            get { return Thread.VolatileRead(ref m_IsDCd) == 1; }
            set { Thread.VolatileWrite(ref m_IsDCd, value ? 1 : 0); }
        }

        private static bool m_IsShuttingDown;
        /// <summary>
        /// set to true when we are shutting down, do not log or do other special actions
        /// </summary>
        public static bool IsShuttingDown
        {
            get { return m_IsShuttingDown; }
            set { m_IsShuttingDown = value; }
        }

        private static ThreadItem LogoutTimeThread;

        // Shutdown
        #endregion

        #region Misc Settings

        /// <summary>
        /// List of default settings. The MSN_Username is the settings name
        /// </summary>
        internal static Dictionary<clsGlobalSettings, Dictionary<int, clsLevelSettings>> DefaultSettingsList = new Dictionary<clsGlobalSettings, Dictionary<int, clsLevelSettings>>();

        /// <summary>
        /// Full path to the log file
        /// </summary>
        private static string m_LogFilePath = string.Empty;
        public static string LogFilePath
        {
            get { return m_LogFilePath; }
            set 
            { 
                m_LogFilePath = value; 

                // reset the explore path
                m_ExplorePath = string.Empty;

                // create new instance of log file
                Logging = new clsLogging();
            }
        }

        #region Explore

        private static string m_ExploreFolder = string.Empty;
        /// <summary>
        /// The folder where the explore files reside
        /// </summary>
        private static string ExploreFolder
        {
            get
            {
                // return if we have the path already
                if (!string.IsNullOrEmpty(m_ExploreFolder))
                    return m_ExploreFolder;

                // we don't have a path, so get it from the log path
                try
                {
                    // get the path above Logs
                    m_ExploreFolder = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(m_LogFilePath)), "Explore");

                    // if this folder doesn't exit, create it
                    if (!Directory.Exists(m_ExploreFolder))
                        Directory.CreateDirectory(m_ExploreFolder);

                    return m_ExploreFolder;
                }
                catch (Exception excep)
                {
                    clsError.ShowError(excep, "Explore Folder");
                }

                return string.Empty;
            }
        }

        private static string m_OrigExplorePath = string.Empty;
        /// <summary>
        /// The original explore file
        /// </summary>
        internal static string OrigExplorePath
        {
            get
            {
                // return if we have the path already
                if (!string.IsNullOrEmpty(m_OrigExplorePath))
                    return m_OrigExplorePath;

                // we don't have a path, so get it from the log path
                try
                {
                    // get the path above Logs
                    m_OrigExplorePath = Path.Combine(ExploreFolder, "Explore.xml");

                    return m_ExploreFolder;
                }
                catch (Exception excep)
                {
                    clsError.ShowError(excep, "Explore Folder");
                }

                return string.Empty;
            }
        }

        public static string m_ExplorePath = string.Empty;
        private static string m_ExploreReadPath = string.Empty;
        /// <summary>
        /// Explore file save path
        /// </summary>
        internal static string ExplorePath
        {
            get 
            { 
                // time to stop trying to copy
                DateTime CopyEndTime = DateTime.Now.AddMinutes(5);

                // return if we have the path already
                if (! string.IsNullOrEmpty(m_ExplorePath))
                    return m_ExplorePath; 

                // we don't have a path, so get it from the log path
                try
                {
                    // get the path above Logs
                    string mPath = ExploreFolder;

                    // copy explore file
                    string origExplore = Path.Combine(mPath, "Explore.xml");
                    string newExplore = Path.Combine(mPath, string.Format("Explore_{0}.xml", GetGuidFilename()));

                    // loop until we can copy the file, or 5 minutes has elapsed
                    while (true)
                    {
                        try
                        {
                            // copy the file
                            File.Copy(origExplore, newExplore);
                            break;
                        }

                        catch (Exception excep)
                        {
                            // if we have waited for 5 minutes, then show the error and exit
                            if (CopyEndTime < DateTime.Now)
                            {
                                clsError.ShowError(new Exception(string.Format("Rhabot - Explore Path - Attempted to copy '{0}' to '{1}' for five minutes. Copy could not be completed. Explore process may not function correctly", origExplore, newExplore), excep), "Explore Path", true, new StackFrame(0, true), false);
                                break;
                            }

                            // the orig file is probably in use (maybe being downloaded?), wait and try again
                            Thread.Sleep(2000);
                        }
                    }

                    // set our varible
                    m_ExplorePath = newExplore;
                    m_ExploreReadPath = Path.Combine(mPath, "ExploreRead.xml");

                    // return
                    return m_ExplorePath;
                }

                catch (Exception excep)
                {
                    clsError.ShowError(excep, "Get ExplorePath File");
                }

                // probably here because of error
                return string.Empty;
            }
        }

            /// <summary>
            /// Path to the explore map file that we can read from
            /// </summary>
            internal static string ExploreReadPath
            {
                get
                {
                    // return the path name
                    return m_ExploreReadPath;
                }
            }

            private static string m_ExploreUploadFile = string.Empty;
            /// <summary>
            /// Explore upload file
            /// </summary>
            internal static string ExploreUploadFile
            {
                get
                {
                    if (string.IsNullOrEmpty(m_ExploreUploadFile))
                        m_ExploreUploadFile = Path.Combine(Path.GetDirectoryName(m_ExplorePath),
                            string.Format("{0}.xml",
                                GetGuidFilename()));

                    return m_ExploreUploadFile;
                }
                set { m_ExploreUploadFile = value; }
            }

            // Explore
        #endregion

        /// <summary>
        /// Returns a guid filename (without extension)
        /// </summary>
        private static string GetGuidFilename()
        {
            return Guid.NewGuid().ToString().Trim().Replace("{", string.Empty).Replace("}", string.Empty).Replace("-", string.Empty);
        }

        /// <summary>
        /// IsMoving thread
        /// </summary>
        private static Thread thread_IsMoving;

        /// <summary>
        /// For CombatOnly bot, when true, will attack selected target when target changes
        /// </summary>
        public static bool CombatOnly_AttackOnTarget;

        /// <summary>
        /// When true, show more logging options
        /// </summary>
        public static bool VerboseLogging;

        /// <summary>
        /// True when the user is not at keyboard, or automated script
        /// </summary>
        public static bool IsUnattended;

        /// <summary>
        /// Holds a connection to the logging file
        /// </summary>
        public static clsLogging Logging = new clsLogging();

        private static ISXWoW.ISXWoW m_isxwow;
        /// <summary>
        /// The ISXWoW object
        /// </summary>
        public static ISXWoW.ISXWoW isxwow
        {
            get 
            { 
                // get if we don't have it already
                if (m_isxwow == null)
                {
                    using (new clsFrameLock.LockBuffer())
                        m_isxwow = new ISXWoW.ISXWoW();
                }

                return m_isxwow;
            }
        }

        /// <summary>
        /// The log window
        /// </summary>
        internal static ISXLog frmLog = new ISXLog();

        /// <summary>
        /// Update Text
        /// </summary>
        public static string UpdateText = string.Empty;

        /// <summary>
        /// Which type of mobs to search for
        /// </summary>
        public static EMobSearchType MobSearchType = EMobSearchType.Unfriendly;

        public static bool SearchForHostiles
        {
            get { return MobSearchType == EMobSearchType.Hostile; }
        }

        /// <summary>
        /// Chatter
        /// </summary>
        public static clsChatter chatter = new clsChatter();

        /// <summary>
        /// Character status update thread
        /// </summary>
        internal static readonly clsCharacterStatusMain characterStatusMain = new clsCharacterStatusMain();

        private static bool m_SendMsgOnDead;
        /// <summary>
        /// When true, sends an email when toon dies
        /// </summary>
        public static bool SendMsgOnDead
        {
            get { return m_SendMsgOnDead; }
            set { m_SendMsgOnDead = value; }
        }

        // Misc Settings
        #endregion

        #region clsPath

        /// <summary>
        /// How close to consider moving to the next point when navigating a path
        /// </summary>
        public static int PathPrecision = 5;

        /// <summary>
        /// Distance to have between points when saving a new path
        /// </summary>
        public static int SavePathPrecision = 12;

        // clsPath
        #endregion

        #region Blacklists

        /// <summary>
        /// List of hostile units that are blacklisted. This list is not persisted, as I 
        /// think the guids change each time WoW is loaded
        /// </summary>
        public static List<clsBlacklist> BlackList_Combat = new List<clsBlacklist>();

        /// <summary>
        /// List of herbs/chests/mines that are blacklisted. This list is not persisted, as I 
        /// think the guids change each time WoW is loaded
        /// </summary>
        public static List<clsBlacklist> Blacklist_GameObjects = new List<clsBlacklist>();

        // Blacklists
        #endregion

        #region Login

        public static string MyGuid = string.Empty;
        //internal static Rhabot.RhabotService.clsLoginInfo LoginInfo = new Rhabot.RhabotService.clsLoginInfo();
        internal static clsLoginInfo LoginInfo = new clsLoginInfo();
        internal static string Username = string.Empty;

        private static int iGuidValid;
        /// <summary>
        /// Checks if the guid's match
        /// </summary>
        public static bool GuidValid
        {
            get
            {
                // debug values
                if (VerboseLogging)
                    Logging.DebugWrite(string.Format("MyGUID: {0}   -  UserGUID: {1}", MyGuid, LoginInfo.UserGUID));

                // if we already checked, return the result
                if (iGuidValid != 0)
                    return iGuidValid == 1;

                // get valid
                bool rVal = (MyGuid == LoginInfo.UserGUID);

                iGuidValid = rVal ? 1 : 2;

                // if not valid, shutdown
                if (!rVal)
                {
                    clsError.ShowError(new Exception("Your login information is incorrect. Rhabot is shutting down"), LoginInfo.UserID.ToString().Trim());
                    Shutdown();
                    clsEvent.Raise_ForcingShutdown();
                }

                return rVal;
            }
        }

        /// <summary>
        /// Rhabot version
        /// </summary>
        public static int BotVersion
        {
            get { return 168; }
        }

        public static bool IsPaid
        {
            get { return LoginInfo.IsPaid; }
        }

        public static bool IsBetaTester
        {
            get { return LoginInfo.IsBetaTester; }
        }

        private static bool m_HasLogin;
        /// <summary>
        /// True if the user used a username to log into Rhabot
        /// </summary>
        public static bool HasLogin
        {
            get { return m_HasLogin; }
            set { m_HasLogin = value; }
        }

        /// <summary>
        /// Returns if this user can use the full version of Rhabot
        /// </summary>
        public static bool IsFullVersion
        {
            get
            {
                // return true if beta tester or paid
                if ((LoginInfo.IsBetaTester) || (LoginInfo.IsPaid))
                    return true;

                return false;
            }
        }

        // Login
        #endregion

        #region Talents

        private static readonly clsTalents Talents = new clsTalents();

        // Talents
        #endregion

        #region Thread List

        #region ThreadItem

        internal class ThreadItem
        {
            public Thread thread;
            public object ClassRunning;

            public ThreadItem(Thread NewThread, object classRunning)
            {
                thread = NewThread;
                ClassRunning = classRunning;
            }
        }

        // ThreadItem
        #endregion

        /// <summary>
        /// List of threads that are active until script stops
        /// </summary>
        internal static List<ThreadItem> ThreadList = new List<ThreadItem>();

        /// <summary>
        /// List of threads that are active until Rhabot shuts down
        /// </summary>
        internal static List<ThreadItem> GlobalThreadList = new List<ThreadItem>();

        public static void AbortThreads()
        {
            // exit if no threads
            if ((ThreadList == null) || (ThreadList.Count == 0))
                return;

            // loop through the thread list and kill the threads
            try
            {
                foreach (ThreadItem tItem in ThreadList)
                {
                    if ((tItem != null) && (tItem.thread != null) && (tItem.thread.IsAlive))
                    {
                        // kill the thread
                        if (Logging != null)
                            Logging.AddToLog(string.Format("AbortThreads: Killing Thread '{1}'. Thread Count = {0}",
                                ThreadList.Count, string.IsNullOrEmpty(tItem.thread.Name) ? string.Empty : tItem.thread.Name));

                        // kill this thread
                        KillThread(tItem, "AbortThreads");
                    }
                }
            }

            catch (Exception excep)
            {
                // debug the error, will probably be a lot of thread aborting errors
                if (Logging != null)
                    Logging.DebugWrite(string.Format("AbortThreads: Error - {0}", excep.Message));
            }
            finally
            {
                ThreadList.Clear();
            }
        }

        /// <summary>
        /// Aborts all threads in GlobalThreadList
        /// </summary>
        private static void AbortGlobalThreads()
        {
            // exit if no threads
            if ((GlobalThreadList == null) || (GlobalThreadList.Count == 0))
                return;

            try
            {
                foreach (ThreadItem tItem in GlobalThreadList)
                {
                    // only process if thread is running
                    if ((tItem != null) && (tItem.thread != null) && (tItem.thread.IsAlive))
                    {
                        // kill the thread
                        if (Logging != null)
                            Logging.AddToLog(string.Format("AbortGlobalThreads: Killing Thread '{0}'",
                                                           string.IsNullOrEmpty(tItem.thread.Name) ? string.Empty : tItem.thread.Name));

                        KillThread(tItem, "Abort Global Threads");
                    }
                }
            }

            catch (Exception excep)
            {
                // debug the error, will probably be a lot of thread aborting errors
                if (Logging != null)
                    Logging.DebugWrite(string.Format("AbortGlobalThreads: Error - {0}", excep.Message));
            }

            finally
            {
                GlobalThreadList.Clear();
            }
        }

        // Thread List
        #endregion

        #region Global Settings (saved in RSettings.xml)

        private static bool IsRhabotSettingsLoading;

        private static string m_RhabotSettingsFile = string.Empty;
        /// <summary>
        /// The rhabot settings file
        /// </summary>
        public static string RhabotSettingsFile
        {
            get 
            { 
                if (string.IsNullOrEmpty(m_RhabotSettingsFile))
                    m_RhabotSettingsFile = Path.Combine(GetAppPath, "RSettings.xml");

                return m_RhabotSettingsFile; 
            }
        }

        private static string m_LastSettingsFolder = string.Empty;
        /// <summary>
        /// Last settings folder location
        /// </summary>
        public static string LastSettingsFolder
        {
        	get {return m_LastSettingsFolder;}
        	set 
            { 
                m_LastSettingsFolder = value;
                SaveRhabotSettings();
            }
        }

        private static string m_LastChatterFile = string.Empty;
        /// <summary>
        /// Last chatter file location
        /// </summary>
        public static string LastChatterFile
        {
        	get {return m_LastChatterFile;}
        	set 
            { 
                m_LastChatterFile = value;
                SaveRhabotSettings();
            }
        }

        private static bool m_RunExplore;
        /// <summary>
        /// True to run the explore threads
        /// </summary>
        public static bool RunExplore
        {
        	get {return m_RunExplore;}
        	set 
            { 
                m_RunExplore = value;
                SaveRhabotSettings();
            }
        }

        private static bool m_ShowQuickGuideNag = true;
        /// <summary>
        /// true to show the prompt to view the quick start guide
        /// </summary>
        public static bool ShowQuickGuideNag
        {
        	get {return m_ShowQuickGuideNag;}
        	set 
            { 
                m_ShowQuickGuideNag = value;
                SaveRhabotSettings();
            }
        }

        private static bool m_ShowTrialExpire = true;
        /// <summary>
        /// true to show the trial expire dialog
        /// </summary>
        public static bool ShowTrialExpire
        {
            get { return m_ShowTrialExpire; }
            set { m_ShowTrialExpire = value; }
        }

        private static string m_NologinGUID = string.Empty;
        /// <summary>
        /// Guid if no login
        /// </summary>
        public static string NologinGUID
        {
            get { return m_NologinGUID; }
            set 
            { 
                m_NologinGUID = value;
                SaveRhabotSettings();
            }
        }

        private static string m_LastLoginName = string.Empty;
        /// <summary>
        /// Last user logged in username
        /// </summary>
        public static string LastLoginName
        {
            get { return m_LastLoginName; }
            set 
            { 
                m_LastLoginName = value;
                SaveRhabotSettings();
            }
        }

        private static bool m_SettingsUploadWizardShown;
        /// <summary>
        /// When true, we have already shown the settings upload wizard
        /// </summary>
        public static bool SettingsUploadWizardShown
        {
            get { return m_SettingsUploadWizardShown; }
            set 
            { 
                m_SettingsUploadWizardShown = value;
                //SaveRhabotSettings();
            }
        }

        /// <summary>
        /// Reset the MPQ path location
        /// </summary>
        private static void ResetMPQPath()
        {
#if DEBUG
            m_MPQPath = @"G:\Program Files\WoWS\Data\";
#else
            // use %ProgramFiles% as default folder
            m_MPQPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles), @"World of Warcraft\Data\");
#endif
        }

        private static string m_MPQPath = "";
        /// <summary>
        /// The path to the MPQ files
        /// </summary>
        public static string MPQPath
        {
            get 
            { 
                // reset path if we don't have it
                if (string.IsNullOrEmpty(m_MPQPath))
                    ResetMPQPath();

                // add ending "\"
                if (!m_MPQPath.EndsWith("\\"))
                    m_MPQPath += "\\";

                return m_MPQPath; 
            }
            set 
            { 
                m_MPQPath = value;

                // reset if it got cleared
                if (string.IsNullOrEmpty(m_MPQPath))
                    ResetMPQPath();

                // add ending "\"
                if (!m_MPQPath.EndsWith("\\"))
                    m_MPQPath += "\\";

                SaveRhabotSettings();
            }
        }

        /// <summary>
        /// Removes path delimiters, such as \ and :
        /// </summary>
        /// <param name="PathToStrip">the path to strip</param>
        internal static string StripPathCharacter(string PathToStrip)
        {
            return PathToStrip.Replace(":", string.Empty).Replace("\\", "_").Replace(ListDelim, "_").ToLower();
        }

        // Global Settings (saved in GlobalSettings.xml)
        #endregion

        #region Settings

        /// <summary>
        /// Current settings name
        /// </summary>
        public static string CurrentSettingsName = string.Empty;

        // Settings
        #endregion

        // Variables
        #endregion

        #region Load/Save

        /// <summary>
        /// Deletes a temp file
        /// </summary>
        /// <param name="TempFile"></param>
        internal static void DeleteFile(string TempFile)
        {
            try
            {
                // delete if it exists
                if ((!string.IsNullOrEmpty(TempFile)) && (File.Exists(TempFile)))
                    File.Delete(TempFile);
            }
            catch { } // don't need to show any errors, as we'll try to delete it again on the next load
        }

        internal static string GetTempFolder()
        {
            string tempDir = string.Empty;

            try
            {
                // get the temp folder
                tempDir = Path.Combine(GetAppPath, "temp");

                // create if it doesnt' exist
                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.Settings);
            }

            return tempDir;
        }

        /// <summary>
        /// Returns a temporary file from the rhabot\temp folder
        /// </summary>
        internal static string GetTempFileName()
        {
            string rVal = string.Empty;

            try
            {
                // return the file
                rVal = Path.Combine(GetTempFolder(), string.Format("{0}.xml", Guid.NewGuid().ToString().Replace("-", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty)));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.Settings, Resources.GetTempFileName);
            }

            return rVal;
        }

        #region Load/Save Settings

        #region Load Default Settings

        /// <summary>
        /// Loads the default global settings
        /// </summary>
        private static void LoadDefaultSettingsGlobal()
        {
            // get the settings list
            foreach (clsGlobalSettings gs in DefaultSettingsList.Keys)
            {
                // return if we have a match
                if (gs.MSN_Username == CurrentSettingsName)
                {
                    gclsGlobalSettings = gs.Serialize().Deserialize(typeof(clsGlobalSettings)) as clsGlobalSettings;
                    gclsGlobalSettings.MSN_Username = "";
                    break;
                }
            }
        }

        /// <summary>
        /// Loads default settings, by level
        /// </summary>
        /// <param name="Level"></param>
        private static void LoadDefaultSettingsLevel(int Level)
        {
            gclsLevelSettings = LoadDefaultPanelSettings(Level);
        }

        /// <summary>
        /// Returns the settings for this level
        /// </summary>
        /// <param name="Level"></param>
        /// <returns></returns>
        private static clsLevelSettings LoadDefaultPanelSettings(int Level)
        {
            // get the settings list
            foreach (clsGlobalSettings gs in DefaultSettingsList.Keys)
            {
                // skip if no match
                if (gs.MSN_Username != CurrentSettingsName)
                    continue;

                // return the level
                return DefaultSettingsList[gs][Level];
            }

            // nothing found, return null
            return null;
        }

        // Load Default Settings
        #endregion

        #region Load/Save Global Settings

        /// <summary>
        /// Load the global settings
        /// </summary>
        public static void LoadGlobalSettings()
        {
            try
            {
                // log it
                Logging.AddToLog("LoadGlobalSettings");

                // if this is a default setting, load it from the list intead
                if (CurrentSettingsName.ToLower().StartsWith("default"))
                {
                    LoadDefaultSettingsGlobal();
                    return;
                }

                // get from server
                string encSettings = new clsRS().LoadUserSettings(LoginInfo.UserID, CurrentSettingsName, 0, UpdateText, IsDCd);

                // deserialize
                gclsGlobalSettings = encSettings.DeserializeGSettings() ?? new clsGlobalSettings();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Unable to load global settings");
            }            
        }

        /// <summary>
        /// Save the global settings
        /// </summary>
        public static void SaveGlobalSettings()
        {
            try
            {
                // log it
                Logging.AddToLog("SaveGlobalSettings");

                // skip if a Default setting
                if (CurrentSettingsName.ToLower().StartsWith("default"))
                {
                    Logging.AddToLog("Skipping settings save. This is a default setting and cannot be saved");
                    return;
                }

                // serialize
                string encSettings = gclsGlobalSettings.Serialize();

                // save to server
                new clsRS().SaveUserSettings(LoginInfo.UserID, CurrentSettingsName, 0, encSettings, UpdateText, IsDCd);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Unable to save global settings");
            }
        }

        // Load/Save Global Settings
        #endregion

        #region Load / Save Settings

        /// <summary>
        /// Loads the settings from the file
        /// </summary>
        public static void LoadSettings(int Level)
        {
            try
            {
                // log it
                Logging.AddToLog("Loading Settings");

                // if this is a default setting, load it from the list intead
                if (CurrentSettingsName.ToLower().StartsWith("default"))
                {
                    LoadDefaultSettingsLevel(Level);
                    return;
                }

                // load
                string encSettings = new clsRS().LoadUserSettings(LoginInfo.UserID, CurrentSettingsName, Level, UpdateText, IsDCd);

                // deserialize
                gclsLevelSettings = encSettings.DeserializeLevel() ?? new clsLevelSettings();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Unable to load settings");
            }
        }

        /*
        /// <summary>
        /// Saves the settings to the settings file
        /// </summary>
        public static void SaveSettings(int Level)
        {
            try
            {
                // log
                Logging.AddToLog("Saving Settings");

                // serialize the settings
                string encSettings = gclsLevelSettings.Serialize();

                // save the settings
                new clsRS().SaveUserSettings(LoginInfo.UserID, CurrentSettingsName, Level, encSettings, UpdateText, IsDCd);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Unable to save settings");
            }
        }
        */

        // Load / Save Settings
        #endregion

        #region Load/Save List To XML

        /// <summary>
        /// Loads the list from the xml file
        /// </summary>
        /// <param name="Filename">file name to load</param>
        /// <param name="section">the section name</param>
        internal static List<string> LoadListFromXML(string Filename, string section)
        {
            List<string> retList = new List<string>();

            try
            {
                Xml xml = new Xml(Filename);

                // buffer
                using (xml.Buffer())
                {
                    // get all the names in this section
                    string[] items = xml.GetEntryNames(section);

                    // exit if no items
                    if ((items == null) || (items.Length == 0))
                        return new List<string>();

                    // loop through the items and add them to the list
                    foreach (string item in items)
                        retList.Add((string)xml.GetValue(section, item));
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "LoadListFromXML");
            }

            return retList;
        }

        // Save List To XML
        #endregion

        #region Load/Save Rhabot Settings

        /// <summary>
        /// Loads Rhabot global settings
        /// </summary>
        public static void LoadRhabotSettings()
        {
            const string section = "Settings";

            try
            {
                // set loading flag
                IsRhabotSettingsLoading = true;

                // log
                Logging.AddToLog(string.Format("Loading Rhabot Global Settings from: {0}", RhabotSettingsFile));

                // open the file
                Xml xml = new Xml(RhabotSettingsFile);

                // buffer for safety
                using (xml.Buffer(false))
                {
                    if (xml.HasSection(section))
                    {
                        LastChatterFile = XMLGet_String(section, "LastChatterFile", xml);
                        LastSettingsFolder = XMLGet_String(section, "LastSettingsFolder", xml);
                        RunExplore = XMLGet_Bool(section, "RunExplore", xml, true);
                        ShowQuickGuideNag = XMLGet_Bool(section, "ShowQuickGuideNag", xml, true);
                        ShowTrialExpire = XMLGet_Bool(section, "ShowTrialExpire", xml, true);
                        LastLoginName = XMLGet_String(section, "LastLoginName", xml);
                        MPQPath = XMLGet_String(section, "MPQPath", xml);

                        // path precision
                        PathPrecision = XMLGet_Int(section, "PathPrecision", xml, 5);
                        SavePathPrecision = XMLGet_Int(section, "SavePathPrecision", xml, 10);

                        // no login guid (encrypted)
                        string nlGuid = XMLGet_String(section, "NoLoginGUID", xml);
                        if (! string.IsNullOrEmpty(nlGuid))
                            nlGuid = new Crypt().DecryptString(nlGuid);
                        NologinGUID = nlGuid;

                        // settings uploaded to server
                        SettingsUploadWizardShown = XMLGet_Bool(section, "SettingsUploadWizardShown", xml, false);
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Format("Load Rhabot Global Settings - {0}", RhabotSettingsFile));
            }

            finally
            {
                IsRhabotSettingsLoading = false;
            }
        }

        public static void SaveRhabotSettings()
        {
            const string section = "Settings";

            // exit if we are in the middle of a load
            if (IsRhabotSettingsLoading)
                return;

            try
            {
                // log
                if (Logging != null)
                    Logging.AddToLog(string.Format("Saving Rhabot Global Settings to: {0}", RhabotSettingsFile));

                // open the settings file
                Xml xml = new Xml(RhabotSettingsFile);

                // buffer for safety
                using (xml.Buffer(true))
                {
                    xml.SetValue(section, "LastChatterFile", LastChatterFile);
                    xml.SetValue(section, "LastSettingsFolder", LastSettingsFolder);
                    xml.SetValue(section, "RunExplore", RunExplore.ToString().Trim());
                    xml.SetValue(section, "ShowQuickGuideNag", ShowQuickGuideNag.ToString().Trim());
                    xml.SetValue(section, "ShowTrialExpire", ShowTrialExpire.ToString().Trim());
                    xml.SetValue(section, "LastLoginName", LastLoginName);
                    xml.SetValue(section, "MPQPath", MPQPath);

                    // path precision
                    xml.SetValue(section, "PathPrecision", PathPrecision.ToString().Trim());
                    xml.SetValue(section, "SavePathPrecision", SavePathPrecision.ToString().Trim());

                    // no login guid
                    if (! string.IsNullOrEmpty(NologinGUID))
                        xml.SetValue(section, "NoLoginGUID", new Crypt().EncryptString(NologinGUID));
                    else
                        xml.SetValue(section, "NoLoginGUID", string.Empty);

                    // settings uploaded to server
                    xml.SetValue(section, "SettingsUploadWizardShown", SettingsUploadWizardShown.ToString());
                }
            }
            catch (Exception excep)
            {
                clsError.ShowError(excep, "Save Rhabot Global Settings");
            }
        }

        // Load/Save Rhabot Settings
        #endregion

        #region Load/Save AutoNav Paths

        /// <summary>
        /// Saves the autonav actions
        /// </summary>
        /// <param name="AutoNavSaveList">list of plans</param>
        public static void SaveAutoNav(clsAutoNavSaveList AutoNavSaveList)
        {
            try
            {
                // serialize the auto nav data
                string aData = AutoNavSaveList.Serialize();

                // save it
                new clsRS().SaveAutoNav(LoginInfo.UserID, aData, AutoNavSaveList.PlanName, UpdateText, IsDCd);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "SaveAutoNav");
            }            
        }

        public static clsAutoNavSaveList LoadAutoNav(string PlanName)
        {
            clsAutoNavSaveList retList = new clsAutoNavSaveList();

            try
            {
                // get the data from the server
                string aData = new clsRS().LoadAutoNav(LoginInfo.UserID, PlanName, UpdateText, IsDCd);

                // if no data, it means we had an error or invalid login
                if (string.IsNullOrEmpty(aData))
                    return retList;

                // deserialize the data
                retList = aData.DeserializeAutoNav();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "LoadAutoNav");
            }

            return retList;
        }

        /// <summary>
        /// Returns the list of AutoNav plans for this user
        /// </summary>
        public static List<string> LoadAutoNavList()
        {
            try
            {
                // return the list from the server
                return new clsRS().LoadAutoNavList(LoginInfo.UserID, UpdateText, IsDCd);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "LoadAutoNavList");
            }
            return new List<string>();
        }

        // Load/Save AutoNav Paths
        #endregion

        #region Load / Save Panel Settings

        public enum ESavePanel
        {
            Settings = 0,
            CombatSettings,
            ClassCombatSettings
        }

        /// <summary>
        /// Loads the settings from the file
        /// </summary>
        public static clsLevelSettings LoadPanelSettings(int Level)
        {
            clsLevelSettings rSettings = null;

            try
            {
                // log it
                Logging.AddToLog("Loading Panel Settings");

                // if this is a default setting, load it from the list intead
                if (CurrentSettingsName.ToLower().StartsWith("default"))
                    return LoadDefaultPanelSettings(Level);

                // load
                string encSettings = new clsRS().LoadUserSettings(LoginInfo.UserID, CurrentSettingsName, Level, UpdateText, IsDCd);

                // deserialize
                rSettings = encSettings.DeserializeLevel() ?? new clsLevelSettings();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Unable to load panel settings");
            }

            return rSettings;
        }

        /// <summary>
        /// Saves the settings to the settings file
        /// </summary>
        /// <param name="Level">the level to save</param>
        /// <param name="LevelSettings"&gt; </param>
        public static void SavePanelSettings(int Level, clsLevelSettings LevelSettings)
        {
            try
            {
                // log
                Logging.AddToLog("Saving Panel Settings");

                // skip if a Default setting
                if (CurrentSettingsName.ToLower().StartsWith("default"))
                {
                    Logging.AddToLog("Skipping settings save. This is a default setting and cannot be saved");
                    return;
                }

                // serialize the settings
                string encSettings = LevelSettings.Serialize();

                // save the settings
                new clsRS().SaveUserSettings(LoginInfo.UserID, CurrentSettingsName, Level, encSettings, UpdateText, IsDCd);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Unable to save panel settings");
            }
        }

        // Load / Save Panel Settings
        #endregion

        // Load/Save Settings
        #endregion

        // Load/Save
        #endregion

        #region Functions

        #region MSN

        /// <summary>
        /// MSN Chat object
        /// </summary>
        private static readonly clsMSNChat msnChat = new clsMSNChat();

        /// <summary>
        /// Runs the MSN chat clients. You must first set the MSN properties
        /// </summary>
        public static void StartMSN()
        {
            // exit if invalid
            if (!GuidValid)
                return;

            msnChat.Connect();
        }

        public static void StopMSN()
        {
            msnChat.DoShutdown();
        }

        // MSN
        #endregion

        #region Helpers

        private const string ListDelim = "~";

        /// <summary>
        /// Converts comma delimited list to a List&gt;string&lt;
        /// </summary>
        /// <param name="StringList">the list to parse</param>
        internal static List<string> ConvertStringToList(string StringList)
        {
            List<string> retList = new List<string>();

            // if empty, return empty list
            if (string.IsNullOrEmpty(StringList))
                return retList;

            // get the list
            string[] sList = Regex.Split(StringList, ListDelim);

            // return a parsed list
            foreach (string item in sList)
            {
                if (! string.IsNullOrEmpty(item.Trim()))
                    retList.Add(item.Trim());
            }

            return retList;
        }

        /// <summary>
        /// Converts a string list to a string
        /// </summary>
        /// <param name="ListString">the list to convert</param>
        internal static string ConvertListToString(List<string> ListString)
        {
            StringBuilder sb = new StringBuilder();

            // get each item from the list
            foreach (string lString in ListString)
            {
                // skip if empty
                if (string.IsNullOrEmpty(lString))
                    continue;

                // add the comma if not the first item
                if (sb.Length > 0)
                    sb.Append(ListDelim);

                // add the item
                sb.Append(lString.Trim());
            }

            // return the list
            return sb.ToString();
        }

        // Helpers
        #endregion

        #region Logging

        /// <summary>
        /// Shows/hides the log window
        /// </summary>
        /// <param name="show">true to show the window, false to hide it</param>
        public static void ShowLogWindow(bool show)
        {
            // show/hide the window
            if (show)
            {
                // show the form
                if (!frmLog.Visible)
                    frmLog.Show();
            }
            else
            {
                // hide the form
                if (frmLog.Visible)
                    frmLog.Hide();
            }
        }

        /// <summary>
        /// Shows/hides the log window
        /// </summary>
        public static void ShowLogWindow()
        {
            // show/hide the form
            if (!frmLog.Visible)
                frmLog.Show();
            else if (frmLog.Visible)
                frmLog.Hide();
        }

        // Logging
        #endregion

        #region Start

        /// <summary>
        /// Initiates the settings (call after loading everything)
        /// </summary>
        public static void Start()
        {
            try
            {
                // undo stop
                Stop = false;
                Pause = false;

                // start stuff from a new thread
                //new Thread(new ThreadStart(Start_Thread)).Start();
                Start_Thread();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Start");
            }
        }

        /// <summary>
        /// Runs start items in a new thread
        /// </summary>
        private static void Start_Thread()
        {
            try
            {
                // start IsMoving thread
                clsPath.clsIsMovingThread imt = new clsPath.clsIsMovingThread();
                thread_IsMoving = new Thread(imt.IsMoving_Thread);
                thread_IsMoving.Name = "IsMoving Thread";
                ThreadList.Add(new ThreadItem(thread_IsMoving, imt));
                thread_IsMoving.Start();
                Thread.Sleep(200);

                // start chat auto responder
                Logging.AddToLog("Start: Chat Auto Responder");
                clsAutoResponder.Start();
                Thread.Sleep(200);

                // hook wow events
                Logging.AddToLog("Start: Hooking all monitored WoW events");
                ThreadPool.QueueUserWorkItem(clsWowEvents.Start);
                Thread.Sleep(200);

                // hook invite events
                Logging.AddToLog("Start: Hooking Group/Guild/Duel invite events");
                ThreadPool.QueueUserWorkItem(clsInvites.StartHook);
                Thread.Sleep(200);

                // hook character level
                Logging.AddToLog("Start: Hooking Character Level Event");
                ThreadPool.QueueUserWorkItem(clsCharacter.HookCharacterLevelEvent);
                Thread.Sleep(200);

                // start talents
                if (IsFullVersion)
                {
                    Logging.AddToLog("Start: Starting Talent Thread. Applies talents when you level");
                    Talents.Start();
                    Thread.Sleep(200);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Start_Thread");
            }
        }

        // Start
        #endregion

        #region Stop/Pause

        private static bool m_Pause;
        /// <summary>
        /// Pauses combat, movethroughpath, and looting
        /// </summary>
        public static bool Pause
        {
            get { return m_Pause; }
            set
            {
                m_Pause = value;
                Logging.AddToLogFormatted("Pause", "Script Pause: {0}", m_Pause.ToString());
            }
        }

        private static bool m_Stop;
        /// <summary>
        /// stops combat, movethroughpath, and looting
        /// </summary>
        public static bool Stop
        {
            get { return m_Stop; }
            set
            {
                m_Stop = value;
                clsGlobals.ScriptStopped = value;

                Logging.AddToLogFormatted("Stop", "Script Stop: {0}", m_Stop.ToString());

                if (m_Stop)
                {
                    // raise script stop
                    clsEvent.Raise_ScriptStop();

                    // send msn message
                    if (clsCharacter.CharacterIsValid)
                    {
                        using (new clsFrameLock.LockBuffer())
                            clsSend.SendToAll_MSN(string.Format("Rhabot is stopping. Character: {0}; Level: {1}", isxwow.Me.Name, isxwow.Me.Level));
                    }

                    // clear autonav quests
                    clsQuest.AutoNavQuestList.Clear();

                    // stop character
                    clsCharacter.Shutdown();

                    // stop humancheck
                    clsHumanCheck.DoShutdown();

                    // stop invite check
                    clsInvites.Shutdown();

                    // stop talent check
                    Talents.Stop();

                    // stop Chatter
                    chatter.DoShutdown();

                    // stop MSN chat
                    msnChat.DoShutdown();

                    // stop chat logging
                    clsChat.Shutdown();

                    // stop hooking events
                    clsWowEvents.Shutdown();
                }
            }
        }

        /// <summary>
        /// Runs a sleep/app doevents loop until scripting is unpaused or stopped.
        /// Returns true if unpaused, false if stopped
        /// </summary>
        public static bool PauseLoop()
        {
            try
            {
                // exit if not paused
                if (!m_Pause)
                    return true;

                // stop moving
                clsPath.StopMoving();

                // we are paused, so sleep, do events, and test for stopped
                while (m_Pause)
                {
                    // sleep half a second
                    Thread.Sleep(500);

                    if (VerboseLogging)
                        Logging.DebugWrite(string.Format("PauseLoop: m_Pause = {0}, m_Stop = {1}", m_Pause, m_Stop));

                    // check for stopped
                    if ((m_Stop) || (m_IsShuttingDown))
                        return false;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "PauseLoop");
            }

            // default to return true
            return true;
        }

        /// <summary>
        /// Tests if script stopped/paused. If paused, the pause loop is called. Returns false if stopped
        /// </summary>
        /// <param name="SectionName"></param>
        /// <param name="LogMessage">the message to write to teh log if stopped</param>
        public static bool TestPauseStop(string SectionName, string LogMessage)
        {
            return TestPauseStop(string.Format("{0}: {1}", SectionName, LogMessage));
        }

        /// <summary>
        /// Tests if script stopped/paused. If paused, the pause loop is called. Returns false if stopped
        /// </summary>
        /// <param name="LogMessage">the message to write to teh log if stopped</param>
        public static bool TestPauseStop(string LogMessage)
        {
            // check if stopped/paused
            if ((m_Stop) || (m_IsShuttingDown) || (!PauseLoop()))
            {
                clsPath.StopMoving();
                if (Logging != null)
                    Logging.AddToLog(LogMessage);
                return false;
            }

            // not stopped
            return true;
        }

        // Stop/Pause
        #endregion

        #region Shutdown

        /// <summary>
        /// This function will gracefully shutdown the bot helper
        /// All classes' Shutdown will be called from here
        /// </summary>
        public static void Shutdown()
        {
            try
            {
                // kill the logout time thread
                KillThread(LogoutTimeThread, string.Empty);

                // if we already shutdown, just exit
                if (m_IsShuttingDown)
                    return;

                // set the shutdown flag
                m_IsShuttingDown = true;

                // Stop
                Stop = true;

                // stop character status monitor
                characterStatusMain.Stop();

                // send msn message
                using (new clsFrameLock.LockBuffer())
                {
                    if (clsCharacter.CharacterIsValid)
                        clsSend.SendToAll_MSN(string.Format("Rhabot is shutting down. Character: {0}; Level: {1}", isxwow.Me.Name, isxwow.Me.Level));
                }

                // stop event
                clsWowEvents.Shutdown();

                // abort threads
                AbortThreads();

                // abort global thread
                AbortGlobalThreads();

                // stop logging
                Logging.Shutdown();

                // close logging
                Logging = null;

                // force garbage collections to force dispose methods to be called
                GC.Collect();
            }

            catch (Exception excep)
            {
                try
                {
                    clsError.ShowError(excep, "Shutdown");
                }
                catch 
                {
                    MessageBox.Show(string.Format("Error shutting down: \r\n{0}", clsError.BuildErrorInfoString(excep, new StackFrame(0, true), "Shutting Down")));
                }
            }
        }

        // Shutdown
        #endregion

        #region LUA

        /// <summary>
        /// Checks if we are sitting. If so, makes us stand up
        /// </summary>
        public static void StandUp()
        {
            using (new clsFrameLock.LockBuffer())
            {
                if (isxwow.Me.Sitting)
                    isxwow.WoWScript("SitStandOrDescendStart()");
            }
        }

        /// <summary>
        /// executes wow/lua commands. returns false on error
        /// </summary>
        /// <returns></returns>
        public static bool ExecuteWoWAPI(string LUACommand)
        {
            bool rVal = false;

            try
            {
                using (new clsFrameLock.LockBuffer())
                    isxwow.WoWScript(LUACommand);
                rVal = true;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "ExecuteWoWAPI");
            }

            return rVal;
        }

        // LUA
        #endregion

        #region App Path

        /// <summary>
        /// returns the path this application resides in. Does not return the ending \
        /// </summary>
        /// <returns></returns>
        public static string GetAppPath
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }

        // App Path
        #endregion

        #region Logout

        /// <summary>
        /// Hearths home and logs out.
        /// NOTE: This does NOT shutdown. You must call shutdown afterwards if you need a shutdown
        /// </summary>
        /// <param name="UseHearthstone">true to hearth home first</param>
        public static void Logout(bool UseHearthstone)
        {
            try
            {
                // stop scripting
                Stop = true;

                // wait for one minute so everything can stop
                Thread.Sleep(60000);

                // stop moving
                clsPath.StopMoving();

                // dismount
                clsMount.Dismount();

                // abort thread
                AbortThreads();

                // cast hearthstone
                if (UseHearthstone)
                    clsCharacter.StoneHome();

                // logout
                using (new clsFrameLock.LockBuffer())
                    isxwow.WoWScript("Quit()");

                // wait 60 seconds for the logout/quit
                Thread.Sleep(60000);

                // force shutdown
                Shutdown();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Logout");
            }            
        }

        /// <summary>
        /// Logs out after the specified time.
        /// </summary>
        /// <param name="LogoutTime">the time to logout</param>
        public static void LogoutAtTime(DateTime LogoutTime)
        {
            try
            {
                // cancel thread if it is running
                KillThread(LogoutTimeThread, "Killing Previous Logout Thread");

                // if minvalue, then exit
                if (LogoutTime == DateTime.MinValue)
                    return;

                // launch in a new thread
                LogoutAtTimeThread latt = new LogoutAtTimeThread();
                Thread thread = new Thread(latt.LogoutAtTime_Thread);
                LogoutTimeThread = new ThreadItem(thread, latt);
                thread.Name = "Logout Out At Time";
                ThreadList.Add(LogoutTimeThread);
                thread.Start(LogoutTime);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "LogoutAtTime");
            }
        }

        internal class LogoutAtTimeThread : ThreadBase
        {
            public void LogoutAtTime_Thread(object objLogoutTime)
            {
                try
                {
                    // convert the object
                    DateTime LogoutTime = (DateTime)objLogoutTime;

                    // sleep for the specified time
                    while (DateTime.Now.Ticks < LogoutTime.Ticks)
                    {
                        // exit if shutting down
                        if (Shutdown)
                            return;

                        // sleep 2 seconds
                        Thread.Sleep(new TimeSpan(0, 0, 2));
                    }

                    // logout
                    Logout(true);
                }

                catch (Exception excep)
                {
                    clsError.ShowError(excep, "LogoutAtTime_Thread");
                }
            }
        }

        // Logout
        #endregion

        #region Misc Functions

        /// <summary>
        /// returns true ONLY if the entire string is numeric. Returns false on empty string
        /// </summary>
        /// <param name="input">the string to test</param>
        public static bool IsNumeric(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            return new Regex(@"^[0-9]*\.?[0-9]+$").IsMatch(input);
        }

        /// <summary>
        /// Creates a list of strings for the textbox
        /// </summary>
        /// <param name="stringList">the list to parse</param>
        public static string BuildListString(List<string> stringList)
        {
            // exit if no list
            if ((stringList == null) || (stringList.Count == 0))
                return string.Empty;

            // build list
            StringBuilder sb = new StringBuilder();
            foreach (string item in stringList)
                sb.AppendFormat("{0}\r\n", item);

            // return the list
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Builds the save list
        /// </summary>
        public static List<string> BuildSaveList(TextBox tBox)
        {
            // exit if nothing in the list
            if (string.IsNullOrEmpty(tBox.Text.Trim()))
                return new List<string>();

            // build the list
            List<string> retList = new List<string>();
            retList.AddRange(tBox.Lines);
            return retList;
        }

        // Misc Functions
        #endregion

        #region Encrypt / Decrypt

        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="text">the text to encrypt</param>
        public static string EncryptString(string text)
        {
            // exit if invalid
            if (!GuidValid)
                return string.Empty;

            return new Crypt().EncryptString(text);
        }

        /// <summary>
        /// Decrypts the string.
        /// </summary>
        /// <param name="text">The string to decrypt.</param>
        public static string DecryptString(string text)
        {
            // exit if invalid
            if (!GuidValid)
                return string.Empty;

            return new Crypt().DecryptString(text);
        }

        // Encrypt / Decrypt
        #endregion

        #region Threads

        /// <summary>
        /// kills the thread
        /// </summary>
        /// <param name="tItem">the thread to kill</param>
        /// <param name="StopMsg">message to log on stop</param>
        internal static void KillThread(ThreadItem tItem, string StopMsg)
        {
            try
            {
                Application.DoEvents();

                if ((tItem != null) && (tItem.thread != null) && (tItem.thread.IsAlive))
                {
                    // log it
                    if ((! string.IsNullOrEmpty(tItem.thread.Name)) && (Logging != null))
                        Logging.AddToLogFormatted("KillThread", "Killing thread '{0}'", tItem.thread.Name);

                    // set the shutdown
                    ((ThreadBase)tItem.ClassRunning).Shutdown = true;

                    // join for 5 seconds
                    tItem.thread.Join(new TimeSpan(0, 0, 5));

                    // stop our thread
                    if ((! string.IsNullOrEmpty(StopMsg)) && (Logging != null))
                        Logging.AddToLog(StopMsg);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "KillThread");
            }

            finally
            {
                // make sure thread is dead
                if ((tItem != null) && (tItem.thread != null) && (tItem.thread.IsAlive))
                    tItem.thread.Abort();
                if (tItem != null)
                    tItem.thread = null;
            }
        }

        /// <summary>
        /// Closes and removes all threads from the thread list
        /// </summary>
        internal static void ClearThreadList()
        {
            try
            {
                // loop once to kill all threads
                int i, j = ThreadList.Count;
                for (i = 0; i < j; i++)
                {
                    // get the thread item
                    ThreadItem tItem = ThreadList[i];

                    // kill the thread
                    KillThread(tItem, string.Empty);
                }

                // empty the list
                ThreadList.Clear();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "ClearThreadLIst");
            }
        }

        // Threads
        #endregion

        #region Quick Start Nag

        /// <summary>
        /// Shows the quick start prompt
        /// </summary>
        public static void ShowQuickStartNag()
        {
#if Rhabot
            try
            {
                if (!ShowQuickGuideNag)
                    return;

                // show the nag prompt
                if (MessageBox.Show("Would you like to see the Rhabot Quick Start Guide?\r\n\n If you click No, you can always see the guide by clicking Help > \"Rhabot Quick Start Guide\". Click Yes to view the guide now", "Rhabot Quick Start Guide", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    // user clicked No
                    ShowQuickGuideNag = false;
                    return;
                }

                // user clicked yes
                ShowQuickGuideNag = true;

                // show the guide
                ExecuteFile(@"http://www.rhabot.com/rhabot-videos.aspx");
            }
            catch { }
#endif
        }

        /// <summary>
        /// Executes a file
        /// </summary>
        /// <param name="Filename"></param>
        public static void ExecuteFile(string Filename)
        {
            try
            {
                // open the web browser
                Process proc = new Process();
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.FileName = Filename;
                proc.Start();
            }

            catch { }
        }

        // Quick Start Nag
        #endregion

        #region XML Get

        internal static string XMLGet_String(string section, string entryName, Xml xml)
        {
            try
            {
                //Logging.AddToLogFormatted("EntryName = {0}", entryName);
                if (xml.HasEntry(section, entryName))
                    return xml.GetValue(section, entryName).ToString();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "XMLGet_String");
            }

            return string.Empty;
        }

        internal static uint XMLGet_Uint(string section, string entryName, Xml xml)
        {
            try
            {
                //Logging.AddToLogFormatted("EntryName = {0}", entryName);
                if (xml.HasEntry(section, entryName))
                    return Convert.ToUInt32(xml.GetValue(section, entryName));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "XMLGet_Uint");
            }

            return 0;
        }

        internal static bool XMLGet_Bool(string section, string entryName, Xml xml)
        {
            try
            {
                //Logging.AddToLogFormatted("EntryName = {0}", entryName);
                if (xml.HasEntry(section, entryName))
                    return Convert.ToBoolean(xml.GetValue(section, entryName));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "XMLGet_Bool");
            }

            return false;
        }

        internal static bool XMLGet_Bool(string section, string entryName, Xml xml, bool DefaultValue)
        {
            try
            {
                //Logging.AddToLogFormatted("EntryName = {0}", entryName);
                if (xml.HasEntry(section, entryName))
                    return Convert.ToBoolean(xml.GetValue(section, entryName));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "XMLGet_Bool");
            }

            return DefaultValue;
        }

        internal static double XMLGet_Double(string section, string entryName, Xml xml)
        {
            try
            {
                //Logging.AddToLogFormatted("EntryName = {0}", entryName);
                if (xml.HasEntry(section, entryName))
                    return Convert.ToDouble(xml.GetValue(section, entryName));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "XMLGet_Double");
            }

            return 0;
        }

        internal static int XMLGet_Int(string section, string entryName, Xml xml)
        {
            try
            {
                //Logging.AddToLogFormatted("EntryName = {0}", entryName);
                if (xml.HasEntry(section, entryName))
                    return Convert.ToInt32(xml.GetValue(section, entryName));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "XMLGet_Int");
            }

            return 0;
        }

        internal static int XMLGet_Int(string section, string entryName, Xml xml, int DefaultVal)
        {
            try
            {
                //Logging.AddToLogFormatted("EntryName = {0}", entryName);
                if (xml.HasEntry(section, entryName))
                    return Convert.ToInt32(xml.GetValue(section, entryName));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "XMLGet_Int");
            }

            return DefaultVal;
        }

        internal static long XMLGet_Long(string section, string entryName, Xml xml)
        {
            try
            {
                //Logging.AddToLogFormatted("EntryName = {0}", entryName);
                if (xml.HasEntry(section, entryName))
                    return Convert.ToInt64(xml.GetValue(section, entryName));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "XMLGet_Long");
            }

            return 0;
        }

        // XML Get
        #endregion

        // Functions
        #endregion
    }
}
