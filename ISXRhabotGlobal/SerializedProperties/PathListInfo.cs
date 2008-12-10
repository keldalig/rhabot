using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using ISXRhabotGlobal.SerializedProperties;

namespace ISXBotHelper
{
    /// <summary>
    /// A full path list
    /// </summary>
    [Serializable]
    public partial class PathListInfo : IXmlSerializable
    {
        #region Properties

        /// <summary>
        /// The list of points to traverse
        /// </summary>
        public List<PathPoint> PathList = new List<PathPoint>();

        /// <summary>
        /// List of path names this path can connect to (connects at element 0 of both paths)
        /// </summary>
        public List<string> ConnectsTo = new List<string>();

        private int m_CurrentStep = -1;
        /// <summary>
        /// The current step of the path or -1 for no step
        /// </summary>
        public int CurrentStep
        {
            get { return m_CurrentStep; }
            set { m_CurrentStep = value; }
        }

        private string m_PathName = string.Empty;
        /// <summary>
        /// The name of this path
        /// </summary>
        public string PathName
        {
            get { return m_PathName; }
            set { m_PathName = value; }
        }

        private bool m_CanMount = false;
        /// <summary>
        /// True if we can cast MountUp on this path
        /// </summary>
        public bool CanMount
        {
            get { return m_CanMount; }
            set { m_CanMount = value; }
        }

        private bool m_CanFly = false;
        /// <summary>
        /// True if the path uses a flying mount
        /// </summary>
        public bool CanFly
        {
            get { return m_CanFly; }
            set { m_CanFly = value; }
        }

        private bool m_CanSwim = false;
        /// <summary>
        /// True if the path is underwater
        /// </summary>
        public bool CanSwim
        {
            get { return m_CanSwim; }
            set { m_CanSwim = value; }
        }

        private bool m_PathReversed = false;
        /// <summary>
        /// True if the path is reveresed
        /// </summary>
        public bool PathReversed
        {
            get { return m_PathReversed; }
            set { m_PathReversed = value; }
        }

        // Properties
        #endregion

        #region Init

        /// <summary>
        /// Initializes a new instance of the PathListInfo class.
        /// </summary>
        public PathListInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PathListInfo class.
        /// </summary>
        /// <param name="pathList"></param>
        /// <param name="connectsTo"></param>
        /// <param name="pathName"></param>
        /// <param name="canMount"></param>
        /// <param name="canFly"></param>
        /// <param name="canSwim"></param>
        public PathListInfo(string pathName, bool canMount, bool canFly, bool canSwim, List<PathPoint> pathList, List<string> connectsTo)
        {
            PathList = pathList;
            ConnectsTo = connectsTo;
            m_PathName = pathName;
            m_CanMount = canMount;
            m_CanFly = canFly;
            m_CanSwim = canSwim;
        }

        /// <summary>
        /// Initializes a new instance of the PathListInfo class.
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="canMount"></param>
        /// <param name="canFly"></param>
        /// <param name="canSwim"></param>
        /// <param name="AutoNavPointList"></param>
        public PathListInfo(string pathName, bool canMount, bool canFly, bool canSwim, List<AutoNav.clsPathPoint> AutoNavPointList)
        {
            m_PathName = pathName;
            m_CanMount = canMount;
            m_CanFly = canFly;
            m_CanSwim = canSwim;

            // pop the point list
            PathList = new List<PathPoint>();
            foreach (AutoNav.clsPathPoint aPoint in AutoNavPointList)
                PathList.Add(new PathPoint(aPoint));
        }

        // Init
        #endregion

        #region Functions

        /// <summary>
        /// Returns the next point in the path, or null if no point found
        /// </summary>
        /// <returns></returns>
        public PathPoint GetNextPoint()
        {
            m_CurrentStep++;

            // get the point
            return GetPoint();
        }

        /// <summary>
        /// Returns the previous point in the path, or null if no point found
        /// </summary>
        /// <returns></returns>
        public PathPoint GetPreviousPoint()
        {
            m_CurrentStep--;

            // get the point
            return GetPoint();
        }

        /// <summary>
        /// Returns the point referred to by CurrentStep
        /// </summary>
        public PathPoint GetPoint()
        {
            // if no point, return null
            if (CurrentStep < 0)
            {
                CurrentStep = 0;
                return null;
            }
            if (CurrentStep >= PathList.Count)
            {
                CurrentStep = PathList.Count - 1;
                return null;
            }

            // return point in the path list
            return PathList[CurrentStep];
        }

        /// <summary>
        /// Reverses the path and resets current point to 0
        /// </summary>
        public void ReversePath(int NearestPointIndex)
        {
            // get the nearest point
            PathPoint NearPoint = PathList[NearestPointIndex];

            // loop through the current list and add it's inverse
            PathList.Reverse();

            // reset current point
            PathReversed = !PathReversed;

            // get our current step
            CurrentStep = PathList.IndexOf(NearPoint);
        }

        /// <summary>
        /// If the path is not reversed, nothing happens. If the
        /// path is reversed: 1) re-reverse path, 2) reset currentstep to your current location
        /// </summary>
        public void RevertToPathForward(int NearestPointIndex)
        {
            // exit if path is not reversed
            if (!PathReversed)
                return;

            // reverse the path
            PathList.Reverse();
            PathReversed = !PathReversed;

            // get our current step
            CurrentStep = NearestPointIndex;
        }
        /// <summary>
        /// Sets CurrentStep to the point nearest to your location. Returns false on failure
        /// </summary>
        public bool SetCurrentStep(int NearestPointIndex)
        {
            // change current step
            CurrentStep = NearestPointIndex;
            return true;
        }

        // Functions
        #endregion

        #region Clone

        /// <summary>
        /// Clones this path
        /// </summary>
        /// <returns></returns>
        public PathListInfo Clone()
        {
            PathListInfo retPI = new PathListInfo();
            retPI.CanMount = CanMount;
            retPI.CanFly = CanFly;
            retPI.CanSwim = CanSwim;

            // clone connects to
            foreach (string connect in ConnectsTo)
                retPI.ConnectsTo.Add((string)connect.Clone());

            retPI.CurrentStep = CurrentStep;
            retPI.PathName = (string)PathName.Clone();
            retPI.PathReversed = PathReversed;

            // clone path points
            foreach (PathPoint pPoint in PathList)
                retPI.PathList.Add(pPoint);

            // return
            return retPI;
        }

        // Clone
        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            // get properties
            m_CurrentStep = 0;
            m_PathReversed = false;
            m_PathName = reader.GetAttribute("PathName");
            m_CanMount = reader.GetAttribute("CanMount").ConvertToBool(false);
            m_CanFly = reader.GetAttribute("CanFly").ConvertToBool(false);
            m_CanSwim = reader.GetAttribute("CanSwim").ConvertToBool(false);

            // get connects
            ConnectsTo = clsXMLSerialList.ReadXMLList<string>("ConnectsTo", typeof(string), reader);

            // get path points
            PathList = clsXMLSerialList.ReadXMLList<PathPoint>("PathList", typeof(PathPoint), reader);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            int i;

            // add properties
            writer.WriteAttributeString("PathName", this.PathName);
            writer.WriteAttributeString("CanMount", this.CanMount.ToString());
            writer.WriteAttributeString("CanFly", this.CanFly.ToString());
            writer.WriteAttributeString("CanSwim", this.CanSwim.ToString());

            // serialize connects to
            clsXMLSerialList.WriteXMLList<string>("ConnectsTo", ConnectsTo, typeof(string), ref writer);

            // serialize path points
            clsXMLSerialList.WriteXMLList<PathPoint>("PathList", PathList, typeof(PathPoint), ref writer);
        }

        #endregion
    }
}