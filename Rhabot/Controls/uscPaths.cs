using System;
using System.Windows.Forms;
using ISXBotHelper;
using System.Collections.Generic;

namespace Rhabot
{
    public partial class uscPaths : UserControl
    {
        private ISXNewPathWiz frmPathWiz = null;

        public uscPaths()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the path form.
        /// </summary>
        /// <param name="PathName">Name of the path.</param>
        private void ShowPathForm(string PathName)
        {
            try
            {
                // exit if the form is open
                if ((frmPathWiz != null) && (frmPathWiz.Visible))
                    return;

                // get a new form instances
                frmPathWiz = new ISXNewPathWiz();

                // set field properties
                if (!string.IsNullOrEmpty(PathName))
                {
                    frmPathWiz.txtPathName.Text = PathName;
                    frmPathWiz.txtPathName.Enabled = false;
                }

                // show the form
                frmPathWiz.Show(this);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "ShowPathForm");
            }
        }

        /// <summary>
        /// Vendor path
        /// </summary>
        private void cmdVendor_Click(object sender, EventArgs e)
        {
            ShowPathForm(clsGlobals.Path_Vendor);
        }

        /// <summary>
        /// Start Hunting
        /// </summary>
        private void cmdHuntStart_Click(object sender, EventArgs e)
        {
            ShowPathForm(clsGlobals.Path_StartHunt);
        }

        /// <summary>
        /// Hunting
        /// </summary>
        private void cmdHunting_Click(object sender, EventArgs e)
        {
            ShowPathForm(clsGlobals.Path_Hunting);
        }

        /// <summary>
        /// Graveyard 1
        /// </summary>
        private void cmdCorpse1_Click(object sender, EventArgs e)
        {
            ShowPathForm(clsGlobals.Path_Graveyard1);
        }

        /// <summary>
        /// Graveyard 2
        /// </summary>
        private void cmdCorpse2_Click(object sender, EventArgs e)
        {
            ShowPathForm(clsGlobals.Path_Graveyard2);
        }

        /// <summary>
        /// General path
        /// </summary>
        private void cmdGeneral_Click(object sender, EventArgs e)
        {
            ShowPathForm(string.Empty);
        }

        private void cmdMailbox_Click(object sender, EventArgs e)
        {
            ShowPathForm(clsGlobals.Path_Mailbox);
        }

        private void cmdWoWBotImport_Click(object sender, EventArgs e)
        {
            // show the wowbot path form
            ISXWbImport frmWBImport = new ISXWbImport();
            frmWBImport.IsWoWBot = true;
            frmWBImport.Show(this);
        }

        private void cmdImportGlider_Click(object sender, EventArgs e)
        {
            // show the glider path form
            ISXWbImport frmWBImport = new ISXWbImport();
            frmWBImport.IsWoWBot = false;
            frmWBImport.Show(this);
        }

        #region Individual Paths

        private void cmdBuildNewPath_Click(object sender, EventArgs e)
        {
            try
            {
                // exit if the form is open
                if ((frmPathWiz != null) && (frmPathWiz.Visible))
                    return;

                // get a new form instances
                frmPathWiz = new ISXNewPathWiz();

                // set save variable
                frmPathWiz.txtPathName.Text = clsGlobals.Path_IndividPath;
                frmPathWiz.txtPathName.Enabled = false;
                frmPathWiz.SaveToNewFile = true;

                // show the form
                frmPathWiz.Show(this);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "ShowPathForm");
            }
        }

        private void uscPaths_Load(object sender, EventArgs e)
        {
            // hide new invidivual path wizard
            if (!clsSettings.IsFullVersion)
                this.cmdBuildNewPath.Visible = false;
        }

        private void RefreshPaths()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // clear the grid
                this.grvPathList.Rows.Clear();

                // get the list
                List<rs.PathService.clsPathListInfo> pathList = new rs.clsRS().GetPathGroupList(clsSettings.LoginInfo.UserID, clsSettings.UpdateText, clsSettings.IsDCd);

                // pop the gridview
                foreach (rs.PathService.clsPathListInfo pInfo in pathList)
                {
                    this.grvPathList.Rows.Add(pInfo.GroupName, pInfo.PathName, pInfo.PathLevel);
                    this.grvPathList.Rows[this.grvPathList.RowCount - 1].Tag = pInfo.PathID;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "PopPathLists");
            }   
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Refresh the path list
        /// </summary>
        private void cmdRefresh_Click(object sender, EventArgs e)
        {
            RefreshPaths();
        }

        /// <summary>
        /// Delete marked paths
        /// </summary>
        private void cmdDeletePath_Click(object sender, EventArgs e)
        {
            rs.clsRS cRS = new rs.clsRS();

            try
            {
                this.Cursor = Cursors.WaitCursor;

                // loop through each item in the grid and get checked items
                foreach (DataGridViewRow row in this.grvPathList.Rows)
                {
                    // skip if not marked
                    if (!Convert.ToBoolean(row.Cells["Mark"].Value))
                        continue;

                    // delete this path
                    cRS.DeletePath(clsSettings.LoginInfo.UserID, (int)row.Tag, clsSettings.UpdateText, clsSettings.IsDCd);
                }

                // inform user and repop the list
                MessageBox.Show("Rows Deleted");
                RefreshPaths();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "DeletePath");
            }   
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // Individual Paths
        #endregion
    }
}
