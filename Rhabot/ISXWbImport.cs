using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Rhabot
{
    public partial class ISXWbImport : Form
    {
        #region Properties

        /// <summary>
        /// Set to true if importing wowbot
        /// set to false if importing glider paths
        /// </summary>
        public bool IsWoWBot { get; set; }

        // Properties
        #endregion

        #region Init

        public ISXWbImport()
        {
            InitializeComponent();
        }

        private void ISXWbImport_Load(object sender, EventArgs e)
        {
            // set start/end levels
            this.txtStartLevel.Value = clsGlobals.SettingsLevel;
            this.txtEndLevel.Value = clsGlobals.SettingsLevel;

            // pop the group list
            clsGlobals.PopGroupPathList(this.cmbGroupName, true);

            // set title
            if (IsWoWBot)
                this.Text = "WoWBot Path Importer";
            else
                this.Text = "Glider Path Importer";
        }

        // Init
        #endregion

        #region Button_Clicks

        /// <summary>
        /// Path group changed
        /// </summary>
        private void cmbGroupName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // exit if nothing selected
            if (this.cmbGroupName.SelectedIndex < 0)
                return;

            // show path note
            this.txtPathNote.Text = ((clsPathGroupItem)this.cmbGroupName.SelectedItem).GroupNote;
        }

        /// <summary>
        /// Browse for the file
        /// </summary>
        private void cmdBrowse_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            this.txtFilename.Text = this.openFileDialog1.FileName;
        }

        /// <summary>
        /// Do the import
        /// </summary>
        private void cmdImport_Click(object sender, EventArgs e)
        {
            try
            {
                // exit if no file
                string filename = this.txtFilename.Text.Trim();
                if ((string.IsNullOrEmpty(filename)) || (!File.Exists(filename)))
                {
                    clsError.ShowError(new Exception("Can not start Path Importer. The file name does not exist"), string.Empty, true, new StackFrame(0, true), false);
                    return;
                }

                // exit if no groupname
                if (string.IsNullOrEmpty(this.cmbGroupName.Text))
                {
                    clsError.ShowError(new Exception("Can not start Path Importer. Please enter a group name"), string.Empty, true, new StackFrame(0, true), false);
                    this.cmbGroupName.Focus();
                    return;
                }

                this.Cursor = Cursors.WaitCursor;

                if (IsWoWBot)
                    ImportWoWBotPath(filename);
                else
                    ImportGliderPath(filename);

                // save path note
                new rs.clsRS().SavePathGroupNote(
                    clsSettings.LoginInfo.UserID,
                    this.cmbGroupName.Text,
                    clsSerialize.Serialize_ToByteA(this.txtPathNote.Text, typeof(string)),
                    clsSettings.UpdateText,
                    clsSettings.IsDCd);

                // clear the form
                this.cmbGroupName.SelectedIndex = -1;
                this.txtFilename.Text = "";
                this.txtPathNote.Text = "";
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Import Path");
            }

            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // Button_Clicks
        #endregion

        #region WoWBot

        /// <summary>
        /// Imports the path.
        /// </summary>
        /// <param name="filename">The filename.</param>
        private void ImportWoWBotPath(string filename)
        {
            List<PathListInfo.PathPoint> AdvanceList, PatrolList, MerchantList, CorpseRunList, CorpseRun2List, SafePointList;
            PathListInfo.PathPoint SafePoint = null;
            List<string> FileLines = null;
            int j;

            try
            {
                // exit if file doesn't exist
                if (!File.Exists(filename))
                {
                    MessageBox.Show(string.Format("The file '{0}' does not exist. Please try a different file", filename));
                    return;
                }

                // read the file
                FileLines = new List<string>(File.ReadAllLines(filename));

                // upcase all the lines
                j = FileLines.Count;
                for (int i = 0; i < j; i++)
                    FileLines[i] = FileLines[i].ToUpper();

                // build each list
                SafePointList = ParsePath(FileLines, "Safe_Point");
                AdvanceList = ParsePath(FileLines, "Advance");
                PatrolList = ParsePath(FileLines, "Patrol");
                MerchantList = ParsePath(FileLines, "MerchantRun");
                CorpseRunList = ParsePath(FileLines, "CorpseRun");
                CorpseRun2List = ParsePath(FileLines, "CorpseRun2");

                // add safe point the lists
                if ((SafePointList != null) && (SafePointList.Count > 0))
                {
                    // get the point
                    SafePoint = SafePointList[0];

                    // insert into the other lists
                    AddSafePoint(AdvanceList, SafePoint);
                    AddSafePoint(MerchantList, SafePoint);
                    AddSafePoint(CorpseRunList, SafePoint);
                    AddSafePoint(CorpseRun2List, SafePoint);
                }

                // remove any points that are too close
                CleanupList(AdvanceList);
                CleanupList(PatrolList);
                CleanupList(MerchantList);
                CleanupList(CorpseRunList);
                CleanupList(CorpseRun2List);

                // save each list
                SaveList(clsGlobals.Path_StartHunt, AdvanceList);
                SaveList(clsGlobals.Path_Hunting, PatrolList);
                SaveList(clsGlobals.Path_Vendor, MerchantList);
                SaveList(clsGlobals.Path_Graveyard1, CorpseRunList);
                SaveList(clsGlobals.Path_Graveyard2, CorpseRun2List);

                // tell user we are done
                this.txtFilename.Text = string.Empty;
                this.txtFilename.Focus();
                MessageBox.Show("Path Imported Successfully");
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.WoWBotPathImport, "ImportPath");
            }
        }

        /// <summary>
        /// Reads fileLines for all references to pathName
        /// </summary>
        /// <param name="fileLines">the xml file to read through</param>
        private List<PathListInfo.PathPoint> ParsePath(List<string> fileLines, string pathName)
        {
            int Counter = 1, ListCounter, i, j = fileLines.Count, gtIndex, ltIndex;
            string PointName = string.Empty, PointVal = string.Empty, tempStr = string.Empty;
            bool SafePoint = false;
            List<PathListInfo.PathPoint> retList = new List<PathListInfo.PathPoint>();
            double x = 0, y = 0, z = 0;

            try
            {
                // get if safepoint
                SafePoint = string.Compare(pathName, "Safe_Point", true) == 0;

                // loop until no more points
                while (true)
                {
                    // build the point name
                    if (SafePoint)
                        PointName = "<Point Name=\"Safe_Point\">".ToUpper();
                    else
                        PointName = string.Format("<Point Name=\"{0}_{1}\">", pathName, Convert.ToString(Counter++).Trim()).ToUpper();

                    // search the list for this name
                    ListCounter = -1;
                    for (i = 0; i < j; i++)
                    {
                        // if we find a match, set listcounter and break
                        if (fileLines[i].Contains(PointName))
                        {
                            ListCounter = i;
                            break;
                        }
                    }

                    // if nothing found, then exit
                    if (ListCounter < 0)
                        return retList;

                    // found a point, get x, y, z
                    for (i = 0; i < 3; i++)
                    {
                        // get the next line
                        ListCounter++;
                        PointVal = fileLines[ListCounter];

                        // find the tags
                        gtIndex = PointVal.IndexOf(">") + 1;
                        ltIndex = PointVal.LastIndexOf("<");

                        // get the value
                        tempStr = PointVal.Substring(gtIndex, ltIndex - gtIndex);

                        // handle accordingly
                        if (PointVal.Contains("<X>"))
                            x = Convert.ToDouble(tempStr);
                        else if (PointVal.Contains("<Y>"))
                            y = Convert.ToDouble(tempStr);
                        else if (PointVal.Contains("<Z>"))
                            z = Convert.ToDouble(tempStr);
                    }

                    // create a new point and add to the list
                    retList.Add(new PathListInfo.PathPoint(x, y, z));

                    // exit if safe point
                    if (SafePoint)
                        break;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.WoWBotPathImport, "ParsePath");
            }

            return retList;
        }

        /// <summary>
        /// Adds the safe point to the proper place in the list
        /// </summary>
        /// <param name="pointList">the list of points to update</param>
        /// <param name="SafePoint">the safe point</param>
        private void AddSafePoint(List<PathListInfo.PathPoint> pointList, PathListInfo.PathPoint SafePoint)
        {
            if ((pointList != null) && (pointList.Count > 0))
            {
                if (SafePoint.Distance(pointList[0]) < SafePoint.Distance(pointList[pointList.Count - 1]))
                    pointList.Insert(0, SafePoint);
                else
                    pointList.Add(SafePoint);
            }
        }

        // WoWBot
        #endregion

        #region Helpers

        /// <summary>
        /// Removes any points that are too close together
        /// </summary>
        /// <param name="CurrentList">the list to update</param>
        private void CleanupList(List<PathListInfo.PathPoint> CurrentList)
        {
            int j = CurrentList.Count - 1, i = 0, removeAt = -1;
            bool doLoop = true;

            // loop through and remove any points that are too close
            while (doLoop)
            {
                // reset variables
                doLoop = false;
                j = CurrentList.Count - 1;

                // loop through and find any points that are too close
                for (i = 0; i < j; i++)
                {
                    // if too close to next point, remove this point
                    if (CurrentList[i].Distance(CurrentList[i + 1]) < clsSettings.SavePathPrecision)
                    {
                        removeAt = i + 1;
                        doLoop = true;
                        break;
                    }
                }

                // remove the item if we have one
                if (doLoop)
                    CurrentList.RemoveAt(removeAt);
            }
        }

        /// <summary>
        /// Saves the list.
        /// </summary>
        /// <param name="pathname">The pathname.</param>
        /// <param name="PointList">The point list.</param>
        private void SaveList(string pathname, List<PathListInfo.PathPoint> PointList)
        {
            clsPath.PathListInfoEx pInfo = new clsPath.PathListInfoEx();

            try
            {
                // exit if no list
                if ((PointList == null) || (PointList.Count == 0))
                    return;

                // name the list
                pInfo.PathName = pathname;

                // pop the point list
                pInfo.PathList = PointList;

                // save the path
                clsPath.SavePathList(pathname, this.cmbGroupName.Text, (int)this.txtStartLevel.Value, (int)this.txtEndLevel.Value, pInfo);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.WoWBotPathImport, "Save Point List", true, new StackFrame(0, true), false);
            }
        }

        // Helpers
        #endregion

        #region GliderPath

        /// <summary>
        /// Import a glider path
        /// </summary>
        private void ImportGliderPath(string PathFilename)
        {
            try
            {
                ParseGliderPath(PathFilename, "Waypoint", clsGlobals.Path_Hunting);
                ParseGliderPath(PathFilename, "GhostWaypoint", clsGlobals.Path_Graveyard1);
                ParseGliderPath(PathFilename, "VendorWaypoint", clsGlobals.Path_Vendor);

                MessageBox.Show("Path Imported Successfully");
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "ImportGliderPath");
            }            
        }

        /// <summary>
        /// Parses and saves one glider path
        /// </summary>
        /// <param name="PathFilename">the glider path file</param>
        /// <param name="ElementName">the element name to read from teh xml file</param>
        /// <param name="PathName">the name to save as on the Rhabot server</param>
        private void ParseGliderPath(string PathFilename, string ElementName, string PathName)
        {
            // get the path
            List<PathListInfo.PathPoint> pList = new List<PathListInfo.PathPoint>();

            // get all the waypoints
            var waypoints = from x in XDocument.Load(PathFilename, LoadOptions.PreserveWhitespace).FirstNode.ElementsAfterSelf("GlideProfile").Elements("Waypoint")
                            select x;

            // pop path
            foreach (var waypoint in waypoints)
            {
                // get x and y
                string[] wVal = Regex.Split(waypoint.Value, " ");

                // add to the path
                // NOTE: Z will always be 1, because Glider does not save a Z
                if (wVal.Length == 2)
                    pList.Add(new PathListInfo.PathPoint(Convert.ToDouble(wVal[0]), Convert.ToDouble(wVal[1]), 1));
            }

            // clean the list
            if ((pList != null) && (pList.Count > 0))
                CleanupList(pList);

            // save it
            if ((pList != null) && (pList.Count > 0))
                SaveList(PathName, pList);
        }

        // GliderPath
        #endregion
    }
}