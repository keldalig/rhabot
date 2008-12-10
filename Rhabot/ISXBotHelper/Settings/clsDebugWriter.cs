using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISXBotHelper.Threading;
using System.Threading;

namespace ISXBotHelper
{
    internal class clsDebugWriter : ThreadBase
    {
        #region Queue

        private static ThreadSpin DebugQSpin = new ThreadSpin();

        private static Queue<string> DebugQ = new Queue<string>();
        /// <summary>
        /// Adds the message to the debug q
        /// </summary>
        /// <param name="msg">the message to add</param>
        public static void AddToDebugQ(string msg)
        {
            DebugQSpin.EnterAcquire();
            try
            {
                // spin until other locks release
                DebugQSpin.Wait();

                // add to q
                DebugQ.Enqueue(msg);
            }

            finally
            {
                DebugQSpin.ExitRelease();
            }
        }

        /// <summary>
        /// Gets an item from the DebugQ
        /// </summary>
        private string GetFromDebugQ()
        {
            DebugQSpin.EnterAcquire();
            try
            {
                // spin until other locks release
                DebugQSpin.Wait();

                // return an item, null string
                return DebugQ.Count > 0 ? DebugQ.Dequeue() : string.Empty;
            }

            finally
            {
                DebugQSpin.ExitRelease();
            }
        }

        // Queue
        #endregion

        #region Thread

        /// <summary>
        /// Run in a new thread, gets items to send to debug events
        /// </summary>
        public void DebugThread_Start()
        {
            try
            {
                // loop until shutdown
                while (! Shutdown)
                {
                    // get an item from q
                    string qMsg = GetFromDebugQ();

                    // if we have something, then add to debug writer
                    if (!string.IsNullOrEmpty(qMsg))
                    {
                        // send to debug
                        clsSettings.Logging.DebugWrite(qMsg);

                        // raise the event
                        clsEvent.Raise_LogItemReceived(qMsg);
                    }
                    else
                        Thread.Sleep(100);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "DebugThread");
            }            
        }

        // Thread
        #endregion
    }
}
