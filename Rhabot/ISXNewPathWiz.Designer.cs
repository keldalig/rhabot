namespace Rhabot
{
    partial class ISXNewPathWiz
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ISXNewPathWiz));
            this.cmdStopRecord = new System.Windows.Forms.Button();
            this.cmdStartRecord = new System.Windows.Forms.Button();
            this.txtPathName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lstPathList = new System.Windows.Forms.ListBox();
            this.pnlPath = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbGroupName = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkPathFly = new System.Windows.Forms.CheckBox();
            this.txtEndLevel = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txtStartLevel = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.chkMountable = new System.Windows.Forms.CheckBox();
            this.cmdPauseRecord = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.txtPathNote = new System.Windows.Forms.RichTextBox();
            this.pnlPath.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStartLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdStopRecord
            // 
            resources.ApplyResources(this.cmdStopRecord, "cmdStopRecord");
            this.cmdStopRecord.Name = "cmdStopRecord";
            this.cmdStopRecord.UseVisualStyleBackColor = true;
            this.cmdStopRecord.Click += new System.EventHandler(this.cmdStopRecord_Click);
            // 
            // cmdStartRecord
            // 
            resources.ApplyResources(this.cmdStartRecord, "cmdStartRecord");
            this.cmdStartRecord.Name = "cmdStartRecord";
            this.cmdStartRecord.UseVisualStyleBackColor = true;
            this.cmdStartRecord.Click += new System.EventHandler(this.cmdStartRecord_Click);
            // 
            // txtPathName
            // 
            resources.ApplyResources(this.txtPathName, "txtPathName");
            this.txtPathName.Name = "txtPathName";
            this.toolTip1.SetToolTip(this.txtPathName, resources.GetString("txtPathName.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lstPathList
            // 
            resources.ApplyResources(this.lstPathList, "lstPathList");
            this.lstPathList.FormattingEnabled = true;
            this.lstPathList.Name = "lstPathList";
            // 
            // pnlPath
            // 
            this.pnlPath.Controls.Add(this.groupBox1);
            this.pnlPath.Controls.Add(this.lstPathList);
            this.pnlPath.Controls.Add(this.panel1);
            resources.ApplyResources(this.pnlPath, "pnlPath");
            this.pnlPath.Name = "pnlPath";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPathNote);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmbGroupName);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.chkPathFly);
            this.panel1.Controls.Add(this.txtEndLevel);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtStartLevel);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.chkMountable);
            this.panel1.Controls.Add(this.cmdPauseRecord);
            this.panel1.Controls.Add(this.cmdStopRecord);
            this.panel1.Controls.Add(this.cmdStartRecord);
            this.panel1.Controls.Add(this.txtPathName);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // cmbGroupName
            // 
            this.cmbGroupName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbGroupName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbGroupName.FormattingEnabled = true;
            resources.ApplyResources(this.cmbGroupName, "cmbGroupName");
            this.cmbGroupName.Name = "cmbGroupName";
            this.toolTip1.SetToolTip(this.cmbGroupName, resources.GetString("cmbGroupName.ToolTip"));
            this.cmbGroupName.SelectedIndexChanged += new System.EventHandler(this.cmbGroupName_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // chkPathFly
            // 
            resources.ApplyResources(this.chkPathFly, "chkPathFly");
            this.chkPathFly.Name = "chkPathFly";
            this.toolTip1.SetToolTip(this.chkPathFly, resources.GetString("chkPathFly.ToolTip"));
            this.chkPathFly.UseVisualStyleBackColor = true;
            this.chkPathFly.CheckedChanged += new System.EventHandler(this.chkPathFly_CheckedChanged);
            // 
            // txtEndLevel
            // 
            resources.ApplyResources(this.txtEndLevel, "txtEndLevel");
            this.txtEndLevel.Maximum = new decimal(new int[] {
            70,
            0,
            0,
            0});
            this.txtEndLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtEndLevel.Name = "txtEndLevel";
            this.toolTip1.SetToolTip(this.txtEndLevel, resources.GetString("txtEndLevel.ToolTip"));
            this.txtEndLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtStartLevel
            // 
            resources.ApplyResources(this.txtStartLevel, "txtStartLevel");
            this.txtStartLevel.Maximum = new decimal(new int[] {
            70,
            0,
            0,
            0});
            this.txtStartLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtStartLevel.Name = "txtStartLevel";
            this.toolTip1.SetToolTip(this.txtStartLevel, resources.GetString("txtStartLevel.ToolTip"));
            this.txtStartLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // chkMountable
            // 
            resources.ApplyResources(this.chkMountable, "chkMountable");
            this.chkMountable.Name = "chkMountable";
            this.toolTip1.SetToolTip(this.chkMountable, resources.GetString("chkMountable.ToolTip"));
            this.chkMountable.UseVisualStyleBackColor = true;
            // 
            // cmdPauseRecord
            // 
            resources.ApplyResources(this.cmdPauseRecord, "cmdPauseRecord");
            this.cmdPauseRecord.Name = "cmdPauseRecord";
            this.cmdPauseRecord.UseVisualStyleBackColor = true;
            this.cmdPauseRecord.Click += new System.EventHandler(this.cmdPauseRecord_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xml";
            resources.ApplyResources(this.saveFileDialog1, "saveFileDialog1");
            // 
            // txtPathNote
            // 
            this.txtPathNote.AcceptsTab = true;
            resources.ApplyResources(this.txtPathNote, "txtPathNote");
            this.txtPathNote.Name = "txtPathNote";
            // 
            // ISXNewPathWiz
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "ISXNewPathWiz";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ISXNewPathWiz_Load);
            this.pnlPath.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStartLevel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdStopRecord;
        private System.Windows.Forms.Button cmdStartRecord;
        public System.Windows.Forms.TextBox txtPathName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstPathList;
        private System.Windows.Forms.Panel pnlPath;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cmdPauseRecord;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkMountable;
        private System.Windows.Forms.NumericUpDown txtEndLevel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown txtStartLevel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkPathFly;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ComboBox cmbGroupName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox txtPathNote;
    }
}