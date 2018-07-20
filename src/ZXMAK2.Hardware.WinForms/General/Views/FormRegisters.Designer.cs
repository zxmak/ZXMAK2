namespace ZXMAK2.Hardware.WinForms.General.Views
{
    partial class FormRegisters
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
            this.lblTitleControl = new System.Windows.Forms.Label();
            this.lblRegPc = new System.Windows.Forms.Label();
            this.txtRegPc = new System.Windows.Forms.TextBox();
            this.txtRegSp = new System.Windows.Forms.TextBox();
            this.lblRegSp = new System.Windows.Forms.Label();
            this.lblTitleGeneral = new System.Windows.Forms.Label();
            this.txtRegAf_ = new System.Windows.Forms.TextBox();
            this.lblRegAf_ = new System.Windows.Forms.Label();
            this.txtRegAf = new System.Windows.Forms.TextBox();
            this.lblRegAf = new System.Windows.Forms.Label();
            this.txtRegHl_ = new System.Windows.Forms.TextBox();
            this.lblRegHl_ = new System.Windows.Forms.Label();
            this.txtRegHl = new System.Windows.Forms.TextBox();
            this.lblRegHl = new System.Windows.Forms.Label();
            this.txtRegDe_ = new System.Windows.Forms.TextBox();
            this.lblRegDe_ = new System.Windows.Forms.Label();
            this.txtRegDe = new System.Windows.Forms.TextBox();
            this.lblRegDe = new System.Windows.Forms.Label();
            this.txtRegBc_ = new System.Windows.Forms.TextBox();
            this.lblRegBc_ = new System.Windows.Forms.Label();
            this.txtRegBc = new System.Windows.Forms.TextBox();
            this.lblRegBc = new System.Windows.Forms.Label();
            this.txtRegIy = new System.Windows.Forms.TextBox();
            this.lblRegIy = new System.Windows.Forms.Label();
            this.txtRegIx = new System.Windows.Forms.TextBox();
            this.lblRegIx = new System.Windows.Forms.Label();
            this.txtRegIm = new System.Windows.Forms.TextBox();
            this.lblRegIm = new System.Windows.Forms.Label();
            this.txtRegIr = new System.Windows.Forms.TextBox();
            this.lblRegIr = new System.Windows.Forms.Label();
            this.chkFlagS = new System.Windows.Forms.CheckBox();
            this.chkFlagZ = new System.Windows.Forms.CheckBox();
            this.chkFlag5 = new System.Windows.Forms.CheckBox();
            this.chkFlagH = new System.Windows.Forms.CheckBox();
            this.chkFlag3 = new System.Windows.Forms.CheckBox();
            this.chkFlagV = new System.Windows.Forms.CheckBox();
            this.chkFlagN = new System.Windows.Forms.CheckBox();
            this.chkFlagC = new System.Windows.Forms.CheckBox();
            this.lblTitleFlags = new System.Windows.Forms.Label();
            this.txtRegWz = new System.Windows.Forms.TextBox();
            this.lblRegsWz = new System.Windows.Forms.Label();
            this.chkHalt = new System.Windows.Forms.CheckBox();
            this.chkIff1 = new System.Windows.Forms.CheckBox();
            this.chkIff2 = new System.Windows.Forms.CheckBox();
            this.txtRegLpc = new System.Windows.Forms.TextBox();
            this.lblRegLpc = new System.Windows.Forms.Label();
            this.chkBint = new System.Windows.Forms.CheckBox();
            this.sepFlags = new ZXMAK2.Host.WinForms.Controls.Separator();
            this.sepGeneral = new ZXMAK2.Host.WinForms.Controls.Separator();
            this.sepControl = new ZXMAK2.Host.WinForms.Controls.Separator();
            this.lblTitleRzx = new System.Windows.Forms.Label();
            this.sepRzx = new ZXMAK2.Host.WinForms.Controls.Separator();
            this.lblRzxInput = new System.Windows.Forms.Label();
            this.lblRzxInputValue = new System.Windows.Forms.Label();
            this.lblRzxFrameValue = new System.Windows.Forms.Label();
            this.lblRzxFrame = new System.Windows.Forms.Label();
            this.lblRzxFetchValue = new System.Windows.Forms.Label();
            this.lblRzxFetch = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblTitleControl
            // 
            this.lblTitleControl.AutoSize = true;
            this.lblTitleControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTitleControl.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblTitleControl.Location = new System.Drawing.Point(10, 10);
            this.lblTitleControl.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitleControl.Name = "lblTitleControl";
            this.lblTitleControl.Size = new System.Drawing.Size(47, 13);
            this.lblTitleControl.TabIndex = 0;
            this.lblTitleControl.Text = "Control";
            // 
            // lblRegPc
            // 
            this.lblRegPc.AutoSize = true;
            this.lblRegPc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegPc.Location = new System.Drawing.Point(10, 29);
            this.lblRegPc.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegPc.Name = "lblRegPc";
            this.lblRegPc.Size = new System.Drawing.Size(27, 13);
            this.lblRegPc.TabIndex = 2;
            this.lblRegPc.Text = "PC:";
            // 
            // txtRegPc
            // 
            this.txtRegPc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegPc.Location = new System.Drawing.Point(43, 29);
            this.txtRegPc.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegPc.Name = "txtRegPc";
            this.txtRegPc.Size = new System.Drawing.Size(50, 13);
            this.txtRegPc.TabIndex = 3;
            this.txtRegPc.Text = "#FFFF";
            this.txtRegPc.WordWrap = false;
            this.txtRegPc.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegPc.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegPc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegPc.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // txtRegSp
            // 
            this.txtRegSp.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegSp.Location = new System.Drawing.Point(135, 29);
            this.txtRegSp.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegSp.Name = "txtRegSp";
            this.txtRegSp.Size = new System.Drawing.Size(50, 13);
            this.txtRegSp.TabIndex = 4;
            this.txtRegSp.Text = "#FFFF";
            this.txtRegSp.WordWrap = false;
            this.txtRegSp.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegSp.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegSp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegSp.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // lblRegSp
            // 
            this.lblRegSp.AutoSize = true;
            this.lblRegSp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegSp.Location = new System.Drawing.Point(99, 29);
            this.lblRegSp.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegSp.Name = "lblRegSp";
            this.lblRegSp.Size = new System.Drawing.Size(27, 13);
            this.lblRegSp.TabIndex = 4;
            this.lblRegSp.Text = "SP:";
            // 
            // lblTitleGeneral
            // 
            this.lblTitleGeneral.AutoSize = true;
            this.lblTitleGeneral.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTitleGeneral.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblTitleGeneral.Location = new System.Drawing.Point(10, 92);
            this.lblTitleGeneral.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitleGeneral.Name = "lblTitleGeneral";
            this.lblTitleGeneral.Size = new System.Drawing.Size(51, 13);
            this.lblTitleGeneral.TabIndex = 13;
            this.lblTitleGeneral.Text = "General";
            // 
            // txtRegAf_
            // 
            this.txtRegAf_.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegAf_.Location = new System.Drawing.Point(135, 111);
            this.txtRegAf_.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegAf_.Name = "txtRegAf_";
            this.txtRegAf_.Size = new System.Drawing.Size(50, 13);
            this.txtRegAf_.TabIndex = 18;
            this.txtRegAf_.Text = "#FFFF";
            this.txtRegAf_.WordWrap = false;
            this.txtRegAf_.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegAf_.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegAf_.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegAf_.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // lblRegAf_
            // 
            this.lblRegAf_.AutoSize = true;
            this.lblRegAf_.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegAf_.Location = new System.Drawing.Point(99, 111);
            this.lblRegAf_.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegAf_.Name = "lblRegAf_";
            this.lblRegAf_.Size = new System.Drawing.Size(29, 13);
            this.lblRegAf_.TabIndex = 17;
            this.lblRegAf_.Text = "AF\':";
            // 
            // txtRegAf
            // 
            this.txtRegAf.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegAf.Location = new System.Drawing.Point(43, 111);
            this.txtRegAf.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegAf.Name = "txtRegAf";
            this.txtRegAf.Size = new System.Drawing.Size(50, 13);
            this.txtRegAf.TabIndex = 16;
            this.txtRegAf.Text = "#FFFF";
            this.txtRegAf.WordWrap = false;
            this.txtRegAf.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegAf.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegAf.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegAf.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // lblRegAf
            // 
            this.lblRegAf.AutoSize = true;
            this.lblRegAf.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegAf.Location = new System.Drawing.Point(10, 111);
            this.lblRegAf.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegAf.Name = "lblRegAf";
            this.lblRegAf.Size = new System.Drawing.Size(26, 13);
            this.lblRegAf.TabIndex = 15;
            this.lblRegAf.Text = "AF:";
            // 
            // txtRegHl_
            // 
            this.txtRegHl_.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegHl_.Location = new System.Drawing.Point(135, 130);
            this.txtRegHl_.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegHl_.Name = "txtRegHl_";
            this.txtRegHl_.Size = new System.Drawing.Size(50, 13);
            this.txtRegHl_.TabIndex = 22;
            this.txtRegHl_.Text = "#FFFF";
            this.txtRegHl_.WordWrap = false;
            this.txtRegHl_.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegHl_.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegHl_.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegHl_.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // lblRegHl_
            // 
            this.lblRegHl_.AutoSize = true;
            this.lblRegHl_.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegHl_.Location = new System.Drawing.Point(99, 130);
            this.lblRegHl_.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegHl_.Name = "lblRegHl_";
            this.lblRegHl_.Size = new System.Drawing.Size(30, 13);
            this.lblRegHl_.TabIndex = 21;
            this.lblRegHl_.Text = "HL\':";
            // 
            // txtRegHl
            // 
            this.txtRegHl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegHl.Location = new System.Drawing.Point(43, 130);
            this.txtRegHl.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegHl.Name = "txtRegHl";
            this.txtRegHl.Size = new System.Drawing.Size(50, 13);
            this.txtRegHl.TabIndex = 20;
            this.txtRegHl.Text = "#FFFF";
            this.txtRegHl.WordWrap = false;
            this.txtRegHl.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegHl.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegHl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegHl.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // lblRegHl
            // 
            this.lblRegHl.AutoSize = true;
            this.lblRegHl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegHl.Location = new System.Drawing.Point(10, 130);
            this.lblRegHl.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegHl.Name = "lblRegHl";
            this.lblRegHl.Size = new System.Drawing.Size(27, 13);
            this.lblRegHl.TabIndex = 19;
            this.lblRegHl.Text = "HL:";
            // 
            // txtRegDe_
            // 
            this.txtRegDe_.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegDe_.Location = new System.Drawing.Point(135, 149);
            this.txtRegDe_.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegDe_.Name = "txtRegDe_";
            this.txtRegDe_.Size = new System.Drawing.Size(50, 13);
            this.txtRegDe_.TabIndex = 26;
            this.txtRegDe_.Text = "#FFFF";
            this.txtRegDe_.WordWrap = false;
            this.txtRegDe_.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegDe_.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegDe_.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegDe_.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // lblRegDe_
            // 
            this.lblRegDe_.AutoSize = true;
            this.lblRegDe_.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegDe_.Location = new System.Drawing.Point(99, 149);
            this.lblRegDe_.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegDe_.Name = "lblRegDe_";
            this.lblRegDe_.Size = new System.Drawing.Size(31, 13);
            this.lblRegDe_.TabIndex = 25;
            this.lblRegDe_.Text = "DE\':";
            // 
            // txtRegDe
            // 
            this.txtRegDe.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegDe.Location = new System.Drawing.Point(43, 149);
            this.txtRegDe.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegDe.Name = "txtRegDe";
            this.txtRegDe.Size = new System.Drawing.Size(50, 13);
            this.txtRegDe.TabIndex = 24;
            this.txtRegDe.Text = "#FFFF";
            this.txtRegDe.WordWrap = false;
            this.txtRegDe.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegDe.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegDe.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegDe.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // lblRegDe
            // 
            this.lblRegDe.AutoSize = true;
            this.lblRegDe.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegDe.Location = new System.Drawing.Point(10, 149);
            this.lblRegDe.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegDe.Name = "lblRegDe";
            this.lblRegDe.Size = new System.Drawing.Size(28, 13);
            this.lblRegDe.TabIndex = 23;
            this.lblRegDe.Text = "DE:";
            // 
            // txtRegBc_
            // 
            this.txtRegBc_.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegBc_.Location = new System.Drawing.Point(135, 168);
            this.txtRegBc_.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegBc_.Name = "txtRegBc_";
            this.txtRegBc_.Size = new System.Drawing.Size(50, 13);
            this.txtRegBc_.TabIndex = 30;
            this.txtRegBc_.Text = "#FFFF";
            this.txtRegBc_.WordWrap = false;
            this.txtRegBc_.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegBc_.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegBc_.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegBc_.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // lblRegBc_
            // 
            this.lblRegBc_.AutoSize = true;
            this.lblRegBc_.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegBc_.Location = new System.Drawing.Point(99, 168);
            this.lblRegBc_.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegBc_.Name = "lblRegBc_";
            this.lblRegBc_.Size = new System.Drawing.Size(30, 13);
            this.lblRegBc_.TabIndex = 29;
            this.lblRegBc_.Text = "BC\':";
            // 
            // txtRegBc
            // 
            this.txtRegBc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegBc.Location = new System.Drawing.Point(43, 168);
            this.txtRegBc.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegBc.Name = "txtRegBc";
            this.txtRegBc.Size = new System.Drawing.Size(50, 13);
            this.txtRegBc.TabIndex = 28;
            this.txtRegBc.Text = "#FFFF";
            this.txtRegBc.WordWrap = false;
            this.txtRegBc.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegBc.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegBc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegBc.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // lblRegBc
            // 
            this.lblRegBc.AutoSize = true;
            this.lblRegBc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegBc.Location = new System.Drawing.Point(10, 168);
            this.lblRegBc.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegBc.Name = "lblRegBc";
            this.lblRegBc.Size = new System.Drawing.Size(27, 13);
            this.lblRegBc.TabIndex = 27;
            this.lblRegBc.Text = "BC:";
            // 
            // txtRegIy
            // 
            this.txtRegIy.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegIy.Location = new System.Drawing.Point(135, 187);
            this.txtRegIy.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegIy.Name = "txtRegIy";
            this.txtRegIy.Size = new System.Drawing.Size(50, 13);
            this.txtRegIy.TabIndex = 34;
            this.txtRegIy.Text = "#FFFF";
            this.txtRegIy.WordWrap = false;
            this.txtRegIy.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegIy.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegIy.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegIy.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // lblRegIy
            // 
            this.lblRegIy.AutoSize = true;
            this.lblRegIy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegIy.Location = new System.Drawing.Point(99, 187);
            this.lblRegIy.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegIy.Name = "lblRegIy";
            this.lblRegIy.Size = new System.Drawing.Size(23, 13);
            this.lblRegIy.TabIndex = 33;
            this.lblRegIy.Text = "IY:";
            // 
            // txtRegIx
            // 
            this.txtRegIx.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegIx.Location = new System.Drawing.Point(43, 187);
            this.txtRegIx.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegIx.Name = "txtRegIx";
            this.txtRegIx.Size = new System.Drawing.Size(50, 13);
            this.txtRegIx.TabIndex = 32;
            this.txtRegIx.Text = "#FFFF";
            this.txtRegIx.WordWrap = false;
            this.txtRegIx.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegIx.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegIx.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegIx.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // lblRegIx
            // 
            this.lblRegIx.AutoSize = true;
            this.lblRegIx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegIx.Location = new System.Drawing.Point(10, 187);
            this.lblRegIx.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegIx.Name = "lblRegIx";
            this.lblRegIx.Size = new System.Drawing.Size(23, 13);
            this.lblRegIx.TabIndex = 31;
            this.lblRegIx.Text = "IX:";
            // 
            // txtRegIm
            // 
            this.txtRegIm.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegIm.Location = new System.Drawing.Point(135, 48);
            this.txtRegIm.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegIm.MaxLength = 1;
            this.txtRegIm.Name = "txtRegIm";
            this.txtRegIm.Size = new System.Drawing.Size(50, 13);
            this.txtRegIm.TabIndex = 8;
            this.txtRegIm.Text = "0";
            this.txtRegIm.WordWrap = false;
            this.txtRegIm.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegIm.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegIm.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegIm.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // lblRegIm
            // 
            this.lblRegIm.AutoSize = true;
            this.lblRegIm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegIm.Location = new System.Drawing.Point(99, 48);
            this.lblRegIm.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegIm.Name = "lblRegIm";
            this.lblRegIm.Size = new System.Drawing.Size(25, 13);
            this.lblRegIm.TabIndex = 7;
            this.lblRegIm.Text = "IM:";
            // 
            // txtRegIr
            // 
            this.txtRegIr.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegIr.Location = new System.Drawing.Point(43, 48);
            this.txtRegIr.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegIr.Name = "txtRegIr";
            this.txtRegIr.Size = new System.Drawing.Size(50, 13);
            this.txtRegIr.TabIndex = 6;
            this.txtRegIr.Text = "#FFFF";
            this.txtRegIr.WordWrap = false;
            this.txtRegIr.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegIr.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegIr.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegIr.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // lblRegIr
            // 
            this.lblRegIr.AutoSize = true;
            this.lblRegIr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegIr.Location = new System.Drawing.Point(10, 48);
            this.lblRegIr.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegIr.Name = "lblRegIr";
            this.lblRegIr.Size = new System.Drawing.Size(24, 13);
            this.lblRegIr.TabIndex = 5;
            this.lblRegIr.Text = "IR:";
            // 
            // chkFlagS
            // 
            this.chkFlagS.AutoSize = true;
            this.chkFlagS.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlagS.Location = new System.Drawing.Point(10, 231);
            this.chkFlagS.Margin = new System.Windows.Forms.Padding(0);
            this.chkFlagS.Name = "chkFlagS";
            this.chkFlagS.Size = new System.Drawing.Size(18, 31);
            this.chkFlagS.TabIndex = 37;
            this.chkFlagS.TabStop = false;
            this.chkFlagS.Text = "S";
            this.chkFlagS.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlagS.UseVisualStyleBackColor = true;
            // 
            // chkFlagZ
            // 
            this.chkFlagZ.AutoSize = true;
            this.chkFlagZ.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlagZ.Location = new System.Drawing.Point(28, 231);
            this.chkFlagZ.Margin = new System.Windows.Forms.Padding(0);
            this.chkFlagZ.Name = "chkFlagZ";
            this.chkFlagZ.Size = new System.Drawing.Size(18, 31);
            this.chkFlagZ.TabIndex = 38;
            this.chkFlagZ.TabStop = false;
            this.chkFlagZ.Text = "Z";
            this.chkFlagZ.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlagZ.UseVisualStyleBackColor = true;
            // 
            // chkFlag5
            // 
            this.chkFlag5.AutoSize = true;
            this.chkFlag5.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlag5.Location = new System.Drawing.Point(46, 231);
            this.chkFlag5.Margin = new System.Windows.Forms.Padding(0);
            this.chkFlag5.Name = "chkFlag5";
            this.chkFlag5.Size = new System.Drawing.Size(17, 31);
            this.chkFlag5.TabIndex = 39;
            this.chkFlag5.TabStop = false;
            this.chkFlag5.Text = "5";
            this.chkFlag5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlag5.UseVisualStyleBackColor = true;
            // 
            // chkFlagH
            // 
            this.chkFlagH.AutoSize = true;
            this.chkFlagH.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlagH.Location = new System.Drawing.Point(64, 231);
            this.chkFlagH.Margin = new System.Windows.Forms.Padding(0);
            this.chkFlagH.Name = "chkFlagH";
            this.chkFlagH.Size = new System.Drawing.Size(19, 31);
            this.chkFlagH.TabIndex = 40;
            this.chkFlagH.TabStop = false;
            this.chkFlagH.Text = "H";
            this.chkFlagH.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlagH.UseVisualStyleBackColor = true;
            // 
            // chkFlag3
            // 
            this.chkFlag3.AutoSize = true;
            this.chkFlag3.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlag3.Location = new System.Drawing.Point(82, 231);
            this.chkFlag3.Margin = new System.Windows.Forms.Padding(0);
            this.chkFlag3.Name = "chkFlag3";
            this.chkFlag3.Size = new System.Drawing.Size(17, 31);
            this.chkFlag3.TabIndex = 41;
            this.chkFlag3.TabStop = false;
            this.chkFlag3.Text = "3";
            this.chkFlag3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlag3.UseVisualStyleBackColor = true;
            // 
            // chkFlagV
            // 
            this.chkFlagV.AutoSize = true;
            this.chkFlagV.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlagV.Location = new System.Drawing.Point(100, 231);
            this.chkFlagV.Margin = new System.Windows.Forms.Padding(0);
            this.chkFlagV.Name = "chkFlagV";
            this.chkFlagV.Size = new System.Drawing.Size(18, 31);
            this.chkFlagV.TabIndex = 42;
            this.chkFlagV.TabStop = false;
            this.chkFlagV.Text = "V";
            this.chkFlagV.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlagV.UseVisualStyleBackColor = true;
            // 
            // chkFlagN
            // 
            this.chkFlagN.AutoSize = true;
            this.chkFlagN.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlagN.Location = new System.Drawing.Point(118, 231);
            this.chkFlagN.Margin = new System.Windows.Forms.Padding(0);
            this.chkFlagN.Name = "chkFlagN";
            this.chkFlagN.Size = new System.Drawing.Size(19, 31);
            this.chkFlagN.TabIndex = 43;
            this.chkFlagN.TabStop = false;
            this.chkFlagN.Text = "N";
            this.chkFlagN.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlagN.UseVisualStyleBackColor = true;
            // 
            // chkFlagC
            // 
            this.chkFlagC.AutoSize = true;
            this.chkFlagC.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlagC.Location = new System.Drawing.Point(136, 231);
            this.chkFlagC.Margin = new System.Windows.Forms.Padding(0);
            this.chkFlagC.Name = "chkFlagC";
            this.chkFlagC.Size = new System.Drawing.Size(18, 31);
            this.chkFlagC.TabIndex = 44;
            this.chkFlagC.TabStop = false;
            this.chkFlagC.Text = "C";
            this.chkFlagC.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkFlagC.UseVisualStyleBackColor = true;
            // 
            // lblTitleFlags
            // 
            this.lblTitleFlags.AutoSize = true;
            this.lblTitleFlags.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTitleFlags.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblTitleFlags.Location = new System.Drawing.Point(10, 212);
            this.lblTitleFlags.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitleFlags.Name = "lblTitleFlags";
            this.lblTitleFlags.Size = new System.Drawing.Size(37, 13);
            this.lblTitleFlags.TabIndex = 35;
            this.lblTitleFlags.Text = "Flags";
            // 
            // txtRegWz
            // 
            this.txtRegWz.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegWz.Location = new System.Drawing.Point(43, 67);
            this.txtRegWz.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegWz.Name = "txtRegWz";
            this.txtRegWz.Size = new System.Drawing.Size(50, 13);
            this.txtRegWz.TabIndex = 10;
            this.txtRegWz.Text = "#FFFF";
            this.txtRegWz.WordWrap = false;
            this.txtRegWz.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegWz.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegWz.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegWz.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // lblRegsWz
            // 
            this.lblRegsWz.AutoSize = true;
            this.lblRegsWz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegsWz.Location = new System.Drawing.Point(10, 67);
            this.lblRegsWz.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegsWz.Name = "lblRegsWz";
            this.lblRegsWz.Size = new System.Drawing.Size(31, 13);
            this.lblRegsWz.TabIndex = 9;
            this.lblRegsWz.Text = "WZ:";
            // 
            // chkHalt
            // 
            this.chkHalt.AutoSize = true;
            this.chkHalt.Location = new System.Drawing.Point(12, 285);
            this.chkHalt.Margin = new System.Windows.Forms.Padding(0);
            this.chkHalt.Name = "chkHalt";
            this.chkHalt.Size = new System.Drawing.Size(57, 17);
            this.chkHalt.TabIndex = 47;
            this.chkHalt.TabStop = false;
            this.chkHalt.Text = "Halted";
            this.chkHalt.UseVisualStyleBackColor = true;
            // 
            // chkIff1
            // 
            this.chkIff1.AutoSize = true;
            this.chkIff1.Location = new System.Drawing.Point(12, 268);
            this.chkIff1.Margin = new System.Windows.Forms.Padding(0);
            this.chkIff1.Name = "chkIff1";
            this.chkIff1.Size = new System.Drawing.Size(47, 17);
            this.chkIff1.TabIndex = 45;
            this.chkIff1.TabStop = false;
            this.chkIff1.Text = "IFF1";
            this.chkIff1.UseVisualStyleBackColor = true;
            // 
            // chkIff2
            // 
            this.chkIff2.AutoSize = true;
            this.chkIff2.Location = new System.Drawing.Point(102, 268);
            this.chkIff2.Name = "chkIff2";
            this.chkIff2.Size = new System.Drawing.Size(47, 17);
            this.chkIff2.TabIndex = 46;
            this.chkIff2.TabStop = false;
            this.chkIff2.Text = "IFF2";
            this.chkIff2.UseVisualStyleBackColor = true;
            // 
            // txtRegLpc
            // 
            this.txtRegLpc.BackColor = System.Drawing.Color.White;
            this.txtRegLpc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRegLpc.Location = new System.Drawing.Point(135, 67);
            this.txtRegLpc.Margin = new System.Windows.Forms.Padding(0);
            this.txtRegLpc.Name = "txtRegLpc";
            this.txtRegLpc.ReadOnly = true;
            this.txtRegLpc.Size = new System.Drawing.Size(50, 13);
            this.txtRegLpc.TabIndex = 12;
            this.txtRegLpc.Text = "#FFFF";
            this.txtRegLpc.WordWrap = false;
            this.txtRegLpc.Click += new System.EventHandler(this.txtReg_OnClick);
            this.txtRegLpc.Enter += new System.EventHandler(this.txtReg_OnEnter);
            this.txtRegLpc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReg_KeyPress);
            this.txtRegLpc.Leave += new System.EventHandler(this.txtReg_OnLeave);
            // 
            // lblRegLpc
            // 
            this.lblRegLpc.AutoSize = true;
            this.lblRegLpc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRegLpc.Location = new System.Drawing.Point(99, 67);
            this.lblRegLpc.Margin = new System.Windows.Forms.Padding(0);
            this.lblRegLpc.Name = "lblRegLpc";
            this.lblRegLpc.Size = new System.Drawing.Size(34, 13);
            this.lblRegLpc.TabIndex = 11;
            this.lblRegLpc.Text = "LPC:";
            // 
            // chkBint
            // 
            this.chkBint.AutoSize = true;
            this.chkBint.Location = new System.Drawing.Point(102, 285);
            this.chkBint.Name = "chkBint";
            this.chkBint.Size = new System.Drawing.Size(51, 17);
            this.chkBint.TabIndex = 48;
            this.chkBint.TabStop = false;
            this.chkBint.Text = "BINT";
            this.chkBint.UseVisualStyleBackColor = true;
            // 
            // sepFlags
            // 
            this.sepFlags.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.sepFlags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sepFlags.Location = new System.Drawing.Point(10, 225);
            this.sepFlags.Margin = new System.Windows.Forms.Padding(0);
            this.sepFlags.Name = "sepFlags";
            this.sepFlags.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.sepFlags.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.sepFlags.Size = new System.Drawing.Size(175, 6);
            this.sepFlags.TabIndex = 36;
            this.sepFlags.TabStop = false;
            this.sepFlags.Text = "separator3";
            // 
            // sepGeneral
            // 
            this.sepGeneral.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.sepGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sepGeneral.Location = new System.Drawing.Point(10, 105);
            this.sepGeneral.Margin = new System.Windows.Forms.Padding(0);
            this.sepGeneral.Name = "sepGeneral";
            this.sepGeneral.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.sepGeneral.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.sepGeneral.Size = new System.Drawing.Size(175, 6);
            this.sepGeneral.TabIndex = 14;
            this.sepGeneral.TabStop = false;
            this.sepGeneral.Text = "separator2";
            // 
            // sepControl
            // 
            this.sepControl.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.sepControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sepControl.Location = new System.Drawing.Point(10, 23);
            this.sepControl.Margin = new System.Windows.Forms.Padding(0);
            this.sepControl.Name = "sepControl";
            this.sepControl.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.sepControl.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.sepControl.Size = new System.Drawing.Size(175, 6);
            this.sepControl.TabIndex = 1;
            this.sepControl.TabStop = false;
            this.sepControl.Text = "separator1";
            // 
            // lblTitleRzx
            // 
            this.lblTitleRzx.AutoSize = true;
            this.lblTitleRzx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTitleRzx.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblTitleRzx.Location = new System.Drawing.Point(10, 314);
            this.lblTitleRzx.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitleRzx.Name = "lblTitleRzx";
            this.lblTitleRzx.Size = new System.Drawing.Size(28, 13);
            this.lblTitleRzx.TabIndex = 49;
            this.lblTitleRzx.Text = "Rzx";
            // 
            // sepRzx
            // 
            this.sepRzx.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.sepRzx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sepRzx.Location = new System.Drawing.Point(10, 327);
            this.sepRzx.Margin = new System.Windows.Forms.Padding(0);
            this.sepRzx.Name = "sepRzx";
            this.sepRzx.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.sepRzx.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.sepRzx.Size = new System.Drawing.Size(175, 6);
            this.sepRzx.TabIndex = 50;
            this.sepRzx.TabStop = false;
            this.sepRzx.Text = "separator4";
            // 
            // lblRzxInput
            // 
            this.lblRzxInput.AutoSize = true;
            this.lblRzxInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRzxInput.Location = new System.Drawing.Point(10, 352);
            this.lblRzxInput.Margin = new System.Windows.Forms.Padding(0);
            this.lblRzxInput.Name = "lblRzxInput";
            this.lblRzxInput.Size = new System.Drawing.Size(49, 13);
            this.lblRzxInput.TabIndex = 51;
            this.lblRzxInput.Text = "INPUT:";
            // 
            // lblRzxInputValue
            // 
            this.lblRzxInputValue.AutoSize = true;
            this.lblRzxInputValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRzxInputValue.Location = new System.Drawing.Point(61, 352);
            this.lblRzxInputValue.Margin = new System.Windows.Forms.Padding(0);
            this.lblRzxInputValue.Name = "lblRzxInputValue";
            this.lblRzxInputValue.Size = new System.Drawing.Size(48, 13);
            this.lblRzxInputValue.TabIndex = 52;
            this.lblRzxInputValue.Text = "0 / 3000";
            // 
            // lblRzxFrameValue
            // 
            this.lblRzxFrameValue.AutoSize = true;
            this.lblRzxFrameValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRzxFrameValue.Location = new System.Drawing.Point(61, 371);
            this.lblRzxFrameValue.Margin = new System.Windows.Forms.Padding(0);
            this.lblRzxFrameValue.Name = "lblRzxFrameValue";
            this.lblRzxFrameValue.Size = new System.Drawing.Size(48, 13);
            this.lblRzxFrameValue.TabIndex = 54;
            this.lblRzxFrameValue.Text = "0 / 3000";
            // 
            // lblRzxFrame
            // 
            this.lblRzxFrame.AutoSize = true;
            this.lblRzxFrame.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRzxFrame.Location = new System.Drawing.Point(10, 371);
            this.lblRzxFrame.Margin = new System.Windows.Forms.Padding(0);
            this.lblRzxFrame.Name = "lblRzxFrame";
            this.lblRzxFrame.Size = new System.Drawing.Size(53, 13);
            this.lblRzxFrame.TabIndex = 53;
            this.lblRzxFrame.Text = "FRAME:";
            // 
            // lblRzxFetchValue
            // 
            this.lblRzxFetchValue.AutoSize = true;
            this.lblRzxFetchValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRzxFetchValue.Location = new System.Drawing.Point(61, 333);
            this.lblRzxFetchValue.Margin = new System.Windows.Forms.Padding(0);
            this.lblRzxFetchValue.Name = "lblRzxFetchValue";
            this.lblRzxFetchValue.Size = new System.Drawing.Size(48, 13);
            this.lblRzxFetchValue.TabIndex = 56;
            this.lblRzxFetchValue.Text = "0 / 3000";
            // 
            // lblRzxFetch
            // 
            this.lblRzxFetch.AutoSize = true;
            this.lblRzxFetch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRzxFetch.Location = new System.Drawing.Point(10, 333);
            this.lblRzxFetch.Margin = new System.Windows.Forms.Padding(0);
            this.lblRzxFetch.Name = "lblRzxFetch";
            this.lblRzxFetch.Size = new System.Drawing.Size(51, 13);
            this.lblRzxFetch.TabIndex = 55;
            this.lblRzxFetch.Text = "FETCH:";
            // 
            // FormRegisters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(195, 399);
            this.Controls.Add(this.lblRzxFetchValue);
            this.Controls.Add(this.lblRzxFetch);
            this.Controls.Add(this.lblRzxFrameValue);
            this.Controls.Add(this.lblRzxFrame);
            this.Controls.Add(this.lblRzxInputValue);
            this.Controls.Add(this.lblRzxInput);
            this.Controls.Add(this.sepRzx);
            this.Controls.Add(this.lblTitleRzx);
            this.Controls.Add(this.chkBint);
            this.Controls.Add(this.txtRegLpc);
            this.Controls.Add(this.lblRegLpc);
            this.Controls.Add(this.chkIff2);
            this.Controls.Add(this.chkIff1);
            this.Controls.Add(this.chkHalt);
            this.Controls.Add(this.txtRegWz);
            this.Controls.Add(this.lblRegsWz);
            this.Controls.Add(this.sepFlags);
            this.Controls.Add(this.lblTitleFlags);
            this.Controls.Add(this.chkFlagC);
            this.Controls.Add(this.chkFlagN);
            this.Controls.Add(this.chkFlagV);
            this.Controls.Add(this.chkFlag3);
            this.Controls.Add(this.chkFlagH);
            this.Controls.Add(this.chkFlag5);
            this.Controls.Add(this.chkFlagZ);
            this.Controls.Add(this.chkFlagS);
            this.Controls.Add(this.txtRegIm);
            this.Controls.Add(this.lblRegIm);
            this.Controls.Add(this.txtRegIr);
            this.Controls.Add(this.lblRegIr);
            this.Controls.Add(this.txtRegIy);
            this.Controls.Add(this.lblRegIy);
            this.Controls.Add(this.txtRegIx);
            this.Controls.Add(this.lblRegIx);
            this.Controls.Add(this.txtRegBc_);
            this.Controls.Add(this.lblRegBc_);
            this.Controls.Add(this.txtRegBc);
            this.Controls.Add(this.lblRegBc);
            this.Controls.Add(this.txtRegDe_);
            this.Controls.Add(this.lblRegDe_);
            this.Controls.Add(this.txtRegDe);
            this.Controls.Add(this.lblRegDe);
            this.Controls.Add(this.txtRegHl_);
            this.Controls.Add(this.lblRegHl_);
            this.Controls.Add(this.txtRegHl);
            this.Controls.Add(this.lblRegHl);
            this.Controls.Add(this.txtRegAf_);
            this.Controls.Add(this.lblRegAf_);
            this.Controls.Add(this.txtRegAf);
            this.Controls.Add(this.lblRegAf);
            this.Controls.Add(this.sepGeneral);
            this.Controls.Add(this.lblTitleGeneral);
            this.Controls.Add(this.txtRegSp);
            this.Controls.Add(this.lblRegSp);
            this.Controls.Add(this.txtRegPc);
            this.Controls.Add(this.lblRegPc);
            this.Controls.Add(this.sepControl);
            this.Controls.Add(this.lblTitleControl);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.Black;
            this.MinimumSize = new System.Drawing.Size(208, 300);
            this.Name = "FormRegisters";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.Text = "Registers";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitleControl;
        private Host.WinForms.Controls.Separator sepControl;
        private System.Windows.Forms.Label lblRegPc;
        private System.Windows.Forms.TextBox txtRegPc;
        private System.Windows.Forms.TextBox txtRegSp;
        private System.Windows.Forms.Label lblRegSp;
        private Host.WinForms.Controls.Separator sepGeneral;
        private System.Windows.Forms.Label lblTitleGeneral;
        private System.Windows.Forms.TextBox txtRegAf_;
        private System.Windows.Forms.Label lblRegAf_;
        private System.Windows.Forms.TextBox txtRegAf;
        private System.Windows.Forms.Label lblRegAf;
        private System.Windows.Forms.TextBox txtRegHl_;
        private System.Windows.Forms.Label lblRegHl_;
        private System.Windows.Forms.TextBox txtRegHl;
        private System.Windows.Forms.Label lblRegHl;
        private System.Windows.Forms.TextBox txtRegDe_;
        private System.Windows.Forms.Label lblRegDe_;
        private System.Windows.Forms.TextBox txtRegDe;
        private System.Windows.Forms.Label lblRegDe;
        private System.Windows.Forms.TextBox txtRegBc_;
        private System.Windows.Forms.Label lblRegBc_;
        private System.Windows.Forms.TextBox txtRegBc;
        private System.Windows.Forms.Label lblRegBc;
        private System.Windows.Forms.TextBox txtRegIy;
        private System.Windows.Forms.Label lblRegIy;
        private System.Windows.Forms.TextBox txtRegIx;
        private System.Windows.Forms.Label lblRegIx;
        private System.Windows.Forms.TextBox txtRegIm;
        private System.Windows.Forms.Label lblRegIm;
        private System.Windows.Forms.TextBox txtRegIr;
        private System.Windows.Forms.Label lblRegIr;
        private System.Windows.Forms.CheckBox chkFlagS;
        private System.Windows.Forms.CheckBox chkFlagZ;
        private System.Windows.Forms.CheckBox chkFlag5;
        private System.Windows.Forms.CheckBox chkFlagH;
        private System.Windows.Forms.CheckBox chkFlag3;
        private System.Windows.Forms.CheckBox chkFlagV;
        private System.Windows.Forms.CheckBox chkFlagN;
        private System.Windows.Forms.CheckBox chkFlagC;
        private Host.WinForms.Controls.Separator sepFlags;
        private System.Windows.Forms.Label lblTitleFlags;
        private System.Windows.Forms.TextBox txtRegWz;
        private System.Windows.Forms.Label lblRegsWz;
        private System.Windows.Forms.CheckBox chkHalt;
        private System.Windows.Forms.CheckBox chkIff1;
        private System.Windows.Forms.CheckBox chkIff2;
        private System.Windows.Forms.TextBox txtRegLpc;
        private System.Windows.Forms.Label lblRegLpc;
        private System.Windows.Forms.CheckBox chkBint;
        private System.Windows.Forms.Label lblTitleRzx;
        private Host.WinForms.Controls.Separator sepRzx;
        private System.Windows.Forms.Label lblRzxInput;
        private System.Windows.Forms.Label lblRzxInputValue;
        private System.Windows.Forms.Label lblRzxFrameValue;
        private System.Windows.Forms.Label lblRzxFrame;
        private System.Windows.Forms.Label lblRzxFetchValue;
        private System.Windows.Forms.Label lblRzxFetch;
    }
}