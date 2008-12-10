/*
 *  Part of PPather
 *  Copyright Pontus Borg 2008
 * 
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WowTriangles
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Vector
    {
        [FieldOffset(0)]
        public float x;
        [FieldOffset(4)]
        public float y;
        [FieldOffset(8)]
        public float z;

        public override string ToString()
        {
            return String.Format("({0:.00} {1:.00} {2:.00})", x, y, z);
        }
    }

    unsafe static class ccode
    {
        // int triBoxOverlap(float boxcenter[3],float boxhalfsize[3],float triverts[3][3]);
        [DllImport("ccode.dll")]
        public static extern int triBoxOverlap(
            float *boxcenter,
            float *boxhalfsize,
            float* trivert0,
            float *trivert1,
            float *trivert2
            );
    }

    internal static unsafe class Utils
    {
        public static float abs(float a)
        {
            return (a < 0.0f) ? -a : a;
        }

        public static float min(float a, float b)
        {
            return (a < b) ? a : b;
        }
        public static float min(float a, float b, float c)
        {
            if (a < b && a < c) return a;
            if (b < c) return b;
            return c;
        }

        public static float max(float a, float b)
        {
            return (a > b) ? a : b;
        }
        public static float max(float a, float b, float c)
        {
            if (a > b && a > c) return a;
            if (b > c) return b;
            return c;
        }

  
        public static float VecLength(ref Vector d)
        {          
            return (float)Math.Sqrt(d.x * d.x + d.y * d.y + d.z * d.z);
        }

        public static void sub( out Vector C, ref Vector A, ref Vector B)
        {
            C.x = A.x - B.x;
            C.y = A.y - B.y;
            C.z = A.z - B.z;
        }

        public static void add(out Vector C, ref Vector A, ref Vector B)
        {
            C.x = A.x + B.x;
            C.y = A.y + B.y;
            C.z = A.z + B.z;
        }

        public static void mul(out Vector C, ref Vector A, float b)
        {
            C.x = A.x * b;
            C.y = A.y * b;
            C.z = A.z * b;
        }
        public static void div(out Vector C, ref Vector A, float b)
        {
            C.x = A.x / b;
            C.y = A.y / b;
            C.z = A.z / b;
        }

        public static void cross(out Vector dest, ref Vector v1, ref Vector v2)
        {
            dest.x = v1.y * v2.z - v1.z * v2.y;
            dest.y = v1.z * v2.x - v1.x * v2.z;
            dest.z = v1.x * v2.y - v1.y * v2.x; 
        }

 
        public static float dot(ref Vector v0,ref  Vector v1)        
        {
            return v0.x*v1.x + v0.y*v1.y + v0.z*v1.z ; 
        }


        public static bool SegmentTriangleIntersect(Vector p0, Vector p1,
                                                    Vector t0, Vector t1, Vector t2, 
                                                    out Vector I)
        {

            I.x = I.y = I.z = 0;

            Vector u;  sub(out u, ref t1, ref t0); // triangle vector 1
            Vector v;  sub(out v, ref t2, ref t0); // triangle vector 2
            Vector n; cross(out n, ref u, ref v); // triangle normal


            Vector dir; sub(out dir, ref p1, ref p0); // ray direction vector
            Vector w0;  sub(out w0, ref p0, ref t0);
            float a = -dot(ref n, ref w0);
            float b = dot(ref n, ref dir);
            if (abs(b) < float.Epsilon) return false; // parallel

            // get intersect point of ray with triangle plane
            float r = a / b;
            if (r < 0.0) return false; // "before" p0
            if (r > 1.0) return false; // "after" p1

            Vector M; mul(out M, ref dir, r);
            add(out I, ref p0, ref M);// intersect point of line and plane

            // is I inside T?
            float uu = dot(ref u, ref u);
            float uv = dot(ref u, ref v);
            float vv = dot(ref v, ref v);
            Vector w;  sub(out w, ref I, ref t0);
            float wu = dot(ref w, ref u);
            float wv = dot(ref w, ref v);
            float D = uv * uv - uu * vv;

            // get and test parametric coords
            float s = (uv * wv - vv * wu) / D;
            if (s < 0.0 || s > 1.0)        // I is outside T
                return false;

            float t = (uv * wu - uu * wv) / D;
            if (t < 0.0 || (s + t) > 1.0)  // I is outside T
                return false;
            
            return true;
        }


        public static float PointDistanceToSegment(Vector p0,
                                           Vector x1, Vector x2)
        {
            Vector L; sub(out L, ref x2, ref x1); // the segment vector
            float l2 = dot(ref L, ref L);   // square length of the segment

            Vector D; sub(out D, ref p0, ref x1);   // vector from point to segment start
            float d = dot(ref D, ref L);     // projection factor [x2-x1].[p0-x1]


            if (d < 0.0f) // closest to x1
                return VecLength(ref D);

            Vector E; mul(out E, ref L, d / l2); // intersect


            if (dot(ref E, ref L) > l2) // closest to x2
            {
                Vector L2;  sub(out L2, ref D, ref L);
                return VecLength(ref L2);
            }

            Vector L3; sub(out L3, ref D, ref E);
            return VecLength(ref L3);

        }

        public static void GetTriangleNormal(Vector t0, Vector t1, Vector t2, out Vector normal)
        {
            Vector u; sub(out u, ref t1, ref t0); // triangle vector 1
            Vector v; sub(out v, ref  t2, ref t0); // triangle vector 2
            cross(out normal, ref u, ref v); // triangle normal
            float l = VecLength(ref normal);
            div(out normal, ref normal,  l); 
        }

        public static float PointDistanceToTriangle(Vector p0,
                                                    Vector t0, Vector t1, Vector t2)
        {
            Vector u; sub(out u, ref t1, ref t0); // triangle vector 1
            Vector v; sub(out v, ref t2, ref  t0); // triangle vector 2
            Vector n; cross(out n, ref u, ref v); // triangle normal
            n.x *= -1E6f;
            n.y *= -1E6f;
            n.z *= -1E6f;

            Vector intersect; 
            bool hit = SegmentTriangleIntersect(p0, n, t0, t1, t2, out intersect);
            if (hit) 
            {
                Vector L; sub(out L, ref intersect, ref p0);
                return VecLength(ref L);
            }

            //float d0 = PointDistanceToSegment(p0, t0, t1);
            //float d1 = PointDistanceToSegment(p0, t0, t1);
            //float d2 = PointDistanceToSegment(p0, t0, t1);

            //return min(d0, d1, d2);
            return min(
                PointDistanceToSegment(p0, t0, t1),
                PointDistanceToSegment(p0, t0, t1),
                PointDistanceToSegment(p0, t0, t1));
        }

        public static bool TestTriangleBoxIntersect(Vector vertex0, Vector vertex1, Vector vertex2,
                                                    Vector boxcenter, Vector  boxhalfsize)
        {
            int i = 0; 
            float* pcenter = (float*)&boxcenter;
            float* phalf = (float*)&boxhalfsize;
            float* ptriangle0 = (float*)&vertex0;
            float* ptriangle1 = (float*)&vertex1;
            float* ptriangle2 = (float*)&vertex2; 

            try
            {
                i = ccode.triBoxOverlap(pcenter, phalf, ptriangle0, ptriangle1, ptriangle2);
            }
            catch { }
            return i == 1;
        }
    }

    internal class SparseFloatMatrix3D<T> : SparseMatrix3D<T>
    {
        private float gridSize;

        public SparseFloatMatrix3D(float gridSize)
        {
            this.gridSize = gridSize;
        }

        private const float offset = 100000f;
        private int LocalToGrid(float f)
        {
            return (int)((f + offset) / gridSize);
        }

        public List<T> GetAllInCube(float min_x, float min_y, float min_z,
                                    float max_x, float max_y, float max_z)
        {
            int startx = LocalToGrid(min_x);
            int starty = LocalToGrid(min_y);
            int startz = LocalToGrid(min_z);

            int stopx = LocalToGrid(max_x);
            int stopy = LocalToGrid(max_y);
            int stopz = LocalToGrid(max_z);
            List<T> l = new List<T>();

            for (; startx <= stopx; startx++)
            {
                for (; starty <= stopy; starty++)
                {
                    for (; startz <= stopz; startz++)
                    {
                        if(base.IsSet(startx, starty, startz))
                            l.Add(base.Get(startx, starty, startz));
                    }
                }
            }
            return l;
        }

            

        public T Get(float x, float y, float z)
        {
            //int ix = LocalToGrid(x);
            //int iy = LocalToGrid(y);
            //int iz = LocalToGrid(z);
            //return base.Get((int)ix, (int)iy, (int)iz);
            return base.Get(LocalToGrid(x), LocalToGrid(y), LocalToGrid(z));
        }

        public bool IsSet(float x, float y, float z)
        {
            //int ix = LocalToGrid(x);
            //int iy = LocalToGrid(y);
            //int iz = LocalToGrid(z);
            //return base.IsSet((int)ix, (int)iy, (int)iz);
            return base.IsSet(LocalToGrid(x), LocalToGrid(y), LocalToGrid(z));
        }

        public void Set(float x, float y, float z, T val)
        {
            //int ix = LocalToGrid(x);
            //int iy = LocalToGrid(y);
            //int iz = LocalToGrid(z);
            //base.Set((int)ix, (int)iy, (int)iz, val);
            base.Set(LocalToGrid(x), LocalToGrid(y), LocalToGrid(z), val);
        }
    }

    internal class SparseFloatMatrix2D<T> : SparseMatrix2D<T>
    {
        private float gridSize;

        public SparseFloatMatrix2D(float gridSize)
            : base(0)
        {
            this.gridSize = gridSize;
        }

        public SparseFloatMatrix2D(float gridSize, int initialCapazity)
            : base(initialCapazity)
        {
            this.gridSize = gridSize;
        }

        public void GetGridStartAt(float x, float y, out float grid_x, out float grid_y)
        {
            grid_x = GridToLocal(LocalToGrid(x));
            grid_y = GridToLocal(LocalToGrid(y));
        }
        private const float offset = 100000f;


        public float GridToLocal(int grid)
        {
            return (float)grid * gridSize - offset; 
        }

        public int LocalToGrid(float f)
        {
            return (int)((f+offset) / gridSize);
        }


        public List<T> GetAllInSquare(float min_x, float min_y,
                                      float max_x, float max_y)
        {
            int startx = LocalToGrid(min_x);
            int stopx  = LocalToGrid(max_x);

            int starty = LocalToGrid(min_y);
            int stopy  = LocalToGrid(max_y);
        
            List<T> l = new List<T>();

            for (int x = startx; x <= stopx; x++)
            {
                for (int y = starty; y <= stopy; y++)
                {
                        if (base.IsSet(x, y))
                            l.Add(base.Get(x, y));                 
                }
            }
            return l;
        }

        public T Get(float x, float y)
        {
            //int ix = LocalToGrid(x);
            //int iy = LocalToGrid(y);
            //return base.Get((int)ix, (int)iy);
            return base.Get(LocalToGrid(x), LocalToGrid(y));
        }

        public bool IsSet(float x, float y)
        {
            //int ix = LocalToGrid(x);
            //int iy = LocalToGrid(y);
            //return base.IsSet((int)ix, (int)iy);
            return base.IsSet(LocalToGrid(x), LocalToGrid(y));
        }

        public void Set(float x, float y, T val)
        {
            //int ix = LocalToGrid(x);
            //int iy = LocalToGrid(y);
            //base.Set((int)ix, (int)iy, val);
            base.Set(LocalToGrid(x), LocalToGrid(y), val);
        }

        public void Clear(float x, float y)
        {
            //int ix = LocalToGrid(x);
            //int iy = LocalToGrid(y);
            //base.Clear((int)ix, (int)iy);
            base.Clear(LocalToGrid(x), LocalToGrid(y));
        }
    }

    internal class SparseMatrix2D<T>
    {
        public SuperMap<XY, T> dic = new SuperMap<XY, T>();

        private bool last = false;
        private bool last_HasValue; 
        private int last_x, last_y;
        private T last_value = default(T);

        internal class XY
        {
            public int x, y;
            public XY(int x, int y)
            {
                this.x = x; this.y = y;
            }

            public override bool Equals(object obj)
            {
                XY other = (XY)obj;
                return (other == this) ? true : (other.x == x && other.y == y);
            }

            public override int GetHashCode()
            {
                return x + y * 100000;
            }
        }
        public SparseMatrix2D(int initialCapacity)
        {
            dic = new SuperMap<XY, T>(initialCapacity);
        }
  
        public bool HasValue(int x, int y)
        {
            if (last && x == last_x && y == last_y) return last_HasValue;
            XY c = new XY(x, y);
            T r = default(T);
            bool b = dic.TryGetValue(c, out r);
            last = true;
            last_x = x; last_y = y; last_HasValue = b; last_value = r;
            return b; 
        }

        public T Get(int x, int y)
        {
            if (last && x == last_x && y == last_y && last_HasValue) return last_value;
            XY c = new XY(x, y);
            T r = default(T);
            bool b = dic.TryGetValue(c, out r);
            last = true;
            last_x = x; last_y = y; last_HasValue = b; last_value = r; 
            return r;
        }

        public void Set(int x, int y, T val)
        {
            XY c = new XY(x, y);
            if (dic.ContainsKey(c))
                dic.Remove(c);
            dic.Add(c, val);
            last = true;
            last_x = x; last_y = y; last_HasValue = true; last_value = val;
        }

        public bool IsSet(int x, int y)
        {
            return HasValue(x, y); 
        }

        public void Clear(int x, int y)
        {
            XY c = new XY(x, y);
            if (dic.ContainsKey(c))
                dic.Remove(c);
            if (last_x == x && last_y == y) last = false;
        }

        public ICollection<T> GetAllElements()
        {
            return dic.GetAllValues();
        }
    }


    internal class SparseMatrix3D<T>
    {
        private SuperMap<XYZ, T> dic = new SuperMap<XYZ, T>();

        private class XYZ
        {
            int x, y, z;
            public XYZ(int x, int y, int z)
            {
                this.x = x; this.y = y; this.z = z;
            }

            public override bool Equals(object obj)
            {
                XYZ other = (XYZ)obj;
                return (other == this) ? true : (other.x == x && other.y == y && other.z == z);
            }

            public override int GetHashCode()
            {
                return x + y * 1000 + z * 1000000;
            }
        }

        public T Get(int x, int y, int z)
        {
            XYZ c = new XYZ(x, y, z);
            T r = default(T);
            dic.TryGetValue(c, out r);
            return r;
        }

        public bool IsSet(int x, int y, int z)
        {
            XYZ c = new XYZ(x, y, z);
            T r = default(T);
            return dic.TryGetValue(c, out r);
        }

        public void Set(int x, int y, int z, T val)
        {
            XYZ c = new XYZ(x, y, z);
            if (dic.ContainsKey(c))
                dic.Remove(c);
            dic.Add(c, val);
        }

        public void Clear(int x, int y, int z)
        {
            XYZ c = new XYZ(x, y, z);
            if (dic.ContainsKey(c))
                dic.Remove(c);
        }

        public ICollection<T> GetAllElements()
        {       
            return dic.GetAllValues(); 
        }

    }


    internal class TrioArray<T>
    {
        const int SIZE = 4096; // Max size if SIZE*SIZE = 16M

        // Jagged array
        // pointer chasing FTL

        // SIZE*(SIZE*3)
        T[][] arrays = null;

        private static void getIndices(int index, out int i0, out int i1)
        {
            i1 = index % SIZE; index /= SIZE;
            i0 = index % SIZE;
        }

        private void allocateAt(int i0, int i1)
        {
            if (arrays == null) arrays = new T[SIZE][];


            T[] a1 = arrays[i0];
            if (a1 == null) { a1 = new T[SIZE * 3]; arrays[i0] = a1; }
        }

        public void SetSize(int new_size)
        {
            if (arrays == null) return;
            int i0, i1;
            getIndices(new_size, out i0, out i1);
            for (int i = i0 + 1; i < SIZE; i++)
                arrays[i] = null;
        }

        public void Set(int index, T x, T y, T z)
        {
            int i0, i1;
            getIndices(index, out i0, out i1);
            allocateAt(i0, i1);
            T[] innermost = arrays[i0];
            i1 *= 3;
            innermost[i1 + 0] = x;
            innermost[i1 + 1] = y;
            innermost[i1 + 2] = z;
        }

        public void Get(int index, out T x, out T y, out T z)
        {
            int i0, i1;
            getIndices(index, out i0, out i1);

            x = default(T);
            y = default(T);
            z = default(T);

            T[] a1 = arrays[i0];
            if (a1 == null) return;

            T[] innermost = arrays[i0];
            i1 *= 3;
            x = innermost[i1 + 0];
            y = innermost[i1 + 1];
            z = innermost[i1 + 2];
        }

    }

    internal class QuadArray<T>
    {
        const int SIZE = 4096; // Max size if SIZE*SIZE = 16M

        // Jagged array
        // pointer chasing FTL

        // SIZE*(SIZE*4)
        T[][] arrays = null;

        private static void getIndices(int index, out int i0, out int i1)
        {
            i1 = index % SIZE; index /= SIZE;
            i0 = index % SIZE;
        }

        private void allocateAt(int i0, int i1)
        {
            if (arrays == null) arrays = new T[SIZE][];


            T[] a1 = arrays[i0];
            if (a1 == null) { a1 = new T[SIZE * 4]; arrays[i0] = a1; }
        }

        public void SetSize(int new_size)
        {
            if (arrays == null) return;
            int i0, i1;
            getIndices(new_size, out i0, out i1);
            for (int i = i0 + 1; i < SIZE; i++)
                arrays[i] = null;
        }

        public void Set(int index, T x, T y, T z, T w)
        {
            int i0, i1;
            getIndices(index, out i0, out i1);
            allocateAt(i0, i1);
            T[] innermost = arrays[i0];
            i1 *= 4;
            innermost[i1 + 0] = x;
            innermost[i1 + 1] = y;
            innermost[i1 + 2] = z;
            innermost[i1 + 3] = w;
        }

        public void Get(int index, out T x, out T y, out T z, out T w)
        {
            int i0, i1;
            getIndices(index, out i0, out i1);

            x = default(T);
            y = default(T);
            z = default(T);
            w = default(T);

            T[] a1 = arrays[i0];
            if (a1 == null) return;


            T[] innermost = arrays[i0];
            i1 *= 4;
            x = innermost[i1 + 0];
            y = innermost[i1 + 1];
            z = innermost[i1 + 2];
            w = innermost[i1 + 3];
        }

    }

    /// <summary>
    /// Default implementation of ISet.
    /// </summary>
    internal class Set<T> : ICollection<T>
    {
        // Use an ISuperMap to implement.

        private SuperHash<T> dictionary = new SuperHash<T>(); // bool?!?!?

        public Set()
        {
        }

        public bool IsReadOnly { get{ return false; } }

        public void CopyTo(T[] a, int off)
        {
            foreach (T e in this)
            {
                a[off++] = e;
            }
        }

        public Set(ICollection<T> objects)
        {
            AddRange(objects);
        }

        #region ISet Members

        public void Add(T anObject)
        {
            dictionary.Add(anObject); 
        }

        public void AddRange(ICollection<T> objects)
        {
            foreach (T obj in objects) Add(obj);
        }

        public void Clear()
        {
            this.dictionary.Clear(0);
        }

        public bool Contains(T anObject)
        {
            return this.dictionary.Contains(anObject);
        }

        public bool Remove(T anObject)
        {
            return this.dictionary.Remove(anObject);
        }

        #endregion

        #region ICollection Members

        public int Count
        {
            get
            {
                return this.dictionary.Count;
            }
        }


        #endregion

        #region IEnumerable Members

        public IEnumerator<T> GetEnumerator()
        {
            return dictionary.GetAll().GetEnumerator(); 
        }



        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); 
        }
        
        
        #endregion
    }
    /// <summary>
    /// Represents an item stored in a priority queue.
    /// </summary>
    /// <typeparam name="TValue">The type of object in the queue.</typeparam>
    /// <typeparam name="TPriority">The type of the priority field.</typeparam>
     struct PriorityQueueItem<TValue, TPriority>
    {
        private TValue value;
        private TPriority priority;

        public PriorityQueueItem(TValue val, TPriority pri)
        {
            this.value = val;
            this.priority = pri;
        }

        /// <summary>
        /// Gets the value of this PriorityQueueItem.
        /// </summary>
        public TValue Value
        {
            get { return value; }
        }

        /// <summary>
        /// Gets the priority associated with this PriorityQueueItem.
        /// </summary>
        public TPriority Priority
        {
            get { return priority; }
        }
    }

    internal class SuperMap<TKey, TValue>
    {
        class Entry
        {
            public TKey key;
            public TValue value;
            public Entry next;

            public Entry(TKey k, TValue v)
            {
                key = k;
                value = v;
            }
        }

        // table of good has size tables
        static uint[] sizes = {
           89, 
           179, 
           359, 
           719, 
           1439, 
           2879, 
           5759, 
           11519, 
           23039, 
           46079, 
           92159, 
           184319, 
           368639, 
           737279, 
           1474559, 
           2949119, 
           5898239, 
           11796479, 
           23592959, 
           47185919, 
           94371839, 
           188743679, 
           377487359, 
           754974719, 
           1509949439
         };


        int elements = 0; // elements in hash
        int size_table_entry = 0;
        Entry[] array;

        public SuperMap()
        {
            Clear(16);
        }
        public SuperMap(int initialCapacity)
        {
            Clear(initialCapacity);
        }

        private static uint GetEntryIn(Entry[] array, TKey key)
        {
            return (uint)key.GetHashCode() % (uint)array.Length;
        }

        private static void AddToArrayNoCheck(Entry[] array, Entry e)
        {
            uint key = GetEntryIn(array, e.key);
            e.next = array[key];
            array[key] = e;
        }

        private static bool HasValue(Entry[] array, TKey k, TValue val)
        {
            uint key = GetEntryIn(array, k);
            Entry rover = array[key];
            while (rover != null)
            {
                if (rover.value.Equals(val))
                    return true;
                rover = rover.next;
            }
            return false;
        }

        private static void AddToArray(Entry[] array, Entry e)
        {
            uint key = GetEntryIn(array, e.key);
            //TValue val = e.value;
            // check for existance
            Entry rover = array[key];
            while (rover != null)
            {
                if (rover.key.Equals(e.key))
                    return;

                rover = rover.next;
            }
            e.next = array[key];
            array[key] = e;
        }

        private void MakeLarger()
        {
            size_table_entry++;
            //uint new_size = sizes[size_table_entry];
            Entry[] new_array = new Entry[sizes[size_table_entry]];

            // add all old stuff to the new one
            int j = array.Length;
            for (int i = 0; i < j; i++)
            {
                Entry rover = array[i];
                while (rover != null)
                {
                    Entry next = rover.next;
                    AddToArrayNoCheck(new_array, rover);
                    rover = next;
                }
            }
            array = new_array;
        }

        private void MakeSmaller()
        {
            if (size_table_entry == 0) return;
            size_table_entry--;
            //uint new_size = sizes[size_table_entry];
            Entry[] new_array = new Entry[sizes[size_table_entry]];

            // add all old stuff to the new one
            int j = array.Length;
            for (int i = 0; i < j; i++)
            {
                Entry rover = array[i];
                while (rover != null)
                {
                    Entry next = rover.next;
                    AddToArrayNoCheck(new_array, rover);
                    rover = next;
                }
            }
            array = new_array;
        }


        public void Add(TKey k, TValue v)
        {
            AddToArray(array, new Entry(k, v));
            elements++;
            if (elements > array.Length * 2)
                MakeLarger();
        }

        public void Clear(int initialCapacity)
        {
            elements = 0;
            size_table_entry = 0;
            for (size_table_entry = 0; sizes[size_table_entry] < initialCapacity; size_table_entry++) ;
            array = new Entry[sizes[size_table_entry]];
        }

        public bool ContainsKey(TKey k)
        {

            uint key = GetEntryIn(array, k);
            Entry rover = array[key];
            while (rover != null)
            {
                if (rover.next == rover)
                {
                    //Console.WriteLine("lsdfjlskfjkl>");
                }
                if (rover.key.Equals(k))
                    return true;
                rover = rover.next;
            }
            return false;
        }


        public bool TryGetValue(TKey k, out TValue v)
        {
            uint key = GetEntryIn(array, k);
            Entry rover = array[key];
            while (rover != null)
            {
                if (rover.next == rover)
                {
                    //Console.WriteLine("ksjdflksdjf");
                }
                if (rover.key.Equals(k))
                {
                    v = rover.value;
                    return true;
                }
                rover = rover.next;
            }
            v = default(TValue);
            return false;
        }

        public bool Remove(TKey k)
        {
            uint key = GetEntryIn(array, k);
            Entry rover = array[key];
            Entry prev = null;
            while (rover != null)
            {
                if (rover.key.Equals(k))
                {
                    if (prev == null)
                        array[key] = rover.next;
                    else
                        prev.next = rover.next;
                    elements--;
                    return true;
                }
                rover = rover.next;
            }
            return false;
        }

        public ICollection<TValue> GetAllValues()
        {
            List<TValue> list = new List<TValue>();
            for (int i = 0; i < array.Length; i++)
            {
                Entry rover = array[i];
                while (rover != null)
                {
                    list.Add(rover.value);
                    rover = rover.next;
                }
            }
            return list;
        }

        public ICollection<TKey> GetAllKeys()
        {
            List<TKey> list = new List<TKey>();
            for (int i = 0; i < array.Length; i++)
            {
                Entry rover = array[i];
                while (rover != null)
                {
                    list.Add(rover.key);
                    rover = rover.next;
                }
            }
            return list;
        }

        public int Count
        {
            get
            {
                return elements;
            }
        }
    }

    internal class SuperHash<T>
    {
        class Entry
        {
            public T value;
            public Entry next;

            public Entry(T v)
            {               
                value = v;
            }

        }

        // table of good has size tables
        static uint[] sizes = {
           89, 
           179, 
           359, 
           719, 
           1439, 
           2879, 
           5759, 
           11519, 
           23039, 
           46079, 
           92159, 
           184319, 
           368639, 
           737279, 
           1474559, 
           2949119, 
           5898239, 
           11796479, 
           23592959, 
           47185919, 
           94371839, 
           188743679, 
           377487359, 
           754974719, 
           1509949439
         };


        int elements = 0; // elements in hash
        int size_table_entry = 0;
        Entry[] array;

        public SuperHash()
        {
            Clear(16);
        }
        public SuperHash(int initialCapacity)
        {
            Clear(initialCapacity);
        }

        private static uint GetEntryIn(Entry[] array, T key)
        {
            return (uint)key.GetHashCode() % (uint)array.Length;
        }
        private static void AddToArrayNoCheck(Entry[] array, Entry e)
        {
            uint key = GetEntryIn(array, e.value);
            e.next = array[key];
            array[key] = e;
        }

        private static bool HasValue(Entry[] array, T k)
        {
            uint key = GetEntryIn(array, k);
            Entry rover = array[key];
            while (rover != null)
            {
                if (rover.value.Equals(k))
                    return true;
                rover = rover.next;
            }
            return false;
        }

        private static void AddToArray(Entry[] array, Entry e)
        {
            uint key = GetEntryIn(array, e.value);
            //T val = e.value;
            // check for existance
            Entry rover = array[key];
            while (rover != null)
            {
                if (rover.value.Equals(e.value))
                    return;

                rover = rover.next;
            }
            e.next = array[key];
            array[key] = e;
        }

        private void MakeLarger()
        {
            size_table_entry++;
            //uint new_size = sizes[size_table_entry];
            Entry[] new_array = new Entry[sizes[size_table_entry]];

            // add all old stuff to the new one

            for (int i = 0; i < array.Length; i++)
            {
                Entry rover = array[i];
                while (rover != null)
                {
                    Entry next = rover.next;
                    AddToArrayNoCheck(new_array, rover);
                    rover = next;
                }
            }
            array = new_array;
        }

        private void MakeSmaller()
        {
            if (size_table_entry == 0) return;
            size_table_entry--;
            //uint new_size = sizes[size_table_entry];
            Entry[] new_array = new Entry[sizes[size_table_entry]];

            // add all old stuff to the new one

            for (int i = 0; i < array.Length; i++)
            {
                Entry rover = array[i];
                while (rover != null)
                {
                    Entry next = rover.next;
                    AddToArrayNoCheck(new_array, rover);
                    rover = next;
                }
            }
            array = new_array;
        }


        public void Add(T k)
        {
            AddToArray(array, new Entry(k));
            elements++;
            if (elements > array.Length * 2)
            {
                MakeLarger();
            }
        }

        public void Clear(int initialCapacity)
        {
            elements = 0;
            size_table_entry = 0;
            for (size_table_entry = 0; sizes[size_table_entry] < initialCapacity; size_table_entry++) ;
            array = new Entry[sizes[size_table_entry]];
        }

   
        public bool Contains(T k)
        {

            uint key = GetEntryIn(array, k);
            Entry rover = array[key];
            while (rover != null)
            {
                if (rover.next == rover)
                {
                    //Console.WriteLine("lsdfjlskfjkl>");
                }
                if (rover.value.Equals(k))
                    return true;
                rover = rover.next;
            }
            return false;
        }




        public bool Remove(T k)
        {
            uint key = GetEntryIn(array, k);
            Entry rover = array[key];
            Entry prev = null;
            while (rover != null)
            {
                if (rover.value.Equals(k))
                {
                    if (prev == null)
                        array[key] = rover.next;
                    else
                        prev.next = rover.next;
                    elements--;
                    return true;
                }
                rover = rover.next;
            }
            return false;
        }

        public ICollection<T> GetAll()
        {
            List<T> list = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                Entry rover = array[i];
                while (rover != null)
                {
                    list.Add(rover.value);
                    rover = rover.next;
                }
            }
            return list;
        }

        public int Count
        {
            get
            {
                return elements;
            }
        }
    }


}
