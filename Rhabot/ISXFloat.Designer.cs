namespace Rhabot
{
    partial class ISXFloat
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ISXFloat));
            this.cmdPause = new System.Windows.Forms.Button();
            this.cmdStop = new System.Windows.Forms.Button();
            this.lblDebug = new Rhabot.Threaded_Controls.ThreadLabel();
            this.SuspendLayout();
            // 
            // cmdPause
            // 
            resources.ApplyResources(this.cmdPause, "cmdPause");
            this.cmdPause.Name = "cmdPause";
            this.cmdPause.UseVisualStyleBackColor = true;
            this.cmdPause.Click += new System.EventHandler(this.cmdPause_Click);
            // 
            // cmdStop
            // 
            resources.ApplyResources(this.cmdStop, "cmdStop");
            this.cmdStop.Name = "cmdStop";
            this.cmdStop.UseVisualStyleBackColor = true;
            this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
            // 
            // lblDebug
            // 
            this.lblDebug.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblDebug, "lblDebug");
            this.lblDebug.Name = "lblDebug";
            // 
            // ISXFloat
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.Controls.Add(this.lblDebug);
            this.Controls.Add(this.cmdStop);
            this.Controls.Add(this.cmdPause);
            this.MaximizeBox = false;
            this.Name = "ISXFloat";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ISXFloat_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdPause;
        private System.Windows.Forms.Button cmdStop;
        private Threaded_Controls.ThreadLabel lblDebug;
    }
}