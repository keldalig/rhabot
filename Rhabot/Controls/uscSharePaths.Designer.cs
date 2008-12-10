namespace ISXBotHelper.Controls
{
    partial class uscSharePaths
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
            this.grpYourPaths = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdRefreshPath = new System.Windows.Forms.Button();
            this.cmdShareUserPath = new System.Windows.Forms.Button();
            this.grdUserPaths = new System.Windows.Forms.DataGridView();
            this.clmGroupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmPathLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmIsShared = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.grpAvailPaths = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdRefreshShared = new System.Windows.Forms.Button();
            this.cmdGetPaths = new System.Windows.Forms.Button();
            this.grdSharedPaths = new System.Windows.Forms.DataGridView();
            this.clmGroupName2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmIsPaid2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clmPathLevel2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmCopy2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtYourPathNote = new System.Windows.Forms.RichTextBox();
            this.txtSharedProfileNote = new System.Windows.Forms.RichTextBox();
            this.grpYourPaths.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdUserPaths)).BeginInit();
            this.grpAvailPaths.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdSharedPaths)).BeginInit();
            this.SuspendLayout();
            // 
            // grpYourPaths
            // 
            this.grpYourPaths.Controls.Add(this.txtYourPathNote);
            this.grpYourPaths.Controls.Add(this.label1);
            this.grpYourPaths.Controls.Add(this.cmdRefreshPath);
            this.grpYourPaths.Controls.Add(this.cmdShareUserPath);
            this.grpYourPaths.Controls.Add(this.grdUserPaths);
            this.grpYourPaths.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpYourPaths.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpYourPaths.ForeColor = System.Drawing.Color.Navy;
            this.grpYourPaths.Location = new System.Drawing.Point(0, 0);
            this.grpYourPaths.Name = "grpYourPaths";
            this.grpYourPaths.Size = new System.Drawing.Size(643, 194);
            this.grpYourPaths.TabIndex = 0;
            this.grpYourPaths.TabStop = false;
            this.grpYourPaths.Text = "Your Paths";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(634, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "To share your path, click the \"Shared\" box for the path, then click \"Update Path " +
                "Shared Statuses\"";
            // 
            // cmdRefreshPath
            // 
            this.cmdRefreshPath.ForeColor = System.Drawing.Color.Black;
            this.cmdRefreshPath.Location = new System.Drawing.Point(562, 165);
            this.cmdRefreshPath.Name = "cmdRefreshPath";
            this.cmdRefreshPath.Size = new System.Drawing.Size(75, 23);
            this.cmdRefreshPath.TabIndex = 3;
            this.cmdRefreshPath.Text = "Refresh";
            this.cmdRefreshPath.UseVisualStyleBackColor = true;
            this.cmdRefreshPath.Click += new System.EventHandler(this.cmdRefreshProfile_Click);
            // 
            // cmdShareUserPath
            // 
            this.cmdShareUserPath.ForeColor = System.Drawing.Color.Black;
            this.cmdShareUserPath.Location = new System.Drawing.Point(562, 39);
            this.cmdShareUserPath.Name = "cmdShareUserPath";
            this.cmdShareUserPath.Size = new System.Drawing.Size(75, 97);
            this.cmdShareUserPath.TabIndex = 1;
            this.cmdShareUserPath.Text = "Update Path Shared Statuses";
            this.toolTip1.SetToolTip(this.cmdShareUserPath, "Click this to update your paths as shared or not shared.");
            this.cmdShareUserPath.UseVisualStyleBackColor = true;
            this.cmdShareUserPath.Click += new System.EventHandler(this.cmdShareUserPath_Click);
            // 
            // grdUserPaths
            // 
            this.grdUserPaths.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdUserPaths.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmGroupName,
            this.clmPathLevel,
            this.clmIsShared});
            this.grdUserPaths.Location = new System.Drawing.Point(6, 39);
            this.grdUserPaths.Name = "grdUserPaths";
            this.grdUserPaths.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdUserPaths.Size = new System.Drawing.Size(550, 97);
            this.grdUserPaths.TabIndex = 0;
            this.grdUserPaths.SelectionChanged += new System.EventHandler(this.grdUserPaths_SelectionChanged);
            // 
            // clmGroupName
            // 
            this.clmGroupName.HeaderText = "Path Group Name";
            this.clmGroupName.Name = "clmGroupName";
            this.clmGroupName.ReadOnly = true;
            this.clmGroupName.Width = 325;
            // 
            // clmPathLevel
            // 
            this.clmPathLevel.HeaderText = "Max Path Level";
            this.clmPathLevel.Name = "clmPathLevel";
            this.clmPathLevel.ReadOnly = true;
            this.clmPathLevel.Width = 85;
            // 
            // clmIsShared
            // 
            this.clmIsShared.HeaderText = "Shared";
            this.clmIsShared.Name = "clmIsShared";
            this.clmIsShared.Width = 50;
            // 
            // grpAvailPaths
            // 
            this.grpAvailPaths.Controls.Add(this.txtSharedProfileNote);
            this.grpAvailPaths.Controls.Add(this.label2);
            this.grpAvailPaths.Controls.Add(this.cmdRefreshShared);
            this.grpAvailPaths.Controls.Add(this.cmdGetPaths);
            this.grpAvailPaths.Controls.Add(this.grdSharedPaths);
            this.grpAvailPaths.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpAvailPaths.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpAvailPaths.Location = new System.Drawing.Point(0, 200);
            this.grpAvailPaths.Name = "grpAvailPaths";
            this.grpAvailPaths.Size = new System.Drawing.Size(643, 200);
            this.grpAvailPaths.TabIndex = 1;
            this.grpAvailPaths.TabStop = false;
            this.grpAvailPaths.Text = "Available Shared Paths";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(3, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(634, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "To copy a path to your account, click the \"Copy\" box for the path, then click \"Co" +
                "py Selected Paths\"";
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
            // cmdGetPaths
            // 
            this.cmdGetPaths.ForeColor = System.Drawing.Color.Black;
            this.cmdGetPaths.Location = new System.Drawing.Point(562, 37);
            this.cmdGetPaths.Name = "cmdGetPaths";
            this.cmdGetPaths.Size = new System.Drawing.Size(75, 99);
            this.cmdGetPaths.TabIndex = 4;
            this.cmdGetPaths.Text = "Copy Selected Paths";
            this.toolTip1.SetToolTip(this.cmdGetPaths, "Copies the selected paths to your list of paths");
            this.cmdGetPaths.UseVisualStyleBackColor = true;
            this.cmdGetPaths.Click += new System.EventHandler(this.cmdGetPaths_Click);
            // 
            // grdSharedPaths
            // 
            this.grdSharedPaths.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdSharedPaths.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmGroupName2,
            this.clmIsPaid2,
            this.clmPathLevel2,
            this.clmCopy2});
            this.grdSharedPaths.Location = new System.Drawing.Point(6, 37);
            this.grdSharedPaths.Name = "grdSharedPaths";
            this.grdSharedPaths.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdSharedPaths.Size = new System.Drawing.Size(550, 99);
            this.grdSharedPaths.TabIndex = 3;
            this.grdSharedPaths.SelectionChanged += new System.EventHandler(this.grdSharedPaths_SelectionChanged);
            // 
            // clmGroupName2
            // 
            this.clmGroupName2.HeaderText = "Path Group Name";
            this.clmGroupName2.Name = "clmGroupName2";
            this.clmGroupName2.ReadOnly = true;
            this.clmGroupName2.Width = 300;
            // 
            // clmIsPaid2
            // 
            this.clmIsPaid2.HeaderText = "Paid User";
            this.clmIsPaid2.Name = "clmIsPaid2";
            this.clmIsPaid2.ReadOnly = true;
            this.clmIsPaid2.Width = 50;
            // 
            // clmPathLevel2
            // 
            this.clmPathLevel2.HeaderText = "Path Level";
            this.clmPathLevel2.Name = "clmPathLevel2";
            this.clmPathLevel2.ReadOnly = true;
            this.clmPathLevel2.Width = 50;
            // 
            // clmCopy2
            // 
            this.clmCopy2.HeaderText = "Copy";
            this.clmCopy2.Name = "clmCopy2";
            this.clmCopy2.Width = 50;
            // 
            // txtYourPathNote
            // 
            this.txtYourPathNote.AcceptsTab = true;
            this.txtYourPathNote.Location = new System.Drawing.Point(6, 142);
            this.txtYourPathNote.Name = "txtYourPathNote";
            this.txtYourPathNote.ReadOnly = true;
            this.txtYourPathNote.Size = new System.Drawing.Size(550, 46);
            this.txtYourPathNote.TabIndex = 5;
            this.txtYourPathNote.Text = "";
            // 
            // txtSharedProfileNote
            // 
            this.txtSharedProfileNote.AcceptsTab = true;
            this.txtSharedProfileNote.Location = new System.Drawing.Point(9, 142);
            this.txtSharedProfileNote.Name = "txtSharedProfileNote";
            this.txtSharedProfileNote.Size = new System.Drawing.Size(547, 46);
            this.txtSharedProfileNote.TabIndex = 8;
            this.txtSharedProfileNote.Text = "";
            // 
            // uscSharePaths
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpAvailPaths);
            this.Controls.Add(this.grpYourPaths);
            this.Name = "uscSharePaths";
            this.Size = new System.Drawing.Size(643, 400);
            this.Load += new System.EventHandler(this.uscSharePaths_Load);
            this.grpYourPaths.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdUserPaths)).EndInit();
            this.grpAvailPaths.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdSharedPaths)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpYourPaths;
        private System.Windows.Forms.GroupBox grpAvailPaths;
        private System.Windows.Forms.DataGridView grdUserPaths;
        private System.Windows.Forms.Button cmdShareUserPath;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button cmdGetPaths;
        private System.Windows.Forms.DataGridView grdSharedPaths;
        private System.Windows.Forms.Button cmdRefreshPath;
        private System.Windows.Forms.Button cmdRefreshShared;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmGroupName2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clmIsPaid2;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPathLevel2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clmCopy2;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmGroupName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPathLevel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clmIsShared;
        private System.Windows.Forms.RichTextBox txtYourPathNote;
        private System.Windows.Forms.RichTextBox txtSharedProfileNote;
    }
}
