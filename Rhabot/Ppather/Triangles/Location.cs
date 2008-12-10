using System;
using System.Collections.Generic;
using System.Text;

namespace PathMaker.Graph {
	public class Location {
		private float x;
		private float y;
		private float z;

		public Location(float x, float y, float z) 
        {
			this.x = x; this.y = y; this.z = z;
		}

		public float X { get { return x; } }
		public float Y { get { return y; } }
		public float Z { get { return z; } }

		public float GetDistanceTo(Location l) 
        {
			//float dx = x - l.X;
			//float dy = y - l.Y;
			//float dz = z - l.Z;
			//return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            return (float)Math.Sqrt(Math.Pow(x - l.X, 2) + Math.Pow(y - l.Y, 2));
		}

		public override String ToString() 
        {
            return string.Format("[{0}, {1}, {2}]", x, y, z);
		}

		public Location InFrontOf(float heading, float d) 
        {
			//float nx = x + (float)Math.Cos(heading) * d;
			//float ny = y + (float)Math.Sin(heading) * d;
			//float nz = z;
			//return new Location(nx, ny, nz);
            return new Location(
                x + (float)Math.Cos(heading) * d,
                y + (float)Math.Sin(heading) * d,
                z);
		}
	}
}
