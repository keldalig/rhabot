using System;
using System.Collections.Generic;
using System.Threading;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;

namespace ISXBotHelper.Communication
{
    /// <summary>
    /// Holds messages in a queue until it is time to respond
    /// </summary>
    internal class clsAutoResponder : ThreadBase
    {
        #region clsAutoRespondInfo

        internal class clsAutoRespondInfo
        {
            public DateTime TimeToRespond = DateTime.Now;
            public string RespondTo = string.Empty;
            public string Message = string.Empty;
            public clsChat.EChannel Channel = clsChat.EChannel.Private;

            /// <summary>
            /// Initializes a new instance of the clsAutoRespondInfo class.
            /// </summary>
            /// <param name="timeToRespond"></param>
            /// <param name="respondTo"></param>
            /// <param name="message"></param>
            /// <param name="channel"></param>
            public clsAutoRespondInfo(DateTime timeToRespond, string respondTo, string message, clsChat.EChannel channel)
            {
                TimeToRespond = timeToRespond;
                RespondTo = respondTo;
                Message = message;
                Channel = channel;
            }
        }

        // clsAutoRespondInfo
        #endregion

        #region Variables

        public static Queue<clsAutoRespondInfo> ResponseList = new Queue<clsAutoRespondInfo>();
        private static clsSettings.ThreadItem threadItem;

        // Variables
        #endregion

        public static void Start()
        {
            // start new thread
            clsAutoResponder ar = new clsAutoResponder();
            Thread thread = new Thread(ar.Start_Responder);
            thread.Name = "Chat Auto Responder";
            threadItem = new clsSettings.ThreadItem(thread, ar);
            clsSettings.ThreadList.Add(threadItem);
            thread.Start();
        }

        private void Start_Responder()
        {
            clsAutoRespondInfo rInfo;
            List<clsAutoRespondInfo> resetList = new List<clsAutoRespondInfo>();

            try
            {
                // log it
                clsSettings.Logging.AddToLog(Resources.ChatAutoResponderThreadStarting);

                // loop until killed
                while (! Shutdown)
                {
                    // exit if shutting down
                    if ((Shutdown) || !clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                        return;

                    // check queue
                    while (ResponseList.Count > 0)
                    {
                        // exit if shutting down
                        if ((Shutdown) || !clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                            return;

                        try
                        {
                            // get an item
                            rInfo = ResponseList.Dequeue();

                            // if not time, skip
                            if (rInfo.TimeToRespond < DateTime.Now)
                            {
                                resetList.Add(rInfo);
                                continue;
                            }

                            // time to send a response
                            clsChat.SendMessage(rInfo.Channel, rInfo.Message, rInfo.RespondTo);
                        }

                        catch (Exception excep)
                        {
                            clsError.ShowError(excep, "Chat Auto Responder - Process Message");
                        }
                    }

                    // add new items back to the list
                    foreach (clsAutoRespondInfo aInfo in resetList)
                        ResponseList.Enqueue(aInfo);
                    resetList.Clear();

                    // sleep
                    Thread.Sleep(1000);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Chat Auto Responder");
            }            
        }

        /// <summary>
        /// Adds a response to the queue
        /// </summary>
        /// <param name="timeToRespond">time to send the response</param>
        /// <param name="respondTo">who to respond to</param>
        /// <param name="message">the message to send</param>
        /// <param name="channel">channel to send on</param>
        public static void AddResponse(DateTime timeToRespond, string respondTo, string message, clsChat.EChannel channel)
        {
            ResponseList.Enqueue(new clsAutoRespondInfo(timeToRespond, respondTo, message, channel));
        }
    }
}
