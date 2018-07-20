namespace ZXMAK2.Hardware.Adlers.Views
{
    partial class TcpHelper
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
            this.progressBarDownloadStatus = new System.Windows.Forms.ProgressBar();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxProxyAdress = new System.Windows.Forms.TextBox();
            this.textBoxProxyPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxProxy = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxIsProxy = new System.Windows.Forms.CheckBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelStatusText = new System.Windows.Forms.Label();
            this.groupBoxProxy.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBarDownloadStatus
            // 
            this.progressBarDownloadStatus.Location = new System.Drawing.Point(13, 149);
            this.progressBarDownloadStatus.Name = "progressBarDownloadStatus";
            this.progressBarDownloadStatus.Size = new System.Drawing.Size(414, 23);
            this.progressBarDownloadStatus.TabIndex = 0;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(13, 120);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(352, 120);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxProxyAdress
            // 
            this.textBoxProxyAdress.Enabled = false;
            this.textBoxProxyAdress.Location = new System.Drawing.Point(51, 13);
            this.textBoxProxyAdress.Name = "textBoxProxyAdress";
            this.textBoxProxyAdress.Size = new System.Drawing.Size(142, 20);
            this.textBoxProxyAdress.TabIndex = 3;
            // 
            // textBoxProxyPort
            // 
            this.textBoxProxyPort.Enabled = false;
            this.textBoxProxyPort.Location = new System.Drawing.Point(304, 13);
            this.textBoxProxyPort.Name = "textBoxProxyPort";
            this.textBoxProxyPort.Size = new System.Drawing.Size(100, 20);
            this.textBoxProxyPort.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Adress";
            // 
            // groupBoxProxy
            // 
            this.groupBoxProxy.Controls.Add(this.label2);
            this.groupBoxProxy.Controls.Add(this.label1);
            this.groupBoxProxy.Controls.Add(this.textBoxProxyPort);
            this.groupBoxProxy.Controls.Add(this.textBoxProxyAdress);
            this.groupBoxProxy.Location = new System.Drawing.Point(15, 49);
            this.groupBoxProxy.Name = "groupBoxProxy";
            this.groupBoxProxy.Size = new System.Drawing.Size(412, 41);
            this.groupBoxProxy.TabIndex = 6;
            this.groupBoxProxy.TabStop = false;
            this.groupBoxProxy.Text = "Proxy";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(272, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Port";
            // 
            // checkBoxIsProxy
            // 
            this.checkBoxIsProxy.AutoSize = true;
            this.checkBoxIsProxy.Location = new System.Drawing.Point(13, 13);
            this.checkBoxIsProxy.Name = "checkBoxIsProxy";
            this.checkBoxIsProxy.Size = new System.Drawing.Size(52, 17);
            this.checkBoxIsProxy.TabIndex = 7;
            this.checkBoxIsProxy.Text = "Proxy";
            this.checkBoxIsProxy.UseVisualStyleBackColor = true;
            this.checkBoxIsProxy.CheckedChanged += new System.EventHandler(this.checkBoxIsProxy_CheckedChanged);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(9, 182);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(40, 13);
            this.labelStatus.TabIndex = 8;
            this.labelStatus.Text = "Status:";
            // 
            // labelStatusText
            // 
            this.labelStatusText.AutoSize = true;
            this.labelStatusText.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelStatusText.Location = new System.Drawing.Point(48, 181);
            this.labelStatusText.Name = "labelStatusText";
            this.labelStatusText.Size = new System.Drawing.Size(61, 15);
            this.labelStatusText.TabIndex = 9;
            this.labelStatusText.Text = "Not started";
            // 
            // TcpHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 200);
            this.Controls.Add(this.labelStatusText);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.checkBoxIsProxy);
            this.Controls.Add(this.groupBoxProxy);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.progressBarDownloadStatus);
            this.KeyPreview = true;
            this.Name = "TcpHelper";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "TcpHelper";
            this.TopMost = true;
            this.groupBoxProxy.ResumeLayout(false);
            this.groupBoxProxy.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBarDownloadStatus;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxProxyAdress;
        private System.Windows.Forms.TextBox textBoxProxyPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxProxy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxIsProxy;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelStatusText;
    }
}