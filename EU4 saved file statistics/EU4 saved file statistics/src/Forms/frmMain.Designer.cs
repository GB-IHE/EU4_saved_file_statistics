namespace EU4_saved_file_statistics
{
    partial class frmMain
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
            this.btnExportStats = new System.Windows.Forms.Button();
            this.lstFiles = new System.Windows.Forms.ListBox();
            this.btnBrowseFiles = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblListTitle = new System.Windows.Forms.Label();
            this.prgTotalProgress = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnExportStats
            // 
            this.btnExportStats.Enabled = false;
            this.btnExportStats.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportStats.Location = new System.Drawing.Point(949, 109);
            this.btnExportStats.Margin = new System.Windows.Forms.Padding(4);
            this.btnExportStats.Name = "btnExportStats";
            this.btnExportStats.Size = new System.Drawing.Size(194, 62);
            this.btnExportStats.TabIndex = 2;
            this.btnExportStats.Text = "Export statistics from files";
            this.btnExportStats.UseVisualStyleBackColor = true;
            this.btnExportStats.Click += new System.EventHandler(this.btnExportStats_Click);
            // 
            // lstFiles
            // 
            this.lstFiles.AllowDrop = true;
            this.lstFiles.FormattingEnabled = true;
            this.lstFiles.HorizontalScrollbar = true;
            this.lstFiles.ItemHeight = 16;
            this.lstFiles.Location = new System.Drawing.Point(17, 39);
            this.lstFiles.Margin = new System.Windows.Forms.Padding(4);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstFiles.Size = new System.Drawing.Size(924, 388);
            this.lstFiles.TabIndex = 3;
            this.lstFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstFiles_KeyDown);
            // 
            // btnBrowseFiles
            // 
            this.btnBrowseFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowseFiles.Location = new System.Drawing.Point(949, 39);
            this.btnBrowseFiles.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowseFiles.Name = "btnBrowseFiles";
            this.btnBrowseFiles.Size = new System.Drawing.Size(194, 62);
            this.btnBrowseFiles.TabIndex = 4;
            this.btnBrowseFiles.Text = "Browse files...";
            this.btnBrowseFiles.UseVisualStyleBackColor = true;
            this.btnBrowseFiles.Click += new System.EventHandler(this.btnBrowseFiles_Click);
            // 
            // btnClear
            // 
            this.btnClear.Enabled = false;
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(949, 179);
            this.btnClear.Margin = new System.Windows.Forms.Padding(4);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(194, 62);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear list";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblListTitle
            // 
            this.lblListTitle.AutoSize = true;
            this.lblListTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblListTitle.Location = new System.Drawing.Point(16, 11);
            this.lblListTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblListTitle.Name = "lblListTitle";
            this.lblListTitle.Size = new System.Drawing.Size(156, 24);
            this.lblListTitle.TabIndex = 6;
            this.lblListTitle.Text = "List of save files";
            // 
            // prgTotalProgress
            // 
            this.prgTotalProgress.Location = new System.Drawing.Point(17, 434);
            this.prgTotalProgress.Name = "prgTotalProgress";
            this.prgTotalProgress.Size = new System.Drawing.Size(924, 30);
            this.prgTotalProgress.Step = 100;
            this.prgTotalProgress.TabIndex = 7;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(17, 467);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(101, 16);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Waiting for work";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1158, 498);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.prgTotalProgress);
            this.Controls.Add(this.lblListTitle);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnBrowseFiles);
            this.Controls.Add(this.lstFiles);
            this.Controls.Add(this.btnExportStats);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmMain";
            this.Text = "frmMain";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnExportStats;
        private System.Windows.Forms.ListBox lstFiles;
        private System.Windows.Forms.Button btnBrowseFiles;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblListTitle;
        public System.Windows.Forms.ProgressBar prgTotalProgress;
        private System.Windows.Forms.Label lblStatus;
    }
}

