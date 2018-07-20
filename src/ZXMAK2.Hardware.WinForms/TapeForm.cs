using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ZXMAK2.Model.Tape.Interfaces;
using ZXMAK2.Resources;
using ZXMAK2.Host.Presentation.Interfaces;
using ZXMAK2.Host.WinForms.Views;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Hardware.WinForms
{
    public class TapeForm : FormView, ITapeView
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private ZXMAK2.Host.WinForms.Controls.ToolStripEx toolBar;
        private System.Windows.Forms.Panel panelList;
        private System.Windows.Forms.ListBox blockList;
        private System.Windows.Forms.ToolStripButton btnRewind;
        private System.Windows.Forms.ToolStripButton btnPrev;
        private System.Windows.Forms.ToolStripButton btnPlay;
        private System.Windows.Forms.ToolStripButton btnNext;
        private System.Windows.Forms.ToolStripProgressBar toolProgressBar;
        private System.Windows.Forms.Timer timerProgress;
        private System.Windows.Forms.ToolStripButton btnUseTraps;
        private System.Windows.Forms.ToolStripButton btnUseAutoPlay;

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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolBar = new ZXMAK2.Host.WinForms.Controls.ToolStripEx();
            this.btnRewind = new System.Windows.Forms.ToolStripButton();
            this.btnPrev = new System.Windows.Forms.ToolStripButton();
            this.btnPlay = new System.Windows.Forms.ToolStripButton();
            this.btnNext = new System.Windows.Forms.ToolStripButton();
            this.toolProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.btnUseTraps = new System.Windows.Forms.ToolStripButton();
            this.btnUseAutoPlay = new System.Windows.Forms.ToolStripButton();
            this.panelList = new System.Windows.Forms.Panel();
            this.blockList = new System.Windows.Forms.ListBox();
            this.timerProgress = new System.Windows.Forms.Timer(this.components);
            this.toolBar.SuspendLayout();
            this.panelList.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolBar
            // 
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRewind,
            this.btnPrev,
            this.btnPlay,
            this.btnNext,
            this.toolProgressBar,
            this.btnUseTraps,
            this.btnUseAutoPlay});
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(390, 27);
            this.toolBar.TabIndex = 2;
            this.toolBar.Text = "toolBar";
            // 
            // btnRewind
            // 
            this.btnRewind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRewind.Image = global::ZXMAK2.Resources.ResourceImages.HardwareTapeRewind;
            this.btnRewind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRewind.Name = "btnRewind";
            this.btnRewind.Size = new System.Drawing.Size(23, 24);
            this.btnRewind.Text = "Rewind";
            this.btnRewind.Click += new System.EventHandler(this.toolButtonRewind_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPrev.Image = global::ZXMAK2.Resources.ResourceImages.HardwareTapePrev;
            this.btnPrev.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(23, 24);
            this.btnPrev.Text = "Previous block";
            this.btnPrev.Click += new System.EventHandler(this.toolButtonPrev_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPlay.Image = global::ZXMAK2.Resources.ResourceImages.HardwareTapePlay;
            this.btnPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(23, 24);
            this.btnPlay.Text = "Play/Stop";
            this.btnPlay.Click += new System.EventHandler(this.toolButtonPlay_Click);
            // 
            // btnNext
            // 
            this.btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNext.Image = global::ZXMAK2.Resources.ResourceImages.HardwareTapeNext;
            this.btnNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(23, 24);
            this.btnNext.Text = "Next block";
            this.btnNext.Click += new System.EventHandler(this.toolButtonNext_Click);
            // 
            // toolProgressBar
            // 
            this.toolProgressBar.Name = "toolProgressBar";
            this.toolProgressBar.Size = new System.Drawing.Size(170, 24);
            this.toolProgressBar.Step = 1;
            this.toolProgressBar.ToolTipText = "Loading progress";
            this.toolProgressBar.Value = 50;
            // 
            // btnTraps
            // 
            this.btnUseTraps.CheckOnClick = true;
            this.btnUseTraps.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUseTraps.Image = global::ZXMAK2.Resources.ResourceImages.HardwareTapeTraps;
            this.btnUseTraps.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUseTraps.Name = "btnTraps";
            this.btnUseTraps.Size = new System.Drawing.Size(23, 24);
            this.btnUseTraps.Text = "Use Traps";
            this.btnUseTraps.Click += new System.EventHandler(this.btnTraps_Click);
            // 
            // btnAutoPlay
            // 
            this.btnUseAutoPlay.CheckOnClick = true;
            this.btnUseAutoPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUseAutoPlay.Image = global::ZXMAK2.Resources.ResourceImages.HardwareTapeAutoplay;
            this.btnUseAutoPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUseAutoPlay.Name = "btnAutoPlay";
            this.btnUseAutoPlay.Size = new System.Drawing.Size(23, 24);
            this.btnUseAutoPlay.Text = "Use Auto Play";
            this.btnUseAutoPlay.Click += new System.EventHandler(this.btnAutoPlay_Click);
            // 
            // panelList
            // 
            this.panelList.Controls.Add(this.blockList);
            this.panelList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelList.Location = new System.Drawing.Point(0, 27);
            this.panelList.Name = "panelList";
            this.panelList.Size = new System.Drawing.Size(390, 183);
            this.panelList.TabIndex = 3;
            // 
            // blockList
            // 
            this.blockList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockList.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.blockList.FormattingEnabled = true;
            this.blockList.IntegralHeight = false;
            this.blockList.ItemHeight = 16;
            this.blockList.Location = new System.Drawing.Point(0, 0);
            this.blockList.Name = "blockList";
            this.blockList.Size = new System.Drawing.Size(390, 183);
            this.blockList.TabIndex = 0;
            this.blockList.Click += new System.EventHandler(this.blockList_Click);
            this.blockList.DoubleClick += new System.EventHandler(this.blockList_DoubleClick);
            // 
            // timerProgress
            // 
            this.timerProgress.Enabled = true;
            this.timerProgress.Interval = 200;
            this.timerProgress.Tick += new System.EventHandler(this.timerProgress_Tick);
            // 
            // TapeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 210);
            this.Controls.Add(this.panelList);
            this.Controls.Add(this.toolBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TapeForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Tape";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TapeForm_FormClosed);
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
            this.panelList.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private ITapeDevice m_tape;

        public TapeForm(ITapeDevice tapeDevice)
        {
            m_tape = tapeDevice;
            InitializeComponent();
            tapeDevice.TapeStateChanged += new EventHandler(OnTapeStateChanged);
            OnTapeStateChanged(null, null);
            OnTapeStateChanged(null, null);
        }

        private void TapeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_tape.TapeStateChanged -= new EventHandler(OnTapeStateChanged);
        }


        private void toolButtonRewind_Click(object sender, EventArgs e)
        {
            m_tape.Rewind();
        }

        private void toolButtonPrev_Click(object sender, EventArgs e)
        {
            m_tape.CurrentBlock--;
        }

        private void toolButtonPlay_Click(object sender, EventArgs e)
        {
            if (m_tape.IsPlay)
                m_tape.Stop();
            else
                m_tape.Play();
        }

        private void toolButtonNext_Click(object sender, EventArgs e)
        {
            m_tape.CurrentBlock++;
        }

        private void OnTapeStateChanged(object sender, EventArgs args)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler(OnTapeStateChanged), sender, args);
                return;
            }
            if (m_tape.Blocks.Count <= 0)
            {
                btnRewind.Enabled =
                   btnPrev.Enabled =
                   btnPlay.Enabled =
                   btnNext.Enabled = false;
                blockList.SelectedIndex = -1;
            }
            else
            {
                btnNext.Enabled = btnPrev.Enabled = !m_tape.IsPlay;
                btnRewind.Enabled = btnPlay.Enabled = true;
                if (m_tape.IsPlay)
                    btnPlay.Image = ResourceImages.HardwareTapePause;
                else
                    btnPlay.Image = ResourceImages.HardwareTapePlay;
                if (checkContentDifferent(blockList.Items, m_tape.Blocks))
                {
                    blockList.Items.Clear();
                    foreach (var tb in m_tape.Blocks)
                        blockList.Items.Add(tb.Description);
                }
                blockList.SelectedIndex = m_tape.CurrentBlock;
            }
            blockList.Enabled = !m_tape.IsPlay;
            btnUseTraps.Checked = m_tape.UseTraps;
            btnUseAutoPlay.Checked = m_tape.UseAutoPlay;
            btnPlay.Enabled = !btnUseAutoPlay.Checked;
        }

        private bool checkContentDifferent(ListBox.ObjectCollection itemList, List<ITapeBlock> list)
        {
            if (itemList.Count != list.Count)
                return true;
            for (int i = 0; i < list.Count; i++)
                if ((string)itemList[i] != list[i].Description)
                    return true;
            return false;
        }

        private void timerProgress_Tick(object sender, EventArgs e)
        {
            toolProgressBar.Minimum = 0;

            int blockCount = m_tape.Blocks.Count;
            int curBlock, position, maximum;
            do
            {
                curBlock = m_tape.CurrentBlock;
                if (curBlock >= 0 && curBlock < blockCount)
                {
                    maximum = m_tape.Blocks[curBlock].Count;
                    position = m_tape.Position;
                }
                else
                {
                    maximum = 65535;
                    position = 0;
                }
            } while (position > maximum);

            toolProgressBar.Maximum = maximum;
            toolProgressBar.Value = position;
        }

        private void blockList_Click(object sender, EventArgs e)
        {
            if (!blockList.Enabled || m_tape.IsPlay) return;
            m_tape.CurrentBlock = blockList.SelectedIndex;
        }

        private void blockList_DoubleClick(object sender, EventArgs e)
        {
            if (!blockList.Enabled || m_tape.IsPlay) return;
            m_tape.CurrentBlock = blockList.SelectedIndex;
            m_tape.Play();
        }

        private void btnTraps_Click(object sender, EventArgs e)
        {
            m_tape.UseTraps = btnUseTraps.Checked;
        }

        private void btnAutoPlay_Click(object sender, EventArgs e)
        {
            m_tape.UseAutoPlay = btnUseAutoPlay.Checked;
            btnPlay.Enabled = !btnUseAutoPlay.Checked;
        }
    }
}