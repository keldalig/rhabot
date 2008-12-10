using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ISXBotHelper.Threading;

using ISXBotHelper.MSNChat;

namespace ISXBotHelper
{
    public class clsLogging :IDisposable
    {
        #region Variables

        private readonly Queue<string> LogQueue = new Queue<string>();
        private Thread thread = null;
        private delegate void LogDelegate(string logText);
        private readonly clsLogLock LogLock = new clsLogLock();
        private bool HasShutDown = false; // set to true once the logs have been flushed
        private const string Channel = "ISXDebug";

        private const string LogLineBreak = "\r\n----------\r\n";

        private clsSettings.ThreadItem DebugThreadItem = null;

        // Variables
        #endregion

        #region Init / Finalize

        /// <summary>
        /// Initializes a new instance of the clsLogging class.
        /// </summary>
        public clsLogging()
        {
            Startup();
        }

        /// <summary>
        /// Initializes a new instance of the clsLogging class.
        /// </summary>
        /// <param name="Filepath">path to the log file if one is not already set</param>
        public clsLogging(string Filepath)
        {
            // change the log path
            clsSettings.LogFilePath = Filepath;

            Startup();
        }

        /// <summary>
        /// all the startup functions
        /// </summary>
        private void Startup()
        {
            // open the log file
            OpenLogFile();

            // hook the new log entry from the global dll
            ISXRhabotGlobal.clsGlobals.NewLogEntry += clsGlobals_NewLogEntry;
        }

        // Init / Finalize
        #endregion

        #region Lock Class

        private class clsLogLock
        {
            public bool Locked = false;
        }

        // Lock Class
        #endregion

        #region Functions

        /// <summary>
        /// Opens the log file for writing
        /// </summary>
        private void OpenLogFile()
        {
            if (string.IsNullOrEmpty(clsSettings.LogFilePath))
                return;
            
            try
            {
                // close the thread if it is running
                if ((thread != null) && (thread.IsAlive))
                    thread.Abort();

                // add Rhabot's version to the queue
                LogQueue.Enqueue(string.Format("{0} version: {1}",
                    ISXBotHelper.Properties.Resources.Rhabot,
                    clsSettings.BotVersion));

                // launch the thread
                thread = new Thread(AddToLog_Thread);
                thread.Name = "Logging Thread";
                thread.Start();

                // launch the debug thread
                clsDebugWriter DebugWriter = new clsDebugWriter();
                Thread debugThread = new Thread(DebugWriter.DebugThread_Start);
                debugThread.Name = "Debug Writer";
                DebugThreadItem = new clsSettings.ThreadItem(debugThread, DebugWriter);
                clsSettings.GlobalThreadList.Add(DebugThreadItem);
                debugThread.Start();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Format("Unable to open {0}", clsSettings.LogFilePath));
            }
        }

        /// <summary>
        /// Run in a new thread, adds the message to the log file
        /// </summary>
        private void AddToLog_Thread()
        {
            StringBuilder sb = new StringBuilder();
            StreamWriter swriter = null;

            try
            {
                // infinite loop
                while (true)
                {
                    bool AddToLog = this.LogQueue.Count > 0;

                    // only add to log if we need to
                    if (AddToLog)
                    {
                        // while the file is locked, keep sleeping
                        while (LogLock.Locked)
                        {
                            DebugWrite(string.Format("Log Locked.{0}", LogLineBreak));
                            UpdateLogWindow(string.Format("Log Locked.{0}", LogLineBreak));
                            Thread.Sleep(100);
                            Application.DoEvents();

                            if (clsSettings.IsShuttingDown)
                            {
                                HasShutDown = true;
                                return;
                            }
                        }

                        // lock
                        LogLock.Locked = true;

                        // if we have no writer, create one
                        if (swriter == null)
                            swriter = new StreamWriter(clsSettings.LogFilePath, true, Encoding.ASCII);
                        swriter.AutoFlush = true;

                        // get all messages from the log queue
                        while (LogQueue.Count > 0)
                        {
                            try
                            {
                                // get the message
                                string LogMsg = this.LogQueue.Dequeue();

                                // add to the stringbuilder
                                sb.Append(LogMsg);
                                sb.Append(LogLineBreak);

                                // add the message to the log file
                                swriter.WriteLine(string.Format("{0}  -  {1}\r\n", DateTime.Now, LogMsg));
                                swriter.Flush();
                            }

                            catch (Exception excep)
                            {
                                clsError.ShowError(excep, "While getting log message from queue", false);
                            }
                        }

                        // close the log file
                        swriter.Close();
                        swriter = null;

                        // add to the log window
                        UpdateLogWindow(sb.ToString());

                        // clean up log if too large (remove the first 500,000 characters
                        if (sb.Length > 1000000) // 1,000,000
                            sb.Remove(0, 500000);
                    }

                    // release the log lock
                    LogLock.Locked = false;

                    // stop logging if shutting down
                    if (clsSettings.IsShuttingDown)
                    {
                        HasShutDown = true;
                        return;
                    }

                    // sleep for 1 second
                    Thread.Sleep(1000);

                    // do event
                    Application.DoEvents();
                }
            }

            catch (Exception excep)
            {
                DebugWrite(string.Format("AddToLog_Thread exiting abnormally.\r\n{0}", excep.Message)); 
                clsError.ShowError(excep, "Unable to write the log file", false);
            }

            finally
            {
                // close the stream writer if it is not already
                if (swriter != null)
                {
                    swriter.Close();
                    swriter = null;
                }

                LogLock.Locked = false;
            }
        }

        /// <summary>
        /// sends text to the log window
        /// </summary>
        /// <param name="message"></param>
        private void UpdateLogWindow(string message)
        {
            // skip if shutting down
            if (clsSettings.IsShuttingDown)
                return;

            if ((clsSettings.frmLog != null) && (clsSettings.frmLog.txtLogText != null) && (clsSettings.frmLog.txtLogText.Visible))
            {
                // add to the log window
                if (clsSettings.frmLog.txtLogText.InvokeRequired)
                    clsSettings.frmLog.txtLogText.Invoke(new LogDelegate(UpdateFormLog), message);
                else
                    UpdateFormLog(message);
            }

            // send the message via msn chat
            try
            {
                string msnMessage = string.Format("LogStream: {0}", message);

                // loop through conversations and send to each one who request it
                foreach (clsMSNConversation mConver in clsMSNChat.conversateList)
                {
                    // if streaming, and within the time, send the message
                    if (mConver.StreamEndTime >= DateTime.Now)
                        mConver.SendMessage(msnMessage);
                }                    
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "UpdateLogWindow - MSN Chat Log Stream");
            }            
        }

        /// <summary>
        /// Updates the log window
        /// </summary>
        /// <param name="logText"></param>
        private void UpdateFormLog(string logText)
        {
            clsSettings.frmLog.txtLogText.Text = logText;

            // move to the end of the textbox
            clsSettings.frmLog.txtLogText.Select(logText.Length - 1, 0);
            clsSettings.frmLog.txtLogText.ScrollToCaret();
        }

        /// <summary>
        /// Empties the log file
        /// </summary>
        public void ClearLog()
        {
            try
            {
                DebugWrite("Clearing log");

                // lock the writer
                LogLock.Locked = true;

                // delete the file
                File.Delete(clsSettings.LogFilePath);

                // wait one second
                Thread.Sleep(1000);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "ClearLog", false);
            }

            finally
            {
                LogLock.Locked = false;
            }
        }

        /// <summary>
        /// Forces a shutdown
        /// </summary>
        internal void Shutdown()
        {
            int counter = 0;

            // log it
            DebugWrite("Shutting down clsLogging");

            while (! HasShutDown)
            {
                counter++;

                // sleep
                Thread.Sleep(1000);
                Application.DoEvents();

                // if we've slept for more than 15 seconds, exit
                if (counter > 15)
                    return;
            }

            DebugWrite("clsLogging has shutdown");
        }

        // Functions
        #endregion

        #region Debugging

        public void DebugWrite(string Message)
        {
            // append the date/time stamp to message
            Message = string.Format("{0} - {1}", DateTime.Now.ToLongTimeString(), Message).Replace("\r\n", Environment.NewLine);

            // send to the debug channel
            XDMessaging.XDBroadcast.SendToChannel(Channel, Message);

            // debug console
#if DEBUG
            System.Diagnostics.Debug.WriteLine(Message);
#endif

            // raise event
            clsEvent.Raise_DebugLogItemReceived(Message);
        }

        public void DebugWrite(string Message, params object[] args)
        {
            DebugWrite(string.Format(Message, args));
        }

        // Debugging
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                // stop the thread
                if ((thread != null) && (thread.IsAlive))
                    thread.Abort();
                thread = null;
            }
            catch (Exception excep)
            {
#if DEBUG
                 MessageBox.Show(string.Format("Error: \r\n{0}", clsError.BuildErrorInfoString(excep, new System.Diagnostics.StackFrame(0, true), "Dispose of Logging")));
#endif
            }
        }

        #endregion

        #region New Log Event

        /// <summary>
        /// Raised by the external combat routine
        /// </summary>
        /// <param name="LogMessage"></param>
        void clsGlobals_NewLogEntry(string LogMessage)
        {
            AddToLog(LogMessage);
        }

        // New Log Event
        #endregion

        #region AddToLog

        /// <summary>
        /// Adds the mesasge to the log. DateTime stamp is automatically added
        /// </summary>
        /// <param name="LogMessage">the message to log</param>
        public void AddToLog(string LogMessage)
        {
            AddToLog(string.Empty, LogMessage);
        }

        /// <summary>
        /// Adds the mesasge to the log. DateTime stamp is automatically added
        /// </summary>
        /// <param name="LogMessage">the message to log</param>
        /// <param name="SectionName">The section logging in</param>
        public void AddToLog(string SectionName, string LogMessage)
        {
            string msg = string.IsNullOrEmpty(SectionName) ? LogMessage : string.Format("{0}: {1}", SectionName, LogMessage);
            msg = msg.Replace("\r\n", System.Environment.NewLine);

            // add the message to the queue
            LogQueue.Enqueue(msg);

            // run in background thread
            new Thread(AddToLogThread).Start(msg);
        }

        private void AddToLogThread(object objMsg)
        {
            string msg = (string)objMsg;

            // send to debug
            DebugWrite(msg);

            // raise the event
            clsEvent.Raise_LogItemReceived(msg);
        }

        /// <summary>
        /// Adds the mesasge to the log. Applies string formatting first.
        /// DateTime stamp is automatically added
        /// </summary>
        /// <param name="SectionName">Section Name</param>
        /// <param name="LogMessage">the message to log</param>
        /// <param name="args">formatting values</param>
        public void AddToLogFormatted(string SectionName, string LogMessage, params object[] args)
        {
            AddToLog(SectionName, string.Format(LogMessage, args));
        }

        //public void AddToLogFormatted(string LogMessage, params object[] args)
        //{
        //    AddToLogFormatted(string.Empty, LogMessage, args);
        //}

        // AddToLog
        #endregion
    }
}
