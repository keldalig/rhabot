namespace Rhabot
{
    partial class uscSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscSettings));
            this.chkUnattended = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkChatter = new System.Windows.Forms.CheckBox();
            this.chkHumanCheck = new System.Windows.Forms.CheckBox();
            this.cmdApplySettings = new System.Windows.Forms.Button();
            this.txtLowAttack = new System.Windows.Forms.NumericUpDown();
            this.txtHighAttack = new System.Windows.Forms.NumericUpDown();
            this.txtSearchRange = new System.Windows.Forms.NumericUpDown();
            this.chkTargetElites = new System.Windows.Forms.CheckBox();
            this.txtStuckTimeout = new System.Windows.Forms.NumericUpDown();
            this.chkLogoutStuck = new System.Windows.Forms.CheckBox();
            this.txtTargetRange = new System.Windows.Forms.NumericUpDown();
            this.txtLogoutTime = new System.Windows.Forms.NumericUpDown();
            this.txtDurability = new System.Windows.Forms.NumericUpDown();
            this.chkDeclineDuel = new System.Windows.Forms.CheckBox();
            this.chkDeclineGroup = new System.Windows.Forms.CheckBox();
            this.chkDeclineGuild = new System.Windows.Forms.CheckBox();
            this.txtEndLevel = new System.Windows.Forms.NumericUpDown();
            this.txtStartLevel = new System.Windows.Forms.NumericUpDown();
            this.chkDisableRendering = new System.Windows.Forms.CheckBox();
            this.chkDisableBkgRender = new System.Windows.Forms.CheckBox();
            this.chkCharStatus = new System.Windows.Forms.CheckBox();
            this.chkSendOnDead = new System.Windows.Forms.CheckBox();
            this.grpChatter = new System.Windows.Forms.GroupBox();
            this.cmdBrowseChatter = new System.Windows.Forms.Button();
            this.txtChatterPath = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.grpBotControl = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.grpDecline = new System.Windows.Forms.GroupBox();
            this.grpStuck = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.chkSearchChest = new System.Windows.Forms.CheckBox();
            this.chkRogue = new System.Windows.Forms.CheckBox();
            this.chkFlower = new System.Windows.Forms.CheckBox();
            this.chkMiner = new System.Windows.Forms.CheckBox();
            this.chkSkinner = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblLogoutTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtLowAttack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHighAttack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStuckTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTargetRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogoutTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDurability)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStartLevel)).BeginInit();
            this.grpChatter.SuspendLayout();
            this.grpBotControl.SuspendLayout();
            this.grpDecline.SuspendLayout();
            this.grpStuck.SuspendLayout();
            this.grpSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkUnattended
            // 
            resources.ApplyResources(this.chkUnattended, "chkUnattended");
            this.chkUnattended.Name = "chkUnattended";
            this.toolTip1.SetToolTip(this.chkUnattended, resources.GetString("chkUnattended.ToolTip"));
            this.chkUnattended.UseVisualStyleBackColor = true;
            this.chkUnattended.CheckedChanged += new System.EventHandler(this.chkUnattended_CheckedChanged);
            // 
            // chkChatter
            // 
            resources.ApplyResources(this.chkChatter, "chkChatter");
            this.chkChatter.Name = "chkChatter";
            this.toolTip1.SetToolTip(this.chkChatter, resources.GetString("chkChatter.ToolTip"));
            this.chkChatter.UseVisualStyleBackColor = true;
            this.chkChatter.CheckedChanged += new System.EventHandler(this.chkChatter_CheckedChanged);
            // 
            // chkHumanCheck
            // 
            resources.ApplyResources(this.chkHumanCheck, "chkHumanCheck");
            this.chkHumanCheck.Name = "chkHumanCheck";
            this.toolTip1.SetToolTip(this.chkHumanCheck, resources.GetString("chkHumanCheck.ToolTip"));
            this.chkHumanCheck.UseVisualStyleBackColor = true;
            this.chkHumanCheck.CheckedChanged += new System.EventHandler(this.chkHumanCheck_CheckedChanged);
            // 
            // cmdApplySettings
            // 
            resources.ApplyResources(this.cmdApplySettings, "cmdApplySettings");
            this.cmdApplySettings.Name = "cmdApplySettings";
            this.toolTip1.SetToolTip(this.cmdApplySettings, resources.GetString("cmdApplySettings.ToolTip"));
            this.cmdApplySettings.UseVisualStyleBackColor = true;
            this.cmdApplySettings.Click += new System.EventHandler(this.cmdApplySettings_Click);
            // 
            // txtLowAttack
            // 
            resources.ApplyResources(this.txtLowAttack, "txtLowAttack");
            this.txtLowAttack.Maximum = new decimal(new int[] {
            69,
            0,
            0,
            0});
            this.txtLowAttack.Minimum = new decimal(new int[] {
            69,
            0,
            0,
            -2147483648});
            this.txtLowAttack.Name = "txtLowAttack";
            this.toolTip1.SetToolTip(this.txtLowAttack, resources.GetString("txtLowAttack.ToolTip"));
            // 
            // txtHighAttack
            // 
            resources.ApplyResources(this.txtHighAttack, "txtHighAttack");
            this.txtHighAttack.Maximum = new decimal(new int[] {
            69,
            0,
            0,
            0});
            this.txtHighAttack.Minimum = new decimal(new int[] {
            69,
            0,
            0,
            -2147483648});
            this.txtHighAttack.Name = "txtHighAttack";
            this.toolTip1.SetToolTip(this.txtHighAttack, resources.GetString("txtHighAttack.ToolTip"));
            // 
            // txtSearchRange
            // 
            resources.ApplyResources(this.txtSearchRange, "txtSearchRange");
            this.txtSearchRange.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.txtSearchRange.Name = "txtSearchRange";
            this.toolTip1.SetToolTip(this.txtSearchRange, resources.GetString("txtSearchRange.ToolTip"));
            // 
            // chkTargetElites
            // 
            resources.ApplyResources(this.chkTargetElites, "chkTargetElites");
            this.chkTargetElites.Name = "chkTargetElites";
            this.toolTip1.SetToolTip(this.chkTargetElites, resources.GetString("chkTargetElites.ToolTip"));
            this.chkTargetElites.UseVisualStyleBackColor = true;
            // 
            // txtStuckTimeout
            // 
            resources.ApplyResources(this.txtStuckTimeout, "txtStuckTimeout");
            this.txtStuckTimeout.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.txtStuckTimeout.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.txtStuckTimeout.Name = "txtStuckTimeout";
            this.toolTip1.SetToolTip(this.txtStuckTimeout, resources.GetString("txtStuckTimeout.ToolTip"));
            this.txtStuckTimeout.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // chkLogoutStuck
            // 
            resources.ApplyResources(this.chkLogoutStuck, "chkLogoutStuck");
            this.chkLogoutStuck.Name = "chkLogoutStuck";
            this.toolTip1.SetToolTip(this.chkLogoutStuck, resources.GetString("chkLogoutStuck.ToolTip"));
            this.chkLogoutStuck.UseVisualStyleBackColor = true;
            // 
            // txtTargetRange
            // 
            resources.ApplyResources(this.txtTargetRange, "txtTargetRange");
            this.txtTargetRange.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.txtTargetRange.Name = "txtTargetRange";
            this.toolTip1.SetToolTip(this.txtTargetRange, resources.GetString("txtTargetRange.ToolTip"));
            // 
            // txtLogoutTime
            // 
            resources.ApplyResources(this.txtLogoutTime, "txtLogoutTime");
            this.txtLogoutTime.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.txtLogoutTime.Name = "txtLogoutTime";
            this.toolTip1.SetToolTip(this.txtLogoutTime, resources.GetString("txtLogoutTime.ToolTip"));
            this.txtLogoutTime.Leave += new System.EventHandler(this.txtLogoutTime_Leave);
            // 
            // txtDurability
            // 
            resources.ApplyResources(this.txtDurability, "txtDurability");
            this.txtDurability.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.txtDurability.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtDurability.Name = "txtDurability";
            this.toolTip1.SetToolTip(this.txtDurability, resources.GetString("txtDurability.ToolTip"));
            this.txtDurability.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // chkDeclineDuel
            // 
            resources.ApplyResources(this.chkDeclineDuel, "chkDeclineDuel");
            this.chkDeclineDuel.Name = "chkDeclineDuel";
            this.toolTip1.SetToolTip(this.chkDeclineDuel, resources.GetString("chkDeclineDuel.ToolTip"));
            this.chkDeclineDuel.UseVisualStyleBackColor = true;
            // 
            // chkDeclineGroup
            // 
            resources.ApplyResources(this.chkDeclineGroup, "chkDeclineGroup");
            this.chkDeclineGroup.Name = "chkDeclineGroup";
            this.toolTip1.SetToolTip(this.chkDeclineGroup, resources.GetString("chkDeclineGroup.ToolTip"));
            this.chkDeclineGroup.UseVisualStyleBackColor = true;
            // 
            // chkDeclineGuild
            // 
            resources.ApplyResources(this.chkDeclineGuild, "chkDeclineGuild");
            this.chkDeclineGuild.Name = "chkDeclineGuild";
            this.toolTip1.SetToolTip(this.chkDeclineGuild, resources.GetString("chkDeclineGuild.ToolTip"));
            this.chkDeclineGuild.UseVisualStyleBackColor = true;
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
            // chkDisableRendering
            // 
            resources.ApplyResources(this.chkDisableRendering, "chkDisableRendering");
            this.chkDisableRendering.Name = "chkDisableRendering";
            this.toolTip1.SetToolTip(this.chkDisableRendering, resources.GetString("chkDisableRendering.ToolTip"));
            this.chkDisableRendering.UseVisualStyleBackColor = true;
            this.chkDisableRendering.CheckedChanged += new System.EventHandler(this.chkDisableRendering_CheckedChanged);
            // 
            // chkDisableBkgRender
            // 
            resources.ApplyResources(this.chkDisableBkgRender, "chkDisableBkgRender");
            this.chkDisableBkgRender.Name = "chkDisableBkgRender";
            this.toolTip1.SetToolTip(this.chkDisableBkgRender, resources.GetString("chkDisableBkgRender.ToolTip"));
            this.chkDisableBkgRender.UseVisualStyleBackColor = true;
            this.chkDisableBkgRender.CheckedChanged += new System.EventHandler(this.chkDisableBkgRender_CheckedChanged);
            // 
            // chkCharStatus
            // 
            resources.ApplyResources(this.chkCharStatus, "chkCharStatus");
            this.chkCharStatus.Name = "chkCharStatus";
            this.toolTip1.SetToolTip(this.chkCharStatus, resources.GetString("chkCharStatus.ToolTip"));
            this.chkCharStatus.UseVisualStyleBackColor = true;
            this.chkCharStatus.CheckedChanged += new System.EventHandler(this.chkCharStatus_CheckedChanged);
            // 
            // chkSendOnDead
            // 
            resources.ApplyResources(this.chkSendOnDead, "chkSendOnDead");
            this.chkSendOnDead.Name = "chkSendOnDead";
            this.toolTip1.SetToolTip(this.chkSendOnDead, resources.GetString("chkSendOnDead.ToolTip"));
            this.chkSendOnDead.UseVisualStyleBackColor = true;
            this.chkSendOnDead.CheckedChanged += new System.EventHandler(this.chkSendOnDead_CheckedChanged);
            // 
            // grpChatter
            // 
            this.grpChatter.Controls.Add(this.cmdBrowseChatter);
            this.grpChatter.Controls.Add(this.txtChatterPath);
            this.grpChatter.Controls.Add(this.chkChatter);
            resources.ApplyResources(this.grpChatter, "grpChatter");
            this.grpChatter.Name = "grpChatter";
            this.grpChatter.TabStop = false;
            // 
            // cmdBrowseChatter
            // 
            resources.ApplyResources(this.cmdBrowseChatter, "cmdBrowseChatter");
            this.cmdBrowseChatter.Name = "cmdBrowseChatter";
            this.cmdBrowseChatter.UseVisualStyleBackColor = true;
            this.cmdBrowseChatter.Click += new System.EventHandler(this.cmdBrowseChatter_Click);
            // 
            // txtChatterPath
            // 
            resources.ApplyResources(this.txtChatterPath, "txtChatterPath");
            this.txtChatterPath.Name = "txtChatterPath";
            // 
            // grpBotControl
            // 
            this.grpBotControl.Controls.Add(this.txtDurability);
            this.grpBotControl.Controls.Add(this.label7);
            this.grpBotControl.Controls.Add(this.grpDecline);
            this.grpBotControl.Controls.Add(this.grpStuck);
            this.grpBotControl.Controls.Add(this.grpSearch);
            this.grpBotControl.Controls.Add(this.cmdApplySettings);
            resources.ApplyResources(this.grpBotControl, "grpBotControl");
            this.grpBotControl.Name = "grpBotControl";
            this.grpBotControl.TabStop = false;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // grpDecline
            // 
            this.grpDecline.Controls.Add(this.chkDeclineDuel);
            this.grpDecline.Controls.Add(this.chkDeclineGroup);
            this.grpDecline.Controls.Add(this.chkDeclineGuild);
            resources.ApplyResources(this.grpDecline, "grpDecline");
            this.grpDecline.Name = "grpDecline";
            this.grpDecline.TabStop = false;
            // 
            // grpStuck
            // 
            this.grpStuck.Controls.Add(this.chkLogoutStuck);
            this.grpStuck.Controls.Add(this.txtStuckTimeout);
            this.grpStuck.Controls.Add(this.label4);
            resources.ApplyResources(this.grpStuck, "grpStuck");
            this.grpStuck.Name = "grpStuck";
            this.grpStuck.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // grpSearch
            // 
            this.grpSearch.Controls.Add(this.txtEndLevel);
            this.grpSearch.Controls.Add(this.label16);
            this.grpSearch.Controls.Add(this.txtStartLevel);
            this.grpSearch.Controls.Add(this.label15);
            this.grpSearch.Controls.Add(this.txtTargetRange);
            this.grpSearch.Controls.Add(this.label5);
            this.grpSearch.Controls.Add(this.chkSearchChest);
            this.grpSearch.Controls.Add(this.chkRogue);
            this.grpSearch.Controls.Add(this.chkFlower);
            this.grpSearch.Controls.Add(this.chkMiner);
            this.grpSearch.Controls.Add(this.chkSkinner);
            this.grpSearch.Controls.Add(this.chkTargetElites);
            this.grpSearch.Controls.Add(this.txtSearchRange);
            this.grpSearch.Controls.Add(this.label3);
            this.grpSearch.Controls.Add(this.txtHighAttack);
            this.grpSearch.Controls.Add(this.label2);
            this.grpSearch.Controls.Add(this.txtLowAttack);
            this.grpSearch.Controls.Add(this.label1);
            resources.ApplyResources(this.grpSearch, "grpSearch");
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.TabStop = false;
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.ForeColor = System.Drawing.Color.Firebrick;
            this.label16.Name = "label16";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.ForeColor = System.Drawing.Color.Firebrick;
            this.label15.Name = "label15";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // chkSearchChest
            // 
            resources.ApplyResources(this.chkSearchChest, "chkSearchChest");
            this.chkSearchChest.Name = "chkSearchChest";
            this.chkSearchChest.UseVisualStyleBackColor = true;
            // 
            // chkRogue
            // 
            resources.ApplyResources(this.chkRogue, "chkRogue");
            this.chkRogue.Name = "chkRogue";
            this.chkRogue.UseVisualStyleBackColor = true;
            // 
            // chkFlower
            // 
            resources.ApplyResources(this.chkFlower, "chkFlower");
            this.chkFlower.Name = "chkFlower";
            this.chkFlower.UseVisualStyleBackColor = true;
            // 
            // chkMiner
            // 
            resources.ApplyResources(this.chkMiner, "chkMiner");
            this.chkMiner.Name = "chkMiner";
            this.chkMiner.UseVisualStyleBackColor = true;
            // 
            // chkSkinner
            // 
            resources.ApplyResources(this.chkSkinner, "chkSkinner");
            this.chkSkinner.Name = "chkSkinner";
            this.chkSkinner.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // lblLogoutTime
            // 
            resources.ApplyResources(this.lblLogoutTime, "lblLogoutTime");
            this.lblLogoutTime.Name = "lblLogoutTime";
            // 
            // uscSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.chkSendOnDead);
            this.Controls.Add(this.chkCharStatus);
            this.Controls.Add(this.chkDisableBkgRender);
            this.Controls.Add(this.chkDisableRendering);
            this.Controls.Add(this.lblLogoutTime);
            this.Controls.Add(this.txtLogoutTime);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.grpBotControl);
            this.Controls.Add(this.chkHumanCheck);
            this.Controls.Add(this.grpChatter);
            this.Controls.Add(this.chkUnattended);
            this.Name = "uscSettings";
            this.Load += new System.EventHandler(this.uscSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtLowAttack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHighAttack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStuckTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTargetRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogoutTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDurability)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStartLevel)).EndInit();
            this.grpChatter.ResumeLayout(false);
            this.grpChatter.PerformLayout();
            this.grpBotControl.ResumeLayout(false);
            this.grpBotControl.PerformLayout();
            this.grpDecline.ResumeLayout(false);
            this.grpDecline.PerformLayout();
            this.grpStuck.ResumeLayout(false);
            this.grpStuck.PerformLayout();
            this.grpSearch.ResumeLayout(false);
            this.grpSearch.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkUnattended;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkChatter;
        private System.Windows.Forms.GroupBox grpChatter;
        private System.Windows.Forms.Button cmdBrowseChatter;
        private System.Windows.Forms.TextBox txtChatterPath;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox chkHumanCheck;
        private System.Windows.Forms.GroupBox grpBotControl;
        private System.Windows.Forms.Button cmdApplySettings;
        private System.Windows.Forms.GroupBox grpSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown txtLowAttack;
        private System.Windows.Forms.NumericUpDown txtHighAttack;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown txtSearchRange;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkTargetElites;
        private System.Windows.Forms.CheckBox chkFlower;
        private System.Windows.Forms.CheckBox chkMiner;
        private System.Windows.Forms.CheckBox chkSkinner;
        private System.Windows.Forms.CheckBox chkSearchChest;
        private System.Windows.Forms.CheckBox chkRogue;
        private System.Windows.Forms.GroupBox grpStuck;
        private System.Windows.Forms.CheckBox chkLogoutStuck;
        private System.Windows.Forms.NumericUpDown txtStuckTimeout;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown txtTargetRange;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox grpDecline;
        private System.Windows.Forms.CheckBox chkDeclineDuel;
        private System.Windows.Forms.CheckBox chkDeclineGroup;
        private System.Windows.Forms.CheckBox chkDeclineGuild;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown txtLogoutTime;
        private System.Windows.Forms.Label lblLogoutTime;
        private System.Windows.Forms.NumericUpDown txtDurability;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown txtEndLevel;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown txtStartLevel;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox chkDisableRendering;
        private System.Windows.Forms.CheckBox chkDisableBkgRender;
        private System.Windows.Forms.CheckBox chkCharStatus;
        private System.Windows.Forms.CheckBox chkSendOnDead;
    }
}
