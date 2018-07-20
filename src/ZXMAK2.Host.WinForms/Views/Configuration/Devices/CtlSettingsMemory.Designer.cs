namespace ZXMAK2.Host.WinForms.Views.Configuration.Devices
{
    partial class CtlSettingsMemory
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
            this.lblRomSet = new System.Windows.Forms.Label();
            this.cbxRomSet = new System.Windows.Forms.ComboBox();
            this.lblType = new System.Windows.Forms.Label();
            this.cbxType = new System.Windows.Forms.ComboBox();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.lblRomSet);
            this.groupBox.Controls.Add(this.cbxRomSet);
            this.groupBox.Controls.Add(this.lblType);
            this.groupBox.Controls.Add(this.cbxType);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Location = new System.Drawing.Point(0, 0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(300, 150);
            this.groupBox.TabIndex = 1;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Memory Settings:";
            // 
            // lblRomSet
            // 
            this.lblRomSet.AutoSize = true;
            this.lblRomSet.Location = new System.Drawing.Point(6, 73);
            this.lblRomSet.Name = "lblRomSet";
            this.lblRomSet.Size = new System.Drawing.Size(52, 13);
            this.lblRomSet.TabIndex = 3;
            this.lblRomSet.Text = "ROM-set:";
            // 
            // cbxRomSet
            // 
            this.cbxRomSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxRomSet.FormattingEnabled = true;
            this.cbxRomSet.Location = new System.Drawing.Point(6, 89);
            this.cbxRomSet.Name = "cbxRomSet";
            this.cbxRomSet.Size = new System.Drawing.Size(177, 21);
            this.cbxRomSet.TabIndex = 2;
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(6, 25);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(34, 13);
            this.lblType.TabIndex = 1;
            this.lblType.Text = "Type:";
            // 
            // cbxType
            // 
            this.cbxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxType.FormattingEnabled = true;
            this.cbxType.Location = new System.Drawing.Point(6, 41);
            this.cbxType.Name = "cbxType";
            this.cbxType.Size = new System.Drawing.Size(177, 21);
            this.cbxType.TabIndex = 0;
            this.cbxType.SelectedIndexChanged += new System.EventHandler(this.cbxType_SelectedIndexChanged);
            // 
            // CtlSettingsMemory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox);
            this.Name = "CtlSettingsMemory";
            this.Size = new System.Drawing.Size(300, 150);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cbxType;
        private System.Windows.Forms.Label lblRomSet;
        private System.Windows.Forms.ComboBox cbxRomSet;
    }
}
