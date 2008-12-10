namespace Rhabot
{
    partial class ISXWbImport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ISXWbImport));
            this.label1 = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.cmdBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtStartLevel = new System.Windows.Forms.NumericUpDown();
            this.txtEndLevel = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.cmdImport = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmbGroupName = new System.Windows.Forms.ComboBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtPathNote = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtStartLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndLevel)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtFilename
            // 
            resources.ApplyResources(this.txtFilename, "txtFilename");
            this.txtFilename.Name = "txtFilename";
            this.toolTip1.SetToolTip(this.txtFilename, resources.GetString("txtFilename.ToolTip"));
            // 
            // cmdBrowse
            // 
            resources.ApplyResources(this.cmdBrowse, "cmdBrowse");
            this.cmdBrowse.Name = "cmdBrowse";
            this.cmdBrowse.UseVisualStyleBackColor = true;
            this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
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
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // cmdImport
            // 
            resources.ApplyResources(this.cmdImport, "cmdImport");
            this.cmdImport.Name = "cmdImport";
            this.cmdImport.UseVisualStyleBackColor = true;
            this.cmdImport.Click += new System.EventHandler(this.cmdImport_Click);
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
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "xml";
            this.openFileDialog1.FileName = "openFileDialog1";
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            this.openFileDialog1.ShowHelp = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPathNote);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // txtPathNote
            // 
            this.txtPathNote.AcceptsTab = true;
            resources.ApplyResources(this.txtPathNote, "txtPathNote");
            this.txtPathNote.Name = "txtPathNote";
            // 
            // ISXWbImport
            // 
            this.AcceptButton = this.cmdImport;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmbGroupName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmdImport);
            this.Controls.Add(this.txtEndLevel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtStartLevel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmdBrowse);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ISXWbImport";
            this.Load += new System.EventHandler(this.ISXWbImport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtStartLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndLevel)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button cmdBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown txtStartLevel;
        private System.Windows.Forms.NumericUpDown txtEndLevel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cmdImport;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ComboBox cmbGroupName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox txtPathNote;
    }
}