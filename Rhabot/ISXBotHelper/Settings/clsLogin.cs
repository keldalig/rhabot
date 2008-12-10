using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;
using MerlinEncrypt;
using Rhabot;
using rs;
using System.Net.NetworkInformation;

namespace ISXBotHelper.Settings
{
    /// <summary>
    /// Handles Login of Rhabot to Rhabot Service
    /// </summary>
    public static class clsLogin
    {
        #region Variables

        // instance guid
        private static string InstanceGUID = string.Empty;

        // Variables
        #endregion

        #region Properties

        private static string m_MerlinAd = string.Empty;
        /// <summary>
        /// Ads to display if free account
        /// </summary>
        public static string MerlinAd
        {
            get { return m_MerlinAd; }
        }

        private static string m_DownloadLink = string.Empty;
        /// <summary>
        /// Link to download Rhabot if version is out of sync
        /// </summary>
        public static string DownloadLink
        {
            get { return m_DownloadLink; }
        }

        private static string m_ServerMessage = string.Empty;
        /// <summary>
        /// Message from the server (from me)
        /// </summary>
        public static string ServerMessage
        {
            get { return m_ServerMessage; }
        }

        // Properties
        #endregion

        #region Login

        /// <summary>
        /// Attemps to log in
        /// </summary>
        public static string Login(string username, string password)
        {
            // returning empty string is an error
            Crypt crypt = new Crypt();
            clsSettings.LoginInfo.UserGUID = "123";
            int DaysLeft;

            try
            {
                // log it
                clsSettings.Logging.AddToLog(Resources.Login, Resources.LoggingIntoRhabot);

                // update settings first
                clsSettings.Username = username;

                // if the user used a real login, the update HasLogin
                string nologin = clsGlobals.ConvertToString(new byte[32] { 80, 171, 201, 39, 235, 192, 244, 146, 184, 5, 101, 19, 6, 201, 222, 62, 147, 35, 67, 122, 238, 159, 42, 88, 112, 224, 88, 247, 3, 208, 58, 22 });
                if (username.Trim() != nologin.Trim())
                    clsSettings.HasLogin = true;

                // encryp the password
                string encPswd = crypt.EncryptString(password);

                // try to login
                string loginInfoStr;
                try
                {
                    loginInfoStr = new clsRS().Login(username, encPswd, clsSettings.BotVersion, BuildExtraInfo());
                }

                catch (Exception excep)
                {
                    Debug.WriteLine(excep.Message);

                    MessageBox.Show(Resources.RhabotServerNotFoundError);

                    // return the guid
                    return string.Empty;
                }

                // exit if no string
                if (string.IsNullOrEmpty(loginInfoStr))
                {
                    clsSettings.Logging.AddToLog(Resources.Login, Resources.RhabotLoginFailed);
                    return string.Empty;
                }

                // decrypt and split the string
                string[] LogStrA = Regex.Split(crypt.DecryptString(loginInfoStr), "~~");

                // UserID, UserGUID, Email, IsBetaTester, Isxwow_Name, IsPaid, ad, 
                // DownloadLink, ServerMessage, LoginSuccessful, DaysRemaining

                // set ads
                m_MerlinAd = LogStrA[6].Trim();

                // set download link
                m_DownloadLink = LogStrA[7].Trim();

                // set server message
                m_ServerMessage = LogStrA[8].Trim();

                // if free trial expired, show message box
                DaysLeft = Convert.ToInt32(LogStrA[10]);
                if (DaysLeft > -1)
                {
                    // trial expired, show the dialog if we've not shown it before
                    if ((DaysLeft == 0) && (clsSettings.ShowTrialExpire))
                    {
                        MessageBox.Show(Resources.FreeTrialExpired, Resources.TrialExpiredTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        clsSettings.Logging.AddToLog(Resources.Login, Resources.FreeTrialExpired);
                        clsSettings.ShowTrialExpire = false;
                    }

                    // trial still continuing, show days left
                    if (DaysLeft > 0)
                    {
                        MessageBox.Show(string.Format(Resources.FreeTrialExpireAfterXDays, DaysLeft.ToString().Trim()), Resources.TrialExpiredTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clsSettings.Logging.AddToLogFormatted(Resources.Login, Resources.FreeTrialExpireAfterXDays, DaysLeft.ToString().Trim());
                    }
                }

                // if invalid login, return empty string
                if (!Convert.ToBoolean(LogStrA[9]))
                {
                    clsSettings.Logging.AddToLog(Resources.Login, Resources.RhabotLoginFailed);
                    return string.Empty;
                }

                // set username and password fields
                if (username.ToLower() != "nologin")
                {
                    clsSettings.LoginInfo.Username = username;
                    clsSettings.LoginInfo.Password = password;
                }

                // pop fields
                clsSettings.LoginInfo.IsBetaTester = Convert.ToBoolean(LogStrA[3]);
                clsSettings.LoginInfo.IsPaid = (DaysLeft > 0) ? true : Convert.ToBoolean(LogStrA[5]);
                clsSettings.LoginInfo.UserID = Convert.ToInt32(LogStrA[0]);
                if (LogStrA[1].Length > 36)
                    LogStrA[1] = LogStrA[1].Substring(0, 36);
                clsSettings.LoginInfo.UserGUID = LogStrA[1].Trim();

                // only set nologinguid if we are 'nologin'
                if (username == Resources.nologin)
                    clsSettings.NologinGUID = clsSettings.LoginInfo.UserGUID;
                if ((!string.IsNullOrEmpty(LogStrA[2])) && (LogStrA[2].Length > 2))
                    clsSettings.LoginInfo.Email = crypt.DecryptString(LogStrA[2].Trim());
                if ((!string.IsNullOrEmpty(LogStrA[4])) && (LogStrA[4].Length > 2))
                    clsSettings.LoginInfo.Isxwow_Name = crypt.DecryptString(LogStrA[4]);

                // get the update guid
                clsSettings.LoginInfo.UpdateGUID = LogStrA[11];
                clsSettings.LoginInfo.UpdateKey = LogStrA[12];
                clsSettings.LoginInfo.ExpireInfo = LogStrA[13];
                InstanceGUID = LogStrA[14];

                // start ad thread if not full version
                if (!clsSettings.IsFullVersion)
                    ThreadPool.QueueUserWorkItem(Ads_Thread);

                // start the guid update thread
                GuidUpdateThread gut = new GuidUpdateThread();
                Thread GUIDThread = new Thread(gut.GuidUpdate_Thread);
                GUIDThread.Name = "GUID Updater";
                clsSettings.ThreadItem gThreadItem = new clsSettings.ThreadItem(GUIDThread, gut);
                clsSettings.GlobalThreadList.Add(gThreadItem);
                GUIDThread.Start();

                // return the wrapped guid
                clsSettings.Logging.AddToLog(Resources.Login, Resources.RhabotLoginSuccessful);
                return clsSettings.LoginInfo.UserGUID;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.Login);
                clsSettings.LoginInfo.IsBetaTester = false;
                clsSettings.LoginInfo.IsPaid = false;
                clsSettings.LoginInfo.LoginSuccessful = false;
            }

            // fail message
            clsSettings.Logging.AddToLog(Resources.Login, Resources.RhabotLoginFailed);
            return string.Empty;
        }

        /// <summary>
        /// Builds the extra information to send to the server
        /// </summary>
        private static string BuildExtraInfo()
        {
            StringBuilder sb = new StringBuilder();
            string delimit = ":::";

            try
            {
                // build the extra info string
                using (new clsFrameLock.LockBuffer())
                {
                    sb.AppendFormat("{0}{1}", clsSettings.isxwow.AccountName, delimit);
                    sb.AppendFormat("{0}{1}", GuidUpdateThread.GetPCName() /*SystemInformation.ComputerName*/, delimit);

                    // no login guid
                    sb.AppendFormat("{0}{1}", clsSettings.NologinGUID, delimit);

                    // instance GUID
                    sb.AppendFormat("{0}{1}", Guid.NewGuid().ToString().Trim().Replace("{", string.Empty).Replace("}", string.Empty), delimit);

                    // Num Instance of Rhabot already running
                    sb.AppendFormat("{0}{1}", GetRunningInstances().ToString().Trim(), delimit);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.Login);
            }

            // return the encrypted string
            if (sb.Length > 0)
                return new Crypt().EncryptString(sb.ToString());

            return string.Empty;
        }

        /// <summary>
        /// Checks if this application is already running
        /// </summary>
        private static int GetRunningInstances()
        {
            // http://www.thescripts.com/forum/thread237932.html
            // get the name of our process
            string proc = Process.GetCurrentProcess().ProcessName;

            // get the list of all processes by that name
            return Process.GetProcessesByName(proc).Length - 1;
        }

        // Login
        #endregion

        #region Create User

        /// <summary>
        /// Creates a new user
        /// </summary>
        public static bool CreateUser(string username, string password, string email, string isxwow_name, bool IsBeta, bool IsPaid, string key)
        {
#if DEBUG
                        Crypt crypt = new Crypt();
            
                        try
                        {
                            // exit if no password/username
                            if ((string.IsNullOrEmpty(username)) || (string.IsNullOrEmpty(password)))
                                return false;
            
                            // log it
                            LogCreateError(new Exception(string.Format("Rhabot Create User: Username = {0}; IsBeta = {1}; IsPaid = {2}; Email = {3}; Isxwow Name = {4}",
                                username, IsBeta.ToString(), IsPaid.ToString(), email, isxwow_name)));
            
                            return new rs.UserService.UserService().CreateUserEx(
                                username, crypt.EncryptString(password),
                                string.IsNullOrEmpty(email) ? string.Empty : crypt.EncryptString(email),
                                string.IsNullOrEmpty(isxwow_name) ? string.Empty : crypt.EncryptString(isxwow_name),
                                IsBeta, IsPaid,
                                crypt.EncryptString("{5B0521CC-CB5D-4346-8979-7FFFDD144902}"),
                                0, "Spry981 Created",
                                false, // IsUberGoldBot
                                5);
                        }
            
                        catch (Exception excep)
                        {
                            LogCreateError(excep);
                        }
#endif

            return false;
        }

        /// <summary>
        /// Logs the error for create user
        /// </summary>
        private static void LogCreateError(Exception excep)
        {
            try
            {
                new clsRS().ErrorServiceLog(string.Empty, excep.Message, string.Empty, 0, string.Empty);
            }
            catch { }
        }

        // Create User
        #endregion

        #region Ad Thread

        /// <summary>
        /// Sleeps for 30 minutes, then gets a new ad
        /// </summary>
        public static void Ads_Thread(object objState)
        {
            DateTime startTime = DateTime.Now;

            try
            {
                while (!clsSettings.IsShuttingDown)
                {
                    // sleep for 2 seconds
                    Thread.Sleep(2000);

                    // exit if shutting down
                    if (clsSettings.IsShuttingDown)
                        return;

                    // if it has been less 30 minutes since we started, sleep again
                    if (new TimeSpan(DateTime.Now.Ticks - startTime.Ticks).Minutes < 30)
                        continue;

                    // reset start time
                    startTime = DateTime.Now;

                    // try to get new ads from the rhabot service
                    string newAd = new clsRS().GetAdBlock();
                    Application.DoEvents();

                    // raise the event
                    clsEvent.Raise_AdsReceived(newAd);
                }
            }

            catch (Exception excep)
            {
                // exit if thread aborting
                if (excep.Message.Contains(Resources.abort))
                    return;

                // not thread issue, log it
                clsError.ShowError(excep, Resources.AdThread, string.Empty, false, new StackFrame(0, true), true);
            }
        }

        // Ad Thread
        #endregion

        #region GUID_Update

        internal class GuidUpdateThread : ThreadBase
        {
            /// <summary>
            /// Returns the nic name
            /// </summary>
            internal static string GetPCName()
            {
                // get the first mac id (should be Local Network)
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                //foreach (NetworkInterface adapter in nics)
                //{
                //    IPInterfaceProperties properties = adapter.GetIPProperties();
                //    Debug.WriteLine(adapter.Name);
                //    Debug.WriteLine(adapter.GetPhysicalAddress().ToString());
                //    Debug.WriteLine("--");
                //}

                return nics[0].GetPhysicalAddress().ToString().Trim();
            }

            public void GuidUpdate_Thread()
            {
                StringBuilder sb = new StringBuilder();
                string delimit = ":::";
                clsRS crs = new clsRS();

                try
                {
                    // build the update info
                    sb.AppendFormat("{0}{1}", clsSettings.LoginInfo.UserID.ToString().Trim(), delimit);
                    sb.AppendFormat("{0}{1}", clsSettings.LoginInfo.UserGUID, delimit);
                    
                    // computer name / NIC MAC ID
                    //sb.AppendFormat("{0}{1}", SystemInformation.ComputerName, delimit);
                    sb.AppendFormat("{0}{1}", GetPCName(), delimit);

                    sb.AppendFormat("{0}{1}", clsSettings.Username, delimit);
                    sb.AppendFormat("{0}{1}", InstanceGUID, delimit);

                    // encrypt it
                    clsSettings.UpdateText = new Crypt().EncryptString(sb.ToString(), clsSettings.LoginInfo.UpdateKey);

                    // sleep for one sec
                    Thread.Sleep(1000);

                    // mark us as not disconnected, for the first attempt
                    clsSettings.IsDCd = false;

                    // thread loop
                    while ((!Shutdown) && (!clsSettings.IsShuttingDown))
                    {
                        // update the server, if returned false, it probably means the guid is invalid
                        bool gResult = crs.UpdateGUID(clsSettings.UpdateText, clsSettings.LoginInfo.UserID, clsSettings.IsDCd);

                        // stop the running bot
                        if (!gResult)
                        {
                            // log it
                            clsSettings.Logging.AddToLog(Resources.GUIDUpdate, "Disconnected from server. Shutting down");

                            // set the isdc'd flag
                            clsSettings.IsDCd = true;

                            // raise the bad gupdate event
                            clsGlobals.Raise_FloatStopped();
                            clsGlobals.Raise_BadGUpdate();

                            // force shutdown of bot
                            ForceShutdown();
                            return;
                        }

                        // sleep for 5 minutes
                        DateTime WakeUpTime = DateTime.Now.AddMinutes(5);
                        while ((!clsSettings.IsShuttingDown) &&
                            (DateTime.Now < WakeUpTime))
                            Thread.Sleep(new TimeSpan(0, 0, 3)); // sleep 3 seconds

                        // exit if shutting down
                        if ((Shutdown) || (clsSettings.IsShuttingDown))
                            return;
                    }
                }

                catch (Exception excep)
                {
                    clsError.ShowError(excep, Resources.GUIDUpdate);
                }
            }

            private void ForceShutdown()
            {
                try
                {
                    clsGlobals.Raise_FloatStopped();

                    // wait a few seconds
                    Thread.Sleep(3000);

                    // notify the user
                    clsError.ShowError(new Exception(Resources.GuidUpdateError), Resources.GUIDUpdate, string.Empty, true, new StackFrame(0, true), false);

                    // stop running bots
                    clsSettings.Stop = true;
                    clsGlobals.Raise_FloatStopped();

                    // wait a few seconds
                    Thread.Sleep(3000);
                }

                catch (Exception excep)
                {
                    clsError.ShowError(excep, Resources.GUIDUpdate);
                }

                finally
                {
                    // shutdown
                    clsSettings.Shutdown();
                    clsSettings.AbortThreads();
                    Application.Exit();
                }
            }
        }

        // GUID_Update
        #endregion
    }
}
