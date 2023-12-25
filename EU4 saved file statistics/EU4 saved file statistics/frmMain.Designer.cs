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
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnGetStats = new System.Windows.Forms.Button();
            this.btnExportStats = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(107, 176);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(170, 70);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Load file";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnGetStats
            // 
            this.btnGetStats.Location = new System.Drawing.Point(314, 176);
            this.btnGetStats.Name = "btnGetStats";
            this.btnGetStats.Size = new System.Drawing.Size(170, 70);
            this.btnGetStats.TabIndex = 1;
            this.btnGetStats.Text = "Get statistics  from file";
            this.btnGetStats.UseVisualStyleBackColor = true;
            this.btnGetStats.Click += new System.EventHandler(this.btnGetStats_Click);
            // 
            // btnExportStats
            // 
            this.btnExportStats.Location = new System.Drawing.Point(518, 176);
            this.btnExportStats.Name = "btnExportStats";
            this.btnExportStats.Size = new System.Drawing.Size(170, 70);
            this.btnExportStats.TabIndex = 2;
            this.btnExportStats.Text = "Export statistics  from file";
            this.btnExportStats.UseVisualStyleBackColor = true;
            this.btnExportStats.Click += new System.EventHandler(this.btnExportStats_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnExportStats);
            this.Controls.Add(this.btnGetStats);
            this.Controls.Add(this.btnLoad);
            this.Name = "frmMain";
            this.Text = "frmMain";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnGetStats;
        private System.Windows.Forms.Button btnExportStats;
    }
}

