using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ISXBotHelper;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;

namespace Rhabot.BotThreads
{
    /// <summary>
    /// Runs the bot
    /// </summary>
    public class clsRhabot : ThreadBase
    {
        private clsSettings.ThreadItem threadItem;
        private List<clsPath.PathListInfoEx> Paths = null;
        private readonly List<clsPath.PathListInfoEx> GraveyardPaths = new List<clsPath.PathListInfoEx>();
        private string GroupName = string.Empty;

        #region Start / Stop

        /// <summary>
        /// Starts the bot in a new thread
        /// </summary>
        /// <param name="groupName">path group name</param>
        public void Start(string groupName)
        {
            try
            {
                // get the groupname
                GroupName = groupName;

                clsSettings.Stop = false;
                Thread thread = new Thread(Start_Thread);
                thread.Name = Resources.BotThread_RhabotStandard;
                threadItem = new clsSettings.ThreadItem(thread, this);
                this.Shutdown = false;
                clsSettings.GlobalThreadList.Add(threadItem);
                thread.Start();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.BotThread_RhabotStandard, Resources.RhabotStart);
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
            Shutdown = true;
            clsSettings.Stop = true;
            clsGlobals.Raise_FloatStopped();
            clsSettings.KillThread(threadItem, Resources.RhabotStop);
        }

        // Start / Stop
        #endregion

        #region Run Bot

        /// <summary>
        /// Raised when we are forced to shutdown
        /// </summary>
        private void clsEvent_ForcingShutdown()
        {
            // change shutdown flag
            Shutdown = true;

            // log it
            clsSettings.Logging.AddToLog(Resources.BotThread_RhabotStandard, Resources.RhabotMainForceQuit);
        }

