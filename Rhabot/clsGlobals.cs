using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;
using MerlinEncrypt;
using rs;

namespace Rhabot
{
    internal static class clsGlobals
    {
        #region Global Variables

        internal const string ItemSellFile = "ItemSell.xml"; // global
        internal const string SettingsFile = "Settings.xml";
        internal const string PathListFile = "BotPath.xml";
        internal const string GlobalSettingsFile = "GlobalSettings.xml"; // global
        internal const string LevelStartFile = "LevelStart.xml";
        internal const string LevelEndFile = "LevelEnd.xml";
        internal const string QuestsFile = "Quests.xml"; // global

        internal const string Path_Mailbox = "Mailbox";
        internal const string Path_Graveyard2 = "Graveyard2";
        internal const string Path_Graveyard1 = "Graveyard1";
        internal const string Path_Hunting = "Hunting";
        internal const string Path_Vendor = "Vendor";
        internal const string Path_StartHunt = "StartHunt";
        internal const string Path_IndividPath = "Individual_Path";

        public static int SettingsLevel = 0;

        public static ISXMain frmMain = null;

        private static readonly Crypt crypt = new Crypt();

        // Global Variables
        #endregion

        #region Settings Event

        /// <summary>
        /// Raised when settings have been loaded
        /// </summary>
        public static event EventHandler SettingsLoaded;

        public static void Raise_SettingsLoaded()
        {
            if (SettingsLoaded != null)
                SettingsLoaded(null, new EventArgs());
        }


        /// <summary>
        /// Raised when the user clicks to stop rhabot on the float window
        /// </summary>
        public static event EventHandler FloatStopped;

        public static void Raise_FloatStopped()
        {
            if (FloatStopped != null)
                FloatStopped(null, new EventArgs());
        }

        // Settings Event
        #endregion

        #region Bad_GUpdate

        /// <summary>
        /// Raised when GUpdate has returned false
        /// </summary>
        public static event EventHandler BadGUpdate;

        public static void Raise_BadGUpdate()
        {
            if (BadGUpdate != null)
                BadGUpdate(null, new EventArgs());
        }

        // Bad_GUpdate
        #endregion
        
        #region Properties

        #region Registry

        /// <summary>
        /// Last used settings folder
        /// </summary>
        public static string LastSettingsFolder
        {
            get { return clsSettings.LastSettingsFolder; }
            set { clsSettings.LastSettingsFolder = value; }
        }

        public static string LastChatterFile
        {
            get { return clsSettings.LastChatterFile; }
            set { clsSettings.LastChatterFile = value; }
        }

        // Registry
        #endregion

        /// <summary>
        /// list of loaded paths
        /// </summary>
        public static List<clsPath.PathListInfoEx> Paths = new List<clsPath.PathListInfoEx>();

        /// <summary>
        /// Running Time, set by uscMainInfo
        /// </summary>
        public static TimeSpan RunningTime { get; set; }
        
        // Properties
        #endregion

        #region Functions

        public static int SetNumericValue(int setting, NumericUpDown numBox)
        {
            if (setting < numBox.Minimum)
                return (int)numBox.Minimum;
            else if (setting > numBox.Maximum)
                return (int)numBox.Maximum;
            else
                return setting;
        }

        internal static string ConvertToString(byte[] stringBytes)
        {
            return Encoding.Unicode.GetString(crypt.DecryptByteArray(stringBytes)).Replace("½", string.Empty).Replace(Convert.ToString('\0'), string.Empty).Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\r", "\r");
        }
        
        /// <summary>
        /// Executes a file
        /// </summary>
        /// <param name="Filename"></param>
        public static void ExecuteFile(string Filename)
        {
            clsSettings.ExecuteFile(Filename);
        }

        public static string SetFormText(string text)
        {
            // add ISX to it if it doesn't already exist
            if ((!string.IsNullOrEmpty(text)) && (!text.StartsWith("ISX")))
                return string.Format("ISX {0}", text);
            else
                return text;
        }

        /// <summary>
        /// Pops the group path list
        /// </summary>
        /// <param name="cmbPathGroupName">the combo box to pop</param>
        /// <param name="RemoveNONE">true to remove the NONE group</param>
        public static void PopGroupPathList(ComboBox cmbPathGroupName, bool RemoveNONE)
        {
            try
            {
                // clear the list
                cmbPathGroupName.Items.Clear();

                List<rs.PathService.clsGroupName> svcGroupList = new clsRS().GetGroupList(clsSettings.LoginInfo.UserID, clsSettings.UpdateText, clsSettings.IsDCd);

                // exit if no list
                if ((svcGroupList == null) || (svcGroupList.Count == 0))
                    return;

                // update list
                foreach (rs.PathService.clsGroupName gItem in svcGroupList)
                {
                    // skip if we are to remove NONE
                    if ((RemoveNONE) && (gItem.GroupName.ToLower() == "none"))
                        continue;

                    // update combo
                    cmbPathGroupName.Items.Add(new clsPathGroupItem() 
                        { 
                            GroupName = gItem.GroupName, 
                            GroupNote = clsSerialize.Deserialize_FromByteA(gItem.GroupNote, typeof(string)) as string
                        });
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.PopGroupPathList);
            }
        }

        #region SharingPanels

        public static void ShowSharedProfiles()
        {
            frmMain.tabControl1.SelectedTab = frmMain.tabSharedProfiles;
        }

        public static void ShowSharedPaths()
        {
            frmMain.tabControl1.SelectedTab = frmMain.tabSharedPaths;
        }

        // SharingPanels
        #endregion

        // Functions
        #endregion
    }

    public class clsPathGroupItem
    {
        public string GroupName { get; set; }
        public string GroupNote { get; set; }

        public override string ToString()
        {
            return GroupName;
        }
    }
}
