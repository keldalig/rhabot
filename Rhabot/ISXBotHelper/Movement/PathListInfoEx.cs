using System;
using System.Collections.Generic;
using System.Linq;

namespace ISXBotHelper
{
    public partial class clsPath
    {
        /// <summary>
        /// A full path list
        /// </summary>
        [Serializable]
        public class PathListInfoEx
        {
            #region Variables

            public PathListInfo pathListInfo = new PathListInfo();

            // Variables
            #endregion

            #region Properties

            /// <summary>
            /// The list of points to traverse
            /// </summary>
            public List<PathListInfo.PathPoint> PathList
            {
                get { return pathListInfo.PathList; }
                set { pathListInfo.PathList = value; }
            }

            /// <summary>
            /// List of path names this path can connect to (connects at element 0 of both paths)
            /// </summary>
            public List<string> ConnectsTo
            {
                get { return pathListInfo.ConnectsTo; }
                set { pathListInfo.ConnectsTo = value; }
            }

            /// <summary>
            /// The current step of the path or -1 for no step
            /// </summary>
            public int CurrentStep
            {
                get { return pathListInfo.CurrentStep; }
                set { pathListInfo.CurrentStep = value; }
            }

            /// <summary>
            /// The name of this path
            /// </summary>
            public string PathName
            {
                get { return pathListInfo.PathName; }
                set { pathListInfo.PathName = value; }
            }

            /// <summary>
            /// True if we can cast MountUp on this path
            /// </summary>
            public bool CanMount
            {
                get { return pathListInfo.CanMount; }
                set { pathListInfo.CanMount = value; }
            }

            /// <summary>
            /// True if the path uses a flying mount
            /// </summary>
            public bool CanFly
            {
                get { return pathListInfo.CanFly; }
                set { pathListInfo.CanFly = value; }
            }

            /// <summary>
            /// True if the path is underwater
            /// </summary>
            public bool CanSwim
            {
                get { return pathListInfo.CanSwim; }
                set { pathListInfo.CanSwim = value; }
            }

            /// <summary>
            /// True if the path is reveresed
            /// </summary>
            public bool PathReversed
            {
                get { return pathListInfo.PathReversed; }
                set { pathListInfo.PathReversed = value; }
            }

            // Properties
            #endregion

            #region Init

            /// <summary>
            /// Initializes a new instance of the PathListInfoEx class.
            /// </summary>
            public PathListInfoEx()
            {
            }

            /// <summary>
            /// Initializes a new instance of the PathListInfoEx class.
            /// </summary>
            /// <param name="pathList"></param>
            /// <param name="connectsTo"></param>
            /// <param name="pathName"></param>
            /// <param name="canMount"></param>
            /// <param name="canFly"></param>
            /// <param name="canSwim"></param>
            public PathListInfoEx(string pathName, bool canMount, bool canFly, bool canSwim, List<PathListInfo.PathPoint> pathList, List<string> connectsTo)
            {
                pathListInfo.PathList = pathList;
                pathListInfo.ConnectsTo = connectsTo;
                pathListInfo.PathName = pathName;
                pathListInfo.CanMount = canMount;
                pathListInfo.CanFly = canFly;
                pathListInfo.CanSwim = canSwim;
            }

            /// <summary>
            /// Initializes a new instance of the PathListInfoEx class.
            /// </summary>
            /// <param name="pathName"></param>
            /// <param name="canMount"></param>
            /// <param name="canFly"></param>
            /// <param name="canSwim"></param>
            /// <param name="AutoNavPointList"></param>
            public PathListInfoEx(string pathName, bool canMount, bool canFly, bool canSwim, List<AutoNav.clsPathPoint> AutoNavPointList)
            {
                pathListInfo.PathName = pathName;
                pathListInfo.CanMount = canMount;
                pathListInfo.CanFly = canFly;
                pathListInfo.CanSwim = canSwim;
                pathListInfo.PathList = AutoNavPointList.ConvertAll<PathListInfo.PathPoint>(x => new PathListInfo.PathPoint(x)).ToList();
            }

            public PathListInfoEx(PathListInfo pInfo)
            {
                pathListInfo = pInfo;
            }

            // Init
            #endregion

            #region Functions

            /// <summary>
            /// Returns the next point in the path, or null if no point found
            /// </summary>
            /// <returns></returns>
            public PathListInfo.PathPoint GetNextPoint()
            {
                return pathListInfo.GetNextPoint();
            }

            /// <summary>
            /// Returns the previous point in the path, or null if no point found
            /// </summary>
            /// <returns></returns>
            public PathListInfo.PathPoint GetPreviousPoint()
            {
                return pathListInfo.GetPreviousPoint();
            }

            /// <summary>
            /// Returns the point referred to by CurrentStep
            /// </summary>
            public PathListInfo.PathPoint GetPoint()
            {
                return pathListInfo.GetPoint();
            }

            /// <summary>
            /// Reverses the path and resets current point to 0
            /// </summary>
            public void ReversePath()
            {
                clsPath cPath = new clsPath();

                try
                {
                    // log it
                    clsSettings.Logging.AddToLog("Reversing Path");

                    // loop through the current list and add it's inverse
                    StopMoving();

                    // get our current step
                    int index = 0;
                    NearPathPoint npp = cPath.GetNearestPoint(this, clsCharacter.MyLocation);
                    if ((npp != null) && (npp.PPoint != null))
                        index = npp.Element;

                    // reverse path
                    pathListInfo.ReversePath(index);
                }
                catch (Exception excep)
                {
                    clsError.ShowError(excep, "Reverse Path");
                }
            }

            /// <summary>
            /// If the path is not reversed, nothing happens. If the
            /// path is reversed: 1) re-reverse path, 2) reset currentstep to your current location
            /// </summary>
            public void RevertToPathForward()
            {
                clsPath cPath = new clsPath();

                try
                {
                    // log it
                    clsSettings.Logging.AddToLog("RevertToPathForward", "Reverting Path");

                    // stop moving
                    StopMoving();

                    // get our current step
                    int index = 0;
                    NearPathPoint npp = cPath.GetNearestPoint(this, clsCharacter.MyLocation);
                    if ((npp != null) && (npp.PPoint != null))
                        index = npp.Element;

                    // revert path
                    pathListInfo.RevertToPathForward(index);
                }

                catch (Exception excep)
                {
                    clsError.ShowError(excep, "RevertToPathForward");
                }
            }

            /// <summary>
            /// Sets CurrentStep to the point nearest to your location. Returns false on failure
            /// </summary>
            public bool SetCurrentStep()
            {
                NearPathPoint npp = new clsPath().GetNearestPoint(this, clsCharacter.MyLocation);
                if ((npp == null) || (npp.PPoint == null))
                    return false;

                // change current step
                return pathListInfo.SetCurrentStep(npp.Element);
            }

            // Functions
            #endregion

            #region Clone

            /// <summary>
            /// Clong this object
            /// </summary>
            public PathListInfoEx Clone()
            {
                return new PathListInfoEx(pathListInfo.Clone());
            }

            // Clone
            #endregion
        }
    }
}