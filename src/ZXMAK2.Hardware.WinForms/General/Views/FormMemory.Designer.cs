namespace ZXMAK2.Hardware.WinForms.General.Views
{
    partial class FormMemory
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
            this.dataPanel = new ZXMAK2.Hardware.WinForms.General.DataPanel();
            this.SuspendLayout();
            // 
            // dataPanel
            // 
            this.dataPanel.ColCount = 16;
            this.dataPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataPanel.Font = new System.Drawing.Font("Courier New", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.dataPanel.Location = new System.Drawing.Point(0, 0);
            this.dataPanel.Name = "dataPanel";
            this.dataPanel.Size = new System.Drawing.Size(676, 151);
            this.dataPanel.TabIndex = 0;
            this.dataPanel.Text = "dataPanel1";
            this.dataPanel.TopAddress = ((ushort)(0));
            this.dataPanel.GetData += new ZXMAK2.Hardware.WinForms.General.DataPanel.ONGETDATACPU(this.DasmPanel_OnGetData);
            // 
            // FormMemory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(676, 151);
            this.Controls.Add(this.dataPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "FormMemory";
            this.ShowIcon = false;
            this.Text = "Memory";
            this.ResumeLayout(false);

        }

        #endregion

        private DataPanel dataPanel;
    }
}