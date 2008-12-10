namespace Rhabot.Controls
{
    partial class uscItemList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscItemList));
            this.lblMessage = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lstBagList = new System.Windows.Forms.ListBox();
            this.lstItemList = new System.Windows.Forms.ListBox();
            this.lblItemList = new System.Windows.Forms.Label();
            this.cmdMove = new System.Windows.Forms.Button();
            this.cmdRemove = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmdRefresh = new System.Windows.Forms.Button();
            this.chkGreens = new System.Windows.Forms.CheckBox();
            this.chkWhite = new System.Windows.Forms.CheckBox();
            this.chkGreys = new System.Windows.Forms.CheckBox();
            this.chkBlues = new System.Windows.Forms.CheckBox();
            this.chkPurple = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            resources.ApplyResources(this.lblMessage, "lblMessage");
            this.lblMessage.Name = "lblMessage";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lstBagList
            // 
            this.lstBagList.FormattingEnabled = true;
            resources.ApplyResources(this.lstBagList, "lstBagList");
            this.lstBagList.Name = "lstBagList";
            this.lstBagList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstBagList.Sorted = true;
            this.toolTip1.SetToolTip(this.lstBagList, resources.GetString("lstBagList.ToolTip"));
            // 
            // lstItemList
            // 
            this.lstItemList.FormattingEnabled = true;
            resources.ApplyResources(this.lstItemList, "lstItemList");
            this.lstItemList.Name = "lstItemList";
            this.lstItemList.Sorted = true;
            // 
            // lblItemList
            // 
            resources.ApplyResources(this.lblItemList, "lblItemList");
            this.lblItemList.Name = "lblItemList";
            // 
            // cmdMove
            // 
            resources.ApplyResources(this.cmdMove, "cmdMove");
            this.cmdMove.Name = "cmdMove";
            this.toolTip1.SetToolTip(this.cmdMove, resources.GetString("cmdMove.ToolTip"));
            this.cmdMove.UseVisualStyleBackColor = true;
            this.cmdMove.Click += new System.EventHandler(this.cmdMove_Click);
            // 
            // cmdRemove
            // 
            resources.ApplyResources(this.cmdRemove, "cmdRemove");
            this.cmdRemove.Name = "cmdRemove";
            this.toolTip1.SetToolTip(this.cmdRemove, resources.GetString("cmdRemove.ToolTip"));
            this.cmdRemove.UseVisualStyleBackColor = true;
            this.cmdRemove.Click += new System.EventHandler(this.cmdRemove_Click);
            // 
            // cmdRefresh
            // 
            resources.ApplyResources(this.cmdRefresh, "cmdRefresh");
            this.cmdRefresh.Name = "cmdRefresh";
            this.toolTip1.SetToolTip(this.cmdRefresh, resources.GetString("cmdRefresh.ToolTip"));
            this.cmdRefresh.UseVisualStyleBackColor = true;
            this.cmdRefresh.Click += new System.EventHandler(this.cmdRefresh_Click);
            // 
            // chkGreens
            // 
            resources.ApplyResources(this.chkGreens, "chkGreens");
            this.chkGreens.Name = "chkGreens";
            this.chkGreens.UseVisualStyleBackColor = true;
            this.chkGreens.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // chkWhite
            // 
            resources.ApplyResources(this.chkWhite, "chkWhite");
            this.chkWhite.Name = "chkWhite";
            this.chkWhite.UseVisualStyleBackColor = true;
            this.chkWhite.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // chkGreys
            // 
            resources.ApplyResources(this.chkGreys, "chkGreys");
            this.chkGreys.Name = "chkGreys";
            this.chkGreys.UseVisualStyleBackColor = true;
            this.chkGreys.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // chkBlues
            // 
            resources.ApplyResources(this.chkBlues, "chkBlues");
            this.chkBlues.Name = "chkBlues";
            this.chkBlues.UseVisualStyleBackColor = true;
            this.chkBlues.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // chkPurple
            // 
            resources.ApplyResources(this.chkPurple, "chkPurple");
            this.chkPurple.Name = "chkPurple";
            this.chkPurple.UseVisualStyleBackColor = true;
            this.chkPurple.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // uscItemList
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmdRefresh);
            this.Controls.Add(this.chkPurple);
            this.Controls.Add(this.chkBlues);
            this.Controls.Add(this.chkGreys);
            this.Controls.Add(this.chkWhite);
            this.Controls.Add(this.chkGreens);
            this.Controls.Add(this.cmdRemove);
            this.Controls.Add(this.cmdMove);
            this.Controls.Add(this.lstItemList);
            this.Controls.Add(this.lblItemList);
            this.Controls.Add(this.lstBagList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblMessage);
            this.Name = "uscItemList";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ListBox lstBagList;
        private System.Windows.Forms.ToolTip toolTip1;
        public System.Windows.Forms.ListBox lstItemList;
        private System.Windows.Forms.Label lblItemList;
        private System.Windows.Forms.Button cmdMove;
        private System.Windows.Forms.Button cmdRemove;
        public System.Windows.Forms.CheckBox chkGreens;
        public System.Windows.Forms.CheckBox chkWhite;
        public System.Windows.Forms.CheckBox chkGreys;
        public System.Windows.Forms.CheckBox chkBlues;
        public System.Windows.Forms.CheckBox chkPurple;
        private System.Windows.Forms.Button cmdRefresh;
    }
}
