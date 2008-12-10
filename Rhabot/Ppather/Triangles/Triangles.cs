/*
 *  Part of PPather
 *  Copyright Pontus Borg 2008
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using Wmo;

namespace WowTriangles
{
    // Fully automatic triangle loader for a MPQ/WoW world


    internal abstract class TriangleSupplier
    {
        public abstract void GetTriangles(TriangleCollection to, float min_x, float min_y, float max_x, float max_y);
        public virtual void Close() { }
    }

    /// <summary>
    /// A chunked collection of triangles
    /// </summary>
    internal class ChunkedTriangleCollection
    {
        public static int TriangleFlagDeepWater = 1;
        //public static int TriangleFlagObject    = 2;
        public static int TriangleFlagModel = 4;

        List<TriangleSupplier> suppliers = new List<TriangleSupplier>();
        SparseMatrix2D<TriangleCollection> chunks;

        List<TriangleCollection> loadedChunks = new List<TriangleCollection>();
        int NOW;
        int maxCached = 1000;

        public ChunkedTriangleCollection()
        {
            chunks = new SparseMatrix2D<TriangleCollection>(8);
        }

        public void SetMaxCached(int maxCached)
        {
            this.maxCached = maxCached;
        }

        private void EvictIfNeeded()
        {
            if (loadedChunks.Count >= maxCached)
            {
                TriangleCollection toEvict = null;
                foreach (TriangleCollection tc in loadedChunks)
                {
                    if (toEvict == null || tc.LRU < toEvict.LRU)
                        toEvict = tc;
                }
                loadedChunks.Remove(toEvict);
            }
        }

        public void AddSupplier(TriangleSupplier supplier)
        {
            suppliers.Add(supplier);
        }

        public static void GetGridStartAt(float x, float y, out int grid_x, out int grid_y)
        {
            x = Wmo.ChunkReader.ZEROPOINT - x;
            grid_x = (int)(x / ChunkReader.TILESIZE);
            y = Wmo.ChunkReader.ZEROPOINT - y;
            grid_y = (int)(y / ChunkReader.TILESIZE);

        }
        private static void GetGridLimits(int grid_x, int grid_y,
                                    out float min_x, out float min_y,
                                    out float max_x, out float max_y)
        {
            max_x = ChunkReader.ZEROPOINT - (float)(grid_x) * ChunkReader.TILESIZE;
            min_x = max_x - ChunkReader.TILESIZE;
            max_y = ChunkReader.ZEROPOINT - (float)(grid_y) * ChunkReader.TILESIZE;
            min_y = max_y - ChunkReader.TILESIZE;
        }

        private void LoadChunkAt(float x, float y)
        {
            int grid_x, grid_y;
            GetGridStartAt(x, y, out grid_x, out grid_y);

            if (chunks.IsSet(grid_x, grid_y)) return;
            EvictIfNeeded();
            TriangleCollection tc = new TriangleCollection();

            float min_x, max_x, min_y, max_y;
            GetGridLimits(grid_x, grid_y, out  min_x, out  min_y, out  max_x, out  max_y);

            tc.SetLimits(min_x - 1, min_y - 1, -1E30f, max_x + 1, max_y + 1, 1E30f);
            foreach (TriangleSupplier s in suppliers)
            {
                s.GetTriangles(tc, min_x, min_y, max_x, max_y);
            }
            tc.CompactVertices();
            tc.ClearVertexMatrix(); // not needed anymore

            loadedChunks.Add(tc);
            chunks.Set(grid_x, grid_y, tc);
        }

        public TriangleCollection GetChunkAt(float x, float y)
        {
            LoadChunkAt(x, y);
            int grid_x, grid_y;
            GetGridStartAt(x, y, out grid_x, out grid_y);
            TriangleCollection tc = chunks.Get(grid_x, grid_y);
            tc.LRU = NOW++;
            return tc;
        }


        public bool IsSpotBlocked(float x, float y, float z,
                                  float toonHeight, float toonSize)
        {
            TriangleCollection tc = GetChunkAt(x, y);

            ICollection<int> ts = null;
            TriangleMatrix tm = tc.GetTriangleMatrix();
            ts = tm.GetAllCloseTo(x, y, toonSize);

            Vector toon;
            toon.x = x;
            toon.y = y;
            toon.z = z + toonHeight - toonSize;

            foreach (int t in ts)
            {
                Vector vertex0;
                Vector vertex1;
                Vector vertex2;
                int flags;
                tc.GetTriangleVertices(t,
                        out vertex0.x, out vertex0.y, out vertex0.z,
                        out vertex1.x, out vertex1.y, out vertex1.z,
                        out vertex2.x, out vertex2.y, out vertex2.z, out flags);
                if (Utils.PointDistanceToTriangle(toon, vertex0, vertex1, vertex2) < toonSize)
                    return true;
            }
            return false;
        }

        public bool IsStepBlocked(float x0, float y0, float z0,
                                  float x1, float y1, float z1,
                                  float toonHeight, float toonSize)
        {
            TriangleCollection tc = GetChunkAt(x0, y0);

            float dx = x0 - x1; float dy = y0 - y1; float dz = z0 - z1;
            float stepLength = (float)Math.Sqrt(dx * dx + dy * dy + dz + dz);
            // 1: check steepness

            // TODO

            // 2: check is there is a big step 

            float mid_x = (x0 + x1) / 2.0f;
            float mid_y = (y0 + y1) / 2.0f;
            float mid_z = (z0 + z1) / 2.0f;
            float mid_z_hit = 0;
            float mid_dz = Math.Abs(stepLength);
            int mid_flags = 0;
            if (FindStandableAt(mid_x, mid_y, mid_z - mid_dz, mid_z + mid_dz, out mid_z_hit, out mid_flags, toonHeight, toonSize))
            {
                float dz0 = Math.Abs(z0 - mid_z_hit);
                float dz1 = Math.Abs(z1 - mid_z_hit);

                // too steep
                if ((dz0 > stepLength / 2.0 && dz0 > 1.0) ||
                    (dz1 > stepLength / 2.0 && dz1 > 1.0))
                        return true; 
            }
            else
            {
                // bad!
                return true;
            }

            ICollection<int> ts = null;
            TriangleMatrix tm = tc.GetTriangleMatrix();
            ts = tm.GetAllInSquare(Utils.min(x0, x1), Utils.min(y0, y1), Utils.max(x0, x1), Utils.max(y0, y1));

            // 3: check collision with objects
            bool coll = false;
            Vector from, from_up;
            Vector to, to_up;
            from.x = x0; from.y = y0; from.z = z0 + toonSize;
            from_up.x = x0; from_up.y = y0; from_up.z = z0 + toonHeight - toonSize;

            to.x = x1; to.y = y1; to.z = z1 + toonSize;
            to_up.x = x1; to_up.y = y1; to_up.z = z1 + toonHeight - toonSize;

            foreach (int t in ts)
            {
                Vector vertex0;
                Vector vertex1;
                Vector vertex2;
                Vector intersect;

                tc.GetTriangleVertices(t,
                        out vertex0.x, out vertex0.y, out vertex0.z,
                        out vertex1.x, out vertex1.y, out vertex1.z,
                        out vertex2.x, out vertex2.y, out vertex2.z);

                if (Utils.SegmentTriangleIntersect(from, to_up, vertex0, vertex1, vertex2, out intersect) ||
                    Utils.SegmentTriangleIntersect(from_up, to, vertex0, vertex1, vertex2, out intersect))
                {
                    return true; // blocked!
                }
            }

            return coll;
        }

        public bool FindStandableAt(float x, float y, float min_z, float max_z,
                                   out float z0, out int flags, float toonHeight, float toonSize)
        {
            TriangleCollection tc = GetChunkAt(x, y);
            ICollection<int> ts = null;
            float minCliffD = 0.5f;

            TriangleMatrix tm = tc.GetTriangleMatrix();
            ts = tm.GetAllCloseTo(x, y, 1.0f);

            Vector s0, s1;
            s0.x = x; s0.y = y; s0.z = min_z;
            s1.x = x; s1.y = y; s1.z = max_z;

            float best_z = -1E30f;
            int best_flags = 0;
            bool found = false;


            foreach (int t in ts)
            {
                Vector vertex0;
                Vector vertex1;
                Vector vertex2;
                Vector intersect;
                int t_flags;

                tc.GetTriangleVertices(t,
                        out vertex0.x, out vertex0.y, out vertex0.z,
                        out vertex1.x, out vertex1.y, out vertex1.z,
                        out vertex2.x, out vertex2.y, out vertex2.z, out t_flags);

                Vector normal;
                Utils.GetTriangleNormal(vertex0, vertex1, vertex2, out normal);
                float angle_z = (float)Math.Sin(45.0 / 360.0 * Math.PI * 2); //
                if (Utils.abs(normal.z) > angle_z)
                {
                    if (Utils.SegmentTriangleIntersect(s0, s1, vertex0, vertex1, vertex2, out intersect))
                    {
                        if (intersect.z > best_z)
                        {
                            if (!IsSpotBlocked(intersect.x, intersect.y, intersect.z, toonHeight, toonSize))
                            {

                                best_z = intersect.z;
                                best_flags = t_flags;
                                found = true;

                            }
                        }
                    }
                }
            }
            if (found)
            {
                Vector up, dn, tmp;
                up.z = best_z + 2;
                dn.z = best_z - 5;
                bool[] nearCliff = { true, true, true, true };

                bool allGood = true;
                foreach (int t in ts)
                {
                    Vector vertex0;
                    Vector vertex1;
                    Vector vertex2;

                    tc.GetTriangleVertices(t,
                            out vertex0.x, out vertex0.y, out vertex0.z,
                            out vertex1.x, out vertex1.y, out vertex1.z,
                            out vertex2.x, out vertex2.y, out vertex2.z);

                    float[] dx = { minCliffD, -minCliffD, 0, 0 };
                    float[] dy = { 0, 0, minCliffD, -minCliffD };
                    // Check if it is close to a "cliff"

                    allGood = true;
                    for (int i = 0; i < 4; i++)
                    {
                        if (nearCliff[i])
                        {
                            up.x = dn.x = x + dx[i];
                            up.y = dn.y = y + dy[i];
                            if (Utils.SegmentTriangleIntersect(up, dn, vertex0, vertex1, vertex2, out tmp))
                                nearCliff[i] = false;
                        }
                        allGood &= !nearCliff[i];
                    }
                    if (allGood)
                        break;
                }

                allGood = true;
                for (int i = 0; i < 4; i++)
                    allGood &= !nearCliff[i];
                if (!allGood)
                {
                    z0 = best_z;
                    flags = best_flags;
                    return false; // too close to cliff

                }

            }
            z0 = best_z;
            flags = best_flags;
            return found;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class TriangleCollection
    {
        public int LRU;

        private class VertexArray : TrioArray<float>
        {
        }

        private class IndexArray : QuadArray<int>
        {
        }


        VertexArray vertices = new VertexArray();
        int no_vertices;

        IndexArray triangles = new IndexArray();
        int no_triangles;

        SparseFloatMatrix3D<int> vertexMatrix = new SparseFloatMatrix3D<int>(0.1f);

        TriangleMatrix collisionMatrix;




        public float max_x = -1E30f;
        public float max_y = -1E30f;
        public float max_z = -1E30f;

        public float min_x = 1E30f;
        public float min_y = 1E30f;
        public float min_z = 1E30f;

        float limit_max_x = 1E30f;
        float limit_max_y = 1E30f;
        float limit_max_z = 1E30f;

        float limit_min_x = -1E30f;
        float limit_min_y = -1E30f;
        float limit_min_z = -1E30f;

        // remove unused vertices
        public void CompactVertices()
        {
            bool[] used_indices = new bool[GetNumberOfVertices()];
            int[] old_to_new = new int[GetNumberOfVertices()];

            // check what vertives are used
            for (int i = 0; i < GetNumberOfTriangles(); i++)
            {
                int v0, v1, v2;
                GetTriangle(i, out v0, out v1, out v2);
                used_indices[v0] = true;
                used_indices[v1] = true;
                used_indices[v2] = true;
            }

            // figure out new indices and move
            int sum = 0;
            for (int i = 0; i < used_indices.Length; i++)
            {
                if (used_indices[i])
                {
                    old_to_new[i] = sum;
                    float x, y, z;
                    vertices.Get(i, out x, out y, out z);
                    vertices.Set(sum, x, y, z);
                    sum++;
                }
                else
                    old_to_new[i] = -1;

            }

            vertices.SetSize(sum);

            // Change all triangles
            for (int i = 0; i < GetNumberOfTriangles(); i++)
            {
                int v0, v1, v2, flags;
                GetTriangle(i, out v0, out v1, out v2, out flags);
                triangles.Set(i, old_to_new[v0], old_to_new[v1], old_to_new[v2], flags);
            }
            no_vertices = sum;
        }

        public TriangleMatrix GetTriangleMatrix()
        {
            if (collisionMatrix == null)
                collisionMatrix = new TriangleMatrix(this);
            return collisionMatrix;
        }

        public void SetLimits(float min_x, float min_y, float min_z,
                              float max_x, float max_y, float max_z)
        {
            limit_max_x = max_x;
            limit_max_y = max_y;
            limit_max_z = max_z;

            limit_min_x = min_x;
            limit_min_y = min_y;
            limit_min_z = min_z;
        }

        public int AddVertex(float x, float y, float z)
        {
            // Create new if needed or return old one
            if (vertexMatrix.IsSet(x, y, z)) 
                return vertexMatrix.Get(x, y, z);

            vertices.Set(no_vertices, x, y, z);
            vertexMatrix.Set(x, y, z, no_vertices);
            return no_vertices++;
        }

        // big list if triangles (3 vertice IDs per triangle)
        public int AddTriangle(int v0, int v1, int v2, int flags)
        {
            // check limits
            if (!CheckVertexLimits(v0) &&
                !CheckVertexLimits(v1) &&
                !CheckVertexLimits(v2)) return -1;
            // Create new
            SetMinMax(v0);
            SetMinMax(v1);
            SetMinMax(v2);

            triangles.Set(no_triangles, v0, v1, v2, flags);
            //changed = true;
            return no_triangles++;
        }

        // big list if triangles (3 vertice IDs per triangle)
        public int AddTriangle(int v0, int v1, int v2)
        {
            return AddTriangle(v0, v1, v2, 0);
        }

        private void SetMinMax(int v)
        {
            float x, y, z;
            GetVertex(v, out x, out y, out z);
            if (x < min_x) min_x = x;
            if (y < min_y) min_y = y;
            if (z < min_z) min_z = z;

            if (x > max_x) max_x = x;
            if (y > max_y) max_y = y;
            if (z > max_z) max_z = z;
        }
        private bool CheckVertexLimits(int v)
        {
            float x, y, z;
            GetVertex(v, out x, out y, out z);
            if (x < limit_min_x || x > limit_max_x) return false;
            if (y < limit_min_y || y > limit_max_y) return false;
            if (z < limit_min_z || z > limit_max_z) return false;

            return true;
        }

        public int GetNumberOfTriangles()
        {
            return no_triangles;
        }

        public int GetNumberOfVertices()
        {
            return no_vertices;
        }

        public void GetVertex(int i, out float x, out float y, out float z)
        {
            vertices.Get(i, out x, out y, out z);
        }

        public void GetTriangle(int i, out int v0, out int v1, out int v2)
        {
            int w;
            triangles.Get(i, out v0, out v1, out v2, out w);
        }
        public void GetTriangle(int i, out int v0, out int v1, out int v2, out int flags)
        {
            triangles.Get(i, out v0, out v1, out v2, out flags);
        }

        public void GetTriangleVertices(int i,
                                        out float x0, out float y0, out float z0,
                                        out float x1, out float y1, out float z1,
                                        out float x2, out float y2, out float z2, out int flags)
        {
            int v0, v1, v2;

            triangles.Get(i, out v0, out v1, out v2, out flags);
            vertices.Get(v0, out x0, out y0, out z0);
            vertices.Get(v1, out x1, out y1, out z1);
            vertices.Get(v2, out x2, out y2, out z2);
        }
        public void GetTriangleVertices(int i,
                                        out float x0, out float y0, out float z0,
                                        out float x1, out float y1, out float z1,
                                        out float x2, out float y2, out float z2)
        {
            int v0, v1, v2, flags;

            triangles.Get(i, out v0, out v1, out v2, out flags);
            vertices.Get(v0, out x0, out y0, out z0);
            vertices.Get(v1, out x1, out y1, out z1);
            vertices.Get(v2, out x2, out y2, out z2);
        }

        public void ClearVertexMatrix()
        {
            vertexMatrix = new SparseFloatMatrix3D<int>(0.1f);
        }
    }


    internal class TriangleMatrix
    {
        float resolution = 2.0f;
        SparseFloatMatrix2D<List<int>> matrix;
        int maxAtOne = 0;

        private void AddTriangleAt(float x, float y, int triangle)
        {
            List<int> l = matrix.Get(x, y);
            if (l == null)
            {
                l = new List<int>(8); // hmm
                l.Add(triangle);

                matrix.Set(x, y, l);
            }
            else
            {
                l.Add(triangle);
            }

            if (l.Count > maxAtOne) maxAtOne = l.Count;
        }

        internal TriangleMatrix(TriangleCollection tc)
        {
            matrix = new SparseFloatMatrix2D<List<int>>(resolution, tc.GetNumberOfTriangles());

            Vector vertex0;
            Vector vertex1;
            Vector vertex2;

            for (int i = 0; i < tc.GetNumberOfTriangles(); i++)
            {
                tc.GetTriangleVertices(i,
                        out vertex0.x, out vertex0.y, out vertex0.z,
                        out vertex1.x, out vertex1.y, out vertex1.z,
                        out vertex2.x, out vertex2.y, out vertex2.z);

                Vector box_center;
                Vector box_halfsize;
                box_halfsize.x = resolution / 2;
                box_halfsize.y = resolution / 2;
                box_halfsize.z = 1E6f;

                //float minx = Utils.min(vertex0.x, vertex1.x, vertex2.x);
                //float maxx = Utils.max(vertex0.x, vertex1.x, vertex2.x);
                //float miny = Utils.min(vertex0.y, vertex1.y, vertex2.y);
                //float maxy = Utils.max(vertex0.y, vertex1.y, vertex2.y);

                //int startx = matrix.LocalToGrid(minx);
                //int endx = matrix.LocalToGrid(maxx);
                //int starty = matrix.LocalToGrid(miny);
                //int endy = matrix.LocalToGrid(maxy);

                int startx = matrix.LocalToGrid(Utils.min(vertex0.x, vertex1.x, vertex2.x));
                int endx = matrix.LocalToGrid(Utils.max(vertex0.x, vertex1.x, vertex2.x));
                int starty = matrix.LocalToGrid(Utils.min(vertex0.y, vertex1.y, vertex2.y));
                int endy = matrix.LocalToGrid(Utils.max(vertex0.y, vertex1.y, vertex2.y));

                for (int x = startx; x <= endx; x++)
                    for (int y = starty; y <= endy; y++)
                    {
                        float grid_x = matrix.GridToLocal(x);
                        float grid_y = matrix.GridToLocal(y);
                        box_center.x = grid_x + resolution / 2;
                        box_center.y = grid_y + resolution / 2;
                        box_center.z = 0;
                        if (Utils.TestTriangleBoxIntersect(vertex0, vertex1, vertex2, box_center, box_halfsize))
                            AddTriangleAt(grid_x, grid_y, i);
                    }

            }
        }

        public Set<int> GetAllCloseTo(float x, float y, float distance)
        {
            List<List<int>> close = matrix.GetAllInSquare(x - distance, y - distance, x + distance, y + distance);
            Set<int> all = new Set<int>();

            foreach (List<int> l in close)
                all.AddRange(l);

            return all;
        }

        public ICollection<int> GetAllInSquare(float x0, float y0, float x1, float y1)
        {
            Set<int> all = new Set<int>();
            List<List<int>> close = matrix.GetAllInSquare(x0, y0, x1, y1);

            foreach (List<int> l in close)
                all.AddRange(l);

            return all;
        }

    }
}