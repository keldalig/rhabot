namespace Rhabot
{
    partial class uscCommunication
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscCommunication));
            this.grpMSN = new System.Windows.Forms.GroupBox();
            this.txtMSNPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMSNUsername = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkUseMSN = new System.Windows.Forms.CheckBox();
            this.grpEmail = new System.Windows.Forms.GroupBox();
            this.txtEmailList = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtEmailPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtEmailUsername = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtEmailServerPort = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txtEmailServer = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdTest = new System.Windows.Forms.Button();
            this.grpMSN.SuspendLayout();
            this.grpEmail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmailServerPort)).BeginInit();
            this.SuspendLayout();
            // 
            // grpMSN
            // 
            this.grpMSN.Controls.Add(this.txtMSNPassword);
            this.grpMSN.Controls.Add(this.label2);
            this.grpMSN.Controls.Add(this.txtMSNUsername);
            this.grpMSN.Controls.Add(this.label1);
            this.grpMSN.Controls.Add(this.chkUseMSN);
            resources.ApplyResources(this.grpMSN, "grpMSN");
            this.grpMSN.Name = "grpMSN";
            this.grpMSN.TabStop = false;
            // 
            // txtMSNPassword
            // 
            resources.ApplyResources(this.txtMSNPassword, "txtMSNPassword");
            this.txtMSNPassword.Name = "txtMSNPassword";
            this.toolTip1.SetToolTip(this.txtMSNPassword, resources.GetString("txtMSNPassword.ToolTip"));
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtMSNUsername
            // 
            resources.ApplyResources(this.txtMSNUsername, "txtMSNUsername");
            this.txtMSNUsername.Name = "txtMSNUsername";
            this.toolTip1.SetToolTip(this.txtMSNUsername, resources.GetString("txtMSNUsername.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // chkUseMSN
            // 
            resources.ApplyResources(this.chkUseMSN, "chkUseMSN");
            this.chkUseMSN.Name = "chkUseMSN";
            this.toolTip1.SetToolTip(this.chkUseMSN, resources.GetString("chkUseMSN.ToolTip"));
            this.chkUseMSN.UseVisualStyleBackColor = true;
            this.chkUseMSN.CheckedChanged += new System.EventHandler(this.chkUseMSN_CheckedChanged);
            // 
            // grpEmail
            // 
            this.grpEmail.Controls.Add(this.cmdTest);
            this.grpEmail.Controls.Add(this.txtEmailList);
            this.grpEmail.Controls.Add(this.label7);
            this.grpEmail.Controls.Add(this.txtEmailPassword);
            this.grpEmail.Controls.Add(this.label5);
            this.grpEmail.Controls.Add(this.txtEmailUsername);
            this.grpEmail.Controls.Add(this.label6);
            this.grpEmail.Controls.Add(this.txtEmailServerPort);
            this.grpEmail.Controls.Add(this.label4);
            this.grpEmail.Controls.Add(this.txtEmailServer);
            this.grpEmail.Controls.Add(this.label3);
            resources.ApplyResources(this.grpEmail, "grpEmail");
            this.grpEmail.Name = "grpEmail";
            this.grpEmail.TabStop = false;
            // 
            // txtEmailList
            // 
            resources.ApplyResources(this.txtEmailList, "txtEmailList");
            this.txtEmailList.Name = "txtEmailList";
            this.toolTip1.SetToolTip(this.txtEmailList, resources.GetString("txtEmailList.ToolTip"));
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // txtEmailPassword
            // 
            resources.ApplyResources(this.txtEmailPassword, "txtEmailPassword");
            this.txtEmailPassword.Name = "txtEmailPassword";
            this.toolTip1.SetToolTip(this.txtEmailPassword, resources.GetString("txtEmailPassword.ToolTip"));
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // txtEmailUsername
            // 
            resources.ApplyResources(this.txtEmailUsername, "txtEmailUsername");
            this.txtEmailUsername.Name = "txtEmailUsername";
            this.toolTip1.SetToolTip(this.txtEmailUsername, resources.GetString("txtEmailUsername.ToolTip"));
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // txtEmailServerPort
            // 
            resources.ApplyResources(this.txtEmailServerPort, "txtEmailServerPort");
            this.txtEmailServerPort.Maximum = new decimal(new int[] {
            65000,
            0,
            0,
            0});
            this.txtEmailServerPort.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtEmailServerPort.Name = "txtEmailServerPort";
            this.toolTip1.SetToolTip(this.txtEmailServerPort, resources.GetString("txtEmailServerPort.ToolTip"));
            this.txtEmailServerPort.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtEmailServer
            // 
            resources.ApplyResources(this.txtEmailServer, "txtEmailServer");
            this.txtEmailServer.Name = "txtEmailServer";
            this.toolTip1.SetToolTip(this.txtEmailServer, resources.GetString("txtEmailServer.ToolTip"));
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // cmdSave
            // 
            resources.ApplyResources(this.cmdSave, "cmdSave");
            this.cmdSave.Name = "cmdSave";
            this.toolTip1.SetToolTip(this.cmdSave, resources.GetString("cmdSave.ToolTip"));
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmdTest
            // 
            resources.ApplyResources(this.cmdTest, "cmdTest");
            this.cmdTest.Name = "cmdTest";
            this.toolTip1.SetToolTip(this.cmdTest, resources.GetString("cmdTest.ToolTip"));
            this.cmdTest.UseVisualStyleBackColor = true;
            this.cmdTest.Click += new System.EventHandler(this.cmdTest_Click);
            // 
            // uscCommunication
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.grpEmail);
            this.Controls.Add(this.grpMSN);
            this.Name = "uscCommunication";
            this.Load += new System.EventHandler(this.uscCommunication_Load);
            this.grpMSN.ResumeLayout(false);
            this.grpMSN.PerformLayout();
            this.grpEmail.ResumeLayout(false);
            this.grpEmail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmailServerPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpMSN;
        private System.Windows.Forms.GroupBox grpEmail;
        private System.Windows.Forms.TextBox txtMSNPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMSNUsername;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkUseMSN;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.NumericUpDown txtEmailServerPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtEmailServer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtEmailPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtEmailUsername;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtEmailList;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button cmdTest;
    }
}
