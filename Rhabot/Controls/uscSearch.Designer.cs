namespace Rhabot.Controls
{
    partial class uscSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscSearch));
            this.label1 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.cmdSearch = new System.Windows.Forms.Button();
            this.lstItemList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdTarget = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmdPickup = new System.Windows.Forms.Button();
            this.cmdUse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtSearch
            // 
            resources.ApplyResources(this.txtSearch, "txtSearch");
            this.txtSearch.Name = "txtSearch";
            // 
            // cmdSearch
            // 
            resources.ApplyResources(this.cmdSearch, "cmdSearch");
            this.cmdSearch.Name = "cmdSearch";
            this.cmdSearch.UseVisualStyleBackColor = true;
            this.cmdSearch.Click += new System.EventHandler(this.cmdSearch_Click);
            // 
            // lstItemList
            // 
            this.lstItemList.FormattingEnabled = true;
            resources.ApplyResources(this.lstItemList, "lstItemList");
            this.lstItemList.Name = "lstItemList";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // cmdTarget
            // 
            resources.ApplyResources(this.cmdTarget, "cmdTarget");
            this.cmdTarget.Name = "cmdTarget";
            this.toolTip1.SetToolTip(this.cmdTarget, resources.GetString("cmdTarget.ToolTip"));
            this.cmdTarget.UseVisualStyleBackColor = true;
            this.cmdTarget.Click += new System.EventHandler(this.cmdTarget_Click);
            // 
            // cmdPickup
            // 
            resources.ApplyResources(this.cmdPickup, "cmdPickup");
            this.cmdPickup.Name = "cmdPickup";
            this.toolTip1.SetToolTip(this.cmdPickup, resources.GetString("cmdPickup.ToolTip"));
            this.cmdPickup.UseVisualStyleBackColor = true;
            this.cmdPickup.Click += new System.EventHandler(this.cmdPickup_Click);
            // 
            // cmdUse
            // 
            resources.ApplyResources(this.cmdUse, "cmdUse");
            this.cmdUse.Name = "cmdUse";
            this.toolTip1.SetToolTip(this.cmdUse, resources.GetString("cmdUse.ToolTip"));
            this.cmdUse.UseVisualStyleBackColor = true;
            this.cmdUse.Click += new System.EventHandler(this.cmdUse_Click);
            // 
            // uscSearch
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RosyBrown;
            this.Controls.Add(this.cmdUse);
            this.Controls.Add(this.cmdPickup);
            this.Controls.Add(this.cmdTarget);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstItemList);
            this.Controls.Add(this.cmdSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.label1);
            this.Name = "uscSearch";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button cmdSearch;
        private System.Windows.Forms.ListBox lstItemList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cmdTarget;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button cmdPickup;
        private System.Windows.Forms.Button cmdUse;
    }
}
