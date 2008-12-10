namespace ISXBotHelper.Controls
{
    partial class uscQuests
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
            this.label1 = new System.Windows.Forms.Label();
            this.grdvQuests = new System.Windows.Forms.DataGridView();
            this.clmnChecked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clmnQuestName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmdDoTheseQuests = new System.Windows.Forms.Button();
            this.cmdRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grdvQuests)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Quests In Log";
            // 
            // grdvQuests
            // 
            this.grdvQuests.AllowUserToAddRows = false;
            this.grdvQuests.AllowUserToDeleteRows = false;
            this.grdvQuests.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdvQuests.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmnChecked,
            this.clmnQuestName});
            this.grdvQuests.Location = new System.Drawing.Point(6, 25);
            this.grdvQuests.Name = "grdvQuests";
            this.grdvQuests.Size = new System.Drawing.Size(611, 276);
            this.grdvQuests.TabIndex = 1;
            // 
            // clmnChecked
            // 
            this.clmnChecked.HeaderText = "*";
            this.clmnChecked.Name = "clmnChecked";
            this.clmnChecked.Width = 50;
            // 
            // clmnQuestName
            // 
            this.clmnQuestName.HeaderText = "Quest Name";
            this.clmnQuestName.Name = "clmnQuestName";
            this.clmnQuestName.ReadOnly = true;
            this.clmnQuestName.Width = 350;
            // 
            // cmdDoTheseQuests
            // 
            this.cmdDoTheseQuests.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDoTheseQuests.Location = new System.Drawing.Point(514, 316);
            this.cmdDoTheseQuests.Name = "cmdDoTheseQuests";
            this.cmdDoTheseQuests.Size = new System.Drawing.Size(103, 64);
            this.cmdDoTheseQuests.TabIndex = 2;
            this.cmdDoTheseQuests.Text = "Do These Quests";
            this.cmdDoTheseQuests.UseVisualStyleBackColor = true;
            this.cmdDoTheseQuests.Click += new System.EventHandler(this.cmdDoTheseQuests_Click);
            // 
            // cmdRefresh
            // 
            this.cmdRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdRefresh.Location = new System.Drawing.Point(6, 316);
            this.cmdRefresh.Name = "cmdRefresh";
            this.cmdRefresh.Size = new System.Drawing.Size(103, 64);
            this.cmdRefresh.TabIndex = 3;
            this.cmdRefresh.Text = "Refresh Quest List";
            this.cmdRefresh.UseVisualStyleBackColor = true;
            this.cmdRefresh.Click += new System.EventHandler(this.cmdRefresh_Click);
            // 
            // uscQuests
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmdRefresh);
            this.Controls.Add(this.cmdDoTheseQuests);
            this.Controls.Add(this.grdvQuests);
            this.Controls.Add(this.label1);
            this.Name = "uscQuests";
            this.Size = new System.Drawing.Size(643, 400);
            this.Load += new System.EventHandler(this.uscQuests_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdvQuests)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView grdvQuests;
        private System.Windows.Forms.Button cmdDoTheseQuests;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clmnChecked;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnQuestName;
        private System.Windows.Forms.Button cmdRefresh;
    }
}
