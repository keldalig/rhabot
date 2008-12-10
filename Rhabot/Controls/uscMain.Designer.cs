using System;

namespace Rhabot
{
    partial class uscMain
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
            try
            {
                // unhook the float control events
                clsGlobals.FloatStopped -= new EventHandler(clsGlobals_FloatStopped);

                // stop the monitor
                this.uscMainInfo1.Stop();

                // close the window
                if ((frmFloat != null) && (frmFloat.Visible))
                {
                    frmFloat.Close();
                    frmFloat = null;
                }
            }

            catch (Exception excep)
            {
                ISXBotHelper.clsError.ShowError(excep, "Main Unload");
            }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscMain));
            this.label1 = new System.Windows.Forms.Label();
            this.grpControl = new System.Windows.Forms.GroupBox();
            this.txtPathNotes = new System.Windows.Forms.RichTextBox();
            this.cmdSavePathNote = new System.Windows.Forms.Button();
            this.chkTarget = new System.Windows.Forms.CheckBox();
            this.cmdPathName = new System.Windows.Forms.Button();
            this.cmbPathGroupName = new System.Windows.Forms.ComboBox();
            this.cmbBotType = new System.Windows.Forms.ComboBox();
            this.cmdStopBot = new System.Windows.Forms.Button();
            this.cmdPauseBot = new System.Windows.Forms.Button();
            this.cmdStartBot = new System.Windows.Forms.Button();
            this.cmbGeneralPaths = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmdCreate = new System.Windows.Forms.Button();
            this.cmdLoadSettings = new System.Windows.Forms.Button();
            this.txtLevelToLoad = new System.Windows.Forms.NumericUpDown();
            this.cmdRefresh = new System.Windows.Forms.Button();
            this.cmdSaveProfileNotes = new System.Windows.Forms.Button();
            this.cmdSharedProfiles = new System.Windows.Forms.Button();
            this.cmdSharedPaths = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSettingsName = new System.Windows.Forms.ComboBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.grpProfileNotes = new System.Windows.Forms.GroupBox();
            this.txtProfileNotes = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMPQLocation = new System.Windows.Forms.TextBox();
            this.cmdChangeMPQ = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.uscMainInfo1 = new Rhabot.uscMainInfo();
            this.grpControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLevelToLoad)).BeginInit();
            this.grpProfileNotes.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // grpControl
            // 
            this.grpControl.Controls.Add(this.txtPathNotes);
            this.grpControl.Controls.Add(this.cmdSavePathNote);
            this.grpControl.Controls.Add(this.chkTarget);
            this.grpControl.Controls.Add(this.cmdPathName);
            this.grpControl.Controls.Add(this.cmbPathGroupName);
            this.grpControl.Controls.Add(this.cmbBotType);
            this.grpControl.Controls.Add(this.cmdStopBot);
            this.grpControl.Controls.Add(this.cmdPauseBot);
            this.grpControl.Controls.Add(this.cmdStartBot);
            this.grpControl.Controls.Add(this.cmbGeneralPaths);
            resources.ApplyResources(this.grpControl, "grpControl");
            this.grpControl.Name = "grpControl";
            this.grpControl.TabStop = false;
            // 
            // txtPathNotes
            // 
            this.txtPathNotes.AcceptsTab = true;
            resources.ApplyResources(this.txtPathNotes, "txtPathNotes");
            this.txtPathNotes.Name = "txtPathNotes";
            // 
            // cmdSavePathNote
            // 
            resources.ApplyResources(this.cmdSavePathNote, "cmdSavePathNote");
            this.cmdSavePathNote.Name = "cmdSavePathNote";
            this.toolTip1.SetToolTip(this.cmdSavePathNote, resources.GetString("cmdSavePathNote.ToolTip"));
            this.cmdSavePathNote.Click += new System.EventHandler(this.cmdSavePathNote_Click);
            // 
            // chkTarget
            // 
            resources.ApplyResources(this.chkTarget, "chkTarget");
            this.chkTarget.Name = "chkTarget";
            this.toolTip1.SetToolTip(this.chkTarget, resources.GetString("chkTarget.ToolTip"));
            this.chkTarget.UseVisualStyleBackColor = true;
            // 
            // cmdPathName
            // 
            resources.ApplyResources(this.cmdPathName, "cmdPathName");
            this.cmdPathName.Name = "cmdPathName";
            this.cmdPathName.UseVisualStyleBackColor = true;
            this.cmdPathName.Click += new System.EventHandler(this.cmdPathName_Click);
            // 
            // cmbPathGroupName
            // 
            this.cmbPathGroupName.FormattingEnabled = true;
            resources.ApplyResources(this.cmbPathGroupName, "cmbPathGroupName");
            this.cmbPathGroupName.Name = "cmbPathGroupName";
            this.toolTip1.SetToolTip(this.cmbPathGroupName, resources.GetString("cmbPathGroupName.ToolTip"));
            this.cmbPathGroupName.SelectedIndexChanged += new System.EventHandler(this.cmbPathGroupName_SelectedIndexChanged);
            // 
            // cmbBotType
            // 
            this.cmbBotType.FormattingEnabled = true;
            resources.ApplyResources(this.cmbBotType, "cmbBotType");
            this.cmbBotType.Name = "cmbBotType";
            this.toolTip1.SetToolTip(this.cmbBotType, resources.GetString("cmbBotType.ToolTip"));
            this.cmbBotType.SelectedIndexChanged += new System.EventHandler(this.cmbBotType_SelectedIndexChanged);
            // 
            // cmdStopBot
            // 
            resources.ApplyResources(this.cmdStopBot, "cmdStopBot");
            this.cmdStopBot.Name = "cmdStopBot";
            this.cmdStopBot.UseVisualStyleBackColor = true;
            this.cmdStopBot.Click += new System.EventHandler(this.cmdStopBot_Click);
            // 
            // cmdPauseBot
            // 
            resources.ApplyResources(this.cmdPauseBot, "cmdPauseBot");
            this.cmdPauseBot.Name = "cmdPauseBot";
            this.cmdPauseBot.UseVisualStyleBackColor = true;
            this.cmdPauseBot.Click += new System.EventHandler(this.cmdPauseBot_Click);
            // 
            // cmdStartBot
            // 
            resources.ApplyResources(this.cmdStartBot, "cmdStartBot");
            this.cmdStartBot.Name = "cmdStartBot";
            this.cmdStartBot.UseVisualStyleBackColor = true;
            this.cmdStartBot.Click += new System.EventHandler(this.cmdStartBot_Click);
            // 
            // cmbGeneralPaths
            // 
            this.cmbGeneralPaths.FormattingEnabled = true;
            resources.ApplyResources(this.cmbGeneralPaths, "cmbGeneralPaths");
            this.cmbGeneralPaths.Name = "cmbGeneralPaths";
            this.toolTip1.SetToolTip(this.cmbGeneralPaths, resources.GetString("cmbGeneralPaths.ToolTip"));
            // 
            // cmdCreate
            // 
            resources.ApplyResources(this.cmdCreate, "cmdCreate");
            this.cmdCreate.Name = "cmdCreate";
            this.toolTip1.SetToolTip(this.cmdCreate, resources.GetString("cmdCreate.ToolTip"));
            this.cmdCreate.UseVisualStyleBackColor = true;
            this.cmdCreate.Click += new System.EventHandler(this.cmdCreate_Click);
            // 
            // cmdLoadSettings
            // 
            resources.ApplyResources(this.cmdLoadSettings, "cmdLoadSettings");
            this.cmdLoadSettings.ForeColor = System.Drawing.Color.Red;
            this.cmdLoadSettings.Name = "cmdLoadSettings";
            this.toolTip1.SetToolTip(this.cmdLoadSettings, resources.GetString("cmdLoadSettings.ToolTip"));
            this.cmdLoadSettings.UseVisualStyleBackColor = true;
            this.cmdLoadSettings.Click += new System.EventHandler(this.cmdLoadSettings_Click);
            // 
            // txtLevelToLoad
            // 
            resources.ApplyResources(this.txtLevelToLoad, "txtLevelToLoad");
            this.txtLevelToLoad.Maximum = new decimal(new int[] {
            70,
            0,
            0,
            0});
            this.txtLevelToLoad.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtLevelToLoad.Name = "txtLevelToLoad";
            this.toolTip1.SetToolTip(this.txtLevelToLoad, resources.GetString("txtLevelToLoad.ToolTip"));
            this.txtLevelToLoad.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtLevelToLoad.ValueChanged += new System.EventHandler(this.txtLevelToLoad_ValueChanged);
            // 
            // cmdRefresh
            // 
            resources.ApplyResources(this.cmdRefresh, "cmdRefresh");
            this.cmdRefresh.Name = "cmdRefresh";
            this.toolTip1.SetToolTip(this.cmdRefresh, resources.GetString("cmdRefresh.ToolTip"));
            this.cmdRefresh.UseVisualStyleBackColor = true;
            this.cmdRefresh.Click += new System.EventHandler(this.cmdRefresh_Click);
            // 
            // cmdSaveProfileNotes
            // 
            resources.ApplyResources(this.cmdSaveProfileNotes, "cmdSaveProfileNotes");
            this.cmdSaveProfileNotes.Name = "cmdSaveProfileNotes";
            this.toolTip1.SetToolTip(this.cmdSaveProfileNotes, resources.GetString("cmdSaveProfileNotes.ToolTip"));
            this.cmdSaveProfileNotes.Click += new System.EventHandler(this.cmdSaveProfileNotes_Click);
            // 
            // cmdSharedProfiles
            // 
            resources.ApplyResources(this.cmdSharedProfiles, "cmdSharedProfiles");
            this.cmdSharedProfiles.Name = "cmdSharedProfiles";
            this.toolTip1.SetToolTip(this.cmdSharedProfiles, resources.GetString("cmdSharedProfiles.ToolTip"));
            this.cmdSharedProfiles.UseVisualStyleBackColor = true;
            this.cmdSharedProfiles.Click += new System.EventHandler(this.cmdSharedProfiles_Click);
            // 
            // cmdSharedPaths
            // 
            resources.ApplyResources(this.cmdSharedPaths, "cmdSharedPaths");
            this.cmdSharedPaths.Name = "cmdSharedPaths";
            this.toolTip1.SetToolTip(this.cmdSharedPaths, resources.GetString("cmdSharedPaths.ToolTip"));
            this.cmdSharedPaths.UseVisualStyleBackColor = true;
            this.cmdSharedPaths.Click += new System.EventHandler(this.cmdSharedPaths_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // cmbSettingsName
            // 
            this.cmbSettingsName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSettingsName.FormattingEnabled = true;
            resources.ApplyResources(this.cmbSettingsName, "cmbSettingsName");
            this.cmbSettingsName.Name = "cmbSettingsName";
            this.cmbSettingsName.SelectedIndexChanged += new System.EventHandler(this.cmbSettingsName_SelectedIndexChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // grpProfileNotes
            // 
            this.grpProfileNotes.Controls.Add(this.txtProfileNotes);
            this.grpProfileNotes.Controls.Add(this.cmdSaveProfileNotes);
            resources.ApplyResources(this.grpProfileNotes, "grpProfileNotes");
            this.grpProfileNotes.Name = "grpProfileNotes";
            this.grpProfileNotes.TabStop = false;
            // 
            // txtProfileNotes
            // 
            this.txtProfileNotes.AcceptsTab = true;
            resources.ApplyResources(this.txtProfileNotes, "txtProfileNotes");
            this.txtProfileNotes.Name = "txtProfileNotes";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txtMPQLocation
            // 
            this.txtMPQLocation.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.txtMPQLocation, "txtMPQLocation");
            this.txtMPQLocation.Name = "txtMPQLocation";
            this.txtMPQLocation.ReadOnly = true;
            this.toolTip1.SetToolTip(this.txtMPQLocation, resources.GetString("txtMPQLocation.ToolTip"));
            // 
            // cmdChangeMPQ
            // 
            resources.ApplyResources(this.cmdChangeMPQ, "cmdChangeMPQ");
            this.cmdChangeMPQ.Name = "cmdChangeMPQ";
            this.toolTip1.SetToolTip(this.cmdChangeMPQ, resources.GetString("cmdChangeMPQ.ToolTip"));
            this.cmdChangeMPQ.UseVisualStyleBackColor = true;
            this.cmdChangeMPQ.Click += new System.EventHandler(this.cmdChangeMPQ_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // uscMainInfo1
            // 
            this.uscMainInfo1.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.uscMainInfo1, "uscMainInfo1");
            this.uscMainInfo1.Name = "uscMainInfo1";
            // 
            // uscMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmdChangeMPQ);
            this.Controls.Add(this.txtMPQLocation);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmdSharedPaths);
            this.Controls.Add(this.cmdSharedProfiles);
            this.Controls.Add(this.grpProfileNotes);
            this.Controls.Add(this.cmdRefresh);
            this.Controls.Add(this.cmbSettingsName);
            this.Controls.Add(this.uscMainInfo1);
            this.Controls.Add(this.txtLevelToLoad);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmdLoadSettings);
            this.Controls.Add(this.cmdCreate);
            this.Controls.Add(this.grpControl);
            this.Controls.Add(this.label1);
            this.Name = "uscMain";
            this.Load += new System.EventHandler(this.uscMain_Load);
            this.grpControl.ResumeLayout(false);
            this.grpControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLevelToLoad)).EndInit();
            this.grpProfileNotes.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpControl;
        private System.Windows.Forms.Button cmdStopBot;
        private System.Windows.Forms.Button cmdPauseBot;
        private System.Windows.Forms.Button cmdStartBot;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button cmdCreate;
        private System.Windows.Forms.Button cmdLoadSettings;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown txtLevelToLoad;
        private uscMainInfo uscMainInfo1;
        private System.Windows.Forms.ComboBox cmbBotType;
        private System.Windows.Forms.ComboBox cmbPathGroupName;
        private System.Windows.Forms.ComboBox cmbSettingsName;
        private System.Windows.Forms.Button cmdRefresh;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button cmdPathName;
        private System.Windows.Forms.CheckBox chkTarget;
        private System.Windows.Forms.GroupBox grpProfileNotes;
        private System.Windows.Forms.Button cmdSaveProfileNotes;
        private System.Windows.Forms.Button cmdSharedProfiles;
        private System.Windows.Forms.Button cmdSharedPaths;
        private System.Windows.Forms.Button cmdSavePathNote;
        private System.Windows.Forms.ComboBox cmbGeneralPaths;
        private System.Windows.Forms.RichTextBox txtPathNotes;
        private System.Windows.Forms.RichTextBox txtProfileNotes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMPQLocation;
        private System.Windows.Forms.Button cmdChangeMPQ;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}
