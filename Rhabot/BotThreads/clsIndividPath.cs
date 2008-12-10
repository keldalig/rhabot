using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ISXBotHelper;
using ISXBotHelper.Communication;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;

namespace Rhabot.BotThreads
{
    public class clsIndividPath : ThreadBase
    {
        private clsSettings.ThreadItem threadItem;
        private clsPath.PathListInfoEx pInfo = null;
        private clsPathGroupItem PathItem = null;

        #region Start / Stop

        /// <summary>
        /// Starts the bot in a new thread
        /// </summary>
        public void Start(clsPathGroupItem pathItem)
        {
            try
            {
                // set path name
                PathItem = pathItem;
                if (PathItem == null)
                {
                    // show the error, then exit the bot
                    clsError.ShowError(new Exception("No General Path Selected"), Resources.BotThreadIndivdPathRun, true, new StackFrame(0, true), false);
                    Stop();
                    clsGlobals.Raise_FloatStopped();
                    return;
                }

                // start thread
                clsSettings.Stop = false;
                Thread thread = new Thread(Start_Thread);
                thread.Name = "General Path Run";
                threadItem = new clsSettings.ThreadItem(thread, this);
                clsSettings.GlobalThreadList.Add(threadItem);
                thread.Start();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.BotThreadIndivdPathRun);
                Stop();
                clsGlobals.Raise_FloatStopped();
            }
        }

        /// <summary>
        /// Stops the thread
        /// </summary>
        public void Stop()
        {
            // stop the bot
            clsSettings.Stop = true;
            if (threadItem != null)
                clsSettings.KillThread(threadItem, Resources.RhabotStopping);
        }

        // Start / Stop
        #endregion

        #region Run Bot

        /// <summary>
        /// Raised when we are forced to shutdown
        /// </summary>
        private void clsEvent_ForcingShutdown()
        {
            // log it
            clsSettings.Logging.AddToLog(Resources.RhabotMainForceQuit);
        }

        /// <summary>
        /// Runs Rhabot from a seperate thread
        /// </summary>
        private void Start_Thread()
        {
            try
            {
                // hook forced shutdow
                clsEvent.ForcingShutdown += clsEvent_ForcingShutdown;

                // do startup
                clsSettings.Start();

                // load the path
                pInfo = clsPath.LoadPathList(Convert.ToInt32(PathItem.GroupNote));
                
                // exit if no path
                if ((pInfo == null) || (pInfo.PathList.Count == 0))
                {
                    clsSettings.Stop = true;
                    clsError.ShowError(new Exception(Resources.CouldNotContinueNoPathsLoaded), string.Empty, true, new StackFrame(0, true), false);
                    return;
                }

                // get the nearest point
                clsPath.NearPathPoint nearPoint = new clsPath().GetNearestPoint(pInfo, clsCharacter.MyLocation);
                if ((nearPoint != null) && (nearPoint.Element > -1))
                    pInfo.CurrentStep = nearPoint.Element;

                // run the path
                clsPath.EMovementResult result = clsPath.RunPath(pInfo);

                // handle result
                switch (result)
                {
                    case clsPath.EMovementResult.Success:
                        // success, exit
                        clsSend.SendToAll(Resources.IndividPathRunSuccessful, Resources.IndividPathRunSuccessful);
                        return;

                    case clsPath.EMovementResult.Error:
                        clsError.ShowError(new Exception(Resources.IndividPathError), Resources.BotType_IndividPath, true, new StackFrame(0, true), false);
                        return;

                    case clsPath.EMovementResult.PathObstructed:
                    case clsPath.EMovementResult.Stuck:
                        clsError.ShowError(new Exception(Resources.IndividPathStuck), Resources.BotType_IndividPath, true, new StackFrame(0, true), false);
                        return;
                        
                    case clsPath.EMovementResult.Dead:
                        clsError.ShowError(new Exception(string.Format(Resources.IndividPathDeadNoGY, clsCharacter.CharacterName)), Resources.BotType_IndividPath, true, new StackFrame(0, true), false);
                        return;
                }
            }

            catch (Exception excep)
            {
                // skip if thread abort
                if (excep.Message.ToLower().Contains(Resources.abort))
                    return;

                clsError.ShowError(excep, Resources.RhabotMainThread);
            }

            finally
            {
                clsSettings.Logging.AddToLog(Resources.IndividPathThreadExit);

                clsSettings.Stop = true;
                clsGlobals.Raise_FloatStopped();
            }
        }

        // Run Bot
        #endregion
    }
}
