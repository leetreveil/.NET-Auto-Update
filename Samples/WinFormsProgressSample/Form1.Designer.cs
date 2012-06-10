namespace WinFormsProgressSample
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
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.btnStart = new System.Windows.Forms.Button();
			this.lblOverview = new System.Windows.Forms.Label();
			this.lblDetails = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(12, 74);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(480, 28);
			this.progressBar1.TabIndex = 0;
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(499, 75);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(81, 27);
			this.btnStart.TabIndex = 1;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// lblOverview
			// 
			this.lblOverview.AutoSize = true;
			this.lblOverview.Location = new System.Drawing.Point(12, 9);
			this.lblOverview.Name = "lblOverview";
			this.lblOverview.Size = new System.Drawing.Size(185, 17);
			this.lblOverview.TabIndex = 2;
			this.lblOverview.Text = "Click the start button to start";
			// 
			// lblDetails
			// 
			this.lblDetails.AutoSize = true;
			this.lblDetails.Location = new System.Drawing.Point(12, 36);
			this.lblDetails.Name = "lblDetails";
			this.lblDetails.Size = new System.Drawing.Size(119, 17);
			this.lblDetails.TabIndex = 2;
			this.lblDetails.Text = "Waiting on user...";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(587, 115);
			this.Controls.Add(this.lblDetails);
			this.Controls.Add(this.lblOverview);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.progressBar1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Label lblOverview;
		private System.Windows.Forms.Label lblDetails;
	}
}

