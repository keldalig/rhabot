using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using rs.SharingService;

namespace ISXBotHelper.Controls
{
    public partial class uscSharePaths : UserControl
    {
        #region Init

        public uscSharePaths()
        {
            InitializeComponent();
        }

        private void uscSharePaths_Load(object sender, EventArgs e)
        {
            try
            {
                this.grdSharedPaths.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
                this.grdUserPaths.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);

                // set cursor
                this.Cursor = Cursors.WaitCursor;

                // pop the user's profiles
                PopUserPaths();

                // pop the shared profiles
                PopSharedPaths();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Load Shared Paths");
            }            

            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // Init
        #endregion

        #region ButtonClicks

        #region UserProfiles

        /// <summary>
        /// Update the user's profiles' sharing flags
        /// </summary>
        private void cmdShareUserPath_Click(object sender, EventArgs e)
        {
            rs.clsRS crs = new rs.clsRS();

            try
            {
                // set cursor
                this.Cursor = Cursors.Default;

                // loop through each row and update the item
                foreach (DataGridViewRow row in this.grdUserPaths.Rows)
                {
                    // skip if not a row
                    if (row.Tag == null)
                        continue;

                    // get the profile object
                    clsSharedPathInfo PathInfo = row.Tag as clsSharedPathInfo;

                    // update shared flag
                    crs.UpdatePathSharedFlag(
                        clsSettings.LoginInfo.UserID,
                        PathInfo.GroupName,
                        Convert.ToBoolean(row.Cells["clmIsShared"].Value),
                        clsSettings.UpdateText,
                        clsSettings.IsDCd);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Update Shared User Paths");
            }
            
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void cmdRefreshProfile_Click(object sender, EventArgs e)
        {
            // pop the user's profiles
            PopUserPaths();
        }

        /// <summary>
        /// Show the profile note
        /// </summary>
        private void grdUserPaths_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                // exit if nothing selected
                if ((this.grdUserPaths.SelectedRows.Count == 0) ||
                    (this.grdUserPaths.SelectedRows[0].Tag == null))
                    return;

                // get the tag
                clsSharedPathInfo PathInfo = this.grdUserPaths.SelectedRows[0].Tag as clsSharedPathInfo;

                // set the note
                this.txtYourPathNote.Text = clsSerialize.Deserialize_FromByteA(PathInfo.GroupNote, typeof(string)) as string;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "User Path Select");
            }            
        }

        // UserProfiles
        #endregion

        #region SharedProfiles

        /// <summary>
        /// Clone the selected profiles
        /// </summary>
        private void cmdGetPaths_Click(object sender, EventArgs e)
        {
            rs.clsRS crs = new rs.clsRS();

            try
            {
                // set cursor
                this.Cursor = Cursors.Default;

                // loop through each row and update the item
                foreach (DataGridViewRow row in this.grdSharedPaths.Rows)
                {
                    // skip if not checked
                    if ((row.Tag == null) || (!Convert.ToBoolean(row.Cells["clmCopy2"].Value)))
                        continue;

                    // get the profile object
                    clsSharedPathInfo PathInfo = row.Tag as clsSharedPathInfo;

                    // update shared flag
                    crs.ClonePath(
                        clsSettings.LoginInfo.UserID,
                        PathInfo.UG,
                        PathInfo.GroupName,
                        PathInfo.PathLevel,
                        clsSettings.UpdateText,
                        clsSettings.IsDCd);

                    // uncheck
                    row.Cells["clmCopy2"].Value = false;
                }

                // repop the user profiles list
                PopUserPaths();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Clone Shared Paths");
            }

            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void cmdRefreshShared_Click(object sender, EventArgs e)
        {
            // pop the shared profiles
            PopSharedPaths();
        }

        /// <summary>
        /// Show the profile note
        /// </summary>
        private void grdSharedPaths_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                // exit if nothing selected
                if ((this.grdSharedPaths.SelectedRows.Count == 0) ||
                    (this.grdSharedPaths.SelectedRows[0].Tag == null))
                    return;

                // get the tag
                clsSharedPathInfo PathInfo = this.grdSharedPaths.SelectedRows[0].Tag as clsSharedPathInfo;

                // set the note
                this.txtSharedProfileNote.Text = clsSerialize.Deserialize_FromByteA(PathInfo.GroupNote, typeof(string)) as string;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Shared Path Select");
            }            
        }

        // SharedProfiles
        #endregion

        // ButtonClicks
        #endregion

        #region Popping

        /// <summary>
        /// Pop the grid with the list of the user's profiles
        /// </summary>
        private void PopUserPaths()
        {
            try
            {
                // clear the grid
                this.grdUserPaths.Rows.Clear();

                // get the profile list
                List<clsSharedPathInfo> SharedList = new rs.clsRS().GetSharedUserPathList(clsSettings.LoginInfo.UserID, clsSettings.UpdateText, clsSettings.IsDCd);
            
                // pop the grid
                foreach (clsSharedPathInfo sharedItem in SharedList)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(this.grdUserPaths);
                    row.Cells[0].Value = sharedItem.GroupName;
                    row.Cells[1].Value = sharedItem.PathLevel;
                    row.Cells[2].Value = sharedItem.IsPaid;
                    row.Tag = sharedItem;
                    this.grdUserPaths.Rows.Add(row);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "PopUserPaths");
            }            
        }

        /// <summary>
        /// Pop the grid with the list of all shared profiles
        /// </summary>
        private void PopSharedPaths()
        {
            try
            {
                // clear the grid
                this.grdSharedPaths.Rows.Clear();

                // get the profile list
                List<clsSharedPathInfo> SharedList = new rs.clsRS().GetSharedPathList(clsSettings.LoginInfo.UserID, 1, 70, clsSettings.UpdateText, clsSettings.IsDCd);

                // pop the grid
                foreach (clsSharedPathInfo sharedItem in SharedList)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(this.grdSharedPaths);
                    row.Cells[0].Value = sharedItem.GroupName;
                    row.Cells[1].Value = sharedItem.IsPaid;
                    row.Cells[2].Value = sharedItem.PathLevel;
                    row.Cells[3].Value = false;
                    row.Tag = sharedItem;
                    this.grdSharedPaths.Rows.Add(row);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "PopSharedPaths");
            }            
        }

        // Popping
        #endregion
    }
}
