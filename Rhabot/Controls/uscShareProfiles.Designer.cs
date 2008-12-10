namespace ISXBotHelper.Controls
{
    partial class uscShareProfiles
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.grpYourProfiles = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdRefreshProfile = new System.Windows.Forms.Button();
            this.cmdShareUserProfile = new System.Windows.Forms.Button();
            this.grdUserProfiles = new System.Windows.Forms.DataGridView();
            this.clmProfileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmMinLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmMaxLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmIsShared = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.grpAvailProfiles = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdRefreshShared = new System.Windows.Forms.Button();
            this.cmdGetProfiles = new System.Windows.Forms.Button();
            this.grdSharedProfiles = new System.Windows.Forms.DataGridView();
            this.clmProfileName2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmIsPaid2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clmMinLevel2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmMaxLevel2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmCopy2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtYourProfileNote = new System.Windows.Forms.RichTextBox();
            this.txtSharedProfileNote = new System.Windows.Forms.RichTextBox();
            this.grpYourProfiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdUserProfiles)).BeginInit();
            this.grpAvailProfiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdSharedProfiles)).BeginInit();
            this.SuspendLayout();
            // 
            // grpYourProfiles
            // 
            this.grpYourProfiles.Controls.Add(this.txtYourProfileNote);
            this.grpYourProfiles.Controls.Add(this.label1);
            this.grpYourProfiles.Controls.Add(this.cmdRefreshProfile);
            this.grpYourProfiles.Controls.Add(this.cmdShareUserProfile);
            this.grpYourProfiles.Controls.Add(this.grdUserProfiles);
            this.grpYourProfiles.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpYourProfiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpYourProfiles.ForeColor = System.Drawing.Color.Navy;
            this.grpYourProfiles.Location = new System.Drawing.Point(0, 0);
            this.grpYourProfiles.Name = "grpYourProfiles";
            this.grpYourProfiles.Size = new System.Drawing.Size(643, 194);
            this.grpYourProfiles.TabIndex = 0;
            this.grpYourProfiles.TabStop = false;
            this.grpYourProfiles.Text = "Your Profiles";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(634, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "To share your profile, click the \"Shared\" box for the profile, then click \"Update" +
                " Profile Shared Statuses\"";
            // 
            // cmdRefreshProfile
            // 
            this.cmdRefreshProfile.ForeColor = System.Drawing.Color.Black;
            this.cmdRefreshProfile.Location = new System.Drawing.Point(562, 165);
            this.cmdRefreshProfile.Name = "cmdRefreshProfile";
            this.cmdRefreshProfile.Size = new System.Drawing.Size(75, 23);
            this.cmdRefreshProfile.TabIndex = 3;
            this.cmdRefreshProfile.Text = "Refresh";
            this.cmdRefreshProfile.UseVisualStyleBackColor = true;
            this.cmdRefreshProfile.Click += new System.EventHandler(this.cmdRefreshProfile_Click);
            // 
            // cmdShareUserProfile
            // 
            this.cmdShareUserProfile.ForeColor = System.Drawing.Color.Black;
            this.cmdShareUserProfile.Location = new System.Drawing.Point(562, 39);
            this.cmdShareUserProfile.Name = "cmdShareUserProfile";
            this.cmdShareUserProfile.Size = new System.Drawing.Size(75, 97);
            this.cmdShareUserProfile.TabIndex = 1;
            this.cmdShareUserProfile.Text = "Update Profile Shared Statuses";
            this.toolTip1.SetToolTip(this.cmdShareUserProfile, "Click this to update your profiles as shared or not shared.");
            this.cmdShareUserProfile.UseVisualStyleBackColor = true;
            this.cmdShareUserProfile.Click += new System.EventHandler(this.cmdShareUserProfile_Click);
            // 
            // grdUserProfiles
            // 
            this.grdUserProfiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdUserProfiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmProfileName,
            this.clmMinLevel,
            this.clmMaxLevel,
            this.clmIsShared});
            this.grdUserProfiles.Location = new System.Drawing.Point(6, 39);
            this.grdUserProfiles.Name = "grdUserProfiles";
            this.grdUserProfiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdUserProfiles.Size = new System.Drawing.Size(550, 97);
            this.grdUserProfiles.TabIndex = 0;
            this.grdUserProfiles.SelectionChanged += new System.EventHandler(this.grdUserProfiles_SelectionChanged);
            // 
            // clmProfileName
            // 
            this.clmProfileName.HeaderText = "Profile Name";
            this.clmProfileName.Name = "clmProfileName";
            this.clmProfileName.ReadOnly = true;
            this.clmProfileName.Width = 325;
            // 
            // clmMinLevel
            // 
            this.clmMinLevel.HeaderText = "Min Level";
            this.clmMinLevel.Name = "clmMinLevel";
            this.clmMinLevel.ReadOnly = true;
            this.clmMinLevel.Width = 50;
            // 
            // clmMaxLevel
            // 
            this.clmMaxLevel.HeaderText = "Max Level";
            this.clmMaxLevel.Name = "clmMaxLevel";
            this.clmMaxLevel.ReadOnly = true;
            this.clmMaxLevel.Width = 50;
            // 
            // clmIsShared
            // 
            this.clmIsShared.HeaderText = "Shared";
            this.clmIsShared.Name = "clmIsShared";
            this.clmIsShared.Width = 50;
            // 
            // grpAvailProfiles
            // 
            this.grpAvailProfiles.Controls.Add(this.txtSharedProfileNote);
            this.grpAvailProfiles.Controls.Add(this.label2);
            this.grpAvailProfiles.Controls.Add(this.cmdRefreshShared);
            this.grpAvailProfiles.Controls.Add(this.cmdGetProfiles);
            this.grpAvailProfiles.Controls.Add(this.grdSharedProfiles);
            this.grpAvailProfiles.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpAvailProfiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpAvailProfiles.Location = new System.Drawing.Point(0, 200);
            this.grpAvailProfiles.Name = "grpAvailProfiles";
            this.grpAvailProfiles.Size = new System.Drawing.Size(643, 200);
            this.grpAvailProfiles.TabIndex = 1;
            this.grpAvailProfiles.TabStop = false;
            this.grpAvailProfiles.Text = "Available Shared Profiles";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(3, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(634, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "To copy a profile to your account, click the \"Copy\" box for the profile, then cli" +
                "ck \"Copy Selected Profiles\"";
            // 
            // cmdRefreshShared
            // 
            this.cmdRefreshShared.ForeColor = System.Drawing.Color.Black;
            this.cmdRefreshShared.Location = new System.Drawing.Point(562, 165);
            this.cmdRefreshShared.Name = "cmdRefreshShared";
            this.cmdRefreshShared.Size = new System.Drawing.Size(75, 23);
            this.cmdRefreshShared.TabIndex = 6;
            this.cmdRefreshShared.Text = "Refresh";
            this.cmdRefreshShared.UseVisualStyleBackColor = true;
            this.cmdRefreshShared.Click += new System.EventHandler(this.cmdRefreshShared_Click);
            // 
            // cmdGetProfiles
            // 
            this.cmdGetProfiles.ForeColor = System.Drawing.Color.Black;
            this.cmdGetProfiles.Location = new System.Drawing.Point(562, 37);
            this.cmdGetProfiles.Name = "cmdGetProfiles";
            this.cmdGetProfiles.Size = new System.Drawing.Size(75, 99);
            this.cmdGetProfiles.TabIndex = 4;
            this.cmdGetProfiles.Text = "Copy Selected Profiles";
            this.toolTip1.SetToolTip(this.cmdGetProfiles, "Copies the selected profiles to your list of profile settings");
            this.cmdGetProfiles.UseVisualStyleBackColor = true;
            this.cmdGetProfiles.Click += new System.EventHandler(this.cmdGetProfiles_Click);
            // 
            // grdSharedProfiles
            // 
            this.grdSharedProfiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdSharedProfiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmProfileName2,
            this.clmIsPaid2,
            this.clmMinLevel2,
            this.clmMaxLevel2,
            this.clmCopy2});
            this.grdSharedProfiles.Location = new System.Drawing.Point(6, 37);
            this.grdSharedProfiles.Name = "grdSharedProfiles";
            this.grdSharedProfiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdSharedProfiles.Size = new System.Drawing.Size(550, 99);
            this.grdSharedProfiles.TabIndex = 3;
            this.grdSharedProfiles.SelectionChanged += new System.EventHandler(this.grdSharedProfiles_SelectionChanged);
            // 
            // clmProfileName2
            // 
            this.clmProfileName2.HeaderText = "Profile Name";
            this.clmProfileName2.Name = "clmProfileName2";
            this.clmProfileName2.ReadOnly = true;
            this.clmProfileName2.Width = 275;
            // 
            // clmIsPaid2
            // 
            this.clmIsPaid2.HeaderText = "Paid User";
            this.clmIsPaid2.Name = "clmIsPaid2";
            this.clmIsPaid2.ReadOnly = true;
            this.clmIsPaid2.Width = 50;
            // 
            // clmMinLevel2
            // 
            this.clmMinLevel2.HeaderText = "Min Level";
            this.clmMinLevel2.Name = "clmMinLevel2";
            this.clmMinLevel2.ReadOnly = true;
            this.clmMinLevel2.Width = 50;
            // 
            // clmMaxLevel2
            // 
            this.clmMaxLevel2.HeaderText = "Max Level";
            this.clmMaxLevel2.Name = "clmMaxLevel2";
            this.clmMaxLevel2.ReadOnly = true;
            this.clmMaxLevel2.Width = 50;
            // 
            // clmCopy2
            // 
            this.clmCopy2.HeaderText = "Copy";
            this.clmCopy2.Name = "clmCopy2";
            this.clmCopy2.Width = 50;
            // 
            // txtYourProfileNote
            // 
            this.txtYourProfileNote.AcceptsTab = true;
            this.txtYourProfileNote.Location = new System.Drawing.Point(9, 142);
            this.txtYourProfileNote.Name = "txtYourProfileNote";
            this.txtYourProfileNote.Size = new System.Drawing.Size(547, 46);
            this.txtYourProfileNote.TabIndex = 5;
            this.txtYourProfileNote.Text = "";
            // 
            // txtSharedProfileNote
            // 
            this.txtSharedProfileNote.AcceptsTab = true;
            this.txtSharedProfileNote.Location = new System.Drawing.Point(6, 142);
            this.txtSharedProfileNote.Name = "txtSharedProfileNote";
            this.txtSharedProfileNote.Size = new System.Drawing.Size(550, 46);
            this.txtSharedProfileNote.TabIndex = 8;
            this.txtSharedProfileNote.Text = "";
            // 
            // uscShareProfiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpAvailProfiles);
            this.Controls.Add(this.grpYourProfiles);
            this.Name = "uscShareProfiles";
            this.Size = new System.Drawing.Size(643, 400);
            this.Load += new System.EventHandler(this.uscShareProfiles_Load);
            this.grpYourProfiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdUserProfiles)).EndInit();
            this.grpAvailProfiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdSharedProfiles)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpYourProfiles;
        private System.Windows.Forms.GroupBox grpAvailProfiles;
        private System.Windows.Forms.DataGridView grdUserProfiles;
        private System.Windows.Forms.Button cmdShareUserProfile;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button cmdGetProfiles;
        private System.Windows.Forms.DataGridView grdSharedProfiles;
        private System.Windows.Forms.Button cmdRefreshProfile;
        private System.Windows.Forms.Button cmdRefreshShared;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmProfileName2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clmIsPaid2;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmMinLevel2;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmMaxLevel2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clmCopy2;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmProfileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmMinLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmMaxLevel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clmIsShared;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox txtYourProfileNote;
        private System.Windows.Forms.RichTextBox txtSharedProfileNote;
    }
}
