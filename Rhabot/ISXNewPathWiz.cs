using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;

namespace Rhabot
{
    public partial class ISXNewPathWiz : Form
    {
        #region Variables

        clsPath cPath = new clsPath();
        private delegate void UpdateListHandler(List<PathListInfo.PathPoint> PathList, PathListInfo.PathPoint PointAdded);
        clsPath.PathListInfoEx pInfo = null;

        /// <summary>
        /// When true, the path is saved to a new file, not a level file
        /// </summary>
        public bool SaveToNewFile = false;

        // Variables
        #endregion

        public ISXNewPathWiz()
        {
            InitializeComponent();
        }

        private void ISXNewPathWiz_Load(object sender, EventArgs e)
        {
            try
            {
                // set start/end levels
                this.txtStartLevel.Value = clsGlobals.SettingsLevel;
                this.txtEndLevel.Value = clsGlobals.SettingsLevel;

                // hide stuff if saving new path
                if (SaveToNewFile)
                {
                    this.txtEndLevel.Visible = false;
                    this.txtStartLevel.Visible = false;
                }

                // pop the group list
                clsGlobals.PopGroupPathList(this.cmbGroupName, !this.txtPathName.Enabled);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.NewPathWizard);
            }
        }

        private void cmbGroupName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // exit if nothing selected
            if (this.cmbGroupName.SelectedIndex < 0)
                return;

            // pop path note
            this.txtPathNote.Text = ((clsPathGroupItem)this.cmbGroupName.SelectedItem).GroupNote;
        }

        #region Record Path

        private void cmdStartRecord_Click(object sender, EventArgs e)
        {
            #region Validation

            // exit if no path name
            if (string.IsNullOrEmpty(this.txtPathName.Text.Trim()))
            {
                MessageBox.Show(Resources.Pleaseenterapathname);
                this.txtPathName.Focus();
                return;
            }

            // exit path levels are wrong
            if (this.txtStartLevel.Value > this.txtEndLevel.Value)
            {
                MessageBox.Show(Resources.Startlevelmustbe);
                return;
            }

            // exit if groupname not selected
            if (string.IsNullOrEmpty(this.cmbGroupName.Text))
            {
                MessageBox.Show(Resources.Pleaseenteragroupname);
                this.cmbGroupName.Focus();
                return;
            }

            // Validation
            #endregion

            try
            {
                // create new path object
                cPath = new clsPath();

                // hook the point added event
                clsEvent.PathPointAdded += clsEvent_PathPointAdded;

                // set path tag
                this.txtPathName.Tag = this.txtPathName.Enabled;

                // disable thebuttons
                this.cmdStartRecord.Enabled = false;
                this.cmdStopRecord.Enabled = true;
                this.cmdPauseRecord.Enabled = true;
                this.txtPathName.Enabled = false;
                this.txtEndLevel.Enabled = false;
                this.txtStartLevel.Enabled = false;
                this.chkMountable.Enabled = false;
                this.cmbGroupName.Enabled = false;

                // start recording the path
                cPath.RecordPath_Start(this.chkMountable.Checked || this.chkPathFly.Checked);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Record path");
            }
        }

        /// <summary>
        /// Raised when a pathpoint is added
        /// </summary>
        /// <param name="PathList"></param>
        /// <param name="PointAdded"></param>
        void clsEvent_PathPointAdded(List<PathListInfo.PathPoint> PathList, PathListInfo.PathPoint PointAdded)
        {
            if (this.lstPathList.InvokeRequired)
                this.lstPathList.Invoke(new UpdateListHandler(this.UpdateList), PathList, PointAdded);
            else
                UpdateList(PathList, PointAdded);
        }

        private void cmdPauseRecord_Click(object sender, EventArgs e)
        {
            if (this.cmdPauseRecord.Text.Contains(Resources.Pause))
                this.cmdPauseRecord.Text = Resources.RestartRecording;
            else
                this.cmdPauseRecord.Text = Resources.PauseRecording;

            // pause/unpause recording
            cPath.RecordPath_Pause();
        }

        private void cmdStopRecord_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // create new pathlist info
                pInfo = new clsPath.PathListInfoEx();

                // stop recording the path
                pInfo.PathList = cPath.RecordPath_Stop();

                // pop the rest of the list info
                pInfo.PathName = this.txtPathName.Text;
                pInfo.CanMount = this.chkMountable.Checked;
                pInfo.CanFly = this.chkPathFly.Checked;

                // save the path
                clsPath.SavePathList(this.txtPathName.Text, this.cmbGroupName.Text, (int)this.txtStartLevel.Value, (int)this.txtEndLevel.Value, pInfo);

                // save the path note
                new rs.clsRS().SavePathGroupNote(
                    clsSettings.LoginInfo.UserID,
                    this.cmbGroupName.Text,
                    clsSerialize.Serialize_ToByteA(this.txtPathNote.Text, typeof(string)),
                    clsSettings.UpdateText,
                    clsSettings.IsDCd);

                // tell user we are done
                MessageBox.Show(Resources.Pathlistsaved);

                // close the form
                this.Close();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Stop Recording");

                // enable the buttons
                this.cmdStartRecord.Enabled = true;
                this.cmdStopRecord.Enabled = false;
                this.cmdPauseRecord.Enabled = false;
                this.chkMountable.Enabled = true;
                this.txtStartLevel.Enabled = true;
                this.txtEndLevel.Enabled = true;
                this.cmbGroupName.Enabled = true;

                if ((bool) this.txtPathName.Tag)
                    this.txtPathName.Enabled = true;
            }

            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void UpdateList(List<PathListInfo.PathPoint> PathList, PathListInfo.PathPoint PointAdded)
        {
            try
            {
                // add to the list
                this.lstPathList.Items.Insert(0, string.Format("Added {0}", PointAdded));
                this.lstPathList.Refresh();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "UpdateList");
            }
        }

        private void chkPathFly_CheckedChanged(object sender, EventArgs e)
        {
            // uncheck mountable if we are flying
            if (this.chkPathFly.Checked)
                this.chkMountable.Checked = false;
        }

        // Record Path
        #endregion
    }
}