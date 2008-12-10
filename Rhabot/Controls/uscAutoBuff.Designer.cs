using System.ComponentModel;
using System.Windows.Forms;

namespace Rhabot
{
    partial class uscAutoBuff
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscAutoBuff));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.chkAutoBuff = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtHealSpell = new System.Windows.Forms.TextBox();
            this.txtHealthPercent = new System.Windows.Forms.NumericUpDown();
            this.grdBuffs = new System.Windows.Forms.DataGridView();
            this.clmBuffs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.txtHealthPercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdBuffs)).BeginInit();
            this.SuspendLayout();
            // 
            // chkAutoBuff
            // 
            this.chkAutoBuff.AccessibleDescription = null;
            this.chkAutoBuff.AccessibleName = null;
            resources.ApplyResources(this.chkAutoBuff, "chkAutoBuff");
            this.chkAutoBuff.BackgroundImage = null;
            this.chkAutoBuff.Name = "chkAutoBuff";
            this.chkAutoBuff.Text = global::ISXBotHelper.Properties.Resources.AutoBuff_NonParty_Players;
            this.toolTip1.SetToolTip(this.chkAutoBuff, resources.GetString("chkAutoBuff.ToolTip"));
            this.chkAutoBuff.UseVisualStyleBackColor = true;
            this.chkAutoBuff.CheckedChanged += new System.EventHandler(this.chkAutoBuff_CheckedChanged);
            // 
            // txtHealSpell
            // 
            this.txtHealSpell.AccessibleDescription = null;
            this.txtHealSpell.AccessibleName = null;
            resources.ApplyResources(this.txtHealSpell, "txtHealSpell");
            this.txtHealSpell.BackgroundImage = null;
            this.txtHealSpell.Font = null;
            this.txtHealSpell.Name = "txtHealSpell";
            this.toolTip1.SetToolTip(this.txtHealSpell, resources.GetString("txtHealSpell.ToolTip"));
            // 
            // txtHealthPercent
            // 
            this.txtHealthPercent.AccessibleDescription = null;
            this.txtHealthPercent.AccessibleName = null;
            resources.ApplyResources(this.txtHealthPercent, "txtHealthPercent");
            this.txtHealthPercent.Font = null;
            this.txtHealthPercent.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.txtHealthPercent.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtHealthPercent.Name = "txtHealthPercent";
            this.toolTip1.SetToolTip(this.txtHealthPercent, resources.GetString("txtHealthPercent.ToolTip"));
            this.txtHealthPercent.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // grdBuffs
            // 
            this.grdBuffs.AccessibleDescription = null;
            this.grdBuffs.AccessibleName = null;
            resources.ApplyResources(this.grdBuffs, "grdBuffs");
            this.grdBuffs.BackgroundImage = null;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdBuffs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.grdBuffs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdBuffs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmBuffs});
            this.grdBuffs.Font = null;
            this.grdBuffs.Name = "grdBuffs";
            this.toolTip1.SetToolTip(this.grdBuffs, resources.GetString("grdBuffs.ToolTip"));
            this.grdBuffs.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdBuffs_CellDoubleClick);
            // 
            // clmBuffs
            // 
            resources.ApplyResources(this.clmBuffs, "clmBuffs");
            this.clmBuffs.Name = "clmBuffs";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // cmdSave
            // 
            this.cmdSave.AccessibleDescription = null;
            this.cmdSave.AccessibleName = null;
            resources.ApplyResources(this.cmdSave, "cmdSave");
            this.cmdSave.BackgroundImage = null;
            this.cmdSave.Font = null;
            this.cmdSave.Name = "cmdSave";
            this.toolTip1.SetToolTip(this.cmdSave, resources.GetString("cmdSave.ToolTip"));
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // uscAutoBuff
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.grdBuffs);
            this.Controls.Add(this.txtHealthPercent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtHealSpell);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkAutoBuff);
            this.Font = null;
            this.Name = "uscAutoBuff";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.uscAutoBuff_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtHealthPercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdBuffs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox chkAutoBuff;
        private ToolTip toolTip1;
        private Label label1;
        private TextBox txtHealSpell;
        private Label label2;
        private NumericUpDown txtHealthPercent;
        private DataGridView grdBuffs;
        private Button cmdSave;
        private DataGridViewTextBoxColumn clmBuffs;
    }
}