        /// <summary>
        /// Runs Rhabot from a seperate thread
        /// </summary>
        private void Start_Thread()
        {
            clsPath cPath = new clsPath();
            clsPath.PathListInfoEx ActivePath;
            bool NeedsAdvance = false;
            clsPath.NearPathPoint nearPoint = null;

            try
            {
                // hook forced shutdow
                clsEvent.ForcingShutdown += clsEvent_ForcingShutdown;

                // do startup
                clsSettings.Start();

//#if (DEBUG)
//                if (! Debugger.IsAttached)
//                    Debugger.Launch();
//#endif

                // load paths
                Paths = cPath.LoadAllPaths(GroupName, clsCharacter.CurrentLevel);
                clsGlobals.Paths = Paths;

                // exit if no paths
                if ((Paths == null) || (Paths.Count == 0))
                {
                    Shutdown = true;
                    clsSettings.Stop = true;
                    clsError.ShowError(new Exception(Resources.CouldNotContinueNoPathsLoaded), string.Empty, true, new StackFrame(0, true), false);
                    return;
                }

                // load graveyard paths
                clsPath.PathListInfoEx GY = clsPath.GetPathFromList(Paths, clsGlobals.Path_Graveyard1);
                if (GY != null)
                    GraveyardPaths.Add(GY);
                GY = clsPath.GetPathFromList(Paths, clsGlobals.Path_Graveyard2);
                if (GY != null)
                    GraveyardPaths.Add(GY);


                #region Find Nearest Path Point

                // check if we are near our hunt path
                clsSettings.Logging.AddToLog(Resources.BotThread_RhabotStandard, Resources.CheckNearHuntPath);
                ActivePath = clsPath.GetPathFromList(Paths, clsGlobals.Path_Hunting);

                // get the nearest point
                if (ActivePath != null)
                {
                    nearPoint = FindPath(ActivePath);
                    if (nearPoint == null)
                        ActivePath = null;
                }

                // hunt advance
                if ((ActivePath == null) || (nearPoint.Element < 0))
                {
                    clsSettings.Logging.AddToLog(Resources.BotThread_RhabotStandard, Resources.FindingHuntStart);

                    ActivePath = clsPath.GetPathFromList(Paths, clsGlobals.Path_StartHunt);
                    nearPoint = FindPath(ActivePath);
                    if (nearPoint == null)
                        ActivePath = null;
                    else
                        // not on a hunt path, so we need to find nearest path, and set needsadvance flag
                        NeedsAdvance = true; // required to advance us to hunt path
                }

                // loop through all paths to find a match
                if ((ActivePath == null) || (nearPoint.Element < 0))
                {
                    clsSettings.Logging.AddToLog(Resources.BotThread_RhabotStandard, Resources.TryingOtherPaths);

                    foreach (clsPath.PathListInfoEx pInfo in Paths)
                    {
                        if (! string.IsNullOrEmpty(pInfo.PathName))
                            clsSettings.Logging.AddToLogFormatted(Resources.BotThread_RhabotStandard, Resources.TryingPathX, pInfo.PathName);

                        // if we find a point, use it and break out of loop
                        nearPoint = FindPath(ActivePath);
                        if (nearPoint != null)
                        {
                            ActivePath = pInfo;
                            NeedsAdvance = true;
                            break;
                        }
                    }
                }

                // if we still don't have a path, throw an error and exit
                if (ActivePath == null)
                {
                    clsError.ShowError(new Exception(Resources.NoPathFoundInZone), Resources.BotThread_RhabotStandard, string.Empty, true, new StackFrame(0, true), false);
                    return;
                }

                // notify
                if (! string.IsNullOrEmpty(ActivePath.PathName))
                    clsSettings.Logging.AddToLogFormatted(Resources.BotThread_RhabotStandard, Resources.FoundPointOnX, ActivePath.PathName);

                // Find Nearest Path Point
                #endregion

                // loop until shutdown
                while ((!Shutdown) && (clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop)))
                {
                    #region Advance to Hunt path

                    // if we need to advance, do so now
                    if (NeedsAdvance)
                    {
                        NeedsAdvance = false;
                        clsSettings.Logging.AddToLog(Resources.BotThread_RhabotStandard, Resources.AdvanceToHunt);

                        // get the hunt start path
                        clsPath.PathListInfoEx AdvancePath;
                        if (ActivePath.PathName != clsGlobals.Path_StartHunt)
                        {
                            clsPath.PathListInfoEx FinalPath = clsPath.GetPathFromList(Paths, clsGlobals.Path_StartHunt);
                            if (FinalPath == null)
                            {
                                clsError.ShowError(new Exception(Resources.NoPathToHuntPath), Resources.BotThread_RhabotStandard, string.Empty, true, new StackFrame(0, true), false);
                                return;
                            }

                            // get a path to our hunt start path
                            AdvancePath = cPath.CreatePath(ActivePath, FinalPath, true);
                        }
                        else
                            AdvancePath = ActivePath.Clone();

                        // move from AdvancePath to HuntStart
                        if (clsPath.RunPath(AdvancePath) != clsPath.EMovementResult.Success)
                            break;
                    }

                    // Advance to Hunt path
                    #endregion

                    // we should now be on the hunt path
                    // check for forced shutdown
                    if ((Shutdown) || (!clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop)))
                        return;

                    #region Vendor Run (If Needed)

                    // check durability. run to vendor if we need
                    if (clsCharacter.NeedsVendor)
                    {
                        if (clsPath.VendorRun(Paths, GraveyardPaths, ActivePath) != clsPath.EMovementResult.Success)
                        {
                            clsError.ShowError(new Exception(Resources.BadVendorRun), Resources.BotThread_RhabotStandard, string.Empty, true, new StackFrame(0, true), false);
                            Stop();
                            return;
                        }

                        else // get hunt path
                            ActivePath = clsPath.GetPathFromList(Paths, clsGlobals.Path_StartHunt);
                    }

                    // Vendor Run (If Needed)
                    #endregion

                    // check for forced shutdown
                    if ((Shutdown) || (!clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop)))
                        return;

                    // on hunt path, make sure we have the right path
                    if ((ActivePath == null) || (ActivePath.PathName != clsGlobals.Path_Hunting))
                    {
                        // get the path
                        ActivePath = clsPath.GetPathFromList(Paths, clsGlobals.Path_Hunting);

                        // get nearest point
                        nearPoint = cPath.GetNearestPoint(ActivePath, clsCharacter.MyLocation);

                        // if no point, error
                        if (nearPoint == null)
                        {
                            clsError.ShowError(new Exception(Resources.NoPathToHuntPath), Resources.BotThread_RhabotStandard, string.Empty, true, new StackFrame(0, true), false);
                            return;
                        }

                        // set current step
                        if (ActivePath != null)
                            ActivePath.CurrentStep = nearPoint.Element;
                    }

