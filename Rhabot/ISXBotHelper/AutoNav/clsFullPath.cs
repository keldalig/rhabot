using System;
using System.Collections.Generic;
using System.Diagnostics;
using ISXBotHelper.Properties;
using Rhabot;

namespace ISXBotHelper.AutoNav
{
    internal class clsFullPath
    {
        public clsPath.PathListInfoEx ActivePath = null;
        public List<clsPath.PathListInfoEx> GraveyardPaths = new List<clsPath.PathListInfoEx>();

        private List<clsPath.PathListInfoEx> Paths = new List<clsPath.PathListInfoEx>();

        /// <summary>
        /// Runs Rhabot from a seperate thread
        /// </summary>
        public clsPath.EMovementResult RunPath(string GroupName, List<string> ObjectsToFind)
        {
            clsPath cPath = new clsPath();
            bool NeedsAdvance = false;
            clsPath.NearPathPoint nearPoint = null;
            clsPath.PathListInfoEx AdvancePath = null;

            try
            {
                // load paths
                Paths = cPath.LoadAllPaths(GroupName, clsCharacter.CurrentLevel);
                clsGlobals.Paths = Paths;

                // exit if no paths
                if ((Paths == null) || (Paths.Count == 0))
                    return clsPath.EMovementResult.Error;

                // load graveyard paths
                clsPath.PathListInfoEx GY = clsPath.GetPathFromList(Paths, clsGlobals.Path_Graveyard1);
                if (GY != null)
                    GraveyardPaths.Add(GY);
                GY = clsPath.GetPathFromList(Paths, clsGlobals.Path_Graveyard2);
                if (GY != null)
                    GraveyardPaths.Add(GY);

                #region Find Nearest Path Point

                // check if we are near our hunt path
                clsSettings.Logging.AddToLog(Resources.CheckNearHuntPath);
                ActivePath = clsPath.GetPathFromList(Paths, clsGlobals.Path_Hunting);

                // get the nearest point
                if (ActivePath != null)
                {
                    nearPoint = FindPath(ActivePath);
                    if (nearPoint == null)
                        ActivePath = null;
                }

                // hunt advance
                if ((ActivePath == null) || ((nearPoint == null) || (nearPoint.Element < 0)))
                {
                    clsSettings.Logging.AddToLog(Resources.FindingHuntStart);

                    ActivePath = clsPath.GetPathFromList(Paths, clsGlobals.Path_StartHunt);
                    nearPoint = FindPath(ActivePath);
                    if (nearPoint == null)
                        ActivePath = null;
                    else
                        // not on a hunt path, so we need to find nearest path, and set needsadvance flag
                        NeedsAdvance = true; // required to advance us to hunt path
                }

                // loop through all paths to find a match
                if ((ActivePath == null) || ((nearPoint == null) || (nearPoint.Element < 0)))
                {
                    clsSettings.Logging.AddToLog(Resources.TryingOtherPaths);

                    foreach (clsPath.PathListInfoEx pInfo in Paths)
                    {
                        if (!string.IsNullOrEmpty(pInfo.PathName))
                            clsSettings.Logging.AddToLogFormatted(Resources.TryingPathX, pInfo.PathName);

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
                    return clsPath.EMovementResult.Error;

                // notify
                if (!string.IsNullOrEmpty(ActivePath.PathName))
                    clsSettings.Logging.AddToLogFormatted(Resources.FoundPointOnX, ActivePath.PathName);

                // Find Nearest Path Point
                #endregion

                #region Advance to Hunt path

                // if we need to advance, do so now
                if (NeedsAdvance)
                {
                    NeedsAdvance = false;
                    clsSettings.Logging.AddToLog(Resources.AdvanceToHunt);

                    // get the hunt start path
                    if (ActivePath.PathName != clsGlobals.Path_StartHunt)
                    {
                        clsPath.PathListInfoEx FinalPath = clsPath.GetPathFromList(Paths, clsGlobals.Path_StartHunt);
                        if (FinalPath == null)
                        {
                            clsError.ShowError(new Exception(Resources.NoPathToHuntPath), Resources.AutoNav, true, new StackFrame(0, true), false);
                            return clsPath.EMovementResult.Error; ;
                        }

                        // get a path to our hunt start path
                        AdvancePath = cPath.CreatePath(ActivePath, FinalPath, true);
                    }
                    else
                        AdvancePath = ActivePath.Clone();

                    // move from AdvancePath to HuntStart
                    if (clsPath.RunPath(AdvancePath) != clsPath.EMovementResult.Success)
                        return clsPath.EMovementResult.Error;
                }

                // Advance to Hunt path
                #endregion

                // we should now be on the hunt path
                // check for forced shutdown
                if (!clsSettings.TestPauseStop(Properties.Resources.RhabotStopped))
                    return clsPath.EMovementResult.Stopped;

                #region Vendor Run (If Needed)

                // check durability. run to vendor if we need
                if (clsCharacter.NeedsVendor)
                {
                    if (clsPath.VendorRun(Paths, GraveyardPaths, ActivePath) != clsPath.EMovementResult.Success)
                    {
                        clsError.ShowError(new Exception(Resources.BadVendorRun), Resources.Rhabot, true, new StackFrame(0, true), false);
                        return clsPath.EMovementResult.Error;
                    }

                    else // get hunt path
                        ActivePath = clsPath.GetPathFromList(Paths, clsGlobals.Path_StartHunt);
                }

                // Vendor Run (If Needed)
                #endregion

                // check for forced shutdown
                if (!clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                    return clsPath.EMovementResult.Stopped;

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
                        clsError.ShowSpecialError("Can not build path from current location to a point on the hunt path");
                        return clsPath.EMovementResult.Error;
                    }

                    // set current step
                    clsSettings.Logging.AddToLogFormatted(Resources.RunPath, "Setting CurrentStep Index to {0}", nearPoint.Element);
                    ActivePath.CurrentStep = nearPoint.Element;
                }

                if (ActivePath == null)
                {
                    clsError.ShowError(new Exception(Resources.NoPathFromCurrPoint), Resources.BotThread_RhabotStandard, true, new StackFrame(0, true), false);
                    return clsPath.EMovementResult.Error;
                }

                // run the path
                return clsPath.RunPath(ActivePath, true, true, null, ObjectsToFind, GraveyardPaths);
            }

            catch (Exception excep)
            {
                // skip if thread abort
                if (!excep.Message.ToLower().Contains(Resources.abort))
                    clsError.ShowError(excep, Resources.RhabotMainThread);
                return clsPath.EMovementResult.Error;
            }

            finally
            {
                clsSettings.Logging.AddToLog(Resources.Rhabot, Resources.Stopped);
            }

            return clsPath.EMovementResult.Success;
        }

        #region FindPath

        /// <summary>
        /// Finds the point that is nearest to our location. Returns a path, or null if nothing found
        /// </summary>
        /// <param name="Paths">the path lists to parse</param>
        /// <param name="PathName">the path name</param>
        /// <returns></returns>
        private clsPath.PathListInfoEx FindPath(List<clsPath.PathListInfoEx> Paths, string PathName)
        {
            clsPath.PathListInfoEx tempPath = null;

            try
            {
                // log it
                clsSettings.Logging.AddToLogFormatted(Resources.FindingNearestPointForX, PathName);

                // find our path
                tempPath = clsPath.GetPathFromList(Paths, PathName);

                // find the nearest point on our path
                if (FindPath(tempPath) != null)
                    return tempPath;
                else
                    return null;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.FindPath);
            }

            // return the path
            return null;
        }

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
                    clsSettings.Logging.AddToLog("No paths found in path list that match path name");
                    return null;
                }

                // log it
                if (!string.IsNullOrEmpty(PathInfo.PathName))
                    clsSettings.Logging.AddToLogFormatted(Resources.FindingNearestPointForX, PathInfo.PathName);

                // find the nearest point on our path
                nearPoint = cPath.GetNearestPoint(PathInfo, clsCharacter.MyLocation);

                // if nothing found, return false
                if (nearPoint == null)
                {
                    clsSettings.Logging.AddToLogFormatted(Resources.NoPointInXNearUs, PathInfo.PathName);
                    return null;
                }

                // pop path info
                clsSettings.Logging.AddToLogFormatted(Resources.FoundPathXLocationY, PathInfo.PathName, nearPoint.PPoint);
                PathInfo.CurrentStep = nearPoint.Element;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.FindPath);
            }

            // return the path
            return nearPoint;
        }

        // FindPath
        #endregion
    }
}
