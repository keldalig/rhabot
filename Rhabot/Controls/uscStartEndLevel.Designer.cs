namespace Rhabot.Controls
{
    partial class uscStartEndLevel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscStartEndLevel));
            this.txtEndLevel = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.txtStartLevel = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStartLevel)).BeginInit();
            this.SuspendLayout();
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
            this.txtEndLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
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
            this.txtStartLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // uscStartEndLevel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.txtEndLevel);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.txtStartLevel);
            this.Controls.Add(this.label15);
            this.Name = "uscStartEndLevel";
            this.Load += new System.EventHandler(this.uscStartEndLevel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtEndLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStartLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown txtEndLevel;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown txtStartLevel;
        private System.Windows.Forms.Label label15;
    }
}
