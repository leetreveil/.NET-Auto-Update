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
            this.btnCheckForUpdatesCustom = new System.Windows.Forms.Button();
            this.btnRollback = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCheckForUpdates
            // 
            this.btnCheckForUpdates.Location = new System.Drawing.Point(12, 11);
            this.btnCheckForUpdates.Name = "btnCheckForUpdates";
            this.btnCheckForUpdates.Size = new System.Drawing.Size(148, 55);
            this.btnCheckForUpdates.TabIndex = 0;
            this.btnCheckForUpdates.Text = "Check for updates";
            this.btnCheckForUpdates.UseVisualStyleBackColor = true;
            this.btnCheckForUpdates.Click += new System.EventHandler(this.btnCheckForUpdates_Click);
            // 
            // btnPrepareUpdates
            // 
            this.btnPrepareUpdates.Location = new System.Drawing.Point(12, 133);
            this.btnPrepareUpdates.Name = "btnPrepareUpdates";
            this.btnPrepareUpdates.Size = new System.Drawing.Size(148, 56);
            this.btnPrepareUpdates.TabIndex = 0;
            this.btnPrepareUpdates.Text = "Prepare updates";
            this.btnPrepareUpdates.UseVisualStyleBackColor = true;
            this.btnPrepareUpdates.Click += new System.EventHandler(this.btnPrepareUpdates_Click);
            // 
            // btnInstallUpdates
            // 
            this.btnInstallUpdates.Location = new System.Drawing.Point(286, 9);
            this.btnInstallUpdates.Name = "btnInstallUpdates";
            this.btnInstallUpdates.Size = new System.Drawing.Size(148, 57);
            this.btnInstallUpdates.TabIndex = 0;
            this.btnInstallUpdates.Text = "Install updates";
            this.btnInstallUpdates.UseVisualStyleBackColor = true;
            this.btnInstallUpdates.Click += new System.EventHandler(this.btnInstallUpdates_Click);
            // 
            // btnCheckForUpdatesCustom
            // 
            this.btnCheckForUpdatesCustom.Location = new System.Drawing.Point(12, 72);
            this.btnCheckForUpdatesCustom.Name = "btnCheckForUpdatesCustom";
            this.btnCheckForUpdatesCustom.Size = new System.Drawing.Size(148, 55);
            this.btnCheckForUpdatesCustom.TabIndex = 0;
            this.btnCheckForUpdatesCustom.Text = "Check for updates by custom feed";
            this.btnCheckForUpdatesCustom.UseVisualStyleBackColor = true;
            this.btnCheckForUpdatesCustom.Click += new System.EventHandler(this.btnCheckForUpdatesCustom_Click);
            // 
            // btnRollback
            // 
            this.btnRollback.Location = new System.Drawing.Point(286, 72);
            this.btnRollback.Name = "btnRollback";
            this.btnRollback.Size = new System.Drawing.Size(148, 57);
            this.btnRollback.TabIndex = 0;
            this.btnRollback.Text = "Rollback updates";
            this.btnRollback.UseVisualStyleBackColor = true;
            this.btnRollback.Click += new System.EventHandler(this.btnRollback_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Location = new System.Drawing.Point(286, 252);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(148, 38);
            this.btnQuit.TabIndex = 0;
            this.btnQuit.Text = "Quit";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 277);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Status";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 302);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnRollback);
            this.Controls.Add(this.btnInstallUpdates);
            this.Controls.Add(this.btnPrepareUpdates);
            this.Controls.Add(this.btnCheckForUpdatesCustom);
            this.Controls.Add(this.btnCheckForUpdates);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCheckForUpdates;
        private System.Windows.Forms.Button btnPrepareUpdates;
        private System.Windows.Forms.Button btnInstallUpdates;
        private System.Windows.Forms.Button btnCheckForUpdatesCustom;
        private System.Windows.Forms.Button btnRollback;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Label lblStatus;
    }
}

