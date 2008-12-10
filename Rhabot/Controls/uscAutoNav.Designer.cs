namespace Rhabot
{
    partial class uscAutoNav
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscAutoNav));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmdRemove = new System.Windows.Forms.Button();
            this.cmdAdd = new System.Windows.Forms.Button();
            this.cmbPathName = new System.Windows.Forms.ComboBox();
            this.cmdSaveItem = new System.Windows.Forms.Button();
            this.chkCanFly = new System.Windows.Forms.CheckBox();
            this.cmdLoad = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lstvItems = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblPathName = new System.Windows.Forms.Label();
            this.txtZ = new System.Windows.Forms.TextBox();
            this.lblZ = new System.Windows.Forms.Label();
            this.txtY = new System.Windows.Forms.TextBox();
            this.lblY = new System.Windows.Forms.Label();
            this.txtX = new System.Windows.Forms.TextBox();
            this.lblX = new System.Windows.Forms.Label();
            this.txtQuantity = new System.Windows.Forms.NumericUpDown();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.cmbZoneName = new System.Windows.Forms.ComboBox();
            this.lblZoneName = new System.Windows.Forms.Label();
            this.cmbNPCName = new System.Windows.Forms.ComboBox();
            this.lblNPCName = new System.Windows.Forms.Label();
            this.trvActionPlan = new System.Windows.Forms.TreeView();
            this.cmdSave = new System.Windows.Forms.Button();
            this.txtPlanName = new System.Windows.Forms.TextBox();
            this.lblMessage = new System.Windows.Forms.Label();
            this.cmdNewPlan = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtQuantity)).BeginInit();
            this.SuspendLayout();
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 30000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // cmdRemove
            // 
            resources.ApplyResources(this.cmdRemove, "cmdRemove");
            this.cmdRemove.Name = "cmdRemove";
            this.toolTip1.SetToolTip(this.cmdRemove, resources.GetString("cmdRemove.ToolTip"));
            this.cmdRemove.UseVisualStyleBackColor = true;
            this.cmdRemove.Click += new System.EventHandler(this.cmdRemove_Click);
            // 
            // cmdAdd
            // 
            resources.ApplyResources(this.cmdAdd, "cmdAdd");
            this.cmdAdd.Name = "cmdAdd";
            this.toolTip1.SetToolTip(this.cmdAdd, resources.GetString("cmdAdd.ToolTip"));
            this.cmdAdd.UseVisualStyleBackColor = true;
            this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
            // 
            // cmbPathName
            // 
            this.cmbPathName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPathName.FormattingEnabled = true;
            resources.ApplyResources(this.cmbPathName, "cmbPathName");
            this.cmbPathName.Name = "cmbPathName";
            this.toolTip1.SetToolTip(this.cmbPathName, resources.GetString("cmbPathName.ToolTip"));
            // 
            // cmdSaveItem
            // 
            resources.ApplyResources(this.cmdSaveItem, "cmdSaveItem");
            this.cmdSaveItem.Name = "cmdSaveItem";
            this.toolTip1.SetToolTip(this.cmdSaveItem, resources.GetString("cmdSaveItem.ToolTip"));
            this.cmdSaveItem.UseVisualStyleBackColor = true;
            this.cmdSaveItem.Click += new System.EventHandler(this.cmdSaveItem_Click);
            // 
            // chkCanFly
            // 
            resources.ApplyResources(this.chkCanFly, "chkCanFly");
            this.chkCanFly.Name = "chkCanFly";
            this.toolTip1.SetToolTip(this.chkCanFly, resources.GetString("chkCanFly.ToolTip"));
            this.chkCanFly.UseVisualStyleBackColor = true;
            // 
            // cmdLoad
            // 
            resources.ApplyResources(this.cmdLoad, "cmdLoad");
            this.cmdLoad.Name = "cmdLoad";
            this.toolTip1.SetToolTip(this.cmdLoad, resources.GetString("cmdLoad.ToolTip"));
            this.cmdLoad.UseVisualStyleBackColor = true;
            this.cmdLoad.Click += new System.EventHandler(this.cmdLoad_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lstvItems
            // 
            this.lstvItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstvItems.FullRowSelect = true;
            this.lstvItems.HideSelection = false;
            resources.ApplyResources(this.lstvItems, "lstvItems");
            this.lstvItems.MultiSelect = false;
            this.lstvItems.Name = "lstvItems";
            this.lstvItems.ShowItemToolTips = true;
            this.lstvItems.UseCompatibleStateImageBehavior = false;
            this.lstvItems.View = System.Windows.Forms.View.Details;
            this.lstvItems.DoubleClick += new System.EventHandler(this.lstvItems_DoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkCanFly);
            this.groupBox1.Controls.Add(this.cmdSaveItem);
            this.groupBox1.Controls.Add(this.cmbPathName);
            this.groupBox1.Controls.Add(this.lblPathName);
            this.groupBox1.Controls.Add(this.txtZ);
            this.groupBox1.Controls.Add(this.lblZ);
            this.groupBox1.Controls.Add(this.txtY);
            this.groupBox1.Controls.Add(this.lblY);
            this.groupBox1.Controls.Add(this.txtX);
            this.groupBox1.Controls.Add(this.lblX);
            this.groupBox1.Controls.Add(this.txtQuantity);
            this.groupBox1.Controls.Add(this.lblQuantity);
            this.groupBox1.Controls.Add(this.cmbZoneName);
            this.groupBox1.Controls.Add(this.lblZoneName);
            this.groupBox1.Controls.Add(this.cmbNPCName);
            this.groupBox1.Controls.Add(this.lblNPCName);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // lblPathName
            // 
            resources.ApplyResources(this.lblPathName, "lblPathName");
            this.lblPathName.Name = "lblPathName";
            // 
            // txtZ
            // 
            resources.ApplyResources(this.txtZ, "txtZ");
            this.txtZ.Name = "txtZ";
            // 
            // lblZ
            // 
            resources.ApplyResources(this.lblZ, "lblZ");
            this.lblZ.Name = "lblZ";
            // 
            // txtY
            // 
            resources.ApplyResources(this.txtY, "txtY");
            this.txtY.Name = "txtY";
            // 
            // lblY
            // 
            resources.ApplyResources(this.lblY, "lblY");
            this.lblY.Name = "lblY";
            // 
            // txtX
            // 
            resources.ApplyResources(this.txtX, "txtX");
            this.txtX.Name = "txtX";
            // 
            // lblX
            // 
            resources.ApplyResources(this.lblX, "lblX");
            this.lblX.Name = "lblX";
            // 
            // txtQuantity
            // 
            resources.ApplyResources(this.txtQuantity, "txtQuantity");
            this.txtQuantity.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.txtQuantity.Name = "txtQuantity";
            // 
            // lblQuantity
            // 
            resources.ApplyResources(this.lblQuantity, "lblQuantity");
            this.lblQuantity.Name = "lblQuantity";
            // 
            // cmbZoneName
            // 
            this.cmbZoneName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZoneName.FormattingEnabled = true;
            resources.ApplyResources(this.cmbZoneName, "cmbZoneName");
            this.cmbZoneName.Name = "cmbZoneName";
            this.cmbZoneName.SelectedIndexChanged += new System.EventHandler(this.cmbZoneName_SelectedIndexChanged);
            // 
            // lblZoneName
            // 
            resources.ApplyResources(this.lblZoneName, "lblZoneName");
            this.lblZoneName.Name = "lblZoneName";
            // 
            // cmbNPCName
            // 
            this.cmbNPCName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNPCName.FormattingEnabled = true;
            resources.ApplyResources(this.cmbNPCName, "cmbNPCName");
            this.cmbNPCName.Name = "cmbNPCName";
            this.cmbNPCName.SelectedIndexChanged += new System.EventHandler(this.cmbNPCName_SelectedIndexChanged);
            // 
            // lblNPCName
            // 
            resources.ApplyResources(this.lblNPCName, "lblNPCName");
            this.lblNPCName.Name = "lblNPCName";
            // 
            // trvActionPlan
            // 
            this.trvActionPlan.FullRowSelect = true;
            this.trvActionPlan.HideSelection = false;
            resources.ApplyResources(this.trvActionPlan, "trvActionPlan");
            this.trvActionPlan.Name = "trvActionPlan";
            this.trvActionPlan.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvActionPlan_AfterSelect);
            // 
            // cmdSave
            // 
            resources.ApplyResources(this.cmdSave, "cmdSave");
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // txtPlanName
            // 
            resources.ApplyResources(this.txtPlanName, "txtPlanName");
            this.txtPlanName.Name = "txtPlanName";
            // 
            // lblMessage
            // 
            resources.ApplyResources(this.lblMessage, "lblMessage");
            this.lblMessage.Name = "lblMessage";
            // 
            // cmdNewPlan
            // 
            resources.ApplyResources(this.cmdNewPlan, "cmdNewPlan");
            this.cmdNewPlan.Name = "cmdNewPlan";
            this.cmdNewPlan.UseVisualStyleBackColor = true;
            this.cmdNewPlan.Click += new System.EventHandler(this.cmdNewPlan_Click);
            // 
            // uscAutoNav
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmdNewPlan);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.txtPlanName);
            this.Controls.Add(this.cmdLoad);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.trvActionPlan);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lstvItems);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdRemove);
            this.Controls.Add(this.cmdAdd);
            this.Controls.Add(this.label2);
            this.Name = "uscAutoNav";
            this.Load += new System.EventHandler(this.uscAutoNav_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtQuantity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cmdRemove;
        private System.Windows.Forms.Button cmdAdd;
        private System.Windows.Forms.ListView lstvItems;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView trvActionPlan;
        private System.Windows.Forms.ComboBox cmbNPCName;
        private System.Windows.Forms.Label lblNPCName;
        private System.Windows.Forms.ComboBox cmbZoneName;
        private System.Windows.Forms.Label lblZoneName;
        private System.Windows.Forms.NumericUpDown txtQuantity;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.TextBox txtZ;
        private System.Windows.Forms.Label lblZ;
        private System.Windows.Forms.TextBox txtY;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.ComboBox cmbPathName;
        private System.Windows.Forms.Label lblPathName;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Button cmdSaveItem;
        private System.Windows.Forms.CheckBox chkCanFly;
        private System.Windows.Forms.Button cmdLoad;
        private System.Windows.Forms.TextBox txtPlanName;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button cmdNewPlan;
    }
}
