namespace Rhabot.Controls
{
    partial class uscHealBot
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscHealBot));
            this.txtHealPercent = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtHealSpell = new System.Windows.Forms.TextBox();
            this.txtRemoveCurse = new System.Windows.Forms.TextBox();
            this.txtRemoveDisease = new System.Windows.Forms.TextBox();
            this.txtRemovePoison = new System.Windows.Forms.TextBox();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.txtBuffList = new System.Windows.Forms.TextBox();
            this.cmdSaveSettings = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtHealPercent)).BeginInit();
            this.SuspendLayout();
            // 
            // txtHealPercent
            // 
            resources.ApplyResources(this.txtHealPercent, "txtHealPercent");
            this.txtHealPercent.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.txtHealPercent.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtHealPercent.Name = "txtHealPercent";
            this.toolTip1.SetToolTip(this.txtHealPercent, resources.GetString("txtHealPercent.ToolTip"));
            this.txtHealPercent.Value = new decimal(new int[] {
            75,
            0,
            0,
            0});
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtHealSpell
            // 
            resources.ApplyResources(this.txtHealSpell, "txtHealSpell");
            this.txtHealSpell.Name = "txtHealSpell";
            this.toolTip1.SetToolTip(this.txtHealSpell, resources.GetString("txtHealSpell.ToolTip"));
            // 
            // txtRemoveCurse
            // 
            resources.ApplyResources(this.txtRemoveCurse, "txtRemoveCurse");
            this.txtRemoveCurse.Name = "txtRemoveCurse";
            this.toolTip1.SetToolTip(this.txtRemoveCurse, resources.GetString("txtRemoveCurse.ToolTip"));
            // 
            // txtRemoveDisease
            // 
            resources.ApplyResources(this.txtRemoveDisease, "txtRemoveDisease");
            this.txtRemoveDisease.Name = "txtRemoveDisease";
            this.toolTip1.SetToolTip(this.txtRemoveDisease, resources.GetString("txtRemoveDisease.ToolTip"));
            // 
            // txtRemovePoison
            // 
            resources.ApplyResources(this.txtRemovePoison, "txtRemovePoison");
            this.txtRemovePoison.Name = "txtRemovePoison";
            this.toolTip1.SetToolTip(this.txtRemovePoison, resources.GetString("txtRemovePoison.ToolTip"));
            // 
            // txtTarget
            // 
            resources.ApplyResources(this.txtTarget, "txtTarget");
            this.txtTarget.Name = "txtTarget";
            this.toolTip1.SetToolTip(this.txtTarget, resources.GetString("txtTarget.ToolTip"));
            // 
            // txtBuffList
            // 
            resources.ApplyResources(this.txtBuffList, "txtBuffList");
            this.txtBuffList.Name = "txtBuffList";
            this.toolTip1.SetToolTip(this.txtBuffList, resources.GetString("txtBuffList.ToolTip"));
            // 
            // cmdSaveSettings
            // 
            resources.ApplyResources(this.cmdSaveSettings, "cmdSaveSettings");
            this.cmdSaveSettings.Name = "cmdSaveSettings";
            this.cmdSaveSettings.UseVisualStyleBackColor = true;
            this.cmdSaveSettings.Click += new System.EventHandler(this.cmdSaveSettings_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // uscHealBot
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.Controls.Add(this.txtBuffList);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtTarget);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtRemovePoison);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtRemoveDisease);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtRemoveCurse);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtHealSpell);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmdSaveSettings);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtHealPercent);
            this.Name = "uscHealBot";
            this.Load += new System.EventHandler(this.uscHealBot_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtHealPercent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown txtHealPercent;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdSaveSettings;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtHealSpell;
        private System.Windows.Forms.TextBox txtRemoveCurse;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtRemoveDisease;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtRemovePoison;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtBuffList;
        private System.Windows.Forms.Label label7;
    }
}
