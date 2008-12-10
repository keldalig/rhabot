namespace Rhabot
{
    partial class uscTalents
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscTalents));
            this.lstTalents = new System.Windows.Forms.ListBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdApplyTalents = new System.Windows.Forms.Button();
            this.cmdFullList = new System.Windows.Forms.Button();
            this.cmdCurrentTalents = new System.Windows.Forms.Button();
            this.lblAvailableTalentPts = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.sData = new System.Windows.Forms.DataGridView();
            this.Level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Talent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sData)).BeginInit();
            this.SuspendLayout();
            // 
            // lstTalents
            // 
            resources.ApplyResources(this.lstTalents, "lstTalents");
            this.lstTalents.FormattingEnabled = true;
            this.lstTalents.Name = "lstTalents";
            // 
            // cmdSave
            // 
            resources.ApplyResources(this.cmdSave, "cmdSave");
            this.cmdSave.Name = "cmdSave";
            this.toolTip1.SetToolTip(this.cmdSave, resources.GetString("cmdSave.ToolTip"));
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmdApplyTalents
            // 
            resources.ApplyResources(this.cmdApplyTalents, "cmdApplyTalents");
            this.cmdApplyTalents.Name = "cmdApplyTalents";
            this.toolTip1.SetToolTip(this.cmdApplyTalents, resources.GetString("cmdApplyTalents.ToolTip"));
            this.cmdApplyTalents.UseVisualStyleBackColor = true;
            this.cmdApplyTalents.Click += new System.EventHandler(this.cmdApplyTalents_Click);
            // 
            // cmdFullList
            // 
            resources.ApplyResources(this.cmdFullList, "cmdFullList");
            this.cmdFullList.Name = "cmdFullList";
            this.toolTip1.SetToolTip(this.cmdFullList, resources.GetString("cmdFullList.ToolTip"));
            this.cmdFullList.UseVisualStyleBackColor = true;
            this.cmdFullList.Click += new System.EventHandler(this.cmdFullList_Click);
            // 
            // cmdCurrentTalents
            // 
            resources.ApplyResources(this.cmdCurrentTalents, "cmdCurrentTalents");
            this.cmdCurrentTalents.Name = "cmdCurrentTalents";
            this.toolTip1.SetToolTip(this.cmdCurrentTalents, resources.GetString("cmdCurrentTalents.ToolTip"));
            this.cmdCurrentTalents.UseVisualStyleBackColor = true;
            this.cmdCurrentTalents.Click += new System.EventHandler(this.cmdCurrentTalents_Click);
            // 
            // lblAvailableTalentPts
            // 
            resources.ApplyResources(this.lblAvailableTalentPts, "lblAvailableTalentPts");
            this.lblAvailableTalentPts.Name = "lblAvailableTalentPts";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdSave);
            this.panel1.Controls.Add(this.cmdApplyTalents);
            this.panel1.Controls.Add(this.cmdFullList);
            this.panel1.Controls.Add(this.cmdCurrentTalents);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // sData
            // 
            this.sData.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.sData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Level,
            this.Talent});
            resources.ApplyResources(this.sData, "sData");
            this.sData.Name = "sData";
            // 
            // Level
            // 
            this.Level.Frozen = true;
            resources.ApplyResources(this.Level, "Level");
            this.Level.Name = "Level";
            this.Level.ReadOnly = true;
            // 
            // Talent
            // 
            resources.ApplyResources(this.Talent, "Talent");
            this.Talent.Name = "Talent";
            // 
            // uscTalents
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Controls.Add(this.sData);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblAvailableTalentPts);
            this.Controls.Add(this.lstTalents);
            this.Name = "uscTalents";
            this.Load += new System.EventHandler(this.uscTalents_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstTalents;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblAvailableTalentPts;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Button cmdApplyTalents;
        private System.Windows.Forms.Button cmdFullList;
        private System.Windows.Forms.Button cmdCurrentTalents;
        public System.Windows.Forms.DataGridView sData;
        private System.Windows.Forms.DataGridViewTextBoxColumn Level;
        private System.Windows.Forms.DataGridViewTextBoxColumn Talent;
    }
}
