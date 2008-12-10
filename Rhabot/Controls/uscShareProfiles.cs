using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using rs.SharingService;

namespace ISXBotHelper.Controls
{
    public partial class uscShareProfiles : UserControl
    {
        #region Init

        public uscShareProfiles()
        {
            InitializeComponent();
        }

        private void uscShareProfiles_Load(object sender, EventArgs e)
        {
            try
            {
                this.grdSharedProfiles.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
                this.grdUserProfiles.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);

                // set cursor
                this.Cursor = Cursors.WaitCursor;

                // pop the user's profiles
                PopUserProfiles();

                // pop the shared profiles
                PopSharedProfiles();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Load Shared Profiles");
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
        private void cmdShareUserProfile_Click(object sender, EventArgs e)
        {
            rs.clsRS crs = new rs.clsRS();

            try
            {
                // set cursor
                this.Cursor = Cursors.Default;

                // loop through each row and update the item
                foreach (DataGridViewRow row in this.grdUserProfiles.Rows)
                {
                    // skip if not a row
                    if (row.Tag == null)
                        continue;

                    // get the profile object
                    clsSharedSettingsInfo ProfileInfo = row.Tag as clsSharedSettingsInfo;

                    // update shared flag
                    crs.UpdateProfileSharedFlag(
                        clsSettings.LoginInfo.UserID,
                        ProfileInfo.SettingsName,
                        Convert.ToBoolean(row.Cells["clmIsShared"].Value),
                        clsSettings.UpdateText,
                        clsSettings.IsDCd);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Update Shared User Profiles");
            }
            
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void cmdRefreshProfile_Click(object sender, EventArgs e)
        {
            // pop the user's profiles
            PopUserProfiles();
        }

        /// <summary>
        /// Show the profile note
        /// </summary>
        private void grdUserProfiles_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                // exit if nothing selected
                if ((this.grdUserProfiles.SelectedRows.Count == 0) ||
                    (this.grdUserProfiles.SelectedRows[0].Tag == null))
                    return;

                // get the tag
                clsSharedSettingsInfo ProfileInfo = this.grdUserProfiles.SelectedRows[0].Tag as clsSharedSettingsInfo;

                // set the note
                this.txtYourProfileNote.Text = clsSerialize.Deserialize_FromByteA(ProfileInfo.GroupNote, typeof(string)) as string;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "User Profile Select");
            }            
        }

        // UserProfiles
        #endregion

        #region SharedProfiles

        /// <summary>
        /// Clone the selected profiles
        /// </summary>
        private void cmdGetProfiles_Click(object sender, EventArgs e)
        {
            rs.clsRS crs = new rs.clsRS();

            try
            {
                // set cursor
                this.Cursor = Cursors.Default;

                // loop through each row and update the item
                foreach (DataGridViewRow row in this.grdSharedProfiles.Rows)
                {
                    // skip if not checked
                    if ((row.Tag == null) || (!Convert.ToBoolean(row.Cells["clmCopy2"].Value)))
                        continue;

                    // get the profile object
                    clsSharedSettingsInfo ProfileInfo = row.Tag as clsSharedSettingsInfo;

                    // update shared flag
                    crs.CloneProfile(
                        clsSettings.LoginInfo.UserID,
                        ProfileInfo.UG,
                        ProfileInfo.SettingsName,
                        clsSettings.UpdateText,
                        clsSettings.IsDCd);

                    // uncheck
                    row.Cells["clmCopy2"].Value = false;
                }

                // repop the user profiles list
                PopUserProfiles();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Clone Shared Profiles");
            }

            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void cmdRefreshShared_Click(object sender, EventArgs e)
        {
            // pop the shared profiles
            PopSharedProfiles();
        }

        /// <summary>
        /// Show the profile note
        /// </summary>
        private void grdSharedProfiles_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                // exit if nothing selected
                if ((this.grdSharedProfiles.SelectedRows.Count == 0) ||
                    (this.grdSharedProfiles.SelectedRows[0].Tag == null))
                    return;

                // get the tag
                clsSharedSettingsInfo ProfileInfo = this.grdSharedProfiles.SelectedRows[0].Tag as clsSharedSettingsInfo;

                // set the note
                this.txtSharedProfileNote.Text = clsSerialize.Deserialize_FromByteA(ProfileInfo.GroupNote, typeof(string)) as string;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Shared Profile Select");
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
        private void PopUserProfiles()
        {
            try
            {
                // clear the grid
                this.grdUserProfiles.Rows.Clear();

                // get the profile list
                List<clsSharedSettingsInfo> SharedList = new rs.clsRS().GetSharedUserSettingsList(clsSettings.LoginInfo.UserID, clsSettings.UpdateText, clsSettings.IsDCd);
            
                // pop the grid
                foreach (clsSharedSettingsInfo sharedItem in SharedList)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(this.grdUserProfiles);
                    row.Cells[0].Value = sharedItem.SettingsName;
                    row.Cells[1].Value = sharedItem.MinLevel;
                    row.Cells[2].Value = sharedItem.MaxLevel;
                    row.Cells[3].Value = sharedItem.IsPaid;
                    row.Tag = sharedItem;
                    this.grdUserProfiles.Rows.Add(row);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "PopUserProfiles");
            }            
        }

        /// <summary>
        /// Pop the grid with the list of all shared profiles
        /// </summary>
        private void PopSharedProfiles()
        {
            try
            {
                // clear the grid
                this.grdSharedProfiles.Rows.Clear();

                // get the profile list
                List<clsSharedSettingsInfo> SharedList = new rs.clsRS().GetSharedSettingsList(clsSettings.LoginInfo.UserID, clsSettings.UpdateText, clsSettings.IsDCd);

                // pop the grid
                foreach (clsSharedSettingsInfo sharedItem in SharedList)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(this.grdSharedProfiles);
                    row.Cells[0].Value = sharedItem.SettingsName;
                    row.Cells[1].Value = sharedItem.IsPaid;
                    row.Cells[2].Value = sharedItem.MinLevel;
                    row.Cells[3].Value = sharedItem.MaxLevel;
                    row.Cells[4].Value = false;
                    row.Tag = sharedItem;
                    this.grdSharedProfiles.Rows.Add(row);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "PopSharedProfiles");
            }            
        }

        // Popping
        #endregion
    }
}
