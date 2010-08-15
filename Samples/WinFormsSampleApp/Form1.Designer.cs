namespace WinFormsSampleApp
{
    partial class Form1
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
            this.btnCheckForUpdates = new System.Windows.Forms.Button();
            this.btnPrepareUpdates = new System.Windows.Forms.Button();
            this.btnInstallUpdates = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCheckForUpdates
            // 
            this.btnCheckForUpdates.Location = new System.Drawing.Point(12, 12);
            this.btnCheckForUpdates.Name = "btnCheckForUpdates";
            this.btnCheckForUpdates.Size = new System.Drawing.Size(125, 23);
            this.btnCheckForUpdates.TabIndex = 0;
            this.btnCheckForUpdates.Text = "Check for updates";
            this.btnCheckForUpdates.UseVisualStyleBackColor = true;
            this.btnCheckForUpdates.Click += new System.EventHandler(this.btnCheckForUpdates_Click);
            // 
            // btnPrepareUpdates
            // 
            this.btnPrepareUpdates.Location = new System.Drawing.Point(12, 41);
            this.btnPrepareUpdates.Name = "btnPrepareUpdates";
            this.btnPrepareUpdates.Size = new System.Drawing.Size(125, 23);
            this.btnPrepareUpdates.TabIndex = 0;
            this.btnPrepareUpdates.Text = "Prepare updates";
            this.btnPrepareUpdates.UseVisualStyleBackColor = true;
            this.btnPrepareUpdates.Click += new System.EventHandler(this.btnPrepareUpdates_Click);
            // 
            // btnInstallUpdates
            // 
            this.btnInstallUpdates.Location = new System.Drawing.Point(12, 70);
            this.btnInstallUpdates.Name = "btnInstallUpdates";
            this.btnInstallUpdates.Size = new System.Drawing.Size(125, 23);
            this.btnInstallUpdates.TabIndex = 0;
            this.btnInstallUpdates.Text = "Install updates";
            this.btnInstallUpdates.UseVisualStyleBackColor = true;
            this.btnInstallUpdates.Click += new System.EventHandler(this.btnInstallUpdates_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.btnInstallUpdates);
            this.Controls.Add(this.btnPrepareUpdates);
            this.Controls.Add(this.btnCheckForUpdates);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCheckForUpdates;
        private System.Windows.Forms.Button btnPrepareUpdates;
        private System.Windows.Forms.Button btnInstallUpdates;
    }
}

