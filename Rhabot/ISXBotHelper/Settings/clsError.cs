using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ISXBotHelper.Communication;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;
using ISXWoW;
using rs;

namespace ISXBotHelper
{
    public class clsError : ThreadBase
    {
        #region Events

        public delegate void RhabotErrorHandler(Exception excep, string Message);
        public static event RhabotErrorHandler RhabotError;

        // Events
        #endregion

        private static ISXError frmError = null;
        private static string ExcepMsg_frm = string.Empty;
        private static string Msg_frm = string.Empty;

        /// <summary>
        /// Called for errors when the user is not at the keyboard
        /// </summary>
        private static void ShowError_Unattended(Exception excep, string msg)
        {
            try
            {
                // email/sms/msn user of error
                clsSend.SendToAll(string.Format("ERROR - {0} \r\n\t\t\t\t{1}" , msg, excep.Message), string.Format("{0} Error - {1}", Resources.Rhabot, excep.Message));
            }
            catch { }
        }

        /// <summary>
        /// Called for errors when the user is at the keyboard
        /// </summary>
        private static void ShowError_Display(Exception excep, string msg)
        {
            // if already showing an error, then exit
            if (frmError != null)
                return;

            // set the variables
            ExcepMsg_frm = excep.Message;
            Msg_frm = msg;

            // load new thread
            clsError err = new clsError();
            Thread thread = new Thread(err.ShowError_Display_Thread);
            thread.Name = "Show Error Display";
            clsSettings.ThreadItem threadItem = new clsSettings.ThreadItem(thread, err);
            clsSettings.ThreadList.Add(threadItem);
            thread.Start();
        }

        /// <summary>
        /// Shows the error form in a new thread
        /// </summary>
        public void ShowError_Display_Thread()
        {
            // load and show the error form
            frmError = new ISXError();
            frmError.lblError.Text = ExcepMsg_frm;
            frmError.lblMessage.Text = Msg_frm;
            frmError.FormClosed += frmError_FormClosed;
            frmError.Show(); // don't show modally, so we can continue to process other stuff

            do
            {
                Application.DoEvents();
                Thread.Sleep(1000);
            } while ((frmError != null) && (frmError.Visible));
        }

        private static void frmError_FormClosed(object sender, FormClosedEventArgs e)
        {
            // unhook closing event
            if (frmError != null)
                frmError.FormClosed -= frmError_FormClosed;

            // reset frmError, so other errors can show
            frmError = null;
        }

        /// <summary>
        /// Shows the appropriate error when a user is here or unattended
        /// </summary>
        /// <param name="excep">the exception</param>
        /// <param name="msg">additional extra information</param>
        /// <param name="LogMessage">when true, the message is saved to the log file also</param>
        /// <param name="stackframe">the stackframe</param>
        /// <param name="LogToService">true to save to merlin error service</param>
        /// <param name="sectionName"></param>
        public static void ShowError(Exception excep, string sectionName, string msg, bool LogMessage, StackFrame stackframe, bool LogToService)
        {
            #region Skip_If

            // thread abort
            if (excep.GetType() == typeof(ThreadAbortException))
                return;

            // file not found, don't need to process it
            if (excep.GetType() == typeof(FileNotFoundException))
                return;

            // Skip_If
            #endregion

            // don't log to service if...
            if ((excep.Message.Contains("Could not find a part of the path")) ||
                (excep.Message.Contains("Impossibile trovare")) ||
                (excep.Message.Contains("Impossible de trouver")) ||
                (excep.Message.Contains("Ein Teil des Pfades")) ||
                (excep.Message.Contains("Method not found: 'Boolean ISXWoW.WoWUnit.get_IsTrainer()'")) ||
                (excep.Message.Contains("Der Thread wurde abgebrochen")) ||
                (excep.Message.Contains("Die Auflistung wurde geändert")) ||
                (excep.Message.Contains("Thread interrotto")) ||
                (excep.Message.Contains("Thread") && (excep.Message.Contains(Resources.abort))) ||
                (excep.Message.Contains("Collection was modified") && 
                (excep.StackTrace.Contains("Application.ExitInternal"))))
                    LogToService = false;

            /*
            // object reference not found
            if (excep.GetType() == typeof(NullReferenceException))
            {
                // create a new message to show, and don't log to the web service
                excep = new Exception(clsEnums.GetResource(clsEnums.EResourceName.ObjRefDownloadInstructions));
                LogMessage = true;
                LogToService = false;
            }
            */

            // update msg
            if (sectionName != String.Empty)
            {
                if (! string.IsNullOrEmpty(msg))
                    msg = string.Format("{0} : {1}", sectionName, msg);
                else
                    msg = sectionName;
            }

            // send to the debugger
            string logMsg = string.Format("ERROR - {0} \r\n\t\t\t\t{1}", msg, excep);
            try
            { 
                if (clsSettings.Logging != null)
                    clsSettings.Logging.DebugWrite(logMsg); 
            }
            catch { }

            // try to log the error
            if (LogMessage)
            {
                try
                {
                    if (clsSettings.Logging != null)
                        clsSettings.Logging.AddToLog(logMsg);
                }
                catch { }
            }

            // log to merlin service if allowed
            if (LogToService)
            {
                try
                {
                    // log the error to the rhabot service async
                    new clsRS().ErrorServiceLog(
                        clsSettings.Username,
                        excep.Message,
                        BuildErrorInfoString(excep, stackframe, msg),
                        clsSettings.LoginInfo.UserID,
                        clsSettings.UpdateText);
                }
                catch { }
            }

            // raise the error event
            try
            {
                if (RhabotError != null)
                    RhabotError(excep, msg);
            }
            catch { }

            if (!clsSettings.IsUnattended)
                ShowError_Display(excep, msg);
            else
                ShowError_Unattended(excep, msg);
        }

        /// <summary>
        /// Shows the appropriate error when a user is here or unattended
        /// </summary>
        /// <param name="excep">the exception</param>
        /// <param name="msg">additional extra information</param>
        /// <param name="LogMessage">when true, the message is saved to the log file also</param>
        /// <param name="stackframe">the stackframe</param>
        /// <param name="LogToService">true to save to merlin error service</param>
        public static void ShowError(Exception excep, string msg, bool LogMessage, StackFrame stackframe, bool LogToService)
        {
            ShowError(excep, string.Empty, msg, LogMessage, stackframe, LogToService);
        }

        /// <summary>
        /// Shows the appropriate error when a user is here or unattended
        /// </summary>
        /// <param name="excep">the exception</param>
        /// <param name="msg">additional extra information</param>
        /// <param name="LogMessage"></param>
        public static void ShowError(Exception excep, string msg, bool LogMessage)
        {
            ShowError(excep, string.Empty, msg, LogMessage, new StackFrame(1, true), true);
        }

        /// <summary>
        /// Shows the appropriate error when a user is here or unattended
        /// </summary>
        /// <param name="excep">the exception</param>
        /// <param name="msg">additional extra information</param>
        public static void ShowError(Exception excep, string msg)
        {
            ShowError(excep, string.Empty, msg, true, new StackFrame(1, true), true);
        }

        /// <summary>
        /// Shows the appropriate error when a user is here or unattended
        /// </summary>
        /// <param name="excep">the exception</param>
        /// <param name="msg">additional extra information</param>
        /// <param name="sectionName"></param>
        public static void ShowError(Exception excep, string sectionName, string msg)
        {
            ShowError(excep, sectionName, msg, true, new StackFrame(1, true), true);
        }

        #region Rhabot Service

        /// <summary>
        /// Creates a string that can be used for sending error emails and saving to the database
        /// </summary>
        /// <param name="excep">the raised exception</param>
        /// <param name="Message"></param>
        /// <param name="stackFrame"></param>
        public static string BuildErrorInfoString(Exception excep, StackFrame stackFrame, string Message)
        {
            StringBuilder sb = new StringBuilder();
            string nl = "\r\n", tab = "\t";

            using (new clsFrameLock.LockBuffer())
            {
                // add user information
                sb.AppendFormat("User Information: {0}", nl);
                if (!string.IsNullOrEmpty(clsSettings.Username))
                    sb.AppendFormat("{0}Rhabot Username: {1}{2}", tab, clsSettings.Username, nl);
                if (clsSettings.Username.Contains(Resources.nologin))
                    sb.AppendFormat("{0}CN: {1}{2}", tab, SystemInformation.ComputerName, nl);
                if (!string.IsNullOrEmpty(clsSettings.LoginInfo.Isxwow_Name))
                    sb.AppendFormat("{0}Isxwow Username: {1}{2}", tab, clsSettings.LoginInfo.Isxwow_Name, nl);
                if (!string.IsNullOrEmpty(clsSettings.LoginInfo.Email))
                    sb.AppendFormat("{0}User Email: {1}{2}", tab, clsSettings.LoginInfo.Email, nl);
                sb.AppendFormat("{0}Paid Subscription: {1}{2}", tab, clsSettings.LoginInfo.IsPaid.ToString().Trim(), nl);
                sb.AppendFormat("{0}Beta Tester: {1}{2}", tab, clsSettings.LoginInfo.IsBetaTester.ToString().Trim(), nl);
                sb.AppendFormat("{0}UserGUID: {1}{2}", tab, clsSettings.LoginInfo.UserGUID, nl);
                sb.AppendFormat("{0}User ID: {1}{2}", tab, clsSettings.LoginInfo.UserID.ToString().Trim(), nl);
                sb.AppendFormat("{0}BotVersion: {1} {2}{2}", tab, clsSettings.BotVersion, nl);

                // character info
                if (clsCharacter.CharacterIsValid)
                {
                    sb.AppendFormat("Character Information: {0}", nl);
                    sb.AppendFormat("{0}Zone: {1}{2}", tab, string.IsNullOrEmpty(clsCharacter.ZoneText) ? clsCharacter.RealZoneText : clsCharacter.ZoneText, nl);
                    if (!string.IsNullOrEmpty(clsSettings.isxwow.SubZoneText))
                        sb.AppendFormat("{0}Sub-Zone: {1}{2}", tab, clsSettings.isxwow.SubZoneText, nl);
                    sb.AppendFormat("{0}Location: {1}{2}", tab, clsCharacter.MyLocation, nl);
                    sb.AppendFormat("{0}Level: {1} {2}", tab, clsCharacter.CurrentLevel, nl);

                    // target
                    WoWUnit currTarget = clsCharacter.CurrentTarget;
                    if ((currTarget != null) && (currTarget.IsValid) && (currTarget.GUID != clsSettings.isxwow.Me.GUID))
                    {
                        sb.AppendFormat("{1}{0}Target Information: {1}", tab, nl);
                        sb.AppendFormat("{0}Target Name: {1}{2}", tab, currTarget.Name, nl);
                        sb.AppendFormat("{0}Target Level: {1}{2}", tab, currTarget.Level, nl);
                    }
                    sb.Append(nl);
                }

                // PC Info
                sb.AppendFormat("PC Info:{0}", nl);
                sb.AppendFormat("{0}CN: {1}{2}", tab, SystemInformation.ComputerName, nl);
                if ((LocalIP != null) && (LocalIP.Count > 0))
                {
                    sb.AppendFormat("{0}IP: {1}", tab, nl);
                    foreach (string userIP in LocalIP)
                        sb.AppendFormat("{0}{0}{1}{2}", tab, userIP, nl);
                }
                sb.AppendFormat("{0}OS Version: {1}{2}", tab, Environment.OSVersion.VersionString, nl);
                
                // stack frame info
                MethodBase methodBase = stackFrame.GetMethod();
                sb.AppendFormat("Exception Location:{0}", nl);
                if (!string.IsNullOrEmpty(Message))
                    sb.AppendFormat("Message: {0}{1}", Message, nl);
                sb.AppendFormat("{0}Namespace: {1}{2}", tab, methodBase.ReflectedType.Namespace, nl);
                sb.AppendFormat("{0}Class Name: {1}{2}", tab, methodBase.ReflectedType.Name, nl);
                sb.AppendFormat("{0}Method Name: {1}{2}", tab, methodBase.Name, nl);

                // build the exception info string
                sb.AppendFormat("Exception Information:{0}", nl);
                sb.AppendFormat("{2}Message: {0}{1}", excep.Message, nl, tab);
                sb.AppendFormat("{2}Source: {0}{1}{1}", excep.Source, nl, tab);
                sb.AppendFormat("Stack Trace: {1} {0}{1}{1}", excep.StackTrace, nl);

                // inner exception
                if (excep.InnerException != null)
                {
                    sb.AppendFormat("{1}{0}Inner Exception Info:{0}", nl, tab);
                    sb.AppendFormat("{2}{2}Message: {0}{1}", excep.InnerException.Message, nl, tab);
                    sb.AppendFormat("{2}{2}Source: {0}{1}", excep.InnerException.Source, nl, tab);
                    sb.AppendFormat("{2}{2}Stack Trace: {0}{1}", excep.InnerException.StackTrace, nl, tab);
                }
            }

            // retun
            return sb.ToString();
        }

        // Rhabot Service
        #endregion

        #region Special Errors

        /// <summary>
        /// Shows an error window and stops Rhabot. Does NOT log to the error service
        /// </summary>
        /// <param name="ExcepMessage"></param>
        public static void ShowSpecialError(string ExcepMessage)
        {
            ShowError(
                new Exception(ExcepMessage), string.Empty,
                true, new StackFrame(1, true), false);

            // stop
            clsSettings.Stop = true;

            return;
        }

        // Special Errors
        #endregion

        #region Functions

        private static List<string> LocalIP
        {
            get
            {
                List<string> retList = new List<string>();

                try
                {
                    // get all interfaces on this computer and list them
                    IPHostEntry hosts = Dns.GetHostEntry(Dns.GetHostName());
                    if (hosts.AddressList.Length == 0)
                        return retList;

                    for (int i = 0; i < hosts.AddressList.Length; i++)
                        retList.Add(hosts.AddressList[i].ToString());
                }

                catch //(Exception excep)
                {                    
                }

                return retList;
            }
        }

        // Functions
        #endregion
    }
}
