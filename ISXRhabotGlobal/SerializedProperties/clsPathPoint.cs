using System;
using LavishScriptAPI;
using System.Xml.Serialization;

namespace ISXBotHelper
{
    public partial class PathListInfo
    {
        #region PathPoint

        /// <summary>
        /// Represents a point on the path
        /// </summary>
        [Serializable]
        public class PathPoint : IXmlSerializable
        {
            #region Properties

            private double m_X = 0;
            public double X
            {
                get { return m_X; }
                set { m_X = value; }
            }

            private double m_Y = 0;
            public double Y
            {
                get { return m_Y; }
                set { m_Y = value; }
            }

            private double m_Z = 0;
            public double Z
            {
                get { return m_Z; }
                set { m_Z = value; }
            }

            // Properties
            #endregion

            #region Init

            /// <summary>
            /// Initializes a new instance of the PathPoint class.
            /// </summary>
            public PathPoint()
            {
            }

            /// <summary>
            /// Initializes a new instance of the PathPoint class.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            public PathPoint(double x, double y, double z)
            {
                m_X = x;
                m_Y = y;
                m_Z = z;
            }

            /// <summary>
            /// Initializes a new instance of the PathPoint class.
            /// </summary>
            public PathPoint(Point3f point3f)
            {
                X = point3f.X;
                Y = point3f.Y;
                Z = point3f.Z;
            }

            /// <summary>
            /// Initializes a new instance of the PathPoint class.
            /// </summary>
            public PathPoint(AutoNav.clsPathPoint cPathPoint)
            {
                X = cPathPoint.X;
                Y = cPathPoint.Y;
                Z = cPathPoint.Z;
            }

            // Init
            #endregion

            #region Functions

            /// <summary>
            /// Returns the distance to the compare point
            /// </summary>
            /// <param name="ComparePoint"></param>
            public double Distance(PathPoint ComparePoint)
            {
                return Math.Sqrt(Math.Pow(ComparePoint.X - X, 2) + Math.Pow(ComparePoint.Y - Y, 2));
            }

            /// <summary>
            /// Returns the X,Y,Z distance to the compare point
            /// </summary>
            public double DistanceZ(PathPoint ComparePoint)
            {
                return Math.Sqrt(Math.Pow(ComparePoint.X - X, 2) + Math.Pow(ComparePoint.Y - Y, 2) + Math.Pow(ComparePoint.Z - Z, 2));
            }

            /// <summary>
            /// Returns true if the points are the same
            /// </summary>
            /// <param name="TestPoint">the point to compare</param>
            public bool SamePoint(PathPoint TestPoint)
            {
                return Distance(TestPoint) < 1;
                //return ((X == TestPoint.X) && (Y == TestPoint.Y) && (Z == TestPoint.Z));
            }

            public override string ToString()
            {
                return string.Format("({0}, {1}, {2})", X, Y, Z);
            }

            /// <summary>
            /// Converts the current point to an AutoNav point
            /// </summary>
            public AutoNav.clsPathPoint ToAutoNavPoint()
            {
                return new AutoNav.clsPathPoint(X, Y, Z);
            }

            // Functions
            #endregion

            #region IXmlSerializable Members

            public System.Xml.Schema.XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(System.Xml.XmlReader reader)
            {
                this.X = Convert.ToDouble(reader.GetAttribute("X"));
                this.Y = Convert.ToDouble(reader.GetAttribute("Y"));
                this.Z = Convert.ToDouble(reader.GetAttribute("Z"));
            }

            public void WriteXml(System.Xml.XmlWriter writer)
            {
                writer.WriteAttributeString("X", this.X.ToString().Trim());
                writer.WriteAttributeString("Y", this.Y.ToString().Trim());
                writer.WriteAttributeString("Z", this.Z.ToString().Trim());
            }

            #endregion
        }

        // PathPoint
        #endregion
    }
}
