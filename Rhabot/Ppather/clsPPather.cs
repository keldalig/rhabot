using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WowTriangles;
using PathMaker.Graph;
using System.Threading;
using ISXBotHelper.AutoNav;
using ISXBotHelper;
using ISXBotHelper.Properties;

namespace PathMaker
{
    public static class clsPPatherExtensions
    {
        #region Extensions

        /// <summary>
        /// Convert a pathpoint to a location
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Location ToLocation(this PathListInfo.PathPoint point)
        {
            return new Location((float)point.X, (float)point.Y, (float)point.Z);
        }

        public static Location ToLocation(this ISXBotHelper.AutoNav.clsPathPoint point)
        {
            return new Location((float)point.X, (float)point.Y, (float)point.Z);
        }

        // Extensions
        #endregion
    }

    internal class clsPPather : ISXBotHelper.Threading.ThreadBase
    {
        #region Variables

        /// <summary>
        /// Local copy of the bad point list
        /// </summary>
        private ReadOnlyCollection<Location> BadPointList_Local = null;

        /// <summary>
        /// Distance to put between each point
        /// </summary>
        private float PointDistance = 12f;

        // pather objects
        private MPQTriangleSupplier mpq = new MPQTriangleSupplier();
        private ChunkedTriangleCollection triangleWorld;

        /// <summary>
        /// List of points currently considered good
        /// </summary>
        private List<Location> PathList = new List<Location>();

        /// <summary>
        /// list of points which cannot be used, usually because they are unwalkable
        /// </summary>
        private List<Location> SkipList = new List<Location>();

        /// <summary>
        /// the "halfway" distance between two points. a distance less than this is
        /// considered to be the same point
        /// </summary>
        private float HalfPoint = 0;

        // Variables
        #endregion

        #region BuildPath

        /// <summary>
        /// Builds a path from one point to a list of points
        /// </summary>
        /// <param name="zoneName">the zone name this path will be in</param>
        /// <param name="startLocation">start x,y,z</param>
        /// <param name="endLocations">list of points to find in the path</param>
        /// <param name="SearchRange">the distance the toon performs searching for new mobs</param>
        public List<clsPathPoint> BuildPath(string zoneName, Location startLocation, Location endLocation, int SearchRange)
        {
            return BuildPath(zoneName, startLocation, new List<Location>(1) { endLocation }, SearchRange);
        }

        /// <summary>
        /// Builds a path from one point to a list of points
        /// </summary>
        /// <param name="zoneName">the zone name this path will be in</param>
        /// <param name="startLocation">start x,y,z</param>
        /// <param name="endLocations">list of points to find in the path</param>
        /// <param name="SearchRange">the distance the toon performs searching for new mobs</param>
        public List<clsPathPoint> BuildPath(string zoneName, Location startLocation, List<Location> endLocations, int SearchRange)
        {
            float degree, SlopeX, SlopeY;
            int index;
            Location tempLoc, BestLoc;
            List<Location> DestList, FinalList = new List<Location>();
            bool FirstPoint = true;

            // distance from target
            float EndPointDistance = PointDistance * 2.5f;
            HalfPoint = PointDistance / 2;

            // exit if no list
            if ((endLocations == null) || (endLocations.Count == 0))
                return new List<clsPathPoint>();

            try
            {
                // get a local copy of the bad point list
                BadPointList_Local = BadPointList;

                // I don't know why, but the zone must start with a colon (:)
                if (!zoneName.StartsWith(":"))
                    zoneName = string.Format(":{0}", zoneName);
                mpq.SetZone(zoneName);

                // init the triangleworld
                triangleWorld = new ChunkedTriangleCollection();
                triangleWorld.SetMaxCached(4000);
                triangleWorld.AddSupplier(mpq);

                // set my location
                Location CurrentLocation = new Location((float)startLocation.X, (float)startLocation.Y, (float)startLocation.Z);

                // sort the list by what is closest to the start
                DestList = SortDestList(endLocations, CurrentLocation);

                // if we hit this mark, we timed out
                DateTime TimeOutTime = DateTime.Now.AddMinutes(2);

                // loop through each destination point
                foreach (Location LongEndLoc in DestList)
                {
                    // skip if this point is near to other points on the list
                    if (FinalList.Any(x => x.GetDistanceTo(LongEndLoc) <= SearchRange))
                        continue;

                    // clear variables
                    tempLoc = null;
                    PathList.Clear();
                    SkipList.Clear();

                    // check for timeouts
                    if ((this.Shutdown) || (DateTime.Now > TimeOutTime))
                        break;

                    // set the loop endtime (set to 2 min if only one item in the list)
                    DateTime EndTime = DateTime.Now.AddSeconds(FirstPoint ? 30 : 10);
                    if ((FirstPoint) && (DestList.Count == 1))
                        EndTime = DateTime.Now.AddMinutes(2);
                    FirstPoint = false;

                    // loop until we are close enough
                    while (CurrentLocation.GetDistanceTo(LongEndLoc) > EndPointDistance)
                    {
                        // exit if we have been in this loop too long
                        if (DateTime.Now > EndTime)
                        {
                            PathList.Clear();
                            break;
                        }

                        // reset best location
                        BestLoc = null;

                        // loop through circle to find closest point
                        for (degree = 0; degree < 360; degree += 15)
                        {
                            // get the next point to check
                            SlopeX = CurrentLocation.X + (PointDistance * (float)Math.Cos(degree));
                            SlopeY = CurrentLocation.Y - (PointDistance * (float)Math.Sin(degree));
                            tempLoc = new Location(SlopeX, SlopeY, CurrentLocation.Z);

                            // test the point. if good, assign to best loc
                            if (TestPoint(BestLoc, tempLoc, CurrentLocation, LongEndLoc))
                                BestLoc = tempLoc;
                        }

                        // if we found something, add this point to the list
                        if (BestLoc != null)
                        {
                            PathList.Add(BestLoc);
                            CurrentLocation = BestLoc;
                        }

                        // nothing found, backup one point
                        else
                        {
                            // only remove a point if this is not our last,final test
                            // get the point to remove
                            index = PathList.Count - 1;

                            // if there are no points to remove, then we cannot proceed
                            if (index < 0)
                                break;

                            // remove the point
                            SkipList.Add(PathList[index]);
                            PathList.RemoveAt(index);

                            // reset current location
                            if (PathList.Count > 0)
                                CurrentLocation = PathList[index - 1];
                        }
                    }

                    // update return list
                    if (PathList.Count > 0)
                    {
                        // update finallist
                        FinalList.AddRange(PathList);

                        // add end location
                        FinalList.Add(LongEndLoc);
                    }

                    // set the new start location
                    if (FinalList.Count > 0)
                        CurrentLocation = FinalList[FinalList.Count - 1];
                }
            }

            // skip if thread abort
            catch (ThreadAbortException) { }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.AutoNavPath);
            }

            finally
            {
                // make sure the list isn't null
                if (FinalList == null)
                    FinalList = new List<Location>();
            }

