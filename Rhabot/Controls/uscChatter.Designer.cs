namespace Rhabot.Controls
{
    partial class uscChatter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscChatter));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.trvSaveInput = new System.Windows.Forms.TreeView();
            this.txtInputMessages = new System.Windows.Forms.TextBox();
            this.grdOutputs = new System.Windows.Forms.DataGridView();
            this.cmdSaveMessage = new System.Windows.Forms.Button();
            this.cmdBuild = new System.Windows.Forms.Button();
            this.cmdLoad = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtChatHowTo = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.lblCurrentFile = new System.Windows.Forms.Label();
            this.clmOutputs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grdOutputs)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // trvSaveInput
            // 
            resources.ApplyResources(this.trvSaveInput, "trvSaveInput");
            this.trvSaveInput.Name = "trvSaveInput";
            this.toolTip1.SetToolTip(this.trvSaveInput, resources.GetString("trvSaveInput.ToolTip"));
            // 
            // txtInputMessages
            // 
            resources.ApplyResources(this.txtInputMessages, "txtInputMessages");
            this.txtInputMessages.Name = "txtInputMessages";
            this.toolTip1.SetToolTip(this.txtInputMessages, resources.GetString("txtInputMessages.ToolTip"));
            // 
            // grdOutputs
            // 
            this.grdOutputs.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            this.grdOutputs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdOutputs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmOutputs});
            resources.ApplyResources(this.grdOutputs, "grdOutputs");
            this.grdOutputs.Name = "grdOutputs";
            this.toolTip1.SetToolTip(this.grdOutputs, resources.GetString("grdOutputs.ToolTip"));
            this.grdOutputs.DoubleClick += new System.EventHandler(this.grdOutputs_DoubleClick);
            // 
            // cmdSaveMessage
            // 
            resources.ApplyResources(this.cmdSaveMessage, "cmdSaveMessage");
            this.cmdSaveMessage.Name = "cmdSaveMessage";
            this.toolTip1.SetToolTip(this.cmdSaveMessage, resources.GetString("cmdSaveMessage.ToolTip"));
            this.cmdSaveMessage.UseVisualStyleBackColor = true;
            this.cmdSaveMessage.Click += new System.EventHandler(this.cmdSaveMessage_Click);
            // 
            // cmdBuild
            // 
            resources.ApplyResources(this.cmdBuild, "cmdBuild");
            this.cmdBuild.Name = "cmdBuild";
            this.toolTip1.SetToolTip(this.cmdBuild, resources.GetString("cmdBuild.ToolTip"));
            this.cmdBuild.UseVisualStyleBackColor = true;
            this.cmdBuild.Click += new System.EventHandler(this.cmdBuild_Click);
            // 
            // cmdLoad
            // 
            resources.ApplyResources(this.cmdLoad, "cmdLoad");
            this.cmdLoad.Name = "cmdLoad";
            this.toolTip1.SetToolTip(this.cmdLoad, resources.GetString("cmdLoad.ToolTip"));
            this.cmdLoad.UseVisualStyleBackColor = true;
            this.cmdLoad.Click += new System.EventHandler(this.cmdLoad_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtChatHowTo);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // txtChatHowTo
            // 
            this.txtChatHowTo.BackColor = System.Drawing.Color.PaleTurquoise;
            resources.ApplyResources(this.txtChatHowTo, "txtChatHowTo");
            this.txtChatHowTo.Name = "txtChatHowTo";
            this.txtChatHowTo.ReadOnly = true;
            // 
            // lblCurrentFile
            // 
            resources.ApplyResources(this.lblCurrentFile, "lblCurrentFile");
            this.lblCurrentFile.Name = "lblCurrentFile";
            // 
            // clmOutputs
            // 
            resources.ApplyResources(this.clmOutputs, "clmOutputs");
            this.clmOutputs.Name = "clmOutputs";
            // 
            // uscChatter
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleTurquoise;
            this.Controls.Add(this.lblCurrentFile);
            this.Controls.Add(this.cmdLoad);
            this.Controls.Add(this.cmdBuild);
            this.Controls.Add(this.cmdSaveMessage);
            this.Controls.Add(this.grdOutputs);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtInputMessages);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trvSaveInput);
            this.Controls.Add(this.label1);
            this.Name = "uscChatter";
            this.Load += new System.EventHandler(this.uscChatter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdOutputs)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView trvSaveInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtInputMessages;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView grdOutputs;
        private System.Windows.Forms.Button cmdSaveMessage;
        private System.Windows.Forms.Button cmdBuild;
        private System.Windows.Forms.TextBox txtChatHowTo;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button cmdLoad;
        private System.Windows.Forms.Label lblCurrentFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmOutputs;
    }
}
