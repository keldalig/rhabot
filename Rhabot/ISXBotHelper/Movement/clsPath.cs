// http://www.lavishsoft.com/wiki/index.php/IS:.NET

// Flying: http://www.isxwow.net/forums/viewtopic.php?f=23&t=542&hilit=

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ISXBotHelper.AutoNav;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;
using ISXBotHelper.XProfile;
using ISXWoW;
using LavishScriptAPI;
using LavishVMAPI;
using MerlinEncrypt;
using Rhabot;
using rs;

namespace ISXBotHelper
{
    /// <summary>
    /// Functions for adding and reading paths
    /// </summary>
    public partial class clsPath : ThreadBase
    {
        #region Enums

        public enum EMovementResult
        {
            Success = 0,

            /// <summary>
            /// can't move
            /// </summary>
            Error,

            /// <summary>
            /// can't move
            /// </summary>
            PathObstructed,

            /// <summary>
            /// Stuck and we can't find a way to get out
            /// When this is returned, we should already be doing a shutdown if the settings is enabled
            /// </summary>
            Stuck,

            /// <summary>
            /// aggroed mob, still moving!
            /// </summary>
            Aggroed,

            /// <summary>
            /// Returned when the script is stopped
            /// </summary>
            Stopped,

            /// <summary>
            /// Returned when bags are full or durability is low
            /// </summary>
            NeedVendor,

            /// <summary>
            /// Returned if MoveThroughPath is handling combat and we died
            /// </summary>
            Dead
        }

        /// <summary>
        /// Type of item returned by move through path
        /// </summary>
        public enum EItemType
        {
            HostileUnit = 0,
            Mine,
            Herb,
            Box,

            /// <summary>
            /// returned when path is successfully navigated and no units found. movement is stopped
            /// </summary>
            None,

            /// <summary>
            /// returned when we aggroed one or more units. Movement stops
            /// </summary>
            Aggroed,

            /// <summary>
            /// Error. can't move, movement stopped
            /// </summary>
            Error,

            /// <summary>
            /// Returned when scripting is stopped
            /// </summary>
            Stopped
        }

        // Enums
        #endregion

        #region Variables

        /// <summary>
        /// used for recording a path
        /// </summary>
        private List<PathListInfo.PathPoint> RecordPathList = new List<PathListInfo.PathPoint>();

        /// <summary>
        /// true when path is recording
        /// </summary>
        private bool IsPathRecording = false;

        /// <summary>
        /// True when recording path is paused
        /// </summary>
        private bool IsPathRecPaused = false;

        /// <summary>
        /// Thread the path record is run in
        /// </summary>
        private clsSettings.ThreadItem ThreadItemRecordPath;

        // MTP Threads
        private clsSettings.ThreadItem ThreadItemHerb, ThreadItemHunt;

        /// <summary>
        /// Set to true in the dispose method. Ensures we only dispose once
        /// </summary>
        private bool IsDisposed = false;

        private readonly WoWMovement movement = null;

        private static WoWMovement m_sMovement = null;
        public static WoWMovement sMovement
        {
            get
            {
                // get the movement object if we don't have it
                if (m_sMovement == null)
                {
                    using (new clsFrameLock.LockBuffer())
                        m_sMovement = WoWMovement.Get();
                }

                return m_sMovement;
            }
        }

        internal static int m_Moving = 0; // 0 = false, 1= true
        /// <summary>
        /// Returns true when we are moving
        /// </summary>
        public static bool Moving
        {
            get
            {
                // return our result
                return Thread.VolatileRead(ref m_Moving) == 1;
            }
            set
            {
                Thread.VolatileWrite(ref m_Moving, value ? 1 : 0);
            }
        }

        private static int m_MovingHold = 0; // 0 = false, 1 = true
        /// <summary>
        /// When true, movement will be stopped
        /// </summary>
        public static bool MovingHold
        {
            get { return Thread.VolatileRead(ref m_MovingHold) == 1; }
            set { Thread.VolatileWrite(ref m_MovingHold, value ? 1 : 0); }
        }

        // Variables
        #endregion

        #region NearPathPoint

        /// <summary>
        /// Used as return for GetNearestPoint
        /// </summary>
        public class NearPathPoint
        {
            public PathListInfo.PathPoint PPoint = null;
            public int Element = -1;
        }

        // NearPathPoint
        #endregion

        #region Init / Finalize

        /// <summary>
        /// Initializes a new instance of the clsPath class.
        /// </summary>
        public clsPath()
        {
            using (new clsFrameLock.LockBuffer())
                movement = WoWMovement.Get();
        }

        // Init / Finalize
        #endregion

        #region Record Path

        private bool RecodPath_CanMountOrFly = false;

        /// <summary>
        /// Begins recording a path. Call RecordPath_Stop to return the path
        /// </summary>
        /// <param name="canMountOrFly">true if the path is mountable or flyable</param>
        public void RecordPath_Start(bool canMountOrFly)
        {
            // if already recording, show error
            if ((IsPathRecording) || ((ThreadItemRecordPath != null) && (ThreadItemRecordPath.thread != null)))
            {
                clsError.ShowError(new Exception("Can not start path recording. Another recording process is already running"), string.Empty);
                return;
            }

            // set mount/fly
            RecodPath_CanMountOrFly = canMountOrFly;

            // run path recording in a new thread
            IsPathRecording = true;
            IsPathRecPaused = false;
            Thread threadRecordPath = new Thread(RecordPath);
            threadRecordPath.Name = "Record Path";
            ThreadItemRecordPath = new clsSettings.ThreadItem(threadRecordPath, this);
            clsSettings.GlobalThreadList.Add(ThreadItemRecordPath);
            threadRecordPath.Start();
        }

        /// <summary>
        /// Stops recording a path and returns the path list
        /// </summary>
        /// <returns></returns>
        public List<PathListInfo.PathPoint> RecordPath_Stop()
        {
            // stop path recording
            IsPathRecording = false;
            IsPathRecPaused = false;

            // log the stop
            clsSettings.Logging.AddToLog("Stopping RecordPath");

            // set shutdown flag
            Shutdown = true;

            // return the list
            return RecordPathList;
        }

        /// <summary>
        /// Pauses path recording. Call again to Unpase
        /// </summary>
        public void RecordPath_Pause()
        {
            // set pause variable
            IsPathRecPaused = !IsPathRecPaused;

            // log it
            clsSettings.Logging.AddToLog("Pausing Record Path");
        }

        /// <summary>
        /// Starts recording the path in a new thread
        /// </summary>
        private void RecordPath()
        {
            PathListInfo.PathPoint tempPoint;
            int PathPrecision = clsSettings.SavePathPrecision * (RecodPath_CanMountOrFly ? 1 : 2);
            double LastHeading = 0, CurrDist = 0;

            // log it
            clsSettings.Logging.AddToLog("Recording New Path");

            // reset path list
            RecordPathList = new List<PathListInfo.PathPoint>();

            // loop while we can
            try
            {
                int iPathCount = RecordPathList.Count;
                LastHeading = clsCharacter.MyHeading;

                while ((IsPathRecording) && (!Shutdown))
                {
                    Thread.Sleep(10);

                    // skip if paused
                    while (IsPathRecPaused)
                        Thread.Sleep(100);

                    // lock the frame, get the location, unlock frame
                    tempPoint = clsCharacter.MyLocation;

                    // get distance from last point
                    if (iPathCount > 0)
                        CurrDist = RecordPathList[iPathCount - 1].Distance(tempPoint);

                    // add it to the list
                    if ((iPathCount == 0) || 
                        (CurrDist > PathPrecision) ||
                        ((Math.Abs(LastHeading - clsCharacter.MyHeading) >= 45) && (CurrDist > 7)))
                    {
                        // log it
                        clsSettings.Logging.AddToLogFormatted("Path", "Adding PathPoint: {0}", tempPoint.ToString());

                        // add it to the list
                        RecordPathList.Add(tempPoint);

                        // raise the event
                        clsEvent.Raise_PathPointAdded(RecordPathList, tempPoint);

                        // get the count of items in path list
                        iPathCount = RecordPathList.Count;
                    }
                }
            }

            catch (Exception excep)
            {
                // stop recording
                IsPathRecording = false;
                IsPathRecPaused = false;
                Shutdown = true;

                // show error
                clsError.ShowError(excep, "Path recording stopped due to an error");
            }
        }

        // Record Path
        #endregion

        #region Save / Load Path

        private const string entryA_X = "X"; // X attribute
        private const string entryA_Y = "Y"; // Y attribute
        private const string entryA_Z = "Z"; // Z attribute

        private static string GetPathPointEntryName(int Counter)
        {
            return string.Format("PathPoint_{0}", Counter.ToString().Trim());
        }

        private static string GetPathPointConnectedName(int Counter)
        {
            return string.Format("ConnectsTo_{0}", Counter.ToString().Trim());
        }

        /// <summary>
        /// Loads a path list
        /// </summary>
        /// <param name="PathFile">the full path to the file where paths are stored</param>
        /// <param name="PathName">the name of the path to load from the file</param>
        public static PathListInfoEx LoadPathListFromFile(string PathFile, string PathName)
        {
            PathListInfoEx retPath = new PathListInfoEx();
            PathListInfo.PathPoint pPoint;
            //string sectionCanDo = "CanDo";

            try
            {
                // log it
                clsSettings.Logging.AddToLog(string.Format("Loading Path List: {0} in {1}", PathName, PathFile));

                // fail if the file does not exist
                if (!File.Exists(PathFile))
                    throw new Exception(string.Format("Path file does not exist. - {0}", PathFile));

                // open the file
                Xml xml = new Xml(PathFile);
                using (xml.Buffer(true))
                {
                    // if the path name does not exist, return empty list
                    if (!xml.HasSection(PathName))
                        return null;

                    // get the path point list
                    int pathCounter = 0;
                    string entryName;
                    while (true)
                    {
                        // get the point's name
                        pathCounter++;
                        entryName = GetPathPointEntryName(pathCounter);

                        // if the section doesn't exist, then break
                        if (!xml.HasEntry(PathName, entryName))
                            break;

                        // create a new pathpoint
                        pPoint = new PathListInfo.PathPoint();
                        pPoint.X = Convert.ToDouble(xml.GetValue_Attribute(PathName, entryName, entryA_X));
                        pPoint.Y = Convert.ToDouble(xml.GetValue_Attribute(PathName, entryName, entryA_Y));
                        pPoint.Z = Convert.ToDouble(xml.GetValue_Attribute(PathName, entryName, entryA_Z));

                        // add the point to the list
                        retPath.PathList.Insert(pathCounter - 1, pPoint);
                    }

                    // get the list of connected paths
                    pathCounter = 0;
                    while (true)
                    {
                        // get the connected to name
                        pathCounter++;
                        entryName = GetPathPointConnectedName(pathCounter);

                        // if the section doesn't exist, then break
                        if (!xml.HasEntry(PathName, entryName))
                            break;

                        // create a new connected
                        retPath.ConnectsTo.Add((string)xml.GetValue(PathName, entryName));
                    }

                    // get fly/swim/mount
                    retPath.CanMount = clsSettings.XMLGet_Bool(PathName, "CanMount", xml);
                    retPath.CanFly = clsSettings.XMLGet_Bool(PathName, "CanFly", xml);
                    retPath.CanSwim = clsSettings.XMLGet_Bool(PathName, "CanSwim", xml);
                }

                // set the path name
                retPath.PathName = PathName;
            }

            catch (Exception excep)
            {
                // display error
                clsError.ShowError(excep, "Error while loading path list (LoadPathList)");
            }

            // return the list
            return retPath;
        }

        /// <summary>
        /// Loads all the paths in the path file and returns them in a List
        /// </summary>
        /// <param name="PathFile">the file to load</param>
        public List<PathListInfoEx> LoadAllPathsFromFile(string PathFile)
        {
            List<string> PathNameList = new List<string>();
            List<PathListInfoEx> retList = new List<PathListInfoEx>();

            try
            {
                // log it
                clsSettings.Logging.AddToLogFormatted("Path", "Loading all paths from {0}", PathFile);

                // open the pathfile
                Xml xml = new Xml(PathFile);

                using (xml.Buffer(false))
                {
                    // read all the paths from the file
                    string[] names = xml.GetSectionNames();
                    if ((names != null) && (names.Length > 0))
                        PathNameList.AddRange(xml.GetSectionNames());
                }

                // loop through all paths and load them to the list
                foreach (string pathName in PathNameList)
                    retList.Add(LoadPathListFromFile(PathFile, pathName));
            }

            catch (Exception excep)
            {
                // display error
                clsError.ShowError(excep, "Error while loading all path lists (LoadAllPaths)");
            }

            // log result
            clsSettings.Logging.AddToLogFormatted("Path", "{0} Paths loaded", retList.Count.ToString());

            return retList;
        }

        /// <summary>
        /// Loads a path list
        /// </summary>
        /// <param name="PathName">the name of the path to load</param>
        /// <param name="GroupName">name of the group the path is in</param>
        /// <param name="Level"></param>
        public static PathListInfoEx LoadPathList(string PathName, string GroupName, int Level)
        {
            try
            {
                // get the path from the server
                PathListInfoEx pInfo = new PathListInfoEx();
                pInfo.pathListInfo = clsSerialize.Deserialize(new clsRS().GetPath(clsSettings.LoginInfo.UserID, PathName, GroupName, Level, clsSettings.UpdateText, clsSettings.IsDCd), typeof(PathListInfo)) as PathListInfo;
                return pInfo;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.PathIO, Resources.CouldNotLoadPath);
            }

            return null;
        }

        /// <summary>
        /// Loads a path list
        /// </summary>
        /// <param name="PathID">the path id to load</param>
        public static PathListInfoEx LoadPathList(int PathID)
        {
            try
            {
                // get the path from the server
                PathListInfoEx pInfo = new PathListInfoEx();
                pInfo.pathListInfo = clsSerialize.Deserialize(new clsRS().GetPathByID(clsSettings.LoginInfo.UserID, PathID, clsSettings.UpdateText, clsSettings.IsDCd), typeof(PathListInfo)) as PathListInfo;
                return pInfo;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.PathIO, Resources.CouldNotLoadPath);
            }

            return null;
        }

        /// <summary>
        /// Loads all the paths in the path file and returns them in a List
        /// </summary>
        /// <param name="GroupName">name of the group the path is in</param>
        /// <param name="Level">level of the path to load</param>
        public List<PathListInfoEx> LoadAllPaths(string GroupName, int Level)
        {
            List<PathListInfoEx> retList = new List<PathListInfoEx>();

            try
            {
                // get the list from the server
                Dictionary<string, string> pList = new clsRS().GetPathList(clsSettings.LoginInfo.UserID, GroupName, Level, clsSettings.UpdateText, clsSettings.IsDCd);

                // get all the keys in the dict
                if ((pList != null) && (pList.Count > 0))
                {
                    foreach (string value in pList.Values)
                        retList.Add(new PathListInfoEx(value.DeserializePathListInfo()));
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.PathIO, Resources.CouldNotLoadPath);
            }

            return retList;
        }

        /// <summary>
        /// Saves a path list
        /// </summary>
        /// <param name="PathName">the name of the path to load from the file</param>
        /// <param name="PathInfo">the path list to save</param>
        /// <param name="GroupName"></param>
        /// <param name="PathEndLevel"></param>
        /// <param name="PathStartLevel"></param>
        public static void SavePathList(string PathName, string GroupName, int PathStartLevel, int PathEndLevel, PathListInfoEx PathInfo)
        {
            try
            {
                #region Save To DB

                // log it
                //clsSettings.Logging.AddToLogFormatted("PathIO", clsEnums.GetResource(clsEnums.EResourceName.SavePathXGroupYLevelZZ), PathName, GroupName, PathStartLevel, PathEndLevel);

                // save the path on the server
                new clsRS().SaveMultiPaths(
                    clsSettings.LoginInfo.UserID,
                    PathName,
                    GroupName,
                    PathStartLevel,
                    PathEndLevel,
                    PathInfo.pathListInfo.Serialize(),
                    clsSettings.UpdateText,
                    clsSettings.IsDCd);


                // Save To DB
                #endregion
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.PathIO, Resources.PathCouldNotBeSaved);
            }
        }

        // Save / Load Path
        #endregion

        #region Functions

        /// <summary>
        /// Returns the nearest point in the path, or null if nothing can be found
        /// </summary>
        /// <param name="CurrentPath">the path to search</param>
        /// <param name="CurrentPoint">point you want to move FROM</param>
        public NearPathPoint GetNearestPoint(PathListInfoEx CurrentPath, PathListInfo.PathPoint CurrentPoint)
        {
            return GetNearestPoint(CurrentPath, CurrentPoint, 150);
        }

        /// <summary>
        /// Returns the nearest point in the path, or null if nothing can be found
        /// </summary>
        /// <param name="CurrentPath">the path to search</param>
        /// <param name="CurrentPoint">point you want to move FROM</param>
        /// <param name="NearestDistance">how far from the path to search</param>
        public NearPathPoint GetNearestPoint(PathListInfoEx CurrentPath, PathListInfo.PathPoint CurrentPoint, double NearestDistance)
        {
            // exit if no path
            if ((CurrentPath == null) || (CurrentPoint == null))
                return null;

            int NearestElement = -1, j = CurrentPath.PathList.Count;

            // loop through the list to find the nearest point
            for (int i = 0; i < j; i++)
            {
                // see if this point is closer
                double tempDistance = CurrentPath.PathList[i].Distance(CurrentPoint);

                if (tempDistance < NearestDistance)
                {
                    // this point is closer than the last good point
                    NearestDistance = tempDistance;
                    NearestElement = i;
                }
            }

            // if no element found, return null
            if (NearestElement < 0)
                return null;

            // build our return
            NearPathPoint retVal = new NearPathPoint();
            retVal.Element = NearestElement;
            retVal.PPoint = CurrentPath.PathList[NearestElement];

            return retVal;
        }

        /// <summary>
        /// Returns the path that is closest to our current location. If no path can be found, returns null
        /// </summary>
        /// <param name="PathList">list of paths to search</param>
        internal static PathListInfoEx GetNearestPath(List<PathListInfoEx> PathList)
        {
            PathListInfo.PathPoint NearestPoint = null;
            PathListInfoEx NearestPath = null;
            NearPathPoint nearPP;
            clsPath cPath = new clsPath();
            PathListInfo.PathPoint MyLoc = clsCharacter.MyLocation;

            try
            {
                // exit if nothing sent
                if ((PathList == null) || (PathList.Count == 0))
                    return null;

                // if only one path, return that one
                if (PathList.Count == 1)
                    return PathList[0];

                // loop through each path to find the nearest one
                foreach (PathListInfoEx pInfo in PathList)
                {
                    // get nearest point
                    nearPP = cPath.GetNearestPoint(pInfo, MyLoc);

                    // skip if nothing found
                    if ((nearPP == null) || (nearPP.PPoint == null))
                        continue;

                    // set if we have nothing yet
                    if (NearestPoint == null)
                    {
                        NearestPoint = nearPP.PPoint;
                        NearestPath = pInfo;
                        continue;
                    }

                    // see if the new point is closer than the other point
                    if (nearPP.PPoint.Distance(MyLoc) < NearestPoint.Distance(MyLoc))
                    {
                        // found closer path
                        NearestPoint = nearPP.PPoint;
                        NearestPath = pInfo;
                        continue;
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GetNearestPath");
                NearestPath = null;
            }

            return NearestPath;
        }

        /// <summary>
        /// Returns a PathPoint for this unit's current location
        /// </summary>
        public static PathListInfo.PathPoint GetUnitLocation(WoWObject unit)
        {
            using (new clsFrameLock.LockBuffer())
                return new PathListInfo.PathPoint(unit.Location);
        }

        public static double DistanceToTarget(WoWObject TargetUnit)
        {
            using (new clsFrameLock.LockBuffer())
                return new PathListInfo.PathPoint(clsSettings.isxwow.Me.Location).Distance(new PathListInfo.PathPoint(TargetUnit.Location));
        }

        /// <summary>
        /// Returns the selected path from the path list
        /// </summary>
        public static PathListInfoEx GetPathFromList(List<PathListInfoEx> Paths, string PathName)
        {
            // find our path
            foreach (PathListInfoEx pInfo in Paths)
            {
                // if we found a match, select it and exit loop
                if (pInfo.PathName == PathName)
                    return pInfo;
            }

            // if no path, return null
            clsSettings.Logging.AddToLog("No paths found in path list that match path name");
            return null;
        }

        /// <summary>
        /// Runs the HuntStart path.
        /// </summary>
        /// <param name="HuntStart">the hunt start path. Send null to load the path from memory</param>
        /// <param name="Direction">true to run the path forward, false to run the path backward</param>
        public static bool RunHuntStart(PathListInfoEx HuntStart, bool Direction)
        {
            try
            {
                // if no hunt start, get from memory
                if (HuntStart == null)
                    HuntStart = GetPathFromList(clsGlobals.Paths, clsGlobals.Path_StartHunt);
                if (HuntStart == null)
                    throw new clsRhabotException("Could not find Hunt Start path", "Run Hunt Start");

                // change path direction if needed
                if (HuntStart.PathReversed != Direction)
                    HuntStart.ReversePath();

                // run the path
                return RunPath(HuntStart) == EMovementResult.Success;
            }

            catch (clsRhabotException rExcep)
            {
                rExcep.ShowError();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "RunHuntStart");
            }

            return false;
        }

        // Functions
        #endregion

        #region Movement

        /// <summary>
        /// Moves the character in the specified direction (like an autorun. You must call stop). 
        /// does NOT check for obstacles
        /// </summary>
        /// <param name="direction">the direction to move</param>
        public static void Move(MovementDirection direction)
        {
            // log it
            clsSettings.Logging.AddToLog(string.Format("Moving {0}", direction));

            // skip if locked
            if (MovingHold)
            {
                clsSettings.Logging.AddToLog("Move", "Cannot move, because MovingHold is true");
                return;
            }

            // move in the specified direction
            using (new clsFrameLock.LockBuffer())
                WoWMovement.Get().Go(direction);
        }

        /// <summary>
        /// Stops the current movement
        /// </summary>
        public static void StopMoving()
        {
            using (new clsFrameLock.LockBuffer())
                sMovement.Stop();

            // log it
            if (clsSettings.VerboseLogging)
                clsSettings.Logging.AddToLog("Stopped Moving");

            // reset the moving flag
            Moving = false;
        }

        /// <summary>
        /// Moves to the specifed point, keeping your character facing direction of travel
        /// returns movement result. If Aggroed returned, the unit is stopped
        /// </summary>
        public EMovementResult MoveToPoint(PathListInfo.PathPoint target)
        {
            return MoveToPoint(target, true);
        }

        /// <summary>
        /// Moves to the specifed point, keeping your character facing direction of travel
        /// returns movement result. If Aggroed returned, the unit is stopped
        /// </summary>
        public EMovementResult MoveToPoint(PathListInfo.PathPoint target, bool HandleStuck)
        {
            EMovementResult rVal = EMovementResult.Error;
            DateTime startTime = DateTime.Now;
            PathListInfo.PathPoint myLoc;

            try
            {
                // exit if not valid
                if ((!clsSettings.GuidValid) || (target == null) || (!clsCharacter.CharacterIsValid))
                    return EMovementResult.Error;

                // face the target
                clsFace.FacePointEx(target);

                // move forward
                clsStuck.ResetStuck();
                using (new clsFrameLock.LockBuffer())
                {
                    // log it
                    clsSettings.Logging.AddToLog(
                        string.Format("Moving From {0} To {1}\r\nDistance: {2}",
                        clsCharacter.MyLocation, target,
                        target.Distance(clsCharacter.MyLocation)));

                    Move(MovementDirection.Forward);
                    myLoc = clsCharacter.MyLocation;
                }

                // loop until we are near our destination
                while (myLoc.Distance(target) > clsSettings.PathPrecision)
                {
                    // check if stopped/paused
                    if (!clsSettings.TestPauseStop("MoveToPoint Exiting because of script stop"))
                        return EMovementResult.Stopped;

                    // check for combat
                    if (clsCombat.IsInCombat())
                        return EMovementResult.Aggroed;

                    // start moving if not already
                    if (!Moving)
                        StartMoving();

                    // face target if we've been running more than 2 seconds
                    if (startTime <= DateTime.Now)
                    {
                        startTime = DateTime.Now.AddMilliseconds(2000);

                        // face
                        if (!clsFace.FacePointEx(target))
                            StartMoving();

                        // log it
                        clsSettings.Logging.AddToLog(
                            string.Format("MoveToPoint: Moving From {0} To {1}\r\nDistance: {2}",
                                myLoc, target,
                                myLoc.Distance(target)));

                        // path around obstacles if any are found
                        if (IPO(target))
                            StartMoving();

                        // if stuck, then return stuck
                        if (clsStuck.CheckStuckEx(target, HandleStuck))
                            return EMovementResult.Stuck;
                    }

                    // sleep
                    Thread.Sleep(100);

                    // get our location
                    myLoc = clsCharacter.MyLocation;
                }

                // set return value
                rVal = EMovementResult.Success;
            }

            catch (Exception excep)
            {
                if (target != null)
                    clsError.ShowError(excep, string.Format("MoveToPoint ({0}, {1}, {2})", target.X, target.Y, target.Z));
                else
                    clsError.ShowError(excep, "MoveToPoint - Target is null");
            }

            finally
            {
                // force a stop for safety
                StopMoving();
            }

            return rVal;
        }

        // Movement
        #endregion

        #region MoveThroughPath

        public class MoveThroughPathResult
        {
            public EItemType ItemFoundType = EItemType.None;
            public WoWUnit ItemFound = null;
            public EMovementResult MoveResult = EMovementResult.Success;

            /// <summary>
            /// Initializes a new instance of the MoveThroughPathResult class.
            /// </summary>
            /// <param name="itemFoundType"></param>
            /// <param name="itemFound"></param>
            public MoveThroughPathResult(EItemType itemFoundType, WoWUnit itemFound)
            {
                ItemFoundType = itemFoundType;
                ItemFound = itemFound;
            }

            /// <summary>
            /// Initializes a new instance of the MoveThroughPathResult class.
            /// </summary>
            /// <param name="itemFoundType"></param>
            /// <param name="itemFound"></param>
            /// <param name="moveResult"></param>
            public MoveThroughPathResult(EItemType itemFoundType, WoWUnit itemFound, EMovementResult moveResult)
            {
                ItemFoundType = itemFoundType;
                ItemFound = itemFound;
                MoveResult = moveResult;
            }

            /// <summary>
            /// Initializes a new instance of the MoveThroughPathResult class.
            /// </summary>
            public MoveThroughPathResult()
            {
            }
        }

        /// <summary>
        /// Face the next point and move if we are not moving
        /// </summary>
        /// <param name="NextPoint">the point to face</param>
        private void FaceNextPoint(PathListInfo.PathPoint NextPoint)
        {
            // exit if we faced recently
            if (LastStandFace.AddMilliseconds(3000) < DateTime.Now)
            {
                // stand up if sitting
                clsSettings.StandUp();

                // face our target pitch again
                if (clsCharacter.FlyingOrSwimming)
                    ChangePitch(PitchToPoint(NextPoint));

                // reset last stand face
                LastStandFace = DateTime.Now;
            }

            if (LastFace.AddMilliseconds(1500) < DateTime.Now)
            {
                // face. only move if we haven't moved yet
                if ((!clsFace.FacePointEx(NextPoint)) || (!Moving))
                {
                    // move if not moving
                    clsSettings.Logging.AddToLogFormatted(Resources.FaceNextPoint, Resources.StartMoving);
                    StartMoving();
                }

                // reset last face
                LastFace = DateTime.Now;
            }
        }

        // MoveThroughPath
        #endregion

        #region Merge Paths

        /// <summary>
        /// Generates a new Path from your current point to the FinalPath. 
        /// The path from CurrentStep to the start/end of FinalPath is the return result
        /// Null is returned if a path could not be built
        /// </summary>
        /// <param name="CurrentPath">the current working path</param>
        /// <param name="FinalPath">the path to navigate to. Will navigate from the nearest point to the end of the path</param>
        /// <param name="GoToFinalStart">when true, path is built to FinalPath start. When false, path is built to FinalPath end</param>
        /// <returns>The path from CurrentStep to the start/end of FinalPath is the return result. Null is returned if a path could not be built</returns>
        public PathListInfoEx CreatePath(PathListInfoEx CurrentPath, PathListInfoEx FinalPath, bool GoToFinalStart)
        {
            // exit if no paths
            if ((CurrentPath == null) || (FinalPath == null))
                return null;

            PathListInfoEx retPath = new PathListInfoEx();
            PathListInfo.PathPoint CurrentPoint;
            NearPathPoint nearPoint;
            PathListInfo.PathPoint tempPoint;
            int CurrCounter = CurrentPath.PathList.Count;
            int FinalCounter = FinalPath.PathList.Count;
            int CurrStartEl = -1, FinalStartEl = -1;
            int precision = 10; // how far can be from path

            try
            {
                // get our current location
                CurrentPoint = clsCharacter.MyLocation;

                // log it
                clsSettings.Logging.AddToLog(
                    string.Format("Attempting to merge point ({0}, {1}, {2}) on path {3} to path {4}",
                    CurrentPoint.X, CurrentPoint.Y, CurrentPoint.Z,
                    CurrentPath.PathName, FinalPath.PathName));

                // add our current point to the pathlist
                retPath.PathList.Add(CurrentPoint);

                #region Move to current step

                // if we have no step, then we need to get back onto CurrentPath
                if ((CurrentPath.CurrentStep < 0) || (CurrentPoint.Distance(CurrentPath.PathList[CurrentPath.CurrentStep]) > precision))
                {
                    // get the nearest point
                    nearPoint = GetNearestPoint(CurrentPath, CurrentPoint);

                    // if we have no point, then return null (because we can't get onto path)
                    if (nearPoint == null)
                        return null;

                    // add the point to the list
                    retPath.PathList.Add(nearPoint.PPoint);

                    // set currentstep to the nearest point
                    CurrentPath.CurrentStep = nearPoint.Element;
                }

                // Move to current step
                #endregion

                // see if any points are near
                int CurrIndex;
                int FinalIndex;
                for (CurrIndex = 0; CurrIndex < CurrCounter; CurrIndex++)
                {
                    // get this point
                    tempPoint = CurrentPath.PathList[CurrIndex];

                    // see if this point is near anything on the final path
                    for (FinalIndex = 0; FinalIndex < FinalCounter; FinalIndex++)
                    {
                        // if not near, then skip
                        if (tempPoint.Distance(FinalPath.PathList[FinalIndex]) > 7)
                            continue;

                        // close point, so let's use this point
                        // set the start elements for future use
                        CurrStartEl = CurrIndex;
                        FinalStartEl = FinalIndex;
                        break;
                    }

                    // if we have a startel, then we don't need to look any more
                    if (CurrStartEl > 0)
                        break;
                }

                // if we don't have a startel, then no path can be made
                if (CurrStartEl < 0)
                {
                    // log it
                    clsSettings.Logging.AddToLog(string.Format("Could not create path to {0}. Could not find near point", FinalPath.PathName));

                    // return
                    return null;
                }

                // build a path from here to the currstartel
                int CurrStep = CurrentPath.CurrentStep;
                int i = Math.Abs(CurrentPath.CurrentStep - CurrStartEl);
                for (CurrIndex = 0; CurrIndex < i; CurrIndex++)
                {
                    // get next or previous step
                    if (CurrStartEl > CurrStep)
                        retPath.PathList.Add(CurrentPath.GetNextPoint());
                    else
                        retPath.PathList.Add(CurrentPath.GetPreviousPoint());
                }

                // reset current step
                CurrentPath.CurrentStep = CurrStep;

                // build a path from the finalstartel to the start/end of the path
                if (GoToFinalStart)
                {
                    // loop through from finalstartel to the start of the path
                    for (FinalIndex = FinalStartEl; FinalIndex > -1; FinalIndex--)
                        retPath.PathList.Add(FinalPath.PathList[FinalIndex]);
                }
                else
                {
                    i = FinalPath.PathList.Count;
                    // loop through from finalstartel to the end of the path
                    for (FinalIndex = FinalStartEl; FinalIndex < i; FinalIndex++)
                        retPath.PathList.Add(FinalPath.PathList[FinalIndex]);
                }

                // set flying,mounting
                retPath.CanFly = CurrentPath.CanFly;
                retPath.CanMount = CurrentPath.CanMount;
                retPath.CanSwim = CurrentPath.CanSwim;
            }

            catch (Exception excep)
            {
                retPath = null;
                clsError.ShowError(excep, "Could not create new path");
            }

            // log it
            clsSettings.Logging.AddToLog("New path created successfully");

            // return the list
            return retPath;
        }

        // Merge Paths
        #endregion

        #region Flying

        // Flying code converted from Tenshi's flying/swimming code
        // http://www.isxwow.net/forums/viewtopic.php?f=23&t=542&hilit=

        private const double CPRF_180 = 36.5;

        /// <summary>
        /// Moves to the specifed point, keeping your character facing direction of travel
        /// returns movement result. If Aggroed returned, the unit is stopped.
        /// NOTE: You must already be mounted if you are flying
        /// </summary>
        /// <param name="target">the point to move to</param>
        /// <param name="DoFly">true for this toon to fly, false for the toon to swim</param>
        public EMovementResult MoveToPoint_Fly(PathListInfo.PathPoint target, bool DoFly)
        {
            try
            {
                // Change our pitch so we point at the correct Z
                ChangePitch(PitchToPoint(target));

                // jump if not swimming
                if (DoFly)
                    DoJump();

                // use moveto point to fly there
                return MoveToPoint(target);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "MoveToPoint_Fly");
                return EMovementResult.Error;
            }
        }

        /// <summary>
        /// Executes one jump
        /// </summary>
        public static void DoJump()
        {
            using (new clsFrameLock.LockBuffer())
                LavishScript.ExecuteCommand("wowpress jump");
        }

        /// <summary>
        /// Face the correct Z point
        /// </summary>
        /// <param name="Pitch">the pitch to face</param>
        internal static void ChangePitch(double Pitch)
        {
            /*
               variable float PitchRate = ${Math.Calc[${Degrees}/(180/CPRF_180)]}
               ISXWoW:ManualTurn[0,-CPRF_180]
               waitframe
               ISXWoW:ManualTurn[0,${PitchRate}]
               waitframe
               ISXWoW:ManualTurn[0,0]
             */

            double PitchRate = Pitch / (180 / CPRF_180);
            using (new clsFrameLock.LockBuffer())
                clsSettings.isxwow.ManualTurn(0, -1 * CPRF_180);
            Frame.Wait(false);
            using (new clsFrameLock.LockBuffer())
                clsSettings.isxwow.ManualTurn(0, PitchRate);
            Frame.Wait(false);
            using (new clsFrameLock.LockBuffer())
                clsSettings.isxwow.ManualTurn(0, 0);
        }

        /// <summary>
        /// Gets the pitch we need for the specified target
        /// </summary>
        internal static double PitchToPoint(PathListInfo.PathPoint target)
        {
            /*
            variable float TriangleLength = ${Math.Distance[${Me.X}, ${Me.Y}, ${x}, ${y}]}
   variable float TriangleHeight = ${Math.Calc[${Me.Z} - ${z}]}
   variable float TriangleHypotenuse = ${Math.Distance[${Me.X}, ${Me.Y}, ${Me.Z}, ${x}, ${y}, ${z}]}

   if ${TriangleHeight} < 0
      return ${Math.Calc[${Math.Asin[${Math.Abs[${TriangleHeight}]}/${TriangleHypotenuse}]} + 90]}
   else
      return ${Math.Asin[${TriangleLength}/${TriangleHypotenuse}]}
             */

            double TriangleLength = clsCharacter.MyLocation.Distance(target);
            double TriangleHeight = clsCharacter.MyLocation.Z - target.Z;
            double TriangleHypotenuse = clsCharacter.MyLocation.DistanceZ(target);

            if (TriangleHeight < 0)
                return Math.Asin(Math.Abs(TriangleHeight / TriangleHypotenuse)) + 90;
            else
                return Math.Asin(TriangleLength / TriangleHypotenuse);
        }

        // Flying
        #endregion

        #region MoveToTarget

        /// <summary>
        /// Moves to target.
        /// </summary>
        /// <param name="Distance">The distance to be from target before stopping.</param>
        /// <param name="TargetUnit"></param>
        public static EMovementResult MoveToTarget(WoWObject TargetUnit, int Distance)
        {
            EMovementResult rVal;
            clsPath cPath = new clsPath();

            try
            {
                // reset stuck
                clsStuck.ResetStuck();

                // move closer to the unit, if too far away
                while (DistanceToTarget(TargetUnit) > Distance)
                {
                    // check if script stopped
                    if (!clsSettings.TestPauseStop("MoveToTarget exited because of script stop"))
                        return EMovementResult.Stopped;

                    // get the target location
                    PathListInfo.PathPoint unitLoc = GetUnitLocation(TargetUnit);

                    // face the mob
                    if (!clsFace.FacePointEx(unitLoc))
                        StartMoving();

                    // check for path obstruction
                    if (cPath.IPO(unitLoc))
                    {
                        // try to move somewhere else and do it again
                        clsStuck.MoveAround(unitLoc);

                        // if still stuck, then exit
                        if (cPath.IPO(unitLoc))
                            return EMovementResult.Stuck;
                    }

                    // check if we are stuck
                    if (clsStuck.CheckStuckEx(unitLoc, true))
                        return EMovementResult.Stuck;

                    Thread.Sleep(100);
                }

                rVal = EMovementResult.Success;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "MoveToTarget");
                rVal = EMovementResult.Error;
            }

            finally
            {
                // stop moving
                StopMoving();
            }

            return rVal;
        }

        public static void StartMoving()
        {
            if (!Moving)
                Move(MovementDirection.Forward);
        }

        // MoveToTarget
        #endregion

        #region Obstacle Checks and Fixes

        #region Test Slope

        // heading is 360 degrees
        // heading 0 = north
        // heading 270 = west

        // X,Y:
        //      X increments North, decrements South
        //      Y increments West, decrements East

        /// <summary>
        /// Determines whether the slope at the point is passable. Returns true
        /// if passable, false if not passable.
        /// </summary>
        /// <param name="StartPoint">The current point.</param>
        /// <remarks>Credit: EQJoe - MageBot - CheckSlope() in MoveTo.iss</remarks>
        public static bool IsSlopePassable(PathListInfo.PathPoint StartPoint)
        {
            double heading = clsCharacter.MyHeading;

            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // test 5 units from us
                    for (int i = 1; i < 6; i++)
                    {
                        // set slope x, y
                        double SlopeX = StartPoint.X + (i * Math.Cos(heading));
                        double SlopeY = StartPoint.Y - (i * Math.Sin(heading));

                        // get collision at Z
                        Point3f p3f = clsSettings.isxwow.CollisionTest(
                            SlopeX,
                            SlopeY,
                            StartPoint.Z + 3,
                            SlopeX,
                            SlopeY,
                            StartPoint.Z - 3);

                        // return true if no obstacle, or no obstacle on our z plane
                        if ((p3f == null) || (Math.Abs(p3f.Z) > i))
                            return true;
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "IsSlopePassable");
            }

            return false;
        }

        /// <summary>
        /// Determines whether the slope at the point is passable. Returns true
        /// if passable, false if not passable.
        /// </summary>
        /// <param name="StartPoint">The current point.</param>
        /// <remarks>Credit: EQJoe - MageBot - CheckSlope() in MoveTo.iss</remarks>
        /// <param name="EndPoint"></param>
        public static bool IsSlopePassable(PathListInfo.PathPoint StartPoint, PathListInfo.PathPoint EndPoint)
        {
            double heading = GetHeading(StartPoint, EndPoint);

            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // test 5 units from us
                    for (int i = 1; i < 6; i++)
                    {
                        // set slope x, y
                        double SlopeX = StartPoint.X + (i * Math.Cos(heading));
                        double SlopeY = StartPoint.Y - (i * Math.Sin(heading));

                        // get collision at Z
                        Point3f p3f = clsSettings.isxwow.CollisionTest(
                            SlopeX,
                            SlopeY,
                            StartPoint.Z + 3,
                            SlopeX,
                            SlopeY,
                            StartPoint.Z - 3);

                        // return true if no obstacle, or no obstacle on our z plane
                        if ((p3f == null) || (Math.Abs(p3f.Z) > i))
                            return true;
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "IsSlopePassable");
            }

            return false;
        }

        //http://www.isxwow.net/forums/viewtopic.php?f=17&t=1370
        /// <summary>
        /// Get the heading between two points
        /// </summary>
        /// <param name="StartPoint"></param>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        private static double GetHeading(PathListInfo.PathPoint StartPoint, PathListInfo.PathPoint EndPoint)
        {
            double heading = Math.Atan(EndPoint.Y - StartPoint.Y) / (EndPoint.X - StartPoint.X);
            if (heading < 0)
                heading += 360;
            return heading;
        }

        // Test Slope
        #endregion

        #region IsPathObstructed

        /// <summary>
        /// Checks for an obstructed path.
        /// </summary>
        /// <param name="CurrentPoint">the current point</param>
        /// <param name="TargetPoint">the point to move to</param>
        /// <returns></returns>
        public static bool IsPathObstructed(PathListInfo.PathPoint CurrentPoint, PathListInfo.PathPoint TargetPoint)
        {
            // test if the path has obstacles
            using (new clsFrameLock.LockBuffer())
                return clsSettings.isxwow.IsPathObstructedOCD(CurrentPoint.X, CurrentPoint.Y, CurrentPoint.Z, TargetPoint.X, TargetPoint.Y, TargetPoint.Z).IsValid;
                //return clsSettings.isxwow.CollisionTest(CurrentPoint.X, CurrentPoint.Y, CurrentPoint.Z, TargetPoint.X, TargetPoint.Y, TargetPoint.Z).X == 0;
        }

        /// <summary>
        /// Checks for an obstructed path from your current point
        /// </summary>
        /// <param name="TargetPoint">the point to move to</param>
        /// <returns></returns>
        public static bool IsPathObstructed(PathListInfo.PathPoint TargetPoint)
        {
            // test if the path has obstacles
            using (new clsFrameLock.LockBuffer())
                return clsSettings.isxwow.Me.IsPathObstructed(TargetPoint.X, TargetPoint.Y, TargetPoint.Z);
        }

        // IsPathObstructed
        #endregion

        #region Find and Fix Obstacles

        /// <summary>
        /// Checks for obstacle. If one is found, the toon is stopped, and a path is 
        /// created around the obstacle. PathInfo is updated with new path information.
        /// Success = no change, Stuck = stuck, Error = error
        /// </summary>
        public static EMovementResult Find_Fix_Obstacle(PathListInfoEx PathInfo, PathListInfo.PathPoint CurrentPoint)
        {
            PathListInfo.PathPoint tempPoint = CurrentPoint;
            int StepNum = PathInfo.CurrentStep;

            try
            {
                // exit if no obstacle
                if (!IsPathObstructed(CurrentPoint) && (IsSlopePassable(CurrentPoint)))
                    return EMovementResult.Success;

                // get previous point, so we can reset path
                PathInfo.GetPreviousPoint();

                // get the destination point, exit if no path (end of path, should NEVER happen {here for safety})
                PathListInfo.PathPoint destPoint = PathInfo.PathList[StepNum];
                if (destPoint == null)
                {
                    PathInfo.CurrentStep = StepNum;
                    return EMovementResult.Success;
                }

                // stop, since we have an obstacle
                StopMoving();
                clsSettings.Logging.AddToLog("Found Obstacle. Mapping path around obstacle");

                // get the next point to add to the path
                while (tempPoint != null)
                {
                    // get the next point
                    PathListInfo.PathPoint lastPoint = tempPoint;
                    tempPoint = GetPointAroundObstacle(tempPoint, destPoint);

                    // skip if the points are the same
                    if ((tempPoint.SamePoint(lastPoint)) || (tempPoint.Distance(lastPoint) < 7))
                        tempPoint = null;

                    // insert to list if not null
                    if (tempPoint != null)
                    {
                        clsSettings.Logging.AddToLogFormatted("Path", "Inserting point in path '{0}' to avoid obstacle. Point: {1}", PathInfo.PathName, tempPoint.ToString());
                        PathInfo.PathList.Insert(StepNum++, tempPoint);
                    }
                }

                return EMovementResult.Success;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Find and Fix Obstacles in Path");
                return EMovementResult.Error;
            }
        }

        /// <summary>
        /// Gets the point around the obstacle.
        /// </summary>
        /// <param name="CurrentPoint">The current point.</param>
        /// <param name="destPoint">The destination point.</param>
        /// <returns></returns>
        private static PathListInfo.PathPoint GetPointAroundObstacle(PathListInfo.PathPoint CurrentPoint, PathListInfo.PathPoint destPoint)
        {
            PathListInfo.PathPoint nearestPoint = CurrentPoint, testPoint;
            int degrees = 0, radius = 0;

            try
            {
                // loop through 360, from 0 to 50 radius to find the closest clear point
                using (new clsFrameLock.LockBuffer())
                {
                    do
                    {
                        do
                        {
                            // get the point to test
                            testPoint = new PathListInfo.PathPoint(
                                CurrentPoint.X + (radius * Math.Cos(degrees)),
                                CurrentPoint.Y + (radius * Math.Cos(degrees)),
                                CurrentPoint.Z);

                            // skip if path obsctructed
                            if (IsPathObstructed(CurrentPoint, testPoint))
                            {
                                clsSettings.Logging.AddToLogFormatted("GetPointAroundObstacle", "Path Obstructed at {0}", testPoint.ToString());
                                continue;
                            }

                            // point is clear, see if it is closer then nearestPoint
                            if ((!testPoint.SamePoint(CurrentPoint)) &&
                                (destPoint.Distance(testPoint) < destPoint.Distance(nearestPoint)) &&
                                (!testPoint.SamePoint(CurrentPoint)))
                                nearestPoint = testPoint;

                        } while ((degrees += 20) < 361);
                    } while ((radius += 10) < 51);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Get Point Around Obstacle");
            }

            return nearestPoint;
        }

        // Find and Fix Obstacles
        #endregion

        // Obstacle Checks and Fixes
        #endregion

        #region MoveThroughPathEx

        #region Variables

        private static object LockUnitToAttack = new object();
        private static WoWUnit m_UnitToAttack = null;
        /// <summary>
        /// Unit to attack
        /// </summary>
        public static WoWUnit UnitToAttack
        {
            get
            {
                lock (LockUnitToAttack)
                    return m_UnitToAttack;
            }
            set
            {
                lock (LockUnitToAttack)
                    m_UnitToAttack = value;
            }
        }

        private static int m_MTPInCombat = 0; // 0 = false, 1= true
        /// <summary>
        /// True when combat thread is entering combat
        /// </summary>
        public static bool MTPInCombat
        {
            get { return Thread.VolatileRead(ref m_MTPInCombat) == 1; }
            set { Thread.VolatileWrite(ref m_MTPInCombat, value ? 1 : 0); }
        }

        private static WoWUnit UnitToLoot = null; // found in Find Objects
        private DateTime LastFace = DateTime.MinValue;
        private DateTime LastStandFace = DateTime.MinValue;

        // Variables
        #endregion

        #region Properties

        private bool UnitIsValid
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return ((UnitToAttack != null) && (UnitToAttack.IsValid));
            }
        }

        internal static bool LootUnitIsValid
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return ((UnitToLoot != null) && (UnitToLoot.IsValid));
            }
        }

        // Properties
        #endregion

        #region Threads

        #region Find Objects

        internal class MTPThread_FindGameObjects : ThreadBase
        {
            /// <summary>
            /// list of objects to search for
            /// </summary>
            public List<string> ObjNameList = null;

            /// <summary>
            /// Searches for chests, herbs, and mines
            /// </summary>
            public void MTP_Thread_FindGameObjects()
            {
                int totalCount = 3;
                WoWUnit tempUnit;

                // change total count if we have objects in the list
                if ((ObjNameList != null) && (ObjNameList.Count > 0))
                    totalCount = ObjNameList.Count + 3;

                while (!Shutdown)
                {
                    try
                    {
                        // sleep
                        Thread.Sleep(2000);

                        // test for pause
                        if ((Shutdown) || (!clsCharacter.CharacterIsValid) || (!clsSettings.TestPauseStop("MoveThroughPathEx", "Find Game Objects exiting due to script stop")))
                            return;

                        // skip if we still have a found object, and it is within range
                        using (new clsFrameLock.LockBuffer())
                        {
                            // skip if dead or aggro'd
                            if ((clsCharacter.IsDead) || (clsCombat.IsInCombat()))
                                continue;

                            // skip if we already have something that is nearby
                            if ((LootUnitIsValid) && (DistanceToTarget(UnitToLoot) < clsSettings.gclsLevelSettings.SearchRange))
                                continue;

                            // do our search
                            int SearchCounter = 0;
                            while (SearchCounter < totalCount)
                            {
                                // find an object
                                tempUnit = null;
                                switch (SearchCounter)
                                {
                                    case 0: // flowers
                                        if (clsSettings.gclsLevelSettings.IsFlowerPicker)
                                        {
                                            // search for flowers
                                            if (clsSettings.VerboseLogging)
                                                clsSettings.Logging.AddToLog("MoveThroughPathEx", "Finding Flowers to Pick");
                                            tempUnit = clsSearch.FindGroundObject(clsSearch.EGroundObjectType.Herb);
                                        }
                                        break;

                                    case 1: // mines
                                        if (clsSettings.gclsLevelSettings.IsMiner)
                                        {
                                            // search for mines
                                            if (clsSettings.VerboseLogging)
                                                clsSettings.Logging.AddToLog("MoveThroughPathEx", "Finding Veins to Mine");
                                            tempUnit = clsSearch.FindGroundObject(clsSearch.EGroundObjectType.Mine);
                                        }
                                        break;

                                    case 2: // chest
                                        if (clsSettings.gclsLevelSettings.Search_Chest)
                                        {
                                            // search for chests
                                            if (clsSettings.VerboseLogging)
                                                clsSettings.Logging.AddToLog("MoveThroughPathEx", "Finding Chests to Open");
                                            tempUnit = clsSearch.FindGroundObject(clsSearch.EGroundObjectType.Chest);
                                        }
                                        break;

                                    default: // object name search
                                        // get the item's name
                                        string itemName = ObjNameList[SearchCounter - 3];

                                        // search
                                        //if (clsSettings.VerboseLogging)
                                        clsSettings.Logging.AddToLogFormatted("MoveThroughPathEx", "Finding Object '{0}'", itemName);
                                        tempUnit = clsSearch.FindGroundObject(clsSearch.EGroundObjectType.Other, itemName);
                                        break;
                                }

                                // loot if we have an object
                                if ((tempUnit != null) && (tempUnit.IsValid))
                                {
                                    // loot object. restart loop if aggroed
                                    clsSettings.Logging.AddToLogFormatted("MoveThroughPathEx", "Found Ojbect '{0}' at {1}", tempUnit.Name, GetUnitLocation(tempUnit).ToString());
                                    UnitToLoot = tempUnit;
                                    break;
                                }
                                else
                                    // increment searchcounter if we have no more units
                                    SearchCounter++;
                            }
                        }
                    }

                    catch (Exception excep)
                    {
                        clsError.ShowError(excep, "MoveThroughPathEx", "Find Game Objects");
                    }
                }
            }
        }

        // Find Objects
        #endregion

        #region Combat / Aggro

        internal class MTPThread_Combat : ThreadBase
        {
            /// <summary>
            /// Searches for units to attack and attacking us
            /// </summary>
            public void MTP_Thread_Combat()
            {
                WoWUnit tempUnit;

                while (!Shutdown)
                {
                    try
                    {
                        // test for pause
                        if ((!clsCharacter.CharacterIsValid) || (Shutdown) || (!clsSettings.TestPauseStop("MoveThroughPathEx", "Combat/Aggro exiting due to script stop")))
                            return;

                        // sleep while we are already in combat
                        while (MTPInCombat)
                        {
                            // check for pause/stop
                            if ((Shutdown) || (!clsSettings.TestPauseStop("MoveThroughPathEx", "Combat/Aggro exiting due to script stop")))
                                return;

                            // sleep for 1 second
                            Thread.Sleep(1000);
                        }

                        // get if we need a new unit
                        bool NeedNewUnit;
                        using (new clsFrameLock.LockBuffer())
                        {
                            NeedNewUnit = ((!clsCharacter.IsDead) &&
                                            ((UnitToAttack == null) ||
                                            (!UnitToAttack.IsValid) ||
                                            (UnitToAttack.Dead) ||
                                            (DistanceToTarget(UnitToAttack) > clsSettings.gclsLevelSettings.SearchRange)));
                        }

                        // get new unit if current one is invalid and we are not in combat
                        if ((NeedNewUnit) && (!clsCombat.IsInCombat()))
                        {
                            tempUnit = null;

                            // log it
                            if (clsSettings.VerboseLogging)
                                clsSettings.Logging.AddToLog("Combat Thread: Checking for aggro/finding target to attack");

                            // see if anything is attacking us
                            if (clsCombat.IsInCombat())
                                tempUnit = clsCombat.GetUnitAttackingMe();

                            // find target to attack if not aggro'd
                            if ((tempUnit == null) || (!tempUnit.IsValid))
                                tempUnit = clsSearch.FindTargetToAttack();

                            // set new target
                            using (new clsFrameLock.LockBuffer())
                            {
                                if ((tempUnit != null) && (tempUnit.IsValid))
                                    UnitToAttack = tempUnit;
                            }
                        }

                        // sleep
                        Thread.Sleep(500);
                    }

                    catch (Exception excep)
                    {
                        clsError.ShowError(excep, "MoveThroughPathEx", "Combat/Aggro");
                    }
                }
            }
        }

        /// <summary>
        /// Handles combat and aggro
        /// </summary>
        private EMovementResult MTP_Combat(PathListInfoEx CurrentPath, List<PathListInfoEx> GraveyardPaths)
        {
            WoWUnit tempUnit;
            clsGhost ghost = new clsGhost();
            clsCombat combat = new clsCombat();
            List<WoWUnit> searchList;

            try
            {
                // exit if dead
                if (clsCharacter.IsDead)
                    return EMovementResult.Dead;

                while (true)
                {
                    try
                    {
                        // test for pause
                        if ((Shutdown) || (!clsSettings.TestPauseStop("MoveThroughPathEx", "Combat/Aggro exiting due to script stop")))
                            return EMovementResult.Stopped;

                        if (clsSettings.VerboseLogging)
                            clsSettings.Logging.AddToLog("MoveThroughPathEx", "Checking for aggro/finding target to attack");

                        using (new clsFrameLock.LockBuffer())
                        {
                            // see if anything is attacking us
                            tempUnit = clsCombat.GetUnitAttackingMe();

                            // use found target if not aggro'd
                            if ((tempUnit == null) || (!tempUnit.IsValid) || (tempUnit.Dead))
                                tempUnit = UnitToAttack;

                            // find new target if nothing exists
                            if ((tempUnit == null) || (!tempUnit.IsValid) || (tempUnit.Dead))
                                tempUnit = clsSearch.FindTargetToAttack();

                            // exit if nothing
                            if ((tempUnit == null) || (!tempUnit.IsValid) || (tempUnit.Dead))
                                return EMovementResult.Success;
                        }

                        // found unit, attack it
                        using (new clsFrameLock.LockBuffer())
                        {
                            clsSettings.Logging.AddToLog(string.Format("MoveThroughPathEx", "Found unit to attack: {0}, Location: {1}", tempUnit.Name, new PathListInfo.PathPoint(tempUnit.Location)));
                            StopMoving();
                        }

                        // do combat
                        MTPInCombat = true;
                        ISXRhabotGlobal.clsGlobals.AttackOutcome attackOutcome = combat.BeginCombat(tempUnit);
                        switch (attackOutcome)
                        {
                            case ISXRhabotGlobal.clsGlobals.AttackOutcome.Dead:
                                clsSettings.Logging.AddToLog("MoveThroughPathEx", "Died in Combat. Attempting to rez at corpse");
                                break;
                            case ISXRhabotGlobal.clsGlobals.AttackOutcome.Panic:
                                clsSettings.Logging.AddToLog("MoveThroughPathEx", "Panic Run!");
                                clsCombat_Helper.PanicRun(true);
                                break;
                            case ISXRhabotGlobal.clsGlobals.AttackOutcome.Stopped:
                                clsSettings.Logging.AddToLog("MoveThroughPathEx", "Combat/Aggro exiting due to script stop");
                                return EMovementResult.Stopped;
                        }

                        // handle death
                        if ((clsCharacter.IsDead) && (!ghost.HandleDead(CurrentPath, GraveyardPaths)))
                            return EMovementResult.Dead;

                        // if nearby mobs, then do downtime first, else loot first
                        searchList = clsSearch.Search_Unit(clsSearch.BuildTargetString(20, clsCharacter.MobLowLevel, clsCharacter.MobHighLevel, clsSettings.SearchForHostiles));

                        // found mobs, downtime first
                        if ((searchList != null) && (searchList.Count > 0))
                        {
                            clsSettings.Logging.AddToLog("MTP Combat: Found nearby mobs. Trying Downtime before looting, for safety");

                            // downtime
                            if ((combat.NeedDowntime()) && (combat.DoDowntime()))
                                continue; // aggroed

                            // time to loot
                            if (combat.DoLoot())
                                continue; // aggroed
                        }

                        else
                        {
                            clsSettings.Logging.AddToLog("MTP Combat: No nearby mobs. Trying to loot before downtime");

                            // time to loot
                            if (combat.DoLoot())
                                continue; // aggroed

                            // downtime
                            if ((combat.NeedDowntime()) && (combat.DoDowntime()))
                                continue; // aggroed
                        }
                    }

                    catch (Exception excep)
                    {
                        clsError.ShowError(excep, "MoveThroughPathEx", "Combat/Aggro");
                        return EMovementResult.Error;
                    }

                    finally
                    {
                        // reset combat flag
                        MTPInCombat = false;
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "MoveThroughpathEx Combat/Aggro. Combat/Aggro Exiting Due to Error");
                return EMovementResult.Error;
            }

            finally
            {
                // reset combat flag
                MTPInCombat = false;
            }
        }

        // Combat / Aggro
        #endregion

        // Threads
        #endregion

        #region MoveThroughPath

        /// <summary>
        /// Moves the unit through the path. Does not search for herbs, mines, boxes, and targets.
        /// NOTE: CurrentStep and Path direction must be set
        /// </summary>
        /// <param name="PathInfo">the path list to search</param>
        /// <param name="DoingDeadRun">true if we are doing the dead run</param>
        public EMovementResult MoveThroughPathEx(PathListInfoEx PathInfo, bool DoingDeadRun)
        {
            return MoveThroughPathEx(PathInfo, false, false, null, null, null, DoingDeadRun);
        }

        /// <summary>
        /// Moves the unit through the path. Does not search for herbs, mines, boxes, and targets.
        /// NOTE: CurrentStep and Path direction must be set
        /// </summary>
        /// <param name="PathInfo">the path list to search</param>
        public EMovementResult MoveThroughPathEx(PathListInfoEx PathInfo)
        {
            return MoveThroughPathEx(PathInfo, false, false, null, null, null, false);
        }

        /// <summary>
        /// Moves the unit through the path, searching for herbs, mines, boxes, and targets.
        /// NOTE: CurrentStep and Path direction must be set
        /// </summary>
        /// <param name="PathInfo">the path list to search</param>
        /// <param name="GraveyardPaths"></param>
        public EMovementResult MoveThroughPathEx(PathListInfoEx PathInfo, List<PathListInfoEx> GraveyardPaths)
        {
            return MoveThroughPathEx(PathInfo, false, false, null, null, GraveyardPaths);
        }

        /// <summary>
        /// Moves the unit through the path, searching for herbs, mines, boxes, and targets.
        /// NOTE: CurrentStep and Path direction must be set
        /// </summary>
        /// <param name="PathInfo">the path list to search</param>
        /// <param name="SearchForObjects">true to search for objects, false to skip search. When false, ONLY handles aggro</param>
        /// <param name="CheckCharBags">true to check bags and durability</param>
        public EMovementResult MoveThroughPathEx(PathListInfoEx PathInfo, bool SearchForObjects, bool CheckCharBags)
        {
            return MoveThroughPathEx(PathInfo, SearchForObjects, CheckCharBags, null, null, null);
        }

        /// <summary>
        /// Moves the unit through the path, searching for herbs, mines, boxes, and targets.
        /// NOTE: CurrentStep and Path direction must be set
        /// </summary>
        /// <param name="PathInfo">the path list to search</param>
        /// <param name="SearchForObjects">true to search for objects, false to skip search. When false, ONLY handles aggro</param>
        /// <param name="CheckCharBags">true to check bags and durability</param>
        /// <param name="NeedVendorItemList">list of items that, when quantity reaches a set amount, will cause us to need to return to a vendor</param>
        /// <param name="ObjectsToFind">list of objects to search for</param>
        /// <param name="GraveyardPaths">list of graveyard paths to run on death. pass null for no predefined paths</param>
        public EMovementResult MoveThroughPathEx(PathListInfoEx PathInfo, bool SearchForObjects, bool CheckCharBags, List<clsVendor.clsNeedVendorItem> NeedVendorItemList, List<string> ObjectsToFind, List<PathListInfoEx> GraveyardPaths)
        {
            return MoveThroughPathEx(PathInfo, SearchForObjects, CheckCharBags, NeedVendorItemList, ObjectsToFind, GraveyardPaths, false);
        }

        /// <summary>
        /// Moves the unit through the path, searching for herbs, mines, boxes, and targets.
        /// NOTE: CurrentStep and Path direction must be set
        /// </summary>
        /// <param name="PathInfo">the path list to search</param>
        /// <param name="SearchForObjects">true to search for objects, false to skip search. When false, ONLY handles aggro</param>
        /// <param name="CheckCharBags">true to check bags and durability</param>
        /// <param name="NeedVendorItemList">list of items that, when quantity reaches a set amount, will cause us to need to return to a vendor</param>
        /// <param name="ObjectsToFind">list of objects to search for</param>
        /// <param name="GraveyardPaths">list of graveyard paths to run on death. pass null for no predefined paths</param>
        /// <param name="DoingDeadRun">when true, we are running to our corpse.</param>
        public EMovementResult MoveThroughPathEx(PathListInfoEx PathInfo, bool SearchForObjects, bool CheckCharBags, List<clsVendor.clsNeedVendorItem> NeedVendorItemList, List<string> ObjectsToFind, List<PathListInfoEx> GraveyardPaths, bool DoingDeadRun)
        {
            // the point we are going towards
            clsGhost ghost = new clsGhost();
            EMovementResult result;
            DateTime LastVendorCheck = DateTime.Now, LastMountCheck = DateTime.MinValue;
            DateTime LastSearch = DateTime.MinValue, LastIPOCheck = DateTime.MinValue;

            try
            {
                // exit if invalid
                if (!clsSettings.GuidValid)
                    return EMovementResult.Error;

                if (PathInfo == null)
                {
                    clsError.ShowError(new Exception("No path found"), "MoveThroughPathEx", true, new StackFrame(0, true), false);
                    return EMovementResult.Error;
                }

                // get the next point to travel to
                PathListInfo.PathPoint NextPoint = PathInfo.GetNextPoint();
                if (NextPoint == null)
                {
                    clsSettings.Logging.AddToLog("MoveThroughPathEx", "Exiting. We are at the end of the path");
                    return EMovementResult.Success;
                }
                PathListInfo.PathPoint LastPoint = NextPoint;
                LastStandFace = DateTime.MinValue;
                LastFace = DateTime.MinValue;
                FaceNextPoint(NextPoint);

                #region Threads

                // start the herb thread
                if (SearchForObjects)
                {
                    MTPThread_FindGameObjects fgo = new MTPThread_FindGameObjects();
                    fgo.ObjNameList = ObjectsToFind;
                    Thread threadHerb = new Thread(fgo.MTP_Thread_FindGameObjects);
                    threadHerb.Name = "MoveThroughPathEx - Find Game Objects";
                    ThreadItemHerb = new clsSettings.ThreadItem(threadHerb, fgo);
                    clsSettings.ThreadList.Add(ThreadItemHerb);
                    threadHerb.Start();

                    // start the combat thread
                    MTPThread_Combat mtpc = new MTPThread_Combat();
                    Thread threadCombat = new Thread(mtpc.MTP_Thread_Combat);
                    threadCombat.Name = "MoveThroughPathEx - Find Units to Attack";
                    ThreadItemHunt = new clsSettings.ThreadItem(threadCombat, mtpc);
                    clsSettings.ThreadList.Add(ThreadItemHunt);
                    threadCombat.Start();
                }

                // Threads
                #endregion

                // loop through path
                while (true)
                {
                    // test for script pause/stop
                    if ((!clsCharacter.CharacterIsValid) || (!clsSettings.TestPauseStop("MoveThroughPathEx", "Exiting due to script stop")))
                        return EMovementResult.Stopped;

                    FaceNextPoint(NextPoint);

                    #region Check if too far from point

                    // return to the next point if we are too far away
                    if ((!clsCharacter.IsDead) && (CheckIfTooFarFromPoint(NextPoint)))
                        MoveToPoint(NextPoint);

                    // Check if too far from point
                    #endregion

                    if (LastSearch.AddMilliseconds(1000) <= DateTime.Now)
                    {
                        #region Check for Combat

                        // check for combat
                        if ((clsCombat.IsInCombat()) || ((SearchForObjects) && (UnitIsValid)))
                        {
                            result = MTP_Combat(PathInfo, GraveyardPaths);
                            switch (result)
                            {
                                case EMovementResult.Stopped:
                                case EMovementResult.Stuck:
                                    clsSettings.Logging.AddToLogFormatted("MoveThroughPathEx", "Exiting because of {0}", result.ToString());
                                    return result;
                            }

                            // reset unit to attack
                            UnitToAttack = null;
                        }

                        // if we are dead, try to get to our body, exit if we can't
                        if ((!DoingDeadRun) && (clsCharacter.IsDead) && (!ghost.HandleDead(PathInfo, GraveyardPaths)))
                            return EMovementResult.Error;

                        // Check for Combat
                        #endregion

                        #region Check for Objects

                        // if we have an herb, loot it
                        if ((SearchForObjects) && (LootUnitIsValid) && (!clsCombat.IsInCombat()))
                        {
                            // search for surrounding hostile mobs
                            WoWUnit ObjectUnit = clsSearch.FindSurroundingHostileTarget(clsPath.GetUnitLocation(UnitToLoot), clsCharacter.MobLowLevel, clsCharacter.MobHighLevel);

                            // set the unit to attack, or loot
                            if ((ObjectUnit != null) && (ObjectUnit.IsValid))
                                UnitToAttack = ObjectUnit;
                            else
                            {
                                // loot this item
                                result = clsLoot.LootGameOjbect(UnitToLoot, true);

                                // if we aggro'd something, do combat
                                if (result == EMovementResult.Aggroed)
                                {
                                    result = MTP_Combat(PathInfo, GraveyardPaths);
                                    switch (result)
                                    {
                                        case EMovementResult.Stopped:
                                        case EMovementResult.Stuck:
                                            clsSettings.Logging.AddToLogFormatted("MoveThroughPathEx", "Exiting because of {0}", result.ToString());
                                            return result;
                                    }
                                }

                                // remove loot unit
                                UnitToLoot = null;

                                // if we are dead, try to get to our body, exit if we can't
                                if ((!DoingDeadRun) && (clsCharacter.IsDead) && (!ghost.HandleDead(PathInfo, GraveyardPaths)))
                                    return EMovementResult.Error;
                            }
                        }

                        // Check for Objects
                        #endregion

                        LastSearch = DateTime.Now;
                    }

                    #region Mount Up

                    if (LastMountCheck.AddMilliseconds(5000) <= DateTime.Now)
                    {
                        LastMountCheck = DateTime.Now;

                        // mount if need. exit if error
                        if (((PathInfo.CanFly) || (PathInfo.CanMount)) && (!MountUp(PathInfo.CanFly)))
                        {
                            clsSettings.Logging.AddToLog("MoveThroughPathEx", "Exiting because path is flyable but flying mount could not be mounted");
                            return EMovementResult.Error;
                        }
                        else if ((!PathInfo.CanFly) && (!PathInfo.CanMount))
                            clsMount.Dismount();
                    }

                    // Mount Up
                    #endregion

                    if (LastIPOCheck.AddMilliseconds(2000) <= DateTime.Now)
                    {
                        #region Check if stuck

                        if (clsStuck.CheckStuckEx(NextPoint, true))
                        {
                            // log it
                            StopMoving();
                            clsSettings.Logging.AddToLog("MoveThroughPathEx", "Stuck. Exiting");
                            return EMovementResult.Stuck;
                        }

                        // Check if stuck
                        #endregion

                        #region Check for Obstacles

                        /*
                        // IPO check (by EQJoe)
                        if (IPO(NextPoint))
                        {
                            // try to move somewhere else
                            clsStuck.MoveAround(NextPoint);

                            // if still stuck, then exit
                            if (IPO(NextPoint))
                            {
                                if (SecondChanceIPO)
                                {
                                    // can't find route to next point, consider us stuck
                                    StopMoving();

                                    // log it and exit
                                    clsSettings.Logging.AddToLog("MoveThroughPathEx", "Obstacle found. No path could be found around obstacle. Stopping");
                                    return EMovementResult.Stuck;
                                }
                                else
                                {
                                    SecondChanceIPO = true;

                                    // get next point
                                    LastStandFace = DateTime.MinValue;
                                    LastFace = DateTime.MinValue;

                                    // get the next point
                                    MTP_GetNextPoint(ref PathInfo, out NextPoint, ref LastPoint);

                                    // if null, we finished
                                    if (NextPoint == null)
                                        return EMovementResult.Success;

                                    // face the point
                                    FaceNextPoint(NextPoint);
                                }
                            }
                        }
                        else
                            SecondChanceIPO = false;
                        */

                        // Check for Obstacles
                        #endregion

                        LastIPOCheck = DateTime.Now;
                    }

                    if ((DoingDeadRun) || (!clsCharacter.IsDead))
                    {
                        #region Swimming

                        // if we are holding our breath, jump jump jump
                        if (clsCharacter.IsHoldingBreath)
                        {
                            DoJump();
                            Frame.Wait(false);
                            DoJump();
                            Frame.Wait(false);
                            DoJump();
                        }

                        // Swimming
                        #endregion

                        #region Next Point

                        // get next point if we are near to our point
                        while (clsCharacter.MyLocation.Distance(NextPoint) <= clsSettings.PathPrecision)
                        {
                            LastStandFace = DateTime.MinValue;
                            LastFace = DateTime.MinValue;

                            // get the next point
                            MTP_GetNextPoint(ref PathInfo, out NextPoint, ref LastPoint);

                            // if null, we finished
                            if (NextPoint == null)
                                return EMovementResult.Success;

                            FaceNextPoint(NextPoint);
                        }

                        // face point, change pitch if flying/swimming, and move if not moving
                        FaceNextPoint(NextPoint);

                        // Next Point
                        #endregion

                        #region Vendor and Durability

                        if (LastVendorCheck.AddMilliseconds(2000) <= DateTime.Now)
                        {
                            // check for vendor/durability
                            using (new clsFrameLock.LockBuffer())
                            {
                                if ((CheckCharBags) && (clsCharacter.NeedsVendor))
                                {
                                    clsSettings.Logging.AddToLog("MoveThroughPathEx", "Exiting. Needs Vendor");
                                    return EMovementResult.NeedVendor;
                                }

                                // check if we are low on any specified item
                                if ((NeedVendorItemList != null) && (NeedVendorItemList.Count > 0))
                                {
                                    foreach (clsVendor.clsNeedVendorItem needItem in NeedVendorItemList)
                                    {
                                        // return need vendor if the item quantity is below what it should be
                                        if (needItem.Quantity <= clsSearch.NumItemsInBag(needItem.ItemName))
                                            return EMovementResult.NeedVendor;
                                    }
                                }
                            }

                            LastVendorCheck = DateTime.Now;
                        }

                        // Vendor and Durability
                        #endregion
                    }

                    // sleep
                    Thread.Sleep(10);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "MoveThroughPathEx");
                return EMovementResult.Error;
            }

            finally
            {
                // stop moving
                StopMoving();

                // kill threads
                clsSettings.KillThread(ThreadItemHerb, "Killing Find Game Object Thread");
                clsSettings.KillThread(ThreadItemHunt, "Killing Find Unit to Attack Thread");
            }
        }


        /// <summary>
        /// Gets the next point in the path
        /// </summary>
        /// <param name="PathInfo"></param>
        /// <param name="NextPoint"></param>
        /// <param name="LastPoint"></param>
        private static void MTP_GetNextPoint(ref PathListInfoEx PathInfo, out PathListInfo.PathPoint NextPoint, ref PathListInfo.PathPoint LastPoint)
        {
            clsSettings.Logging.AddToLog("MoveThroughPathEx", "We are near to the destination point. Getting next point in list");

            // get the point
            NextPoint = PathInfo.GetNextPoint();

            // exit if we finished the path
            if ((NextPoint == null) || (NextPoint.SamePoint(LastPoint)))
            {
                clsSettings.Logging.AddToLog("MoveThroughPathEx", "Path completed successfully. Exiting.");
                NextPoint = null;
                return;
            }

            // log next point
            clsSettings.Logging.AddToLogFormatted("MoveThroughPathEx", "GetNextPoint - Moving From {0} to {1}.\r\nDistance {2}",
                clsCharacter.MyLocation.ToString(), NextPoint.ToString(), clsCharacter.MyLocation.Distance(NextPoint));

            // reset lastpoint
            LastPoint = NextPoint;
        }

        // MoveThroughPath
        #endregion

        #region Mount Up

        /// <summary>
        /// Mounts up. Returns false on error
        /// </summary>
        /// <param name="PathIsFly">true if the path can be flown</param>
        /// <returns></returns>
        private static bool MountUp(bool PathIsFly)
        {
            // exit if already flying or dead
            using (new clsFrameLock.LockBuffer())
            {
                if ((clsCharacter.IsDead) || ((PathIsFly) && (clsCharacter.IsFlying)))
                    return true;
            }

            // mount up
            if ((PathIsFly) && (!clsMount.IsFlyMounted))
            {
                // flying mount up
                if (!clsMount.MountFlying())
                    return false;

                // jump to start flying
                using (new clsFrameLock.LockBuffer())
                    LavishScript.ExecuteCommand("wowpress jump");
            }
            else if ((!PathIsFly) && (!clsMount.IsMounted))
                clsMount.MountUp();

            return true;
        }

        // Mount Up
        #endregion

        #region RunPath

        /// <summary>
        /// Runs the path
        /// </summary>
        /// <param name="CurrentPath">the path to run</param>
        public static EMovementResult RunPath(PathListInfoEx CurrentPath)
        {
            return RunPath(CurrentPath, false, false, null, null, null);
        }

        /// <summary>
        /// Runs the path
        /// </summary>
        /// <param name="CurrentPath">the path to run</param>
        /// <param name="GraveyardPaths">list of graveyard paths to run on death</param>
        public static EMovementResult RunPath(PathListInfoEx CurrentPath, List<PathListInfoEx> GraveyardPaths)
        {
            return RunPath(CurrentPath, false, false, null, null, GraveyardPaths);
        }

        /// <summary>
        /// Runs the path
        /// </summary>
        /// <param name="CurrentPath">the path to run</param>
        /// <param name="SearchForUnits">true to search for units to attack, herbs/mines/etc</param>
        /// <param name="CheckCharBags">true to check for full bags and durability</param>
        public static EMovementResult RunPath(PathListInfoEx CurrentPath, bool SearchForUnits, bool CheckCharBags)
        {
            return RunPath(CurrentPath, SearchForUnits, CheckCharBags, null, null, null);
        }

        /// <summary>
        /// Runs the path
        /// </summary>
        /// <param name="CurrentPath">the path to run</param>
        /// <param name="SearchForUnits">true to search for units to attack, herbs/mines/etc</param>
        /// <param name="CheckCharBags">true to check for full bags and durability</param>
        /// <param name="NeedVendorItemList">list of items to maintain in the bag</param>
        /// <param name="ObjectsToFind">list of objects to search for while running</param>
        /// <param name="GraveyardPaths">list of graveyard paths to run on death</param>
        public static EMovementResult RunPath(PathListInfoEx CurrentPath, bool SearchForUnits, bool CheckCharBags, List<clsVendor.clsNeedVendorItem> NeedVendorItemList, List<string> ObjectsToFind, List<PathListInfoEx> GraveyardPaths)
        {
            clsPath cPath = new clsPath();
            clsGhost ghost = new clsGhost();

            try
            {
                // move through the path
                EMovementResult mResult = cPath.MoveThroughPathEx(CurrentPath, SearchForUnits, CheckCharBags, NeedVendorItemList, ObjectsToFind, GraveyardPaths);

                // keep looping until we have success or failure
                while (mResult != EMovementResult.Success)
                {
                    // if shutting down
                    if (!clsSettings.TestPauseStop("RunPath exiting due to script stop"))
                        return EMovementResult.Stopped;

                    // exit for certain conditions
                    switch (mResult)
                    {
                        case EMovementResult.Stuck:
                            // logout if set
                            if (clsSettings.gclsGlobalSettings.LogoutOnStuck)
                            {
                                clsSettings.Logging.AddToLog("RunPath: Stoning Home and Logging Out due to Stuck");
                                clsSettings.Logout(true);
                            }
                            return EMovementResult.Stopped;

                        case EMovementResult.Error:
                        case EMovementResult.Stopped:
                        case EMovementResult.PathObstructed:
                        case EMovementResult.NeedVendor:
                            return mResult;
                    }

                    // handle dead
                    if ((clsCharacter.IsDead) && (!ghost.HandleDead(CurrentPath, GraveyardPaths)))
                        return EMovementResult.Dead;

                    // continue moving through the path
                    clsSettings.Logging.AddToLog("RunPath: Continue moving through the path");
                    mResult = cPath.MoveThroughPathEx(CurrentPath, SearchForUnits, CheckCharBags, null, null, GraveyardPaths);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.RunPath);
                return EMovementResult.Error;
            }

            finally
            {
                clsSettings.Logging.AddToLog("RunPath: Exiting");
            }

            return EMovementResult.Success;
        }

        // RunPath
        #endregion

        /// <summary>
        /// Checks if we are too far (searchrange) from point
        /// </summary>
        /// <param name="target">target point</param>
        public static bool CheckIfTooFarFromPoint(PathListInfo.PathPoint target)
        {
            // return if we are too far from point
            // make sure that if we are dead, we are also a ghost
            using (new clsFrameLock.LockBuffer())
                return ((((clsCharacter.IsDead) && (clsCharacter.IsGhost)) || (!clsCharacter.IsDead)) &&
                    ((!clsCombat.IsInCombat()) && (clsCharacter.MyLocation.Distance(target) > clsSettings.gclsLevelSettings.SearchRange)));
        }

        // MoveThroughPathEx
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // exit if we already disposed
            if (IsDisposed)
                return;

            IsDisposed = true;

            // kill threads
            clsSettings.KillThread(ThreadItemRecordPath, "Killing Record Path Thread");
            clsSettings.KillThread(ThreadItemHerb, "Killing Object Search Thread");
            clsSettings.KillThread(ThreadItemHunt, "Killing Combat Search Thread");

            // suppress garbage collection
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Vendor Run

        /// <summary>
        /// Runs from the Hunt Path to the vendor. Returns to the start of the Hunt path
        /// </summary>
        /// <param name="Paths">list of paths: Hunt, Hunt Start, Vendor, Mailbox</param>
        /// <param name="GraveyardPaths">list of graveyard paths</param>
        /// <param name="CurrentPath">the current path (HUNT path)</param>
        public static EMovementResult VendorRun(List<PathListInfoEx> Paths, List<PathListInfoEx> GraveyardPaths, PathListInfoEx CurrentPath)
        {
            clsPath cPath = new clsPath();
            EMovementResult returnResult;
            NearPathPoint nearPoint;
            string msg = "Vendor Run";
            int OldSearchRange = clsSettings.gclsLevelSettings.SearchRange; // old search range
            clsSettings.EMobSearchType OldSearchForHostiles = clsSettings.MobSearchType;

            try
            {
                // log it
                clsSettings.Logging.AddToLog(Resources.Vendor, "Starting Vendor Run");

                // change search range
                clsSettings.gclsLevelSettings.SearchRange = 15;
                clsSettings.MobSearchType = clsSettings.EMobSearchType.Hostile;

                // reverse the path, so we can return to hunt start
                if (!CurrentPath.PathReversed)
                    CurrentPath.ReversePath();

                // get the vendor path & huntstart path
                PathListInfoEx VendorPath = GetPathFromList(Paths, clsGlobals.Path_Vendor);
                PathListInfoEx HuntStartPath = GetPathFromList(Paths, clsGlobals.Path_StartHunt);
                if (VendorPath == null)
                    throw new clsRhabotException("Could not find vendor path in path list", msg);

                // run to end of path if not near vendor
                if (cPath.GetNearestPoint(VendorPath, clsCharacter.MyLocation) == null)
                {
                    clsSettings.Logging.AddToLog(Resources.Vendor, "Found point near vendor path.");
                    if (RunPath(CurrentPath, true, false, null, null, GraveyardPaths) != EMovementResult.Success)
                        throw new clsRhabotException("Could not return to start of Hunt path", msg);

                    // run hunt start if we are not near vendor path
                    if (cPath.GetNearestPoint(VendorPath, clsCharacter.MyLocation) == null)
                    {
                        if (!RunHuntStart(HuntStartPath, false))
                            throw new clsRhabotException("Could not run Hunt Start path", msg);
                    }
                }

                // raise error if we are not near vendor start
                nearPoint = cPath.GetNearestPoint(VendorPath, clsCharacter.MyLocation);
                if ((nearPoint == null) || (nearPoint.PPoint == null))
                    throw new clsRhabotException("Could not find Vendor path nearby", msg);

                // run to the vendor path
                clsSettings.Logging.AddToLog(Resources.Vendor, "Running to nearest vendor path point");
                cPath.MoveToPoint(nearPoint.PPoint);

                // set current point on vendor path
                VendorPath.CurrentStep = nearPoint.Element;

                // run this path
                clsSettings.Logging.AddToLog(Resources.Vendor, "Running Vendor Path");
                if (RunPath(VendorPath, false, false, null, null, GraveyardPaths) != EMovementResult.Success)
                    throw new clsRhabotException("Could not run to vendor", msg);

                // check for forced shutdown
                if (!clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                    return EMovementResult.Stopped;

                // handle vendor routine
                clsSettings.Logging.AddToLog(Resources.Vendor, "Starting Vendor Routine");
                if (clsVendor.SellToVendor(clsVendor.GetNearestVendor(true)) != EMovementResult.Success)
                    clsError.ShowError(new Exception("Could not sell to vendor, attempting run to mailbox"), msg, true, new StackFrame(0, true), false);

                // get the mailbox path
                PathListInfoEx MailboxPath = GetPathFromList(Paths, clsGlobals.Path_Mailbox);
                if (MailboxPath != null)
                {
                    // run to the mailbox
                    if (RunPath(MailboxPath) != EMovementResult.Success)
                        throw new clsRhabotException("Could not run to mailbox", msg);

                    // mail stuff
                    clsMail.SendMailList();

                    // check for forced shutdown
                    if (!clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                        return EMovementResult.Stopped;

                    // we need to return to our start point
                    MailboxPath.ReversePath();
                    if (RunPath(MailboxPath) != EMovementResult.Success)
                        throw new clsRhabotException("Could not return on mailbox path", msg);
                }

                // we are at vendor, return on vendor to start point
                VendorPath.ReversePath();
                if (RunPath(VendorPath) != EMovementResult.Success)
                    throw new clsRhabotException("Could not return on vendor path", msg);

                // get hunt start if we are not close enough to hunt
                CurrentPath = GetPathFromList(Paths, clsGlobals.Path_Hunting);
                nearPoint = cPath.GetNearestPoint(CurrentPath, clsCharacter.MyLocation);
                if ((nearPoint == null) || (nearPoint.PPoint == null))
                {
                    clsSettings.Logging.AddToLog(Resources.Vendor, "Running Hunt Start Path");
                    HuntStartPath.RevertToPathForward();
                    RunPath(HuntStartPath);
                }

                // reset hunt path
                clsSettings.Logging.AddToLog(Resources.Vendor, "Returning to hunt path");
                nearPoint = cPath.GetNearestPoint(CurrentPath, clsCharacter.MyLocation);
                CurrentPath.CurrentStep = nearPoint.Element;
                returnResult = EMovementResult.Success;
            }

            catch (clsRhabotException rExcep)
            {
                rExcep.ShowError();
                returnResult = EMovementResult.Error;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, msg);
                returnResult = EMovementResult.Error;
            }

            finally
            {
                // reset search range
                clsSettings.gclsLevelSettings.SearchRange = OldSearchRange;
                clsSettings.MobSearchType = OldSearchForHostiles;
            }

            return returnResult;
        }

        // Vendor Run
        #endregion

        #region IsMoving Thread

        internal class clsIsMovingThread : ThreadBase
        {
            /// <summary>
            /// Monitors if we are moving and updates the Moving property
            /// </summary>
            public void IsMoving_Thread()
            {
                PathListInfo.PathPoint MyLoc, LastLoc = null;

                try
                {
                    // begin the loop
                    while (!Shutdown)
                    {
                        // test for stop
                        if (!clsSettings.TestPauseStop("IsMoving Thread exiting due to script stop"))
                            return;

                        // sleep for 1 second
                        Thread.Sleep(1000);

                        using (new clsFrameLock.LockBuffer())
                        {
                            // skip if character invalid
                            if (!clsCharacter.CharacterIsValid)
                                continue;

                            // update moving
                            MyLoc = clsCharacter.MyLocation;
                        }

                        // if no location, set not moving
                        if (LastLoc == null)
                        {
                            LastLoc = MyLoc;
                            Moving = false;
                            continue;
                        }

                        // see if the two locations are the same point
                        Moving = !MyLoc.SamePoint(LastLoc);

                        // reset lastloc
                        LastLoc = MyLoc;
                    }
                }

                catch (Exception excep)
                {
                    clsError.ShowError(excep, "IsMoving Thread");
                }
            }
        }

        // IsMoving Thread
        #endregion

        #region IPO

        // IPO test, by EQJoe
        /// <summary>
        /// Tests for collision to next point, and tries to reroute around collision. Returns true if
        /// path is obstructed and we can't get around it
        /// </summary>
        /// <param name="NextPoint">the target point</param>
        public bool IPO(PathListInfo.PathPoint NextPoint)
        {
            PathListInfo.PathPoint myLoc;
            bool rVal = false;

            try
            {
                // log it
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLog(Resources.IPO, Resources.CheckingForCollision);

                using (new clsFrameLock.LockBuffer())
                {
                    // get my location
                    myLoc = clsCharacter.MyLocation;

                    // get target distance
                    double targetDist = myLoc.Distance(NextPoint) + 2;

                    // exit if no collision and slope is fine
                    if (!clsSettings.isxwow.Me.IsPathObstructed(myLoc.X, myLoc.Y, myLoc.Z, targetDist, NextPoint.X, NextPoint.Y, NextPoint.Z))
                        //(!IsPathObstructed(NextPoint)))
                        return false;

                    // stop moving
                    StopMoving();
                }

                // try, once to move to the point
                if (clsStuck.MoveTo(NextPoint))
                    return false;

                // Create a path around the point
                rVal = clsStuck.PathAround(NextPoint);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.IPO);
            }

            return rVal;
        }

        // IPO
        #endregion
    }
}
