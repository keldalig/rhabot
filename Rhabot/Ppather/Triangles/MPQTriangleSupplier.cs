/*
 *  Part of PPather
 *  Copyright Pontus Borg 2008
 * 
 */
using System;
using System.Collections.Generic;
using Wmo;

namespace WowTriangles
{
    internal class MPQTriangleSupplier : TriangleSupplier, IDisposable
    {
        string continentFile;

        StormDll.ArchiveSet archive;

        WDT wdt;
        WDTFile wdtf;
        ModelManager modelmanager;
        WMOManager wmomanager;

        Dictionary<String, int> zoneToMapId = new Dictionary<string,int>();
        Dictionary<int, String> areaIdToName = new Dictionary<int, string>();        

        public override void Close()
        {
            archive.Close();
            wdt = null;
            wdtf = null;
            modelmanager = null;
            wmomanager = null;
            zoneToMapId = null;
            areaIdToName = null;
            archive = null;
            base.Close(); 
        }

        public MPQTriangleSupplier()
        {
            List<string> archiveNames = new List<string>() {
                "patch-2.MPQ", 
                "patch.MPQ", 
                "enUS\\patch-enUS-2.MPQ",                
                "enUS\\patch-enUS.MPQ",
                "enGB\\patch-enGB-2.MPQ",
                "enGB\\patch-enGB.MPQ",
                "common.MPQ", 
                "expansion.MPQ", 
		        "enUS\\locale-enUS.MPQ", "enUS\\expansion-locale-enUS.MPQ", 
		        "enGB\\locale-enGB.MPQ", "enGB\\expansion-locale-enGB.MPQ"};
            
            archive = new StormDll.ArchiveSet();
            archive.SetGameDirFromReg(); 

            //Console.WriteLine("Game dir is " + regGameDir);
            archive.AddArchives(archiveNames);
            modelmanager = new ModelManager(archive, 80);
            wmomanager = new WMOManager(archive, modelmanager, 30);
           
            archive.ExtractFile("DBFilesClient\\AreaTable.dbc", "AreaTable.dbc");
            DBC areas = new DBC();
            DBCFile af = new DBCFile("AreaTable.dbc", areas);
            for (int i = 0; i < areas.recordCount; i++)
            {
                int AreaID = (int)areas.GetUint(i, 0);
                int WorldID = (int)areas.GetUint(i, 1);
                int Parent = (int)areas.GetUint(i, 2);
                string Name = areas.GetString(i, 11);

                areaIdToName.Add(AreaID, Name);
                //0 	 uint 	 AreaID
                //1 	uint 	Continent (refers to a WorldID)
                //2 	uint 	Region (refers to an AreaID)
            }

            for (int i = 0; i < areas.recordCount; i++)
            {
                int AreaID = (int)areas.GetUint(i, 0);
                int WorldID = (int)areas.GetUint(i, 1);
                int Parent = (int)areas.GetUint(i, 2);
                string Name = areas.GetString(i, 11);

                string TotalName = "", ParentName = "";
                if (!areaIdToName.TryGetValue(Parent, out ParentName))
                    TotalName = ":" + Name; 
                else
                    TotalName = Name + ":"+ParentName;

                try
                {
                    zoneToMapId.Add(TotalName, WorldID);
                }
                catch
                {
                    int id;
                    zoneToMapId.TryGetValue(TotalName, out id);
                }
                //0 	 uint 	 AreaID
                //1 	uint 	Continent (refers to a WorldID)
                //2 	uint 	Region (refers to an AreaID)
            }
        }

        public void SetContinent(string continent)
        {
            continentFile = continent;
    
            wdt = new WDT();
            wdtf = new WDTFile(archive, continentFile, wdt, wmomanager, modelmanager);
            if (!wdtf.loaded)
                wdt = null; // bad
        }
        
        public string SetZone(string zone)
        {
            int continentID;
            if (!zoneToMapId.TryGetValue(zone, out continentID))
            {
                int colon = zone.IndexOf(":", StringComparison.Ordinal); 
                if(colon == -1)
                    return null;
                zone = zone.Substring(colon);
                if (!zoneToMapId.TryGetValue(zone, out continentID))
                    return null; 
            }

            archive.ExtractFile("DBFilesClient\\Map.dbc", "Map.dbc");
            DBC maps = new DBC();
            new DBCFile("Map.dbc", maps);
            
            for (int i = 0; i < maps.recordCount; i++)
            {
                if (maps.GetInt(i, 0) == continentID) // file == continentFile)
                {
                    SetContinent(maps.GetString(i, 1));
                    return continentFile; 
                }
            }

            return (wdt == null) ? "Failed to open file files for continent ID" + continentID : null;
        }
       
        private void GetChunkData(TriangleCollection triangles, int chunk_x, int chunk_y, SparseMatrix3D<WMO> instances)
        {
            //if (chunk_x < 0) return;
            //if (chunk_y < 0) return;
            //if (chunk_x > 63) return;
            //if (chunk_y > 63) return; 
            //if (triangles == null) return;
            //if (wdtf == null) return;
            //if (wdt == null) return;
            
            // exit if things are null
            if ((chunk_x < 0) || (chunk_y < 0) || (chunk_x > 63) || (chunk_y > 63) ||
                (triangles == null) ||
                (wdtf == null) ||
                (wdt == null))
                    return; 
           
            // load a tile
            wdtf.LoadMapTile(chunk_x, chunk_y);
            
            MapTile t = wdt.maptiles[chunk_x, chunk_y];
            if (t != null)
            {
                // Map tiles                
                for (int ci = 0; ci < 16; ci++)
                {
                    for (int cj = 0; cj < 16; cj++)
                    {
                        MapChunk c = t.chunks[ci, cj];
                        if(c != null)
                            AddTriangles(triangles, c);
                    }
                }
  
                // World objects                
                foreach (WMOInstance wi in t.wmois)
                {
                    if (wi != null && wi.wmo != null)
                    {
                        String fn = wi.wmo.fileName;
                        int last = fn.LastIndexOf('\\');
                        fn = fn.Substring(last + 1);
                        if(! string.IsNullOrEmpty(fn))
                        {
                            //WMO old = instances.Get((int)wi.pos.x, (int)wi.pos.y, (int)wi.pos.z);
                            //if (old == wi.wmo)
                            //{
                            //    ////Console.WriteLine("Already got " + fn);
                            //}
                            //else
                            if (wi.wmo != instances.Get((int)wi.pos.x, (int)wi.pos.y, (int)wi.pos.z))
                            {
                                instances.Set((int)wi.pos.x, (int)wi.pos.y, (int)wi.pos.z, wi.wmo);
                                AddTriangles(triangles, wi);
                            }
                         }
                    }
                }
                
                foreach (ModelInstance mi in t.modelis)
                {
                    if (mi != null && mi.model != null)
                    {
                        //String fn = mi.model.fileName;
                        //int last = fn.LastIndexOf('\\');
                        AddTriangles(triangles, mi);
                    }
                }
            }
            wdt.maptiles[chunk_x, chunk_y] = null; // clear it atain
        }

        private static void GetChunkCoord(float x, float y, out int chunk_x, out int chunk_y)
        {
            // yeah, this is ugly. But safe
            for (chunk_x = 0; chunk_x < 64; chunk_x++)
            {
                float max_y = ChunkReader.ZEROPOINT - (float)(chunk_x) * ChunkReader.TILESIZE;
                float min_y = max_y - ChunkReader.TILESIZE;
                if (y >= min_y- 0.1f && y < max_y + 0.1f) break; 
            }
            for (chunk_y = 0; chunk_y < 64; chunk_y++)
            {
                float max_x = ChunkReader.ZEROPOINT - (float)(chunk_y) * ChunkReader.TILESIZE;
                float min_x = max_x - ChunkReader.TILESIZE;
                if (x >= min_x - 0.1f && x < max_x + 0.1f) break; 
            }
            //if (chunk_y == 64 || chunk_x == 64)
            //{
            //    //Console.WriteLine(x + " " + y + " is at " + chunk_x + " " + chunk_y);
            //    //GetChunkCoord(x, y, out chunk_x, out chunk_y); 
            //}
        }

        public override void GetTriangles(TriangleCollection to, float min_x, float min_y, float max_x, float max_y)
        {
            foreach (WMOInstance wi in wdt.gwmois)
                AddTriangles(to, wi);

            SparseMatrix3D<WMO> instances = new SparseMatrix3D<WMO>();
            for (float x = min_x; x < max_x; x += ChunkReader.TILESIZE)
            {
                for (float y = min_y; y < max_y; y += ChunkReader.TILESIZE)
                {
                    int chunk_x, chunk_y;
                    GetChunkCoord(x, y, out chunk_x, out chunk_y);
                    GetChunkData(to, chunk_x, chunk_y, instances);
                }
            }
        }

        private static void AddTriangles(TriangleCollection s, MapChunk c)
        {
            int[,] vertices = new int[9, 9];
            int[,] verticesMid = new int[8, 8];
            
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                {
                    float x, y, z;
                    ChunkGetCoordForPoint(c, row, col, out x, out y, out z);
                    int index = s.AddVertex(x, y, z);
                    vertices[row, col] = index;
                }

            for (int row = 0; row < 8; row++)
                for (int col = 0; col < 8; col++)
                {
                    float x, y, z;
                    ChunkGetCoordForMiddlePoint(c, row, col, out x, out y, out z);
                    int index = s.AddVertex(x, y, z);
                    verticesMid[row, col] = index;
                }
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (!c.isHole(col, row))
                    {
                        int v0 = vertices[row, col];
                        int v1 = vertices[row + 1, col];
                        int v2 = vertices[row + 1, col + 1];
                        int v3 = vertices[row, col + 1];
                        int vMid = verticesMid[row, col];

                        s.AddTriangle(v0, v1, vMid);
                        s.AddTriangle(v1, v2, vMid);
                        s.AddTriangle(v2, v3, vMid);
                        s.AddTriangle(v3, v0, vMid);
                        
                    }
                }
            }

            if (c.haswater)
            { 
                // paint the water
                for (int row = 0; row < 9; row++)
                    for (int col = 0; col < 9; col++)
                    {
                        float x, y, z;
                        ChunkGetCoordForPoint(c, row, col, out x, out y, out z);
                        float height = c.water_height[row, col]-1.5f;
                        int index = s.AddVertex(x, y,  height);
                        vertices[row, col] = index;
                    }
                for (int row = 0; row < 8; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        if (c.water_flags[row, col] != 0xf)
                        { 
                            int v0 = vertices[row, col];
                            int v1 = vertices[row + 1, col];
                            int v2 = vertices[row + 1, col + 1];
                            int v3 = vertices[row, col + 1];

                            s.AddTriangle(v0, v1, v3, ChunkedTriangleCollection.TriangleFlagDeepWater);
                            s.AddTriangle(v1, v2, v3, ChunkedTriangleCollection.TriangleFlagDeepWater); 
                        }
                    }
                }
            }

        }


        private static void AddTriangles(TriangleCollection s, WMOInstance wi)
        {
            float dx = wi.pos.x;
            float dy = wi.pos.y;
            float dz = wi.pos.z;

            float dir_x = wi.dir.z;
            float dir_y = wi.dir.y-90;
            float dir_z = -wi.dir.x;

            WMO wmo = wi.wmo;
            
            foreach (WMOGroup g in wmo.groups)
            {
                int[] vertices = new int[g.nVertices];

                for (int i = 0; i < g.nVertices; i++)
                {
                    int off = i * 3;

                    float x = g.vertices[off];
                    float y = g.vertices[off + 2];
                    float z = g.vertices[off + 1];

                    rotate(z, y, dir_x, out z, out y);
                    rotate(x, y, dir_z, out x, out y);
                    rotate(x, z, dir_y, out x, out z);

                    
                    //float xx = x + dx;
                    //float yy = y + dy;
                    //float zz = -z + dz;

                    //float finalx = ChunkReader.ZEROPOINT - (-z + dz); // zz;
                    //float finaly = ChunkReader.ZEROPOINT - x + dx; // xx;
                    //float finalz = y + dy; // yy;

                    //vertices[i] = s.AddVertex(finalx, finaly, finalz);
                    vertices[i] = s.AddVertex(
                        ChunkReader.ZEROPOINT - (-z + dz),
                        ChunkReader.ZEROPOINT - x + dx,
                        y + dy);
                }
            }
             
            /*
            int doodadset = wi.doodadset;
            if (doodadset < wmo.nDoodadSets)
            {
                uint firstDoodad = wmo.doodads[doodadset].firstInstance;
                uint nDoodads = wmo.doodads[doodadset].nInstances;

                for (uint i = 0; i < nDoodads; i++)
                {
                    uint d = firstDoodad + i;
                    ModelInstance mi = wmo.doodadInstances[d];
                    if (mi != null)
                    {
                        ////Console.WriteLine("I got model " + mi.model.fileName + " at " + mi.pos);
                        //AddTrianglesGroupDoodads(s, mi, wi.dir, wi.pos, 0.0f); // DOes not work :(
                    }
                }
            }
            */
        }

        private static void AddTriangles(TriangleCollection s, ModelInstance mi)
        {
            
            float dx = mi.pos.x;
            float dy = mi.pos.y;
            float dz = mi.pos.z;

            float dir_x = mi.dir.z;
            float dir_y = mi.dir.y-90;
            float dir_z = -mi.dir.x;
       
            Model m = mi.model;
            if (m == null) return; 

            if (m.boundingTriangles != null)
            {
                // We got boiuding stuff, that is better
                int nBoundingVertices = m.boundingVertices.Length / 3;
                int[] vertices = new int[nBoundingVertices];
                for (uint i = 0; i < nBoundingVertices; i++)
                {
                    uint off = i * 3;
                    float x = m.boundingVertices[off];
                    float y = m.boundingVertices[off + 2];
                    float z = m.boundingVertices[off + 1];


                    rotate(z, y, dir_x, out z, out y);
                    rotate(x, y, dir_z, out x, out y);
                    rotate(x, z, dir_y, out x, out z);
                    
                    
                    x *= mi.sc;
                    y *= mi.sc;
                    z *= mi.sc;

                   // float xx = x + dx;
                   // float yy = y + dy;
                   // float zz = -z + dz;
                   //
                   // float finalx = ChunkReader.ZEROPOINT - zz;
                   // float finaly = ChunkReader.ZEROPOINT - xx;
                   // float finalz = yy;

                    //vertices[i] = s.AddVertex(finalx, finaly, finalz);
                    vertices[i] = s.AddVertex(
                        ChunkReader.ZEROPOINT - (-z + dz),
                        ChunkReader.ZEROPOINT - x + dx, 
                        y + dy);
                }


                int nBoundingTriangles = m.boundingTriangles.Length / 3;
                for (uint i = 0; i < nBoundingTriangles; i++)
                {
                    uint off = i * 3;
                    int v0 = vertices[m.boundingTriangles[off]];
                    int v1 = vertices[m.boundingTriangles[off + 1]];
                    int v2 = vertices[m.boundingTriangles[off + 2]];
                    s.AddTriangle(v0, v1, v2, ChunkedTriangleCollection.TriangleFlagModel);
                }
            }
        }

        static void ChunkGetCoordForPoint(MapChunk c, int row, int col,
                                          out float x, out float y, out float z)
        {
            int off = (row * 17 + col) * 3;
            x = ChunkReader.ZEROPOINT - c.vertices[off + 2];
            y = ChunkReader.ZEROPOINT - c.vertices[off];
            z = c.vertices[off + 1];
        }

        static void ChunkGetCoordForMiddlePoint(MapChunk c, int row, int col,
                                            out float x, out float y, out float z)
        {
            int off = (9 + (row * 17 + col)) * 3;
            x = ChunkReader.ZEROPOINT - c.vertices[off + 2];
            y = ChunkReader.ZEROPOINT - c.vertices[off];
            z = c.vertices[off + 1];
        }

        public static void rotate(float x, float y, float angle, out float nx, out float ny)
        {
            double rot = (angle) / 360.0 * Math.PI * 2;
            float c_y = (float)Math.Cos(rot);
            float s_y = (float)Math.Sin(rot);


            nx = c_y * x - s_y * y;
            ny = s_y * x + c_y * y;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose objects properly
                this.wdtf.Dispose();

                // close this
                this.Close();
            }
        }

        #endregion
    }
}
