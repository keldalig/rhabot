using System;
using System.Text;

namespace ISXBotHelper.MSNChat
{
    /// <summary>
    /// List of commands that can be sent/received via MSN
    /// </summary>
    internal static class clsMSNCmd
    {
        // NOTE: all commands in this class MUST be capitalized

        #region Commands

        /// <summary>
        /// Text following this line is plain text and can be ignored as command
        /// </summary>
        public const string cmd_PlainText = "/PT: ";

        /// <summary>
        /// This command will cause the msn bot to return the list of available commands
        /// </summary>
        public const string cmd_Help = "/HELP";

        /// <summary>
        /// The bot will hearth home and logout. This is the same as shutdown
        /// </summary>
        public const string cmd_Logout = "/LOGOUT";

        /// <summary>
        /// Returns the player's location (Zone, Location)
        /// </summary>
        public const string cmd_Location = "/LOC";

        /// <summary>
        /// Returns the character's information (level, isdead, isincombat, zone, location)
        /// </summary>
        public const string cmd_CharacterInfo = "/CHARINFO";

        /// <summary>
        /// streams the log output to chat for five minutes
        /// </summary>
        public const string cmd_StreamLog = "/STREAMLOG";

        /// <summary>
        /// Stops streaming the log via chat
        /// </summary>
        public const string cmd_StopStreamLog = "/STOPSTREAM";

        /// <summary>
        /// Hooks the private and say chat events. Private/Say messages will be copied in MSN chat
        /// </summary>
        public const string cmd_HookChat = "/HOOKCHAT";

        /// <summary>
        /// The bot stops moving and hearths home
        /// </summary>
        public const string cmd_StoneHome = "/STONEHOME";

        /// <summary>
        /// Rhabot generated errors will be sent to the msn user
        /// </summary>
        public const string cmd_HookErrorEvents = "/HOOKERRORS";

        /// <summary>
        /// Tell Rhabot to logout after X seconds. Send nothing or 0 to cancel.
        /// Usage: /LOGOUTX 120
        /// </summary>
        public const string cmd_LogoutX = "/LOGOUTX";

        /// <summary>
        /// Pauses the Rhabot script
        /// </summary>
        public const string cmd_PauseScript = "/PAUSESCRIPT";

        /// <summary>
        /// Unpauses the Rhabot script
        /// </summary>
        public const string cmd_UnpauseScript = "/UNPAUSESCRIPT";

        /// <summary>
        /// Stops the Rhabot script
        /// </summary>
        public const string cmd_StopScript = "/STOPSCRIPT";

        /// <summary>
        /// Returns a list of all items in your inventory
        /// </summary>
        public const string cmd_InvetoryList = "/INVENTORY";

        // Commands
        #endregion

        #region Functions

        /// <summary>
        /// Builds the help string
        /// </summary>
        /// <returns></returns>
        public static string GetHelp()
        {
            StringBuilder sb = new StringBuilder();
            string nl = "\r\n";

            try
            {
                sb.AppendFormat("ISXBotHelper - MSN Chat Help{0}", nl);
                sb.AppendFormat("'{0}' - this help message{1}", cmd_Help, nl);
                sb.AppendFormat("'{0}' - plain text message. this is ignored by the msn bot{1}", cmd_PlainText, nl);
                sb.AppendFormat("'{0}' - The bot will hearth home and logout. This is the same as shutdown{1}", cmd_Logout, nl);
                sb.AppendFormat("'{0}' - Returns the player's location (Zone, Location){1}", cmd_Location, nl);
                sb.AppendFormat("'{0}' - Returns the character's information (level, isdead, isincombat, zone, location){1}", cmd_CharacterInfo, nl);
                sb.AppendFormat("'{0}' - streams the log output to chat for five minutes{1}", cmd_StreamLog, nl);
                sb.AppendFormat("'{0}' - Stops streaming the log via chat{1}", cmd_StopStreamLog, nl);
                sb.AppendFormat("'{0}' - Hooks the private and say chat events. Private/Say messages will be copied in MSN chat{1}", cmd_HookChat, nl);
                sb.AppendFormat("'{0}' - The bot stops moving and hearths home{1}", cmd_StoneHome, nl);
                sb.AppendFormat("'{0}' - Rhabot generated errors will be sent to the msn user{1}", cmd_HookErrorEvents, nl);
                sb.AppendFormat("'{0}' - Tell Rhabot to logout after X seconds. Send nothing or 0 to cancel. Usage: /LOGOUTX 120{1}", cmd_LogoutX, nl);
                sb.AppendFormat("'{0}' - Pauses the Rhabot script{1}", cmd_PauseScript, nl);
                sb.AppendFormat("'{0}' - Unpauses the Rhabot script{1}", cmd_UnpauseScript, nl);
                sb.AppendFormat("'{0}' - Stops the Rhabot script{1}", cmd_StopScript, nl);
                sb.AppendFormat("'{0}' - Returns a list of all items in your inventory{1}", cmd_InvetoryList, nl);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GetHelp");
                
                // add the error message
                sb.AppendFormat("{0}GetHelp() Error: {1}", nl, excep.Message);
            }
            
            // return the message
            return sb.ToString();
        }

        // Functions
        #endregion
    }
}
