namespace Rhabot
{
    partial class uscAutoUpgrade
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscAutoUpgrade));
            this.gridEquip = new System.Windows.Forms.DataGridView();
            this.clmnEquipSlot = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnMaterial = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clmnStat = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.uscStartEndLevel1 = new Rhabot.Controls.uscStartEndLevel();
            this.cmdSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridEquip)).BeginInit();
            this.SuspendLayout();
            // 
            // gridEquip
            // 
            this.gridEquip.AccessibleDescription = null;
            this.gridEquip.AccessibleName = null;
            this.gridEquip.AllowUserToAddRows = false;
            this.gridEquip.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.gridEquip, "gridEquip");
            this.gridEquip.BackgroundColor = System.Drawing.Color.RosyBrown;
            this.gridEquip.BackgroundImage = null;
            this.gridEquip.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridEquip.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmnEquipSlot,
            this.clmnMaterial,
            this.clmnStat});
            this.gridEquip.Font = null;
            this.gridEquip.Name = "gridEquip";
            // 
            // clmnEquipSlot
            // 
            resources.ApplyResources(this.clmnEquipSlot, "clmnEquipSlot");
            this.clmnEquipSlot.Name = "clmnEquipSlot";
            this.clmnEquipSlot.ReadOnly = true;
            // 
            // clmnMaterial
            // 
            this.clmnMaterial.DropDownWidth = 100;
            resources.ApplyResources(this.clmnMaterial, "clmnMaterial");
            this.clmnMaterial.MaxDropDownItems = 3;
            this.clmnMaterial.Name = "clmnMaterial";
            // 
            // clmnStat
            // 
            resources.ApplyResources(this.clmnStat, "clmnStat");
            this.clmnStat.Name = "clmnStat";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // uscStartEndLevel1
            // 
            this.uscStartEndLevel1.AccessibleDescription = null;
            this.uscStartEndLevel1.AccessibleName = null;
            resources.ApplyResources(this.uscStartEndLevel1, "uscStartEndLevel1");
            this.uscStartEndLevel1.BackColor = System.Drawing.Color.Transparent;
            this.uscStartEndLevel1.BackgroundImage = null;
            this.uscStartEndLevel1.Font = null;
            this.uscStartEndLevel1.Name = "uscStartEndLevel1";
            // 
            // cmdSave
            // 
            this.cmdSave.AccessibleDescription = null;
            this.cmdSave.AccessibleName = null;
            resources.ApplyResources(this.cmdSave, "cmdSave");
            this.cmdSave.BackgroundImage = null;
            this.cmdSave.Font = null;
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // uscAutoUpgrade
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RosyBrown;
            this.BackgroundImage = null;
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.uscStartEndLevel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gridEquip);
            this.Font = null;
            this.Name = "uscAutoUpgrade";
            this.Load += new System.EventHandler(this.uscAutoUpgrade_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridEquip)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridEquip;
        private System.Windows.Forms.Label label1;
        private Controls.uscStartEndLevel uscStartEndLevel1;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnEquipSlot;
        private System.Windows.Forms.DataGridViewComboBoxColumn clmnMaterial;
        private System.Windows.Forms.DataGridViewComboBoxColumn clmnStat;
    }
}