                    if (ActivePath == null)
                    {
                        clsError.ShowError(new Exception(Resources.NoPathFromCurrPoint), Resources.BotThread_RhabotStandard, string.Empty, true, new StackFrame(0, true), false);
                        return;
                    }

                    // run the path
                    if (!RunPath(ActivePath, true, true))
                        return;
                }
            }

            catch (Exception excep)
            {
                // skip if thread abort
                if (excep.Message.ToLower().Contains(Resources.abort))
                    return;

                clsError.ShowError(excep, Resources.BotThread_RhabotStandard);

                clsSettings.Stop = true;
                Shutdown = true;
                clsGlobals.Raise_FloatStopped();
            }

            finally
            {
                clsSettings.Logging.AddToLog(Resources.BotThread_RhabotStandard, Resources.RhabotStop);
                clsSettings.Stop = true;
                clsGlobals.Raise_FloatStopped();
            }
        }

        #region FindPath

        /// <summary>
        /// Finds the point that is nearest to our location. Returns true if found, false if not
        /// </summary>
        /// <param name="PathInfo">the selected path</param>
        private clsPath.NearPathPoint FindPath(clsPath.PathListInfoEx PathInfo)
        {
            clsPath cPath = new clsPath();
            clsPath.NearPathPoint nearPoint = null;

            try
            {
                // if no path, return null
                if (PathInfo == null)
                {
                    clsSettings.Logging.AddToLog(Resources.BotThread_RhabotStandard, Resources.NoPathsMatchName);
                    return null;
                }

                // log it
                if (! string.IsNullOrEmpty(PathInfo.PathName))
                    clsSettings.Logging.AddToLogFormatted(Resources.BotThread_RhabotStandard, Resources.FindingNearestPointForX, PathInfo.PathName);

                // find the nearest point on our path
                nearPoint = cPath.GetNearestPoint(PathInfo, clsCharacter.MyLocation);

                // if nothing found, return false
                if (nearPoint == null)
                {
                    clsSettings.Logging.AddToLogFormatted(Resources.BotThread_RhabotStandard, Resources.NoPointInXNearUs, PathInfo.PathName);
                    return null;
                }

                // pop path info
                clsSettings.Logging.AddToLogFormatted(Resources.BotThread_RhabotStandard, Resources.FoundPathXLocationY, PathInfo.PathName, nearPoint.PPoint);
                PathInfo.CurrentStep = nearPoint.Element;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.BotThread_RhabotStandard, Resources.FindPath);
            }

            // return the path
            return nearPoint;
        }

        // FindPath
        #endregion

        #region Run Path

        /// <summary>
        /// Runs the path and returns true on completion. Returns false if we have a problem
        /// </summary>
        /// <param name="CurrentPath">the path to run</param>
        /// <param name="SearchForUnits">send false to skip searching for herbs/chests/attacking units</param>
        /// <param name="CheckCharBags">true to check bags and durability</param>
        private bool RunPath(clsPath.PathListInfoEx CurrentPath, bool SearchForUnits, bool CheckCharBags)
        {
            try
            {
                while (true)
                {
                    // move through the path
                    clsPath.EMovementResult mResult = clsPath.RunPath(CurrentPath, SearchForUnits, CheckCharBags, null, null, this.GraveyardPaths);

                    // keep looping until we have success or failure
                    while (mResult == clsPath.EMovementResult.Success)
                    {
                        // if shutting down
                        if ((Shutdown) || (!clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop)))
                            return true;

                        // reverse direction
                        clsSettings.Logging.AddToLog(Resources.BotThread_RhabotStandard, Resources.ReversePathDirection);
                        clsPath.StopMoving();
                        CurrentPath.ReversePath();

                        // move through the path
                        mResult = clsPath.RunPath(CurrentPath, SearchForUnits, CheckCharBags, null, null, GraveyardPaths);
                    }

                    // handle return
                    if (mResult == clsPath.EMovementResult.NeedVendor)
                        return true;
                    else if ((mResult == clsPath.EMovementResult.Dead) && (!new clsGhost().HandleDead(CurrentPath, GraveyardPaths)))
                        return false;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.BotThread_RhabotStandard, Resources.RunPath);
                return false;
            }

            finally
            {
                clsSettings.Logging.AddToLog(Resources.BotThread_RhabotStandard, Resources.RunPathExiting);
            }
        }

        // Run Path
        #endregion

        // Run Bot
        #endregion
    }
}
