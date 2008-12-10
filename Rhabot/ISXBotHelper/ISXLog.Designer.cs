namespace ISXBotHelper
{
    partial class ISXLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ISXLog));
            this.txtLogText = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // txtLogText
            // 
            this.txtLogText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLogText.Location = new System.Drawing.Point(0, 0);
            this.txtLogText.Name = "txtLogText";
            this.txtLogText.Size = new System.Drawing.Size(292, 273);
            this.txtLogText.TabIndex = 0;
            this.txtLogText.Text = string.Empty;
            // 
            // ISXLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.txtLogText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ISXLog";
            this.Text = "ISXLog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ISXLog_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.RichTextBox txtLogText;
    }
}