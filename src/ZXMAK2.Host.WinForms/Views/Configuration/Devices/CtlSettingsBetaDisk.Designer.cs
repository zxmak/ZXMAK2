namespace ZXMAK2.Host.WinForms.Views.Configuration.Devices
{
    partial class CtlSettingsBetaDisk
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
            this.chkPresentD = new System.Windows.Forms.CheckBox();
            this.chkPresentC = new System.Windows.Forms.CheckBox();
            this.chkPresentB = new System.Windows.Forms.CheckBox();
            this.chkPresentA = new System.Windows.Forms.CheckBox();
            this.chkLogIO = new System.Windows.Forms.CheckBox();
            this.chkProtectD = new System.Windows.Forms.CheckBox();
            this.chkProtectC = new System.Windows.Forms.CheckBox();
            this.chkProtectB = new System.Windows.Forms.CheckBox();
            this.chkProtectA = new System.Windows.Forms.CheckBox();
            this.btnBrowseD = new System.Windows.Forms.Button();
            this.txtPathD = new System.Windows.Forms.TextBox();
            this.btnBrowseC = new System.Windows.Forms.Button();
            this.txtPathC = new System.Windows.Forms.TextBox();
            this.btnBrowseB = new System.Windows.Forms.Button();
            this.txtPathB = new System.Windows.Forms.TextBox();
            this.btnBrowseA = new System.Windows.Forms.Button();
            this.txtPathA = new System.Windows.Forms.TextBox();
            this.chkNoDelay = new System.Windows.Forms.CheckBox();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.chkPresentD);
            this.groupBox.Controls.Add(this.chkPresentC);
            this.groupBox.Controls.Add(this.chkPresentB);
            this.groupBox.Controls.Add(this.chkPresentA);
            this.groupBox.Controls.Add(this.chkLogIO);
            this.groupBox.Controls.Add(this.chkProtectD);
            this.groupBox.Controls.Add(this.chkProtectC);
            this.groupBox.Controls.Add(this.chkProtectB);
            this.groupBox.Controls.Add(this.chkProtectA);
            this.groupBox.Controls.Add(this.btnBrowseD);
            this.groupBox.Controls.Add(this.txtPathD);
            this.groupBox.Controls.Add(this.btnBrowseC);
            this.groupBox.Controls.Add(this.txtPathC);
            this.groupBox.Controls.Add(this.btnBrowseB);
            this.groupBox.Controls.Add(this.txtPathB);
            this.groupBox.Controls.Add(this.btnBrowseA);
            this.groupBox.Controls.Add(this.txtPathA);
            this.groupBox.Controls.Add(this.chkNoDelay);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Location = new System.Drawing.Point(0, 0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(300, 240);
            this.groupBox.TabIndex = 2;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Beta Disk Interface Settings:";
            // 
            // chkPresentD
            // 
            this.chkPresentD.AutoSize = true;
            this.chkPresentD.Location = new System.Drawing.Point(16, 189);
            this.chkPresentD.Name = "chkPresentD";
            this.chkPresentD.Size = new System.Drawing.Size(65, 17);
            this.chkPresentD.TabIndex = 21;
            this.chkPresentD.Text = "Drive D:";
            this.chkPresentD.UseVisualStyleBackColor = true;
            this.chkPresentD.CheckedChanged += new System.EventHandler(this.chkPresent_CheckedChanged);
            // 
            // chkPresentC
            // 
            this.chkPresentC.AutoSize = true;
            this.chkPresentC.Location = new System.Drawing.Point(16, 149);
            this.chkPresentC.Name = "chkPresentC";
            this.chkPresentC.Size = new System.Drawing.Size(64, 17);
            this.chkPresentC.TabIndex = 20;
            this.chkPresentC.Text = "Drive C:";
            this.chkPresentC.UseVisualStyleBackColor = true;
            this.chkPresentC.CheckedChanged += new System.EventHandler(this.chkPresent_CheckedChanged);
            // 
            // chkPresentB
            // 
            this.chkPresentB.AutoSize = true;
            this.chkPresentB.Location = new System.Drawing.Point(16, 109);
            this.chkPresentB.Name = "chkPresentB";
            this.chkPresentB.Size = new System.Drawing.Size(64, 17);
            this.chkPresentB.TabIndex = 19;
            this.chkPresentB.Text = "Drive B:";
            this.chkPresentB.UseVisualStyleBackColor = true;
            this.chkPresentB.CheckedChanged += new System.EventHandler(this.chkPresent_CheckedChanged);
            // 
            // chkPresentA
            // 
            this.chkPresentA.AutoSize = true;
            this.chkPresentA.Location = new System.Drawing.Point(16, 69);
            this.chkPresentA.Name = "chkPresentA";
            this.chkPresentA.Size = new System.Drawing.Size(64, 17);
            this.chkPresentA.TabIndex = 18;
            this.chkPresentA.Text = "Drive A:";
            this.chkPresentA.UseVisualStyleBackColor = true;
            this.chkPresentA.CheckedChanged += new System.EventHandler(this.chkPresent_CheckedChanged);
            // 
            // chkLogIO
            // 
            this.chkLogIO.AutoSize = true;
            this.chkLogIO.Location = new System.Drawing.Point(16, 36);
            this.chkLogIO.Name = "chkLogIO";
            this.chkLogIO.Size = new System.Drawing.Size(71, 17);
            this.chkLogIO.TabIndex = 17;
            this.chkLogIO.Text = "Log all IO";
            this.chkLogIO.UseVisualStyleBackColor = true;
            // 
            // chkProtectD
            // 
            this.chkProtectD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkProtectD.AutoSize = true;
            this.chkProtectD.Location = new System.Drawing.Point(168, 189);
            this.chkProtectD.Name = "chkProtectD";
            this.chkProtectD.Size = new System.Drawing.Size(88, 17);
            this.chkProtectD.TabIndex = 16;
            this.chkProtectD.Text = "Write Protect";
            this.chkProtectD.UseVisualStyleBackColor = true;
            // 
            // chkProtectC
            // 
            this.chkProtectC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkProtectC.AutoSize = true;
            this.chkProtectC.Location = new System.Drawing.Point(168, 149);
            this.chkProtectC.Name = "chkProtectC";
            this.chkProtectC.Size = new System.Drawing.Size(88, 17);
            this.chkProtectC.TabIndex = 15;
            this.chkProtectC.Text = "Write Protect";
            this.chkProtectC.UseVisualStyleBackColor = true;
            // 
            // chkProtectB
            // 
            this.chkProtectB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkProtectB.AutoSize = true;
            this.chkProtectB.Location = new System.Drawing.Point(168, 109);
            this.chkProtectB.Name = "chkProtectB";
            this.chkProtectB.Size = new System.Drawing.Size(88, 17);
            this.chkProtectB.TabIndex = 14;
            this.chkProtectB.Text = "Write Protect";
            this.chkProtectB.UseVisualStyleBackColor = true;
            // 
            // chkProtectA
            // 
            this.chkProtectA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkProtectA.AutoSize = true;
            this.chkProtectA.Location = new System.Drawing.Point(168, 69);
            this.chkProtectA.Name = "chkProtectA";
            this.chkProtectA.Size = new System.Drawing.Size(88, 17);
            this.chkProtectA.TabIndex = 13;
            this.chkProtectA.Text = "Write Protect";
            this.chkProtectA.UseVisualStyleBackColor = true;
            // 
            // btnBrowseD
            // 
            this.btnBrowseD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseD.Location = new System.Drawing.Point(262, 203);
            this.btnBrowseD.Name = "btnBrowseD";
            this.btnBrowseD.Size = new System.Drawing.Size(32, 23);
            this.btnBrowseD.TabIndex = 12;
            this.btnBrowseD.Text = "...";
            this.btnBrowseD.UseVisualStyleBackColor = true;
            this.btnBrowseD.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtPathD
            // 
            this.txtPathD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPathD.Location = new System.Drawing.Point(16, 206);
            this.txtPathD.Name = "txtPathD";
            this.txtPathD.Size = new System.Drawing.Size(240, 20);
            this.txtPathD.TabIndex = 10;
            this.txtPathD.TextChanged += new System.EventHandler(this.txtPath_TextChanged);
            // 
            // btnBrowseC
            // 
            this.btnBrowseC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseC.Location = new System.Drawing.Point(262, 163);
            this.btnBrowseC.Name = "btnBrowseC";
            this.btnBrowseC.Size = new System.Drawing.Size(32, 23);
            this.btnBrowseC.TabIndex = 9;
            this.btnBrowseC.Text = "...";
            this.btnBrowseC.UseVisualStyleBackColor = true;
            this.btnBrowseC.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtPathC
            // 
            this.txtPathC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPathC.Location = new System.Drawing.Point(16, 166);
            this.txtPathC.Name = "txtPathC";
            this.txtPathC.Size = new System.Drawing.Size(240, 20);
            this.txtPathC.TabIndex = 7;
            this.txtPathC.TextChanged += new System.EventHandler(this.txtPath_TextChanged);
            // 
            // btnBrowseB
            // 
            this.btnBrowseB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseB.Location = new System.Drawing.Point(262, 123);
            this.btnBrowseB.Name = "btnBrowseB";
            this.btnBrowseB.Size = new System.Drawing.Size(32, 23);
            this.btnBrowseB.TabIndex = 6;
            this.btnBrowseB.Text = "...";
            this.btnBrowseB.UseVisualStyleBackColor = true;
            this.btnBrowseB.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtPathB
            // 
            this.txtPathB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPathB.Location = new System.Drawing.Point(16, 126);
            this.txtPathB.Name = "txtPathB";
            this.txtPathB.Size = new System.Drawing.Size(240, 20);
            this.txtPathB.TabIndex = 4;
            this.txtPathB.TextChanged += new System.EventHandler(this.txtPath_TextChanged);
            // 
            // btnBrowseA
            // 
            this.btnBrowseA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseA.Location = new System.Drawing.Point(262, 83);
            this.btnBrowseA.Name = "btnBrowseA";
            this.btnBrowseA.Size = new System.Drawing.Size(32, 23);
            this.btnBrowseA.TabIndex = 3;
            this.btnBrowseA.Text = "...";
            this.btnBrowseA.UseVisualStyleBackColor = true;
            this.btnBrowseA.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtPathA
            // 
            this.txtPathA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPathA.Location = new System.Drawing.Point(16, 86);
            this.txtPathA.Name = "txtPathA";
            this.txtPathA.Size = new System.Drawing.Size(240, 20);
            this.txtPathA.TabIndex = 1;
            this.txtPathA.TextChanged += new System.EventHandler(this.txtPath_TextChanged);
            // 
            // chkNoDelay
            // 
            this.chkNoDelay.AutoSize = true;
            this.chkNoDelay.Location = new System.Drawing.Point(16, 19);
            this.chkNoDelay.Name = "chkNoDelay";
            this.chkNoDelay.Size = new System.Drawing.Size(107, 17);
            this.chkNoDelay.TabIndex = 0;
            this.chkNoDelay.Text = "WD93 No delays";
            this.chkNoDelay.UseVisualStyleBackColor = true;
            // 
            // CtlSettingsBetaDisk
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox);
            this.Name = "CtlSettingsBetaDisk";
            this.Size = new System.Drawing.Size(300, 240);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.TextBox txtPathA;
        private System.Windows.Forms.CheckBox chkNoDelay;
        private System.Windows.Forms.Button btnBrowseD;
        private System.Windows.Forms.TextBox txtPathD;
        private System.Windows.Forms.Button btnBrowseC;
        private System.Windows.Forms.TextBox txtPathC;
        private System.Windows.Forms.Button btnBrowseB;
        private System.Windows.Forms.TextBox txtPathB;
        private System.Windows.Forms.Button btnBrowseA;
        private System.Windows.Forms.CheckBox chkProtectD;
        private System.Windows.Forms.CheckBox chkProtectC;
        private System.Windows.Forms.CheckBox chkProtectB;
        private System.Windows.Forms.CheckBox chkProtectA;
        private System.Windows.Forms.CheckBox chkLogIO;
        private System.Windows.Forms.CheckBox chkPresentD;
        private System.Windows.Forms.CheckBox chkPresentC;
        private System.Windows.Forms.CheckBox chkPresentB;
        private System.Windows.Forms.CheckBox chkPresentA;
    }
}