            // return the list
            return FinalList.ConvertAll<clsPathPoint>(p => new clsPathPoint(p.X, p.Y, p.Z));
        }

        // BuildPath
        #endregion

        #region Helpers

        /// <summary>
        /// Sort the destination list. first point is closest to StartPoint
        /// </summary>
        /// <param name="DestList">the list to sort</param>
        /// <param name="StartPoint">the point to test against for distance</param>
        private List<Location> SortDestList(List<Location> DestList, Location StartPoint)
        {
            // build a new list
            int lCount = DestList.Count;
            List<Location> TestList = DestList, retList = new List<Location>(lCount);
            Location TestPoint = StartPoint;

            // loop through until we have everything
            for (int i = 0; i < lCount; i++)
            {
                // build the new list
                if (i > 0)
                    TestList.RemoveAt(0);

                // sort the list
                TestList.Sort((p1, p2) => p1.GetDistanceTo(TestPoint) < p2.GetDistanceTo(TestPoint) ? -1 : 1);

                // set the next TestPoint
                TestPoint = TestList[0];

                // add the first point in this list to the return list, since it is closest
                retList.Add(TestPoint);

                // exit if the final point
                if (TestList.Count == 2)
                {
                    // add the last point, then exit the loop
                    retList.Add(TestList[1]);
                    break;
                }
            }

            // return the list
            return retList;
        }

        private const float TestPointHeight = 0.5f;
        private const float TestPointSize = 2.5f;

        /// <summary>
        /// Test if a point is walkable and not in a skip list.
        /// Returns false if the point cannot be used
        /// </summary>
        /// <param name="tempLoc">the point to test</param>
        /// <param name="LastPoint">the previous point in the list</param>
        /// <param name="DestPoint">the destination point</param>
        private bool TestPoint(Location BestLoc, Location tempLoc, Location LastPoint, Location DestPoint)
        {
            try
            {
                // skip if this is in the path or skip list
                if ((BadPointList_Local.Any<Location>(x => x.GetDistanceTo(tempLoc) <= HalfPoint)) ||
                    (SkipList.Any<Location>(x => x.GetDistanceTo(tempLoc) <= HalfPoint)) ||
                    (PathList.Any<Location>(x => x.GetDistanceTo(tempLoc) <= HalfPoint)))
                    return false;

                // skip if blocked
                if ((!triangleWorld.IsStepBlocked(LastPoint.X, LastPoint.Y, LastPoint.Z, tempLoc.X, tempLoc.Y, tempLoc.Z, TestPointHeight, TestPointSize)))// ||
                    //(!triangleWorld.IsStepBlocked(tempLoc.X, tempLoc.Y, tempLoc.Z, LastPoint.X, LastPoint.Y, LastPoint.Z, TestPointHeight, TestPointSize)))
                {
                    AddToBadPointList(tempLoc);
                    SkipList.Add(tempLoc);
                    return false;
                }

                // if this point is better than the current best, use it
                if ((BestLoc == null) ||
                    (BestLoc.GetDistanceTo(DestPoint) > tempLoc.GetDistanceTo(DestPoint)))
                    return true;
            }

            // skip if thread abort
            catch (ThreadAbortException) { } 

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.AutoNavPath, Resources.AutoNavTestPoint);

                // for safety, add to bad lists (??)
                AddToBadPointList(tempLoc);
                SkipList.Add(tempLoc);
            }

            // everything failed
            return false;
        }

        private double GetHeadingHeading;
        /// <summary>
        /// Get the heading between two points
        /// </summary>
        /// <param name="StartPoint"></param>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        private double GetHeading(Location StartPoint, Location EndPoint)
        {
            GetHeadingHeading = Math.Atan(EndPoint.Y - StartPoint.Y) / (EndPoint.X - StartPoint.X);
            if (GetHeadingHeading < 0)
                GetHeadingHeading += 360;
            return GetHeadingHeading;
        }

        // Helpers
        #endregion

        #region BadPointList

        /// <summary>
        /// List of points which are not walkable
        /// </summary>
        private static List<Location> m_BadPointList = new List<Location>();

        /// <summary>
        /// Lock for the bad point list
        /// </summary>
        private static object BadPointLock = new object();

        /// <summary>
        /// Adds a point to the bad point list
        /// </summary>
        /// <param name="BadPoint"></param>
        private static void AddToBadPointList(Location BadPoint)
        {
            // add the point to the list
            lock (BadPointLock)
                m_BadPointList.Add(BadPoint);
        }

        /// <summary>
        /// Returns a readonly copy of the bad point list
        /// </summary>
        private static ReadOnlyCollection<Location> BadPointList
        {
            get
            {
                // return the list
                lock (BadPointLock)
                    return new ReadOnlyCollection<Location>(m_BadPointList);
            }
        }

        // BadPointList
        #endregion
    }
}
