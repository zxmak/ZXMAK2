namespace ZXMAK2.Host.WinForms.Views.Configuration.Devices
{
	partial class CtlSettingsGenericSound
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
			this.groupBox = new System.Windows.Forms.GroupBox();
			this.lblVolume = new System.Windows.Forms.Label();
			this.trkVolume = new System.Windows.Forms.TrackBar();
			this.txtDevice = new System.Windows.Forms.TextBox();
			this.lblDevice = new System.Windows.Forms.Label();
			this.txtDescription = new System.Windows.Forms.TextBox();
			this.lblDescription = new System.Windows.Forms.Label();
			this.groupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkVolume)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox
			// 
			this.groupBox.Controls.Add(this.lblVolume);
			this.groupBox.Controls.Add(this.trkVolume);
			this.groupBox.Controls.Add(this.txtDevice);
			this.groupBox.Controls.Add(this.lblDevice);
			this.groupBox.Controls.Add(this.txtDescription);
			this.groupBox.Controls.Add(this.lblDescription);
			this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox.Location = new System.Drawing.Point(0, 0);
			this.groupBox.Name = "groupBox";
			this.groupBox.Size = new System.Drawing.Size(300, 240);
			this.groupBox.TabIndex = 3;
			this.groupBox.TabStop = false;
			this.groupBox.Text = "Generic Sound Device:";
			// 
			// lblVolume
			// 
			this.lblVolume.AutoSize = true;
			this.lblVolume.Location = new System.Drawing.Point(6, 54);
			this.lblVolume.Name = "lblVolume";
			this.lblVolume.Size = new System.Drawing.Size(45, 13);
			this.lblVolume.TabIndex = 7;
			this.lblVolume.Text = "Volume:";
			// 
			// trkVolume
			// 
			this.trkVolume.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.trkVolume.Location = new System.Drawing.Point(6, 70);
			this.trkVolume.Maximum = 100;
			this.trkVolume.Name = "trkVolume";
			this.trkVolume.Size = new System.Drawing.Size(288, 50);
			this.trkVolume.TabIndex = 6;
			this.trkVolume.TickFrequency = 2;
			// 
			// txtDevice
			// 
			this.txtDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtDevice.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtDevice.Location = new System.Drawing.Point(56, 26);
			this.txtDevice.Name = "txtDevice";
			this.txtDevice.ReadOnly = true;
			this.txtDevice.Size = new System.Drawing.Size(238, 13);
			this.txtDevice.TabIndex = 5;
			this.txtDevice.Text = "[Name]";
			// 
			// lblDevice
			// 
			this.lblDevice.AutoSize = true;
			this.lblDevice.Location = new System.Drawing.Point(6, 26);
			this.lblDevice.Name = "lblDevice";
			this.lblDevice.Size = new System.Drawing.Size(44, 13);
			this.lblDevice.TabIndex = 3;
			this.lblDevice.Text = "Device:";
			// 
			// txtDescription
			// 
			this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtDescription.Location = new System.Drawing.Point(6, 139);
			this.txtDescription.Multiline = true;
			this.txtDescription.Name = "txtDescription";
			this.txtDescription.ReadOnly = true;
			this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtDescription.Size = new System.Drawing.Size(288, 95);
			this.txtDescription.TabIndex = 2;
			// 
			// lblDescription
			// 
			this.lblDescription.AutoSize = true;
			this.lblDescription.Location = new System.Drawing.Point(6, 123);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(63, 13);
			this.lblDescription.TabIndex = 1;
			this.lblDescription.Text = "Description:";
			// 
			// CtlSettingsGenericSound
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox);
			this.Name = "CtlSettingsGenericSound";
			this.Size = new System.Drawing.Size(300, 240);
			this.groupBox.ResumeLayout(false);
			this.groupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkVolume)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox;
		private System.Windows.Forms.TextBox txtDevice;
		private System.Windows.Forms.Label lblDevice;
		private System.Windows.Forms.TextBox txtDescription;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.TrackBar trkVolume;
		private System.Windows.Forms.Label lblVolume;
	}
}
