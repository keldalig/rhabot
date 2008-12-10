/*
 *  Part of PPather
 *  Copyright Pontus Borg 2008
 * 
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace StormDll
{
    unsafe class StormDll
    {
        /*
         * 
typedef unsigned long       DWORD;
typedef DWORD LCID;  

// Archive opening/closing
LCID  WINAPI SFileSetLocale(LCID lcNewLocale);
LCID  WINAPI SFileGetLocale();
BOOL  WINAPI SFileOpenArchive(const char * szMpqName, DWORD dwPriority, DWORD dwFlags, HANDLE * phMpq);
BOOL  WINAPI SFileCloseArchive(HANDLE hMpq);

// File opening/closing
BOOL  WINAPI SFileOpenFileEx(HANDLE hMpq, const char * szFileName, DWORD dwSearchScope, HANDLE * phFile);
BOOL  WINAPI SFileCloseFile(HANDLE hFile);

// File I/O
DWORD WINAPI SFileGetFilePos(HANDLE hFile, DWORD * pdwFilePosHigh = NULL);
DWORD WINAPI SFileGetFileSize(HANDLE hFile, DWORD * pdwFileSizeHigh = NULL);
DWORD WINAPI SFileSetFilePointer(HANDLE hFile, LONG lFilePos, LONG * pdwFilePosHigh, DWORD dwMethod);
BOOL  WINAPI SFileReadFile(HANDLE hFile, VOID * lpBuffer, DWORD dwToRead, 
                           DWORD * pdwRead = NULL, LPOVERLAPPED lpOverlapped = NULL);

// Adds another listfile into MPQ. The currently added listfile(s) remain,
// so you can use this API to combining more listfiles.
// Note that this function is internally called by SFileFindFirstFile
int   WINAPI SFileAddListFile(HANDLE hMpq, const char * szListFile);

          
         */
        [DllImport("StormLib.dll")]
        public static extern uint SFileGetLocale();

        [DllImport("StormLib.dll")]
        public static extern bool SFileOpenArchive([MarshalAs(UnmanagedType.LPStr)]string szMpqName, 
                              uint dwPriority, uint dwFlags, 
                              void ** phMpq);
        
        [DllImport("StormLib.dll")]
        public static extern bool SFileCloseArchive(void *hMpq);


        [DllImport("StormLib.dll")]
        public static extern bool SFileOpenFileEx(void *hMpq, 
                                [MarshalAs(UnmanagedType.LPStr)] string szFileName, 
                                uint dwSearchScope, 
                                void ** phFile);
        
        [DllImport("StormLib.dll")]
        public static extern bool SFileCloseFile(void * hFile);

        [DllImport("StormLib.dll")]
        public static extern uint SFileGetFilePos(void * hFile, uint * pdwFilePosHigh);
        
        [DllImport("StormLib.dll")]
        public static extern uint SFileGetFileSize(void *  hFile, uint * pdwFileSizeHigh);

        [DllImport("StormLib.dll")]
        public static extern uint SFileSetFilePointer(void *  hFile, 
                    int lFilePos, int * pdwFilePosHigh, uint dwMethod);

        [DllImport("StormLib.dll")]
        public static extern bool SFileReadFile(void *  hFile, void * lpBuffer, uint dwToRead, 
                           uint * pdwRead, void *lpOverlapped);
        
        [DllImport("StormLib.dll")]
        public static extern bool SFileExtractFile(void * hMpq, 
                    [MarshalAs(UnmanagedType.LPStr)] string szToExtract,
                    [MarshalAs(UnmanagedType.LPStr)] string szExtracted);

        [DllImport("StormLib.dll")]
        public static extern bool SFileHasFile(void *hMpq, 
                    [MarshalAs(UnmanagedType.LPStr)] string szFileName);

        public static uint GetLocale()
        {
            return StormDll.SFileGetLocale();
        }
    }
        public unsafe class ArchiveSet : IDisposable
        {
            private List<Archive> archives = new List<Archive>();
            private string GameDir = ".\\";
            public void SetGameDir(string dir)
            {
                GameDir = dir;
            }
            public string SetGameDirFromReg()
            {
                string gameDir = ISXBotHelper.clsSettings.MPQPath;
                SetGameDir(gameDir);
                return gameDir;
//#if DEBUG
//                string s = @"G:\Program Files\World of Warcraft\";
//                SetGameDir(s + "Data\\");
//                return s;
//#else
//                RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Blizzard Entertainment\\World of Warcraft");
//                if (key == null) return null;
//                Object val = key.GetValue("InstallPath");
//                if (val == null) return null;
//                string s = val.ToString();
//                SetGameDir(s+ "Data\\");
//                return s;
//#endif
            }

            public bool AddArchive(string file)
            {
                Archive a = new Archive(Path.Combine(GameDir, file), 0,0);
                if (a.IsOpen())
                {
                    archives.Add(a);
                   //System.Console.WriteLine("Add archive " + file);
                    return true;
                }
                return false; 
            }
            public int AddArchives(List<string> files)
            {
                int n = 0;
                foreach (string s in files)
                {
                    if (AddArchive(s)) n++; 
                }
                return n; 
            }
            public bool HasFile(string name)
            {
                foreach (Archive a in archives)
                {
                    if (a.HasFile(name)) return true;
                }
                return false;
            }

            public bool ExtractFile(string from, string to)
            {
                foreach (Archive a in archives)
                {
                    if (a.HasFile(from))
                        return a.ExtractFile(from, to);
                }
                return false;
            }
            public void Close()
            {
                foreach (Archive a in archives)
                    a.Close();

                archives.Clear(); 
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
                    // close the archives
                    foreach (Archive arc in archives)
                    {
                        if ((arc != null) && (arc.IsOpen()))
                            arc.Close();
                    }
                }
            }

            #endregion
        }

        public unsafe class Archive : IDisposable
        {
            private void* handle = null;

            public Archive(string file, uint Prio, uint Flags)
            {
                Open(file, Prio, Flags);
            }
            public bool IsOpen()
            {
                return handle != null;
            }

            private bool Open(string file, uint Prio, uint Flags)
            {
                void* h; 
                void** hp = &h;
                bool r =  StormDll.SFileOpenArchive(file, Prio, Flags, hp);
                handle = h;
                return r;
            }

            public bool Close()
            {
                bool r = StormDll.SFileCloseArchive(handle);
                if (r) handle = null;
                return r;
            }

            public File OpenFile(string szFileName, uint dwSearchScope)
            {
                void* h;
                void** hp = &h;
                bool r = StormDll.SFileOpenFileEx(handle, szFileName, dwSearchScope, hp);
                return (!r) ? null : new File(this, h);
            }

            public bool HasFile(string name)
            {
                return StormDll.SFileHasFile(handle, name);
            }

            public bool ExtractFile(string from, string to)
            {
                return StormDll.SFileExtractFile(handle, from, to);
            }


            #region IDisposable Members

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                // close the file if it is open
                if ((disposing) && (IsOpen()))
                        Close();
            }

            #endregion
        }

        public unsafe class File : IDisposable
        { 
            void *handle;
            Archive archive;
            public File(Archive a, void *h)
            {
                archive = a;
                handle = h;
            }

            public bool Close()
            {
                bool r = StormDll.SFileCloseFile(handle);
                if (r) handle = null;
                return r;
            }

            public ulong GetSize()
            {
                uint high;
                uint* phigh = &high;
                return StormDll.SFileGetFileSize(handle, phigh);
            }

            #region IDisposable Members

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                // close the file if it is open
                if ((disposing) && (archive != null) && (archive.IsOpen()))
                    archive.Close();
            }


            #endregion
        }
    
}
