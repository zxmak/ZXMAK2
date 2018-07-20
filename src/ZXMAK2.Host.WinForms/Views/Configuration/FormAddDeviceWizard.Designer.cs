namespace ZXMAK2.Host.WinForms.Views
{
    partial class FormAddDeviceWizard
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Memory", 0);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("ULA", 2);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Disk", 3);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Sound", 4);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Music", 5);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Tape", 6);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Keyboard", 7);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Mouse", 8);
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("Other", 1);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAddDeviceWizard));
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblActionAim = new System.Windows.Forms.Label();
            this.lblActionHint = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lstDevices = new System.Windows.Forms.ListBox();
            this.lblDevices = new System.Windows.Forms.Label();
            this.lblCategory = new System.Windows.Forms.Label();
            this.lstCategory = new System.Windows.Forms.ListView();
            this.colCategory = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.separator1 = new ZXMAK2.Host.WinForms.Controls.Separator();
            this.sepTop = new ZXMAK2.Host.WinForms.Controls.Separator();
            this.pnlTop.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pnlTop.Controls.Add(this.lblActionAim);
            this.pnlTop.Controls.Add(this.lblActionHint);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(574, 58);
            this.pnlTop.TabIndex = 0;
            // 
            // lblActionAim
            // 
            this.lblActionAim.AutoSize = true;
            this.lblActionAim.Location = new System.Drawing.Point(43, 27);
            this.lblActionAim.Name = "lblActionAim";
            this.lblActionAim.Size = new System.Drawing.Size(224, 13);
            this.lblActionAim.TabIndex = 1;
            this.lblActionAim.Text = "What category of device do you want to add?";
            // 
            // lblActionHint
            // 
            this.lblActionHint.AutoSize = true;
            this.lblActionHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblActionHint.Location = new System.Drawing.Point(20, 9);
            this.lblActionHint.Name = "lblActionHint";
            this.lblActionHint.Size = new System.Drawing.Size(307, 13);
            this.lblActionHint.TabIndex = 0;
            this.lblActionHint.Text = "From the list below, select the device you are adding";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(493, 370);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 22);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.Location = new System.Drawing.Point(331, 370);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 22);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "< Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(412, 370);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 22);
            this.btnNext.TabIndex = 3;
            this.btnNext.Text = "Next >";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.ItemSize = new System.Drawing.Size(58, 18);
            this.tabControl.Location = new System.Drawing.Point(0, 64);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(574, 291);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabIndex = 4;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtDescription);
            this.tabPage1.Controls.Add(this.lstDevices);
            this.tabPage1.Controls.Add(this.lblDevices);
            this.tabPage1.Controls.Add(this.lblCategory);
            this.tabPage1.Controls.Add(this.lstCategory);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(566, 265);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(173, 191);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.ShortcutsEnabled = false;
            this.txtDescription.Size = new System.Drawing.Size(390, 73);
            this.txtDescription.TabIndex = 6;
            this.txtDescription.WordWrap = false;
            // 
            // lstDevices
            // 
            this.lstDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstDevices.FormattingEnabled = true;
            this.lstDevices.HorizontalScrollbar = true;
            this.lstDevices.IntegralHeight = false;
            this.lstDevices.Location = new System.Drawing.Point(173, 16);
            this.lstDevices.Name = "lstDevices";
            this.lstDevices.Size = new System.Drawing.Size(390, 169);
            this.lstDevices.TabIndex = 5;
            this.lstDevices.SelectedIndexChanged += new System.EventHandler(this.lstDevices_SelectedIndexChanged);
            this.lstDevices.DoubleClick += new System.EventHandler(this.lstDevices_DoubleClick);
            // 
            // lblDevices
            // 
            this.lblDevices.AutoSize = true;
            this.lblDevices.Location = new System.Drawing.Point(170, 0);
            this.lblDevices.Name = "lblDevices";
            this.lblDevices.Size = new System.Drawing.Size(49, 13);
            this.lblDevices.TabIndex = 4;
            this.lblDevices.Text = "Devices:";
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Location = new System.Drawing.Point(0, 0);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(60, 13);
            this.lblCategory.TabIndex = 2;
            this.lblCategory.Text = "Categories:";
            // 
            // lstCategory
            // 
            this.lstCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstCategory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colCategory});
            this.lstCategory.FullRowSelect = true;
            this.lstCategory.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstCategory.HideSelection = false;
            this.lstCategory.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9});
            this.lstCategory.LabelWrap = false;
            this.lstCategory.Location = new System.Drawing.Point(3, 16);
            this.lstCategory.MultiSelect = false;
            this.lstCategory.Name = "lstCategory";
            this.lstCategory.Size = new System.Drawing.Size(164, 247);
            this.lstCategory.SmallImageList = this.imageList;
            this.lstCategory.TabIndex = 1;
            this.lstCategory.UseCompatibleStateImageBehavior = false;
            this.lstCategory.View = System.Windows.Forms.View.Details;
            this.lstCategory.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lstCategory_ItemSelectionChanged);
            // 
            // colCategory
            // 
            this.colCategory.Text = "Category";
            this.colCategory.Width = 160;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "RAMx16.png");
            this.imageList.Images.SetKeyName(1, "PCBx16.png");
            this.imageList.Images.SetKeyName(2, "ULAx16.png");
            this.imageList.Images.SetKeyName(3, "FDDx16.png");
            this.imageList.Images.SetKeyName(4, "BEEPERx16.png");
            this.imageList.Images.SetKeyName(5, "AY8910x16.png");
            this.imageList.Images.SetKeyName(6, "TAPEx16.png");
            this.imageList.Images.SetKeyName(7, "KBDx16.png");
            this.imageList.Images.SetKeyName(8, "MOUSx16.png");
            this.imageList.Images.SetKeyName(9, "DISPLAYx16.png");
            this.imageList.Images.SetKeyName(10, "DEBUGx16.png");
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(566, 265);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // separator1
            // 
            this.separator1.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.separator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator1.Location = new System.Drawing.Point(0, 349);
            this.separator1.Name = "separator1";
            this.separator1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.separator1.Size = new System.Drawing.Size(574, 6);
            this.separator1.TabIndex = 6;
            this.separator1.Text = "separator1";
            // 
            // sepTop
            // 
            this.sepTop.Alignment = System.Drawing.ContentAlignment.TopCenter;
            this.sepTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sepTop.Location = new System.Drawing.Point(0, 58);
            this.sepTop.Name = "sepTop";
            this.sepTop.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.sepTop.Size = new System.Drawing.Size(574, 6);
            this.sepTop.TabIndex = 5;
            this.sepTop.Text = "separator1";
            // 
            // FormAddDeviceWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(574, 397);
            this.Controls.Add(this.separator1);
            this.Controls.Add(this.sepTop);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAddDeviceWizard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Device Wizard";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblActionAim;
        private System.Windows.Forms.Label lblActionHint;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView lstCategory;
        private System.Windows.Forms.ColumnHeader colCategory;
        private System.Windows.Forms.ImageList imageList;
        private ZXMAK2.Host.WinForms.Controls.Separator sepTop;
        private ZXMAK2.Host.WinForms.Controls.Separator separator1;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.Label lblDevices;
        private System.Windows.Forms.ListBox lstDevices;
        private System.Windows.Forms.TextBox txtDescription;
    }
}