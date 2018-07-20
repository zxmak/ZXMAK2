namespace ZXMAK2.Hardware.WinForms.General.Views
{
    partial class FormDisassembly
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
            this.dasmPanel = new ZXMAK2.Hardware.WinForms.General.DasmPanel();
            this.SuspendLayout();
            // 
            // dasmPanel
            // 
            this.dasmPanel.ActiveAddress = ((ushort)(0));
            this.dasmPanel.BreakpointColor = System.Drawing.Color.Red;
            this.dasmPanel.BreakpointForeColor = System.Drawing.Color.Black;
            this.dasmPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dasmPanel.Font = new System.Drawing.Font("Courier New", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.dasmPanel.Location = new System.Drawing.Point(0, 0);
            this.dasmPanel.Margin = new System.Windows.Forms.Padding(0);
            this.dasmPanel.Name = "dasmPanel";
            this.dasmPanel.Size = new System.Drawing.Size(525, 273);
            this.dasmPanel.TabIndex = 0;
            this.dasmPanel.Text = "dasmPanel1";
            this.dasmPanel.TopAddress = ((ushort)(0));
            this.dasmPanel.CheckBreakpoint += new ZXMAK2.Hardware.WinForms.General.DasmPanel.ONCHECKCPU(this.DasmPanel_OnCheckBreakpoint);
            this.dasmPanel.CheckExecuting += new ZXMAK2.Hardware.WinForms.General.DasmPanel.ONCHECKCPU(this.DasmPanel_OnCheckExecuting);
            this.dasmPanel.GetData += new ZXMAK2.Hardware.WinForms.General.DasmPanel.ONGETDATACPU(this.DasmPanel_OnGetData);
            this.dasmPanel.GetDasm += new ZXMAK2.Hardware.WinForms.General.DasmPanel.ONGETDASMCPU(this.DasmPanel_OnGetDasm);
            this.dasmPanel.BreakpointClick += new ZXMAK2.Hardware.WinForms.General.DasmPanel.ONCLICKCPU(this.DasmPanel_OnBreakpointClick);
            // 
            // FormDisassembly
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(525, 273);
            this.Controls.Add(this.dasmPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "FormDisassembly";
            this.ShowIcon = false;
            this.Text = "Disassembly";
            this.ResumeLayout(false);

        }

        #endregion

        private DasmPanel dasmPanel;
    }
}