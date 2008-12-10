namespace Rhabot.Controls
{
    partial class uscItemMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscItemMain));
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdSave = new System.Windows.Forms.Button();
            this.txtMule = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabKeep = new System.Windows.Forms.TabPage();
            this.uscItemList5 = new Rhabot.Controls.uscItemList();
            this.tabMail = new System.Windows.Forms.TabPage();
            this.uscItemList1 = new Rhabot.Controls.uscItemList();
            this.tabSell = new System.Windows.Forms.TabPage();
            this.uscItemList2 = new Rhabot.Controls.uscItemList();
            this.tabDelete = new System.Windows.Forms.TabPage();
            this.uscItemList3 = new Rhabot.Controls.uscItemList();
            this.tabDisenchant = new System.Windows.Forms.TabPage();
            this.uscItemList4 = new Rhabot.Controls.uscItemList();
            this.tabOpen = new System.Windows.Forms.TabPage();
            this.uscItemList6 = new Rhabot.Controls.uscItemList();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabKeep.SuspendLayout();
            this.tabMail.SuspendLayout();
            this.tabSell.SuspendLayout();
            this.tabDelete.SuspendLayout();
            this.tabDisenchant.SuspendLayout();
            this.tabOpen.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdSave);
            this.panel1.Controls.Add(this.txtMule);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // cmdSave
            // 
            resources.ApplyResources(this.cmdSave, "cmdSave");
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // txtMule
            // 
            resources.ApplyResources(this.txtMule, "txtMule");
            this.txtMule.Name = "txtMule";
            this.toolTip1.SetToolTip(this.txtMule, resources.GetString("txtMule.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabKeep);
            this.tabControl1.Controls.Add(this.tabMail);
            this.tabControl1.Controls.Add(this.tabSell);
            this.tabControl1.Controls.Add(this.tabDelete);
            this.tabControl1.Controls.Add(this.tabDisenchant);
            this.tabControl1.Controls.Add(this.tabOpen);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabKeep
            // 
            this.tabKeep.Controls.Add(this.uscItemList5);
            resources.ApplyResources(this.tabKeep, "tabKeep");
            this.tabKeep.Name = "tabKeep";
            this.tabKeep.UseVisualStyleBackColor = true;
            // 
            // uscItemList5
            // 
            resources.ApplyResources(this.uscItemList5, "uscItemList5");
            this.uscItemList5.ItemListType = Rhabot.Controls.uscItemList.EItemListType.Keep;
            this.uscItemList5.Name = "uscItemList5";
            // 
            // tabMail
            // 
            this.tabMail.Controls.Add(this.uscItemList1);
            resources.ApplyResources(this.tabMail, "tabMail");
            this.tabMail.Name = "tabMail";
            this.tabMail.UseVisualStyleBackColor = true;
            // 
            // uscItemList1
            // 
            resources.ApplyResources(this.uscItemList1, "uscItemList1");
            this.uscItemList1.ItemListType = Rhabot.Controls.uscItemList.EItemListType.Mail;
            this.uscItemList1.Name = "uscItemList1";
            // 
            // tabSell
            // 
            this.tabSell.Controls.Add(this.uscItemList2);
            resources.ApplyResources(this.tabSell, "tabSell");
            this.tabSell.Name = "tabSell";
            this.tabSell.UseVisualStyleBackColor = true;
            // 
            // uscItemList2
            // 
            resources.ApplyResources(this.uscItemList2, "uscItemList2");
            this.uscItemList2.ItemListType = Rhabot.Controls.uscItemList.EItemListType.Sell;
            this.uscItemList2.Name = "uscItemList2";
            // 
            // tabDelete
            // 
            this.tabDelete.Controls.Add(this.uscItemList3);
            resources.ApplyResources(this.tabDelete, "tabDelete");
            this.tabDelete.Name = "tabDelete";
            this.tabDelete.UseVisualStyleBackColor = true;
            // 
            // uscItemList3
            // 
            resources.ApplyResources(this.uscItemList3, "uscItemList3");
            this.uscItemList3.ItemListType = Rhabot.Controls.uscItemList.EItemListType.Delete;
            this.uscItemList3.Name = "uscItemList3";
            // 
            // tabDisenchant
            // 
            this.tabDisenchant.Controls.Add(this.uscItemList4);
            resources.ApplyResources(this.tabDisenchant, "tabDisenchant");
            this.tabDisenchant.Name = "tabDisenchant";
            this.tabDisenchant.UseVisualStyleBackColor = true;
            // 
            // uscItemList4
            // 
            resources.ApplyResources(this.uscItemList4, "uscItemList4");
            this.uscItemList4.ItemListType = Rhabot.Controls.uscItemList.EItemListType.Disenchant;
            this.uscItemList4.Name = "uscItemList4";
            // 
            // tabOpen
            // 
            this.tabOpen.Controls.Add(this.uscItemList6);
            resources.ApplyResources(this.tabOpen, "tabOpen");
            this.tabOpen.Name = "tabOpen";
            this.tabOpen.UseVisualStyleBackColor = true;
            // 
            // uscItemList6
            // 
            resources.ApplyResources(this.uscItemList6, "uscItemList6");
            this.uscItemList6.ItemListType = Rhabot.Controls.uscItemList.EItemListType.Open;
            this.uscItemList6.Name = "uscItemList6";
            // 
            // uscItemMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Name = "uscItemMain";
            this.Load += new System.EventHandler(this.uscItemMain_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabKeep.ResumeLayout(false);
            this.tabMail.ResumeLayout(false);
            this.tabSell.ResumeLayout(false);
            this.tabDelete.ResumeLayout(false);
            this.tabDisenchant.ResumeLayout(false);
            this.tabOpen.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.TextBox txtMule;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabMail;
        private System.Windows.Forms.TabPage tabSell;
        private System.Windows.Forms.TabPage tabDelete;
        private uscItemList uscItemList1;
        private uscItemList uscItemList2;
        private uscItemList uscItemList3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabPage tabDisenchant;
        private uscItemList uscItemList4;
        private System.Windows.Forms.TabPage tabKeep;
        private uscItemList uscItemList5;
        private System.Windows.Forms.TabPage tabOpen;
        private uscItemList uscItemList6;

    }
}
