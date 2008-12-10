
namespace ISXBotHelper.Settings
{
    /// <summary>
    /// Summary description for clsLoginInfo
    /// </summary>
    public class clsLoginInfo
    {
        #region Variables

        public int UserID = 0;
        public string UserGUID = string.Empty;
        public string Email = string.Empty;
        public bool IsBetaTester = false;
        public string Isxwow_Name = string.Empty;
        public bool IsPaid = false;
        public string ad = string.Empty;
        public string DownloadLink = string.Empty;
        public string ServerMessage = string.Empty;
        public string UpdateGUID = string.Empty;
        public string UpdateKey = string.Empty;
        public string ExpireInfo = string.Empty;

        public string Username { get; set; }
        public string Password { get; set; }
        
        /// <summary>
        /// Set to false when login fails
        /// </summary>
        public bool LoginSuccessful = false;

        // Variables
        #endregion

        #region Init

        public clsLoginInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the clsLoginInfo class.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userGUID"></param>
        /// <param name="email"></param>
        /// <param name="isBetaTester"></param>
        /// <param name="isxwow_Name"></param>
        /// <param name="isPaid"></param>
        /// <param name="ad"></param>
        /// <param name="downloadLink"></param>
        /// <param name="serverMessage"></param>
        /// <param name="loginSuccessful"></param>
        public clsLoginInfo(int userID, string userGUID, string email, bool isBetaTester, string isxwow_Name, bool isPaid, string ad, string downloadLink, string serverMessage, bool loginSuccessful)
        {
            UserID = userID;
            UserGUID = userGUID;
            Email = email;
            IsBetaTester = isBetaTester;
            Isxwow_Name = isxwow_Name;
            IsPaid = isPaid;
            this.ad = ad;
            DownloadLink = downloadLink;
            ServerMessage = serverMessage;
            LoginSuccessful = loginSuccessful;
        }

        // Init
        #endregion
    }
}