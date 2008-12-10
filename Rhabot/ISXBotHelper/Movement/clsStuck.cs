using System;
using System.Collections.Generic;
using System.Threading;
using ISXBotHelper.Properties;
using ISXWoW;

namespace ISXBotHelper
{
    /// <summary>
    /// Functions for testing if we are stuck
    /// </summary>
    public static class clsStuck
    {
        #region Variables

        private static DateTime PathAroundTimer = DateTime.MinValue;

        // Variables
        #endregion

        #region Functions

        /// <summary>
        /// Resets the stuck variables. Used when we first start moving
        /// </summary>
        public static void ResetStuck()
        {
            Stuck_LastChecked = DateTime.MinValue;
            Stuck_LastPoint = null;
        }

        // Function
        #endregion

        #region CheckStuckEx

        private static PathListInfo.PathPoint Stuck_LastPoint = null;
        private static DateTime Stuck_LastChecked = DateTime.MinValue;
        private static bool DidJump = false;

        /// <summary>
        /// Checks if we are stuck, returns true if we are
        /// </summary>
        /// <param name="TargetPoint">the point we are aiming for</param>
        /// <param name="handleStuck">true to try to unstick ourself</param>
        public static bool CheckStuckEx(PathListInfo.PathPoint TargetPoint, bool handleStuck)
        {
            PathListInfo.PathPoint myloc;

            try
            {
                // if we've never checked, then reset last checked
                if ((Stuck_LastPoint == null) || (Stuck_LastChecked == DateTime.MinValue))
                {
                    Stuck_LastChecked = DateTime.Now;
                    Stuck_LastPoint = clsCharacter.MyLocation;
                    DidJump = false;
                    clsSettings.Logging.AddToLog(Resources.CheckStuckEx, Resources.FirstTimeChecking);
                    return false;
                }

                // if the test time is less than two seconds, skip
                if (Stuck_LastChecked.AddMilliseconds(2000) > DateTime.Now)
                    return false;

                // reset if not stuck
                myloc = clsCharacter.MyLocation;
                if (! myloc.SamePoint(Stuck_LastPoint))
                {
                    Stuck_LastChecked = DateTime.Now;
                    Stuck_LastPoint = myloc;
                    DidJump = false;
                    return false;
                }

                // reset last point
                Stuck_LastPoint = myloc;

                // try jumping, twice
                if (!DidJump)
                {
                    clsPath.DoJump();
                    Thread.Sleep(750);
                    clsPath.DoJump();
                    DidJump = true;
                    return false;
                }

                // we tried jumping, so reset
                DidJump = false;

                // handle stuck according to param settings
                if (handleStuck)
                    return HandleStuck(TargetPoint);
                else
                    return true;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.CheckStuckEx);
            }

            return false;
        }

        /// <summary>
        /// Handles us if we are stuck. Tries to run backward, turn, run forward.
        /// Returns false if we UNSTUCK
        /// </summary>
        /// <param name="TargetPoint">our destination point</param>
        private static bool HandleStuck(PathListInfo.PathPoint TargetPoint)
        {
            clsPath cPath = new clsPath();

            try
            {
                // we will try to unstuck ourselves 10 times before quitting in disgust
                //for (i = 0; i < 10; i++)
                {
                    // stop moving
                    clsPath.StopMoving();

                    // try to build a path around
                    if (!PathAround(TargetPoint))
                        return false;

                    // couldn't path around, try backing up for 3 seconds
                    MoveAround(TargetPoint);

                    // move to the point, exit if we get there
                    ResetStuck();
                    if (cPath.MoveToPoint(TargetPoint, false) == clsPath.EMovementResult.Success)
                    {
                        clsSettings.Logging.AddToLog(Resources.CheckStuckEx, Resources.StuckReachedNextPoint);
                        return false;
                    }
                }

                // if we are here, it means we couldn't get unstuck
                return true;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.HandleStuck);
            }

            finally
            {
                ResetStuck();
            }

            return false;
        }

        /// <summary>
        /// Tries to move to the point for 2 seconds. returns true if we reach the point
        /// </summary>
        internal static bool MoveTo(PathListInfo.PathPoint TargetPoint)
        {
            bool rVal = false;

            // face the point
            clsFace.FacePointExCombat(TargetPoint);

            DateTime EndTime = DateTime.Now.AddMilliseconds(2000);

            // move to the point for two seconds
            clsPath.StartMoving();

            while (EndTime > DateTime.Now)
            {
                // if we are at the point, break
                if (clsCharacter.MyLocation.Distance(TargetPoint) <= 2)
                {
                    rVal = true;
                    break;
                }

                // sleep
                Thread.Sleep(100);
            }

            // stop moving
            clsPath.StopMoving();

            // return
            return rVal;
        }

        /// <summary>
        /// Backs up and randomly moves left or right
        /// </summary>
        /// <param name="TargetPoint">target point, only used for facing</param>
        internal static void MoveAround(PathListInfo.PathPoint TargetPoint)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            double heading = 70; // 70 degress from current facing
            int i = 0;

            try
            {
                // log it
                clsSettings.Logging.AddToLog(Resources.MoveAround, Resources.MovingAround);

                // stop moving
                clsPath.StopMoving();

                // face the point
                clsFace.FacePointExCombat(TargetPoint);

                // move for 2 seconds
                if (MoveTo(TargetPoint))
                    return;

                // back up for 2 seconds
                clsPath.Move(MovementDirection.Backward);
                Thread.Sleep(2000);

                // stop moving
                clsPath.StopMoving();

                // decide which direction to turn
                int turnSide = (rnd.Next(0, 10) > 5) ? 1 : -1;

                // turn, in wider strokes per iteration
                heading = (heading + (i * 5)) * turnSide;

                // face the new angle
                using (new clsFrameLock.LockBuffer())
                    clsSettings.isxwow.FaceHeading(clsSettings.isxwow.Me.Heading + heading);

                // move for 3 seconds
                clsPath.Move(MovementDirection.Forward);
                Thread.Sleep(3000);
                clsPath.StopMoving();

                // face our target
                clsFace.FacePointEx(TargetPoint);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.MoveAround);
            }
        }

        // CheckStuckEx
        #endregion

        #region PathAround

        /// <summary>
        /// Builds a path around an obstacle. Returns true if
        /// path is obstructed and we can't get around it
        /// </summary>
        /// <param name="NextPoint">the destination point</param>
        internal static bool PathAround(PathListInfo.PathPoint NextPoint)
        {
            bool rVal = false;
            clsPath cPath = new clsPath();
            List<PathListInfo.PathPoint> pointList;

            // tracks if our dead state changes (did we die while doing IPO?)
            bool LastDeadState = clsCharacter.IsDead;

            try
            {
                // stop moving
                clsPath.StopMoving();

                // log it
                clsSettings.Logging.AddToLog(Resources.IPO, Resources.PathIsObstructed);

                // try to find a way around
                pointList = PathAroundAStar(NextPoint, 10);

                // if our dead state changed, then exit with null
                if (LastDeadState != clsCharacter.IsDead)
                    return false;

                // exit if nothing found
                if ((pointList == null) || (pointList.Count == 0))
                {
                    // first, try moving once
                    MoveAround(NextPoint);

                    // try to find a way around, again
                    pointList = PathAroundAStar(NextPoint, 10);

                    // if our dead state changed, then exit with null
                    if (LastDeadState != clsCharacter.IsDead)
                        return false;

                    // if no path, handle
                    if ((pointList == null) || (pointList.Count == 0))
                    {
                        clsSettings.Logging.AddToLog(Resources.IPO, Resources.CouldNotBuildPathToTargetPoint);
                        return true;
                    }
                }

                // found a path, let's run it
                foreach (PathListInfo.PathPoint pPoint in pointList)
                {
                    // test for pause/stop
                    if (!clsSettings.TestPauseStop(Resources.PathAround, Resources.ExitingDueToScriptStop))
                        return true;

                    // log it
                    clsSettings.Logging.AddToLogFormatted(Resources.IPO, Resources.AdvancingToPointX, pPoint.ToString());

                    // run to the next point
                    clsPath.EMovementResult result = cPath.MoveToPoint(pPoint, false);

                    // if we died, exit
                    if ((LastDeadState != clsCharacter.IsDead) || (result == clsPath.EMovementResult.Dead) || (result == clsPath.EMovementResult.Aggroed))
                        return false;

                    // if not successful, then exit with error
                    if (result != clsPath.EMovementResult.Success)
                    {
                        clsSettings.Logging.AddToLog(Resources.IPO, Resources.CouldNotRunToPoint);
                        return true;
                    }
                }

                // finished
                return false;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.PathAround);
            }

            return rVal;
        }

        // PathAround
        #endregion

        #region PathAround A*

        /// <summary>
        /// Attempts to build a list of points to the destpoint.
        /// Uses the Last point in CurrentList as the start point.
        /// Exits if we have more than 15 points
        /// </summary>
        /// <param name="DestPoint">destination</param>
        /// <param name="PointLimit">number of points to test</param>
        internal static List<PathListInfo.PathPoint> PathAroundAStar(PathListInfo.PathPoint DestPoint, int PointLimit)
        {
            List<PathListInfo.PathPoint> retList = new List<PathListInfo.PathPoint>();
            PathListInfo.PathPoint LastPoint, testPoint;
            int degrees = 0, radius = 10;

            // sets the minimum distance between points on this list
            int MinDist = 8;

            try
            {
                // log it
                clsSettings.Logging.AddToLogFormatted(Resources.PathAround, Resources.PathAroundAStarLog, 0, DestPoint.ToString());

                // find the best point from here
                using (new clsFrameLock.LockBuffer())
                {
                    // get the current point
                    LastPoint = clsCharacter.MyLocation;
                    int LastDegree = -1;

                    for (int i = 0; i < PointLimit; i++)
                    {
                        bool FoundPoint = false;
                        do
                        {
                            // skip if we already processed this degree (+/- 20 degrees)
                            if ((LastDegree > -1) &&
                                ((LastDegree <= degrees + 20) && (LastDegree >= degrees - 20)))
                            {
                                degrees++;
                                continue;
                            }

                            // set test x and y
                            testPoint = new PathListInfo.PathPoint(
                                LastPoint.X + (radius*Math.Cos(degrees)),
                                LastPoint.Y + (radius*Math.Sin(degrees)),
                                LastPoint.Z);

                            // skip the point is too close to something already in the list
                            bool skipPoint = false;
                            foreach (PathListInfo.PathPoint listPoint in retList)
                            {
                                // if too close, skip
                                if (listPoint.Distance(testPoint) <= MinDist)
                                {
                                    skipPoint = true;
                                    break;
                                }
                            }

                            // if we should skip because it is too close, do so
                            if (skipPoint)
                                continue;

                            // check if we have a good point
                            if ((!clsSettings.isxwow.Me.IsPathObstructed(testPoint.X, testPoint.Y, testPoint.Z, 8, LastPoint.X, LastPoint.Y, LastPoint.Z)) &&
                                (!clsPath.IsPathObstructed(LastPoint, testPoint)))
                            {
                                // check if no collision to the destination
                                if ((!clsSettings.isxwow.Me.IsPathObstructed(testPoint.X, testPoint.Y, testPoint.Z, 8, DestPoint.X, DestPoint.Y, DestPoint.Z)) &&
                                    (!clsPath.IsPathObstructed(testPoint, DestPoint)))
                                {
                                    // found path
                                    retList.Add(testPoint);
                                    retList.Add(DestPoint);
                                    return retList;
                                }

                                // no clear path to destination found
                                // add to the pathlist
                                retList.Add(testPoint);
                                
                                // set lastpoint
                                LastPoint = testPoint;
                                FoundPoint = true;
                                LastDegree = degrees;
                                break;
                            }
                        } while ((degrees += 10) < 361);

                        // if we did not find a point, then exit
                        if (!FoundPoint)
                            return null;
                    }
                }

                // return the list
                if (retList.Count > 1)
                    return retList;
                else
                    return null;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.PathAroundAStar);
            }

            // if we are here, nothing was found
            return null;
        }

        /// <summary>
        /// Attempts to build a list of points to the destpoint.
        /// Uses the Last point in CurrentList as the start point.
        /// Exits if we have more than 15 points
        /// </summary>
        /// <param name="DestPoint">destination</param>
        /// <param name="CurrentList">current list of of points</param>
        /// <param name="PointLimit">number of points to test</param>
        internal static List<PathListInfo.PathPoint> PathAroundAStar_Old(PathListInfo.PathPoint DestPoint, List<PathListInfo.PathPoint> CurrentList, int PointLimit)
        {
            List<PathListInfo.PathPoint> retList = new List<PathListInfo.PathPoint>();
            PathListInfo.PathPoint LastPoint, testPoint;
            int degrees = 0, radius = 10;

            // sets the minimum distance between points on this list
            int MinDist = 8;

            try
            {
                // if we have no current list, then add our current point
                if ((CurrentList == null) || (CurrentList.Count == 0))
                {
                    CurrentList = new List<PathListInfo.PathPoint>();
                    CurrentList.Add(clsCharacter.MyLocation);
                    clsSettings.Logging.AddToLog(Resources.PathAroundAStar, "Resetting PathAroundTime");
                    PathAroundTimer = DateTime.Now.AddMilliseconds(5000);
                }

                // log it
                clsSettings.Logging.AddToLogFormatted(Resources.PathAroundAStar, Resources.PathAroundAStarLog, CurrentList.Count, DestPoint.ToString());

                // if we hit our limit, return
                if ((PointLimit > 0) && (CurrentList.Count > PointLimit))
                    return CurrentList;

                // exit if we took too much time
                if (PathAroundTimer <= DateTime.Now)
                    return PointLimit > 0 ? CurrentList : null;

                // make a copy of current list
                retList.AddRange(CurrentList);

                // get the last point
                LastPoint = retList[retList.Count - 1];

                // find the best point from here
                using (new clsFrameLock.LockBuffer())
                {
                    do
                    {
                        do
                        {
                            // exit if we took too much time
                            if (PathAroundTimer <= DateTime.Now)
                                break;

                            // set test x and y
                            testPoint = new PathListInfo.PathPoint(
                                LastPoint.X + (radius * Math.Cos(degrees)),
                                LastPoint.Y + (radius * Math.Sin(degrees)),
                                LastPoint.Z);

                            // skip the point is too close to something already in the list
                            bool skipPoint = false;
                            foreach (PathListInfo.PathPoint listPoint in retList)
                            {
                                // if too close, skip
                                if (listPoint.Distance(testPoint) <= MinDist)
                                {
                                    skipPoint = true;
                                    break;
                                }
                            }

                            // if we should skip because it is too close, do so
                            if (skipPoint)
                                continue;

                            // check if we have a good point
                            if ((!clsSettings.isxwow.Me.IsPathObstructed(testPoint.X, testPoint.Y, testPoint.Z, 8, LastPoint.X, LastPoint.Y, LastPoint.Z)) &&
                                (!clsPath.IsPathObstructed(LastPoint, testPoint)))
                            {
                                // check if no collision to the destination
                                if ((!clsSettings.isxwow.Me.IsPathObstructed(testPoint.X, testPoint.Y, testPoint.Z, 8, DestPoint.X, DestPoint.Y, DestPoint.Z)) &&
                                    (!clsPath.IsPathObstructed(testPoint, DestPoint)))
                                {
                                    // found path
                                    retList.Add(testPoint);
                                    retList.Add(DestPoint);
                                    return retList;
                                }

                                // no clear path to destination found
                                // add to the pathlist
                                //pointList.Add(testPoint);
                                retList.Add(testPoint);
                                return retList;
                            }

                        } while ((degrees += 10) < 361);

                        // exit if we took too much time
                        if (PathAroundTimer <= DateTime.Now)
                            break;

                        // only loop once
                    } while (false); // ((radius += (MinDist + 1)) < distance);
                }

                // exit if we took too much time
                if (PathAroundTimer <= DateTime.Now)
                    return PointLimit > 0 ? retList : null;

                /*
                // loop through and try each point to see if we can build a path
                foreach (PathListInfo.PathPoint pPoint in pointList)
                {
                    // exit if we took too much time
                    if (PathAroundTimer <= DateTime.Now)
                        return PointLimit > 0 ? retList : null;

                    // test for pause/stop
                    if (! clsSettings.TestPauseStop("PathAround"AStar, "Exiting due to script stop"))
                        return null;

                    // copy retList and add this point
                    paramList = new List<PathListInfo.PathPoint>();
                    paramList.AddRange(retList);
                    paramList.Add(pPoint);

                    // test it
                    testList = PathAroundAStar(DestPoint, paramList, PointLimit);

                    // if we have something, return it
                    if ((testList != null) && (testList.Count > 0))
                        return testList;
                }
                */
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.PathAroundAStar);
            }

            // if we are here, nothing was found
            return null;
        }

        /// <summary>
        /// Sorts the point list so the first element is closest to DestPoint
        /// </summary>
        /// <param name="DestPoint">Destination Point</param>
        /// <param name="pointList">the list to sort</param>
        public static List<PathListInfo.PathPoint> ResortPointList(PathListInfo.PathPoint DestPoint, List<PathListInfo.PathPoint> pointList)
        {
            List<PathListInfo.PathPoint> retList = new List<PathListInfo.PathPoint>();
            int j, k = pointList.Count;

            // loop through the list until we have all the points
            for (j = 0; j < k; j++)
            {
                // reset bestPoint
                PathListInfo.PathPoint bestPoint = null;

                // find the closest point
                int i;
                for (i = 0; i < k; i++)
                {
                    // skip if we already have this point
                    if (retList.Contains(pointList[i]))
                        continue;

                    // we have no best point, or this point is closer to dest, then use it
                    if ((bestPoint == null) || (DestPoint.Distance(bestPoint) > DestPoint.Distance(pointList[i])))
                        bestPoint = pointList[i];
                }

                // add the point to the list
                if (bestPoint != null)
                    retList.Add(bestPoint);
            }

            return retList;
        }

        // PathAround A*
        #endregion
    }
}