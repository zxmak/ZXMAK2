namespace ZXMAK2.Hardware.WinForms
{
    partial class FormMemoryMap
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
            this.components = new System.ComponentModel.Container();
            this.lblCmr0 = new System.Windows.Forms.Label();
            this.lblCmr1 = new System.Windows.Forms.Label();
            this.lblCmrValue0 = new System.Windows.Forms.Label();
            this.lblCmrValue1 = new System.Windows.Forms.Label();
            this.lblWnd0000 = new System.Windows.Forms.Label();
            this.lblWndHint0000 = new System.Windows.Forms.Label();
            this.lblWnd4000 = new System.Windows.Forms.Label();
            this.lblWndHint4000 = new System.Windows.Forms.Label();
            this.lblWnd8000 = new System.Windows.Forms.Label();
            this.lblWndHint8000 = new System.Windows.Forms.Label();
            this.lblWndC000 = new System.Windows.Forms.Label();
            this.lblWndHintC000 = new System.Windows.Forms.Label();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.chkDosen = new System.Windows.Forms.CheckBox();
            this.chkSysen = new System.Windows.Forms.CheckBox();
            this.propGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // lblCmr0
            // 
            this.lblCmr0.AutoSize = true;
            this.lblCmr0.Location = new System.Drawing.Point(14, 18);
            this.lblCmr0.Name = "lblCmr0";
            this.lblCmr0.Size = new System.Drawing.Size(40, 13);
            this.lblCmr0.TabIndex = 0;
            this.lblCmr0.Text = "CMR0:";
            // 
            // lblCmr1
            // 
            this.lblCmr1.AutoSize = true;
            this.lblCmr1.Location = new System.Drawing.Point(14, 40);
            this.lblCmr1.Name = "lblCmr1";
            this.lblCmr1.Size = new System.Drawing.Size(40, 13);
            this.lblCmr1.TabIndex = 1;
            this.lblCmr1.Text = "CMR1:";
            // 
            // lblCmrValue0
            // 
            this.lblCmrValue0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCmrValue0.Location = new System.Drawing.Point(60, 16);
            this.lblCmrValue0.Name = "lblCmrValue0";
            this.lblCmrValue0.Size = new System.Drawing.Size(51, 17);
            this.lblCmrValue0.TabIndex = 2;
            this.lblCmrValue0.Text = "#00";
            this.lblCmrValue0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCmrValue0.DoubleClick += new System.EventHandler(this.lblCmrValue0_DoubleClick);
            // 
            // lblCmrValue1
            // 
            this.lblCmrValue1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCmrValue1.Location = new System.Drawing.Point(60, 38);
            this.lblCmrValue1.Name = "lblCmrValue1";
            this.lblCmrValue1.Size = new System.Drawing.Size(51, 17);
            this.lblCmrValue1.TabIndex = 3;
            this.lblCmrValue1.Text = "#00";
            this.lblCmrValue1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCmrValue1.DoubleClick += new System.EventHandler(this.lblCmrValue1_DoubleClick);
            // 
            // lblWnd0000
            // 
            this.lblWnd0000.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblWnd0000.Location = new System.Drawing.Point(92, 72);
            this.lblWnd0000.Name = "lblWnd0000";
            this.lblWnd0000.Size = new System.Drawing.Size(100, 17);
            this.lblWnd0000.TabIndex = 5;
            this.lblWnd0000.Text = "ROM DOS";
            this.lblWnd0000.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblWndHint0000
            // 
            this.lblWndHint0000.AutoSize = true;
            this.lblWndHint0000.Location = new System.Drawing.Point(14, 74);
            this.lblWndHint0000.Name = "lblWndHint0000";
            this.lblWndHint0000.Size = new System.Drawing.Size(75, 13);
            this.lblWndHint0000.TabIndex = 4;
            this.lblWndHint0000.Text = "#0000-#3FFF:";
            // 
            // lblWnd4000
            // 
            this.lblWnd4000.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblWnd4000.Location = new System.Drawing.Point(92, 89);
            this.lblWnd4000.Name = "lblWnd4000";
            this.lblWnd4000.Size = new System.Drawing.Size(100, 17);
            this.lblWnd4000.TabIndex = 7;
            this.lblWnd4000.Text = "RAM 0";
            this.lblWnd4000.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblWndHint4000
            // 
            this.lblWndHint4000.AutoSize = true;
            this.lblWndHint4000.Location = new System.Drawing.Point(14, 91);
            this.lblWndHint4000.Name = "lblWndHint4000";
            this.lblWndHint4000.Size = new System.Drawing.Size(75, 13);
            this.lblWndHint4000.TabIndex = 6;
            this.lblWndHint4000.Text = "#4000-#7FFF:";
            // 
            // lblWnd8000
            // 
            this.lblWnd8000.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblWnd8000.Location = new System.Drawing.Point(92, 106);
            this.lblWnd8000.Name = "lblWnd8000";
            this.lblWnd8000.Size = new System.Drawing.Size(100, 17);
            this.lblWnd8000.TabIndex = 9;
            this.lblWnd8000.Text = "RAM 1";
            this.lblWnd8000.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblWndHint8000
            // 
            this.lblWndHint8000.AutoSize = true;
            this.lblWndHint8000.Location = new System.Drawing.Point(14, 108);
            this.lblWndHint8000.Name = "lblWndHint8000";
            this.lblWndHint8000.Size = new System.Drawing.Size(76, 13);
            this.lblWndHint8000.TabIndex = 8;
            this.lblWndHint8000.Text = "#8000-#BFFF:";
            // 
            // lblWndC000
            // 
            this.lblWndC000.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblWndC000.Location = new System.Drawing.Point(92, 123);
            this.lblWndC000.Name = "lblWndC000";
            this.lblWndC000.Size = new System.Drawing.Size(100, 17);
            this.lblWndC000.TabIndex = 11;
            this.lblWndC000.Text = "RAM 2";
            this.lblWndC000.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblWndHintC000
            // 
            this.lblWndHintC000.AutoSize = true;
            this.lblWndHintC000.Location = new System.Drawing.Point(14, 125);
            this.lblWndHintC000.Name = "lblWndHintC000";
            this.lblWndHintC000.Size = new System.Drawing.Size(76, 13);
            this.lblWndHintC000.TabIndex = 10;
            this.lblWndHintC000.Text = "#C000-#FFFF:";
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Interval = 250;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // chkDosen
            // 
            this.chkDosen.AutoCheck = false;
            this.chkDosen.AutoSize = true;
            this.chkDosen.Location = new System.Drawing.Point(117, 17);
            this.chkDosen.Name = "chkDosen";
            this.chkDosen.Size = new System.Drawing.Size(64, 17);
            this.chkDosen.TabIndex = 12;
            this.chkDosen.Text = "DOSEN";
            this.chkDosen.UseVisualStyleBackColor = true;
            // 
            // chkSysen
            // 
            this.chkSysen.AutoCheck = false;
            this.chkSysen.AutoSize = true;
            this.chkSysen.Location = new System.Drawing.Point(117, 39);
            this.chkSysen.Name = "chkSysen";
            this.chkSysen.Size = new System.Drawing.Size(62, 17);
            this.chkSysen.TabIndex = 13;
            this.chkSysen.Text = "SYSEN";
            this.chkSysen.UseVisualStyleBackColor = true;
            // 
            // propGrid
            // 
            this.propGrid.Location = new System.Drawing.Point(0, 151);
            this.propGrid.Name = "propGrid";
            this.propGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propGrid.Size = new System.Drawing.Size(204, 245);
            this.propGrid.TabIndex = 14;
            this.propGrid.ToolbarVisible = false;
            // 
            // FormMemoryMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(204, 396);
            this.Controls.Add(this.propGrid);
            this.Controls.Add(this.chkSysen);
            this.Controls.Add(this.chkDosen);
            this.Controls.Add(this.lblWndC000);
            this.Controls.Add(this.lblWndHintC000);
            this.Controls.Add(this.lblWnd8000);
            this.Controls.Add(this.lblWndHint8000);
            this.Controls.Add(this.lblWnd4000);
            this.Controls.Add(this.lblWndHint4000);
            this.Controls.Add(this.lblWnd0000);
            this.Controls.Add(this.lblWndHint0000);
            this.Controls.Add(this.lblCmrValue1);
            this.Controls.Add(this.lblCmrValue0);
            this.Controls.Add(this.lblCmr1);
            this.Controls.Add(this.lblCmr0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMemoryMap";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Memory Map";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCmr0;
        private System.Windows.Forms.Label lblCmr1;
        private System.Windows.Forms.Label lblCmrValue0;
        private System.Windows.Forms.Label lblCmrValue1;
        private System.Windows.Forms.Label lblWnd0000;
        private System.Windows.Forms.Label lblWndHint0000;
        private System.Windows.Forms.Label lblWnd4000;
        private System.Windows.Forms.Label lblWndHint4000;
        private System.Windows.Forms.Label lblWnd8000;
        private System.Windows.Forms.Label lblWndHint8000;
        private System.Windows.Forms.Label lblWndC000;
        private System.Windows.Forms.Label lblWndHintC000;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.CheckBox chkDosen;
        private System.Windows.Forms.CheckBox chkSysen;
        private System.Windows.Forms.PropertyGrid propGrid;
    }
}