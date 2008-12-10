// http://www.codeproject.com/cs/files/CABCompressExtract.asp

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading;
using ISXWoW;
using LavishVMAPI;
using LavishScriptAPI;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using ISXBotHelper.XProfile;

namespace ISXBotHelper.Explore
{
    public class clsProcessExplore
    {
        #region Variables

        public static Queue<clsExploreNode> ExploreQueue = new Queue<clsExploreNode>();
        private static Thread thread = null;
        private static Thread upThread = null;
        public static bool FileLocked = false;
        public static clsExploreLock LockObj = new clsExploreLock();
        public static bool FileCopying = false;
        private static long nodeNum = 0;
        private static bool IsStopped = false;

        // attribute names
        public static string attr_nodeNum = "nodeNum";
        public static string attr_SubZone = "SubZone";
        public static string attr_X = "X";
        public static string attr_Y = "Y";
        public static string attr_Z = "Z";
        public static string attr_Quest = "Quest";
        public static string attr_Level = "Level";
        public static string attr_Type = "Type";
        
        // Variables
        #endregion

        #region Start / Stop

        internal static void Start()
        {
            IsStopped = false;

            // exit if the thread is already running
            if ((thread != null) && (thread.IsAlive))
                return;

            // sleep 5 seconds, so nodenum can be set
            System.Threading.Thread.Sleep(5000);

            // upload items
            upThread = new Thread(new ThreadStart(Upload_Thread));
            upThread.Name = "Thread_Upload_Explored_Items";
            clsSettings.ThreadList.Add(upThread);
            upThread.Start();

            // process items
            thread = new Thread(new ThreadStart(ProcessExplore_Thread));
            thread.Name = "Thread_Process_Explore_Items";
            clsSettings.ThreadList.Add(thread);
            thread.Start();
        }

        internal static void Stop()
        {
            IsStopped = true;
            clsSettings.KillThread(thread, "Stopping Explore Process Thread");
            clsSettings.KillThread(upThread, "Stopping Explore Upload Thread");
        }

        // Start / Stop
        #endregion

        #region Process Thread

        private static void ProcessExplore_Thread()
        {
            Xml xml = null;
            List<Xml.XAttribute> attributeList = new List<Xml.XAttribute>();
            clsExploreNode node = null;

            try
            {
                clsSettings.Logging.AddToLog("Explore Process Thread Running");

                // continue looping until stop or shutdown
                while (true)
                {
                    // exit if stopped
                    if ((IsStopped) || (!clsSettings.TestPauseStop("Process Explore Thread exiting due to script stop")))
                        return;

                    // copy explore
                    if (! File.Exists(clsSettings.ExploreReadPath))
                    {
                        lock(LockObj)
                        {
                            CopyReadFile();
                        }
                    }

                    if (ExploreQueue.Count > 0)
                    {
                        lock (LockObj)
                        {
                            xml = new Xml(clsSettings.ExplorePath);

                            // buffer
                            using (xml.Buffer(true))
                            {
                                // get node num
                                GetNodeNum(xml);

                                // process any items in the queue
                                while (ExploreQueue.Count > 0)
                                {
                                    try
                                    {
                                        node = null;

                                        // exit if stopped
                                        if ((IsStopped) || (!clsSettings.TestPauseStop("Process Explore Thread exiting due to script stop")))
                                            return;

                                        // get the node
                                        node = ExploreQueue.Dequeue();

                                        // skip if null for some reason
                                        if (node == null)
                                            continue;

                                        // if no zonename, checksubzone
                                        if ((string.IsNullOrEmpty(node.ZoneName)) && (!string.IsNullOrEmpty(node.SubZoneName)))
                                            node.ZoneName = node.SubZoneName;
                                        else if (string.IsNullOrEmpty(node.ZoneName))
                                            continue; // no zone/subzone name

                                        // skip if the node has no guid or guid has a ;
                                        // if guid has ;, it means the object is dead
                                        if (!clsExplore.GuidValid(node.unitInfo.UnitGuid))
                                            continue;

                                        // skip if we have it already and it is not a quest
                                        if ((xml.HasEntry(node.ZoneName, node.unitInfo.UnitGuid)) && (node.unitInfo.HasQuest == false))
                                            continue;

                                        // loop through all items for this zone in the XML file
                                        // skip if anything with the same name/type is within 7
                                        // yards of the new unit
                                        if ((!node.unitInfo.HasQuest) && (ItemsTooClose(node, xml)))
                                            continue;

                                        // attributes
                                        attributeList = new List<Xml.XAttribute>();
                                        attributeList.Add(new Xml.XAttribute(attr_nodeNum, nodeNum.ToString().Trim()));
                                        attributeList.Add(new Xml.XAttribute(attr_SubZone, node.SubZoneName));
                                        attributeList.Add(new Xml.XAttribute(attr_X, node.ExplorePoint.X.ToString().Trim()));
                                        attributeList.Add(new Xml.XAttribute(attr_Y, node.ExplorePoint.Y.ToString().Trim()));
                                        attributeList.Add(new Xml.XAttribute(attr_Z, node.ExplorePoint.Z.ToString().Trim()));
                                        attributeList.Add(new Xml.XAttribute(attr_Quest, node.unitInfo.HasQuest.ToString()));
                                        attributeList.Add(new Xml.XAttribute(attr_Level, node.unitInfo.UnitLevel.ToString().Trim()));
                                        attributeList.Add(new Xml.XAttribute(attr_Type, node.unitInfo.UnitTypeString(node.unitInfo.UnitType)));

                                        // save to xml
                                        //clsSettings.Logging.DebugWrite(string.Format("ProcessExplore: Adding Item '{0}'", node.unitInfo.UnitName));
                                        xml.SetValue(node.ZoneName, node.unitInfo.UnitGuid, node.unitInfo.UnitName, attributeList);
                                    }

                                    catch (Exception excep)
                                    {
                                        clsError.ShowError(excep, "Process Explore Items");

                                        // try to delete the xml file
                                        xml = null;
                                        try
                                        {
                                            File.Delete(clsSettings.ExplorePath);
                                        }
                                        finally
                                        {
                                            // unlock the file
                                            FileLocked = false;
                                        }
                                        return;
                                    }
                                }
                            }

                            // release file lock
                            xml = null;
                            FileLocked = false;
                        }
                    }

                    // sleep for 2 seconds
                    System.Threading.Thread.Sleep(2000);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Process Explore - Exiting due to error");
            }            

            finally
            {
                // release file lock
                xml = null;
                FileLocked = false;
            }
        }

        /// <summary>
        /// Checks if existing saved items with the same name are too close (within 7 yards). Returns
        /// true if something is too close
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="xml">The XML file.</param>
        private static bool ItemsTooClose(clsExploreNode node, Xml xml)
        {
            bool rVal = false;
            string[] entries = null;

            try
            {
                // get the list of entries
                entries = xml.GetEntryNames(node.ZoneName);

                // exit if no entries
                if ((entries == null) || (entries.Length == 0))
                    return false;

                // loop through and see if anything is too close
                foreach (string item in entries)
                {
                    // skip if item name's dont' match
                    if (clsSettings.XMLGet_String(node.ZoneName, item, xml) != node.unitInfo.UnitName)
                        continue;

                    // if the point is within 10 yards, return true;
                    if (node.ExplorePoint.Distance(new clsPath.PathPoint(
                        Convert.ToDouble(xml.GetValue_Attribute(node.ZoneName, item, attr_X)),
                        Convert.ToDouble(xml.GetValue_Attribute(node.ZoneName, item, attr_Y)),
                        Convert.ToDouble(xml.GetValue_Attribute(node.ZoneName, item, attr_Z)))) <= 10)
                            return true;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Explore - ItemsTooClose");
            }

            return rVal;
        }

        // Process Thread
        #endregion

        #region Upload Thread

        /// <summary>
        /// This thread runs once every 30 minutes. It uploads the latest changes in the
        /// explore file
        /// </summary>
        private static void Upload_Thread()
        {
            Xml xml = null;
            Xml upXML = null;
            string tempStr = "";
            string[] sections = null, entries = null;
            List<Xml.XAttribute> attributeList = null;
            string UpCab = "";
            DateTime LastSleep = DateTime.Now;

            clsSettings.Logging.AddToLog("Explore Upload Thread Running");

            while (true)
            {
                try
                {
//                    // wait until file unlocked
//                    while (FileLocked)
//                        System.Threading.Thread.Sleep(500);
//
//                    // lock the file
//                    FileLocked = true;

                    // exit if stopped or shutdown
                    if ((IsStopped) || (clsSettings.Stop) || (clsSettings.IsShuttingDown))
                    {
                        clsSettings.Logging.AddToLog("Explore Upload Thread exiting because script stopped or shutdown");
                        return;
                    }

                    lock (LockObj)
                    {
                        CopyReadFile();

                        // open the xml files
                        xml = new Xml(clsSettings.ExplorePath);

                        // keep looping until we have a valid filename
                        while (true)
                        {
                            try
                            {
                                // delete the upXML if it exists
                                if (File.Exists(clsSettings.ExploreUploadFile))
                                    File.Delete(clsSettings.ExploreUploadFile);

                                // create the upload xml file. if valid filename, then exit the loop
                                upXML = new Xml(clsSettings.ExploreUploadFile);
                                break;
                            }

                            catch
                            {
                                // invalid guid, get another one
                                clsSettings.Logging.AddToLogFormatted("ProcessExplore: Upload XML Filename is invalid. Trying another name. '{0}'", clsSettings.ExploreUploadFile);
                                clsSettings.ExploreUploadFile = "";
                            }
                        }

                        // buffer xml
                        using (xml.Buffer(true))
                        {
                            // get the nodenum from the file
                            GetNodeNum(xml);

                            using (upXML.Buffer(true))
                            {
                                // get all section names
                                sections = xml.GetSectionNames();

                                // loop through all sections
                                foreach (string section in sections)
                                {
                                    // get all entries in this section
                                    entries = xml.GetEntryNames(section);

                                    // loop through all entries, only use those with the current nodeNum
                                    foreach (string entry in entries)
                                    {
                                        // skip if not our node
                                        if (Convert.ToInt64(xml.GetValue_Attribute(section, entry, attr_nodeNum)) != nodeNum)
                                            continue;

                                        // this is our node, so let's save it to the upXML
                                        attributeList = new List<Xml.XAttribute>();

                                        // SubZone
                                        tempStr = xml.GetValue_Attribute(section, entry, attr_SubZone);
                                        attributeList.Add(new Xml.XAttribute(attr_SubZone, tempStr));

                                        // X, Y, Z
                                        tempStr = xml.GetValue_Attribute(section, entry, attr_X);
                                        attributeList.Add(new Xml.XAttribute(attr_X, tempStr));
                                        tempStr = xml.GetValue_Attribute(section, entry, attr_Y);
                                        attributeList.Add(new Xml.XAttribute(attr_Y, tempStr));
                                        tempStr = xml.GetValue_Attribute(section, entry, attr_Z);
                                        attributeList.Add(new Xml.XAttribute(attr_Z, tempStr));

                                        // Quest
                                        tempStr = xml.GetValue_Attribute(section, entry, attr_Quest);
                                        attributeList.Add(new Xml.XAttribute(attr_Quest, tempStr));

                                        // Level
                                        tempStr = xml.GetValue_Attribute(section, entry, attr_Level);
                                        attributeList.Add(new Xml.XAttribute(attr_Level, tempStr));

                                        // Type
                                        tempStr = xml.GetValue_Attribute(section, entry, attr_Type);
                                        attributeList.Add(new Xml.XAttribute(attr_Type, tempStr));

                                        // save to upxml
                                        tempStr = (string)xml.GetValue(section, entry);
                                        upXML.SetValue(section, entry, tempStr, attributeList);
                                    }
                                }
                            }

                            // increment nodenum
                            nodeNum++;
                            xml.SetValue("Settings", attr_nodeNum, nodeNum.ToString().Trim());
                        }

                        // close the xml files
                        xml = null;
                        upXML = null;

                        // unlock the xml file
                        FileLocked = false;

                        // only upload if the upxml file was created
                        if (File.Exists(clsSettings.ExploreUploadFile))
                        {
                            // get the upload cab filename
                            UpCab = Path.Combine(Path.GetDirectoryName(clsSettings.ExploreUploadFile), "upload.cab");

                            // delete if the file already exists
                            if (File.Exists(UpCab))
                                File.Delete(UpCab);

                            // compress the upxml file
                            File.WriteAllBytes(UpCab, Compressor.Compress(File.ReadAllBytes(clsSettings.ExploreUploadFile)));

                            // get the file bytes
                            byte[] upFile = File.ReadAllBytes(UpCab);

                            // log it
                            clsSettings.Logging.AddToLog("ExploreUpload: Uploading explore file");

                            // we can now upload the file to the webservice
                            //new Rhabot.RhabotService.Service().UploadExplore(upFile, new MerlinEncrypt.Crypt().EncryptString("wackywacky"));
                            new rs.clsRS().UploadExploreFile(upFile, new MerlinEncrypt.Crypt().EncryptString("wackywacky"));

                            // delete the upXML if it exists
                            if (File.Exists(clsSettings.ExploreUploadFile))
                                File.Delete(clsSettings.ExploreUploadFile);
                        }
                    }

                    // sleep for thirty minutes
                    LastSleep = DateTime.Now.AddMinutes(30);
                    while (DateTime.Now < LastSleep)
                    {
                        // exit if stopped or shutdown
                        if ((IsStopped) || (clsSettings.Stop) || (clsSettings.IsShuttingDown))
                        {
                            clsSettings.Logging.AddToLog("Explore Upload Thread exiting because script stopped or shutdown");
                            return;
                        }

                        // sleep 2 seconds
                        System.Threading.Thread.Sleep(2000);
                    }
                }

                catch (Exception excep)
                {
                    clsError.ShowError(excep, "Upload Explore File Thread");
                }

                finally
                {
                    // release file lock
                    xml = null;
                    upXML = null;
                    FileLocked = false;
                }
            }
        }

        private static void CopyReadFile()
        {
            // copy the xml file
            FileCopying = true;
            try
            {
                File.Copy(clsSettings.ExplorePath, clsSettings.ExploreReadPath, true);
            }
            catch { }
            finally
            {
                FileCopying = false;
            }
        }

        private static void GetNodeNum(Xml xml)
        {
            string section = "Settings";
            
            // get node num
            if (nodeNum <= 0)
            {
                if (xml.HasEntry(section, attr_nodeNum))
                    nodeNum = clsSettings.XMLGet_Long(section, attr_nodeNum, xml);
                else // doesn't exist, so create one
                    nodeNum = 0;

                nodeNum++;
                xml.SetValue(section, attr_nodeNum, nodeNum);
            }
        }

        // Upload Thread
        #endregion

        #region Download File

        // using System.Net;
        // using System.IO;
        /// <summary>
        /// Downloads a file from the internet. Returns number of bytes downloaded
        /// </summary>
        /// <param name="remoteFilename">the full url for the file to download</param>
        /// <param name="localFilename">full path and filename where to save the file</param>
        /// <returns></returns>
        private static int DownloadFile(String remoteFilename, String localFilename)
        {
            // Function will return the number of bytes processed
            // to the caller. Initialize to 0 here.
            int bytesProcessed = 0;

            // Assign values to these objects here so that they can
            // be referenced in the finally block
            Stream remoteStream = null;
            Stream localStream = null;
            WebResponse response = null;

            // Use a try/catch/finally block as both the WebRequest and Stream
            // classes throw exceptions upon error
            try
            {
                // delete if the file exists
                if (File.Exists(localFilename))
                    File.Delete(localFilename);

                // Create a request for the specified remote file name
                WebRequest request = WebRequest.Create(remoteFilename);
                if (request != null)
                {
                    // Send the request to the server and retrieve the
                    // WebResponse object
                    response = request.GetResponse();
                    if (response != null)
                    {
                        // Once the WebResponse object has been retrieved,
                        // get the stream object associated with the response's data
                        remoteStream = response.GetResponseStream();

                        // Create the local file
                        localStream = File.Create(localFilename);

                        // Allocate a 2k buffer
                        byte[] buffer = new byte[2048];
                        int bytesRead;

                        // Simple do/while loop to read from stream until
                        // no bytes are returned
                        do
                        {
                            // Read data (up to 1k) from the stream
                            bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

                            // Write the data to the local file
                            localStream.Write(buffer, 0, bytesRead);

                            // Increment total bytes processed
                            bytesProcessed += bytesRead;
                        } while (bytesRead > 0);
                    }
                }
            }
            catch (Exception e)
            {
                clsError.ShowError(e, "Downloading New Explore Map");
            }
            finally
            {
                // Close the response and streams objects here
                // to make sure they're closed even if an exception
                // is thrown at some point
                if (response != null) response.Close();
                if (remoteStream != null) remoteStream.Close();
                if (localStream != null) localStream.Close();
            }

            // Return total bytes processed to caller.
            return bytesProcessed;
        }
        
        // Download File
        #endregion

        #region Lock Class

        public class clsExploreLock
        {
            public bool IsLocked = false;
        }

        // Lock Class
        #endregion

        #region Download New Explore File

        /// <summary>
        /// Downloads the explore map file
        /// </summary>
        internal static void DownloadExploreFile()
        {
            int version = -1;
            string dURL = "";
            bool needsDownload = false;
            Xml xml = null;
            string section = "Settings";

            try
            {
                lock (LockObj)
                {
                    FileLocked = true;

                    // delete old explore files
                    try
                    {
                        // get the list of old files
                        string[] oldFiles = Directory.GetFiles(Path.GetDirectoryName(clsSettings.OrigExplorePath), "explore_*.xml", SearchOption.TopDirectoryOnly);

                        // if we have files, delete them
                        if ((oldFiles != null) && (oldFiles.Length > 0))
                        {
                            foreach (string dFile in oldFiles)
                                File.Delete(dFile);
                        }
                    }

                    catch (Exception iexcep)
                    {
                        clsError.ShowError(iexcep, "Delete Old Explore Files", true, new System.Diagnostics.StackFrame(0, true), false);
                    }                    

                    // if file does not exist, we need to download
                    if (!File.Exists(clsSettings.OrigExplorePath))
                        needsDownload = true;

                    // get the version if we have the file
                    if (!needsDownload)
                    {
                        // open the xml file to get the version information
                        try
                        {
                            // open the xml file
                            xml = new Xml(clsSettings.OrigExplorePath);
                            using (xml.Buffer(true))
                            {
                                if (!xml.HasEntry(section, "version"))
                                    needsDownload = true;

                                if (!needsDownload)
                                    version = clsSettings.XMLGet_Int(section, "version", xml);

                                // see if the version's match
                                if ((!needsDownload) && (!string.IsNullOrEmpty(new rs.clsRS().GetExploreURL(version))))
                                    needsDownload = true;
                            }
                        }
                        catch (Exception excep)
                        {
                            clsError.ShowError(excep, "Get Explore Version");
                            needsDownload = true;
                        }
                    }

                    if (needsDownload)
                    {
                        // if we need to download, then download get it's download path
                        dURL = new rs.clsRS().GetExploreURL(version);

                        // if we need a download and we have a url, download
                        if (!string.IsNullOrEmpty(dURL))
                        {
                            DownloadFile(dURL, clsSettings.OrigExplorePath);

                            // reset nodenum 
                            nodeNum = 0;

                            // get node num
                            xml = new Xml(clsSettings.OrigExplorePath);
                            GetNodeNum(xml);
                            nodeNum++;
                        }
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Download Explore File");
            }

            finally
            {
                try
                {
                    // delete old explore read
                    if (File.Exists(clsSettings.ExploreReadPath))
                        File.Delete(clsSettings.ExploreReadPath);

                    // copy explore
                    CopyReadFile();
                }

                catch (Exception fexcep)
                {
                    clsError.ShowError(fexcep, "Create Explore Read File", true, new System.Diagnostics.StackFrame(0, true), false);
                }

                // unlock the file
                FileLocked = false;
            }
        }

        // Download New Explore File
        #endregion
    }
}
