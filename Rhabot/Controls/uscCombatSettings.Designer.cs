namespace Rhabot
{
    partial class uscCombatSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscCombatSettings));
            this.label1 = new System.Windows.Forms.Label();
            this.txtHealPct = new System.Windows.Forms.NumericUpDown();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtManaSpam = new System.Windows.Forms.NumericUpDown();
            this.txtCombatWait = new System.Windows.Forms.NumericUpDown();
            this.txtDowntime = new System.Windows.Forms.NumericUpDown();
            this.txtPanicThreshold = new System.Windows.Forms.NumericUpDown();
            this.chkDoPanic = new System.Windows.Forms.CheckBox();
            this.txtPreCombatBuff = new System.Windows.Forms.TextBox();
            this.txtPullSpell = new System.Windows.Forms.TextBox();
            this.txtCombatSpam = new System.Windows.Forms.TextBox();
            this.txtHealingSpell = new System.Windows.Forms.TextBox();
            this.txtProtectionSpell = new System.Windows.Forms.TextBox();
            this.txtHealingOverTime = new System.Windows.Forms.TextBox();
            this.txtCombatDOT = new System.Windows.Forms.TextBox();
            this.chkSpamRandom = new System.Windows.Forms.CheckBox();
            this.txtPostCombatSpells = new System.Windows.Forms.TextBox();
            this.txtStopRunAwayAttempt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lstSPellList = new System.Windows.Forms.ListBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.uscStartEndLevel1 = new Rhabot.Controls.uscStartEndLevel();
            this.label17 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblPullSpell = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmdSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.txtHealPct)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtManaSpam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCombatWait)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDowntime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPanicThreshold)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtHealPct
            // 
            resources.ApplyResources(this.txtHealPct, "txtHealPct");
            this.txtHealPct.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.txtHealPct.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.txtHealPct.Name = "txtHealPct";
            this.toolTip1.SetToolTip(this.txtHealPct, resources.GetString("txtHealPct.ToolTip"));
            this.txtHealPct.Value = new decimal(new int[] {
            45,
            0,
            0,
            0});
            // 
            // txtManaSpam
            // 
            resources.ApplyResources(this.txtManaSpam, "txtManaSpam");
            this.txtManaSpam.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.txtManaSpam.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.txtManaSpam.Name = "txtManaSpam";
            this.toolTip1.SetToolTip(this.txtManaSpam, resources.GetString("txtManaSpam.ToolTip"));
            this.txtManaSpam.Value = new decimal(new int[] {
            35,
            0,
            0,
            0});
            // 
            // txtCombatWait
            // 
            resources.ApplyResources(this.txtCombatWait, "txtCombatWait");
            this.txtCombatWait.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtCombatWait.Name = "txtCombatWait";
            this.toolTip1.SetToolTip(this.txtCombatWait, resources.GetString("txtCombatWait.ToolTip"));
            this.txtCombatWait.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // txtDowntime
            // 
            resources.ApplyResources(this.txtDowntime, "txtDowntime");
            this.txtDowntime.Maximum = new decimal(new int[] {
            95,
            0,
            0,
            0});
            this.txtDowntime.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtDowntime.Name = "txtDowntime";
            this.toolTip1.SetToolTip(this.txtDowntime, resources.GetString("txtDowntime.ToolTip"));
            this.txtDowntime.Value = new decimal(new int[] {
            75,
            0,
            0,
            0});
            // 
            // txtPanicThreshold
            // 
            resources.ApplyResources(this.txtPanicThreshold, "txtPanicThreshold");
            this.txtPanicThreshold.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtPanicThreshold.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.txtPanicThreshold.Name = "txtPanicThreshold";
            this.toolTip1.SetToolTip(this.txtPanicThreshold, resources.GetString("txtPanicThreshold.ToolTip"));
            this.txtPanicThreshold.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // chkDoPanic
            // 
            resources.ApplyResources(this.chkDoPanic, "chkDoPanic");
            this.chkDoPanic.Checked = true;
            this.chkDoPanic.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDoPanic.Name = "chkDoPanic";
            this.toolTip1.SetToolTip(this.chkDoPanic, resources.GetString("chkDoPanic.ToolTip"));
            this.chkDoPanic.UseVisualStyleBackColor = true;
            // 
            // txtPreCombatBuff
            // 
            this.txtPreCombatBuff.AllowDrop = true;
            resources.ApplyResources(this.txtPreCombatBuff, "txtPreCombatBuff");
            this.txtPreCombatBuff.Name = "txtPreCombatBuff";
            this.toolTip1.SetToolTip(this.txtPreCombatBuff, resources.GetString("txtPreCombatBuff.ToolTip"));
            this.txtPreCombatBuff.DragDrop += new System.Windows.Forms.DragEventHandler(this.MultiTextBox_DragDrop);
            this.txtPreCombatBuff.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextBox_DragEnter);
            // 
            // txtPullSpell
            // 
            this.txtPullSpell.AllowDrop = true;
            resources.ApplyResources(this.txtPullSpell, "txtPullSpell");
            this.txtPullSpell.Name = "txtPullSpell";
            this.toolTip1.SetToolTip(this.txtPullSpell, resources.GetString("txtPullSpell.ToolTip"));
            this.txtPullSpell.DragDrop += new System.Windows.Forms.DragEventHandler(this.SingleTextBox_DragDrop);
            this.txtPullSpell.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextBox_DragEnter);
            // 
            // txtCombatSpam
            // 
            this.txtCombatSpam.AllowDrop = true;
            resources.ApplyResources(this.txtCombatSpam, "txtCombatSpam");
            this.txtCombatSpam.Name = "txtCombatSpam";
            this.toolTip1.SetToolTip(this.txtCombatSpam, resources.GetString("txtCombatSpam.ToolTip"));
            this.txtCombatSpam.DragDrop += new System.Windows.Forms.DragEventHandler(this.MultiTextBox_DragDrop);
            this.txtCombatSpam.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextBox_DragEnter);
            // 
            // txtHealingSpell
            // 
            this.txtHealingSpell.AllowDrop = true;
            resources.ApplyResources(this.txtHealingSpell, "txtHealingSpell");
            this.txtHealingSpell.Name = "txtHealingSpell";
            this.toolTip1.SetToolTip(this.txtHealingSpell, resources.GetString("txtHealingSpell.ToolTip"));
            this.txtHealingSpell.DragDrop += new System.Windows.Forms.DragEventHandler(this.SingleTextBox_DragDrop);
            this.txtHealingSpell.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextBox_DragEnter);
            // 
            // txtProtectionSpell
            // 
            this.txtProtectionSpell.AllowDrop = true;
            resources.ApplyResources(this.txtProtectionSpell, "txtProtectionSpell");
            this.txtProtectionSpell.Name = "txtProtectionSpell";
            this.toolTip1.SetToolTip(this.txtProtectionSpell, resources.GetString("txtProtectionSpell.ToolTip"));
            this.txtProtectionSpell.DragDrop += new System.Windows.Forms.DragEventHandler(this.SingleTextBox_DragDrop);
            this.txtProtectionSpell.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextBox_DragEnter);
            // 
            // txtHealingOverTime
            // 
            this.txtHealingOverTime.AllowDrop = true;
            resources.ApplyResources(this.txtHealingOverTime, "txtHealingOverTime");
            this.txtHealingOverTime.Name = "txtHealingOverTime";
            this.toolTip1.SetToolTip(this.txtHealingOverTime, resources.GetString("txtHealingOverTime.ToolTip"));
            this.txtHealingOverTime.DragDrop += new System.Windows.Forms.DragEventHandler(this.SingleTextBox_DragDrop);
            this.txtHealingOverTime.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextBox_DragEnter);
            // 
            // txtCombatDOT
            // 
            this.txtCombatDOT.AllowDrop = true;
            resources.ApplyResources(this.txtCombatDOT, "txtCombatDOT");
            this.txtCombatDOT.Name = "txtCombatDOT";
            this.toolTip1.SetToolTip(this.txtCombatDOT, resources.GetString("txtCombatDOT.ToolTip"));
            this.txtCombatDOT.DragDrop += new System.Windows.Forms.DragEventHandler(this.MultiTextBox_DragDrop);
            this.txtCombatDOT.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextBox_DragEnter);
            // 
            // chkSpamRandom
            // 
            resources.ApplyResources(this.chkSpamRandom, "chkSpamRandom");
            this.chkSpamRandom.Name = "chkSpamRandom";
            this.toolTip1.SetToolTip(this.chkSpamRandom, resources.GetString("chkSpamRandom.ToolTip"));
            this.chkSpamRandom.UseVisualStyleBackColor = true;
            // 
            // txtPostCombatSpells
            // 
            this.txtPostCombatSpells.AllowDrop = true;
            resources.ApplyResources(this.txtPostCombatSpells, "txtPostCombatSpells");
            this.txtPostCombatSpells.Name = "txtPostCombatSpells";
            this.toolTip1.SetToolTip(this.txtPostCombatSpells, resources.GetString("txtPostCombatSpells.ToolTip"));
            this.txtPostCombatSpells.DragDrop += new System.Windows.Forms.DragEventHandler(this.MultiTextBox_DragDrop);
            this.txtPostCombatSpells.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextBox_DragEnter);
            // 
            // txtStopRunAwayAttempt
            // 
            this.txtStopRunAwayAttempt.AllowDrop = true;
            resources.ApplyResources(this.txtStopRunAwayAttempt, "txtStopRunAwayAttempt");
            this.txtStopRunAwayAttempt.Name = "txtStopRunAwayAttempt";
            this.toolTip1.SetToolTip(this.txtStopRunAwayAttempt, resources.GetString("txtStopRunAwayAttempt.ToolTip"));
            this.txtStopRunAwayAttempt.DragDrop += new System.Windows.Forms.DragEventHandler(this.SingleTextBox_DragDrop);
            this.txtStopRunAwayAttempt.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextBox_DragEnter);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.txtStopRunAwayAttempt);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.uscStartEndLevel1);
            this.groupBox1.Controls.Add(this.txtPostCombatSpells);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.chkSpamRandom);
            this.groupBox1.Controls.Add(this.txtCombatDOT);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.txtHealingOverTime);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txtProtectionSpell);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.txtHealingSpell);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txtCombatSpam);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtPullSpell);
            this.groupBox1.Controls.Add(this.lblPullSpell);
            this.groupBox1.Controls.Add(this.txtPreCombatBuff);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lstSPellList);
            this.groupBox2.Controls.Add(this.label12);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // lstSPellList
            // 
            resources.ApplyResources(this.lstSPellList, "lstSPellList");
            this.lstSPellList.FormattingEnabled = true;
            this.lstSPellList.Name = "lstSPellList";
            this.lstSPellList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstSPellList_MouseDown);
            // 
            // label12
            // 
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.label12, "label12");
            this.label12.ForeColor = System.Drawing.Color.Blue;
            this.label12.Name = "label12";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // uscStartEndLevel1
            // 
            this.uscStartEndLevel1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.uscStartEndLevel1, "uscStartEndLevel1");
            this.uscStartEndLevel1.Name = "uscStartEndLevel1";
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // lblPullSpell
            // 
            resources.ApplyResources(this.lblPullSpell, "lblPullSpell");
            this.lblPullSpell.Name = "lblPullSpell";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.label6, "label6");
            this.label6.ForeColor = System.Drawing.Color.Blue;
            this.label6.Name = "label6";
            // 
            // cmdSave
            // 
            resources.ApplyResources(this.cmdSave, "cmdSave");
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // uscCombatSettings
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkDoPanic);
            this.Controls.Add(this.txtPanicThreshold);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtDowntime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtCombatWait);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtManaSpam);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtHealPct);
            this.Controls.Add(this.label1);
            this.Name = "uscCombatSettings";
            this.Load += new System.EventHandler(this.uscCombatSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtHealPct)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtManaSpam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCombatWait)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDowntime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPanicThreshold)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown txtHealPct;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.NumericUpDown txtManaSpam;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown txtCombatWait;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown txtDowntime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown txtPanicThreshold;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkDoPanic;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtPreCombatBuff;
        private System.Windows.Forms.TextBox txtCombatSpam;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtPullSpell;
        private System.Windows.Forms.Label lblPullSpell;
        private System.Windows.Forms.TextBox txtHealingSpell;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtProtectionSpell;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtHealingOverTime;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtCombatDOT;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.CheckBox chkSpamRandom;
        private System.Windows.Forms.TextBox txtPostCombatSpells;
        private System.Windows.Forms.Label label17;
        private Rhabot.Controls.uscStartEndLevel uscStartEndLevel1;
        private System.Windows.Forms.TextBox txtStopRunAwayAttempt;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lstSPellList;
        private System.Windows.Forms.Label label12;
    }
}
