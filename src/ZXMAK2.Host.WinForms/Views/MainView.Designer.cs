namespace ZXMAK2.Host.WinForms.Views
{
    partial class MainView
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
            if (disposing)
            {
                if (_host != null)
                {
                    _host.Dispose();
                }
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.mnuStrip = new ZXMAK2.Host.WinForms.Controls.MenuStripEx();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewCustomize = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewCustomizeShowToolBar = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewCustomizeShowStatusBar = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuViewSize = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewSizeX1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewSizeX2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewSizeX3 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewSizeX4 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewFullScreen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuViewScaleMode = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewScaleModeStretch = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewScaleModeKeepProportion = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewScaleModeFixedPixelSize = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewScaleModeSquarePixelSize = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewVideoFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewVideoFilterNone = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewVideoFilterNoFlick = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuViewFrameSync = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewFrameSyncTime = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewFrameSyncSound = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewFrameSyncVideo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuViewSmoothing = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewMimicTv = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewDisplayIcon = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewDebugInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuVm = new System.Windows.Forms.ToolStripMenuItem();
            this.menuVmPause = new System.Windows.Forms.ToolStripMenuItem();
            this.menuVmMaximumSpeed = new System.Windows.Forms.ToolStripMenuItem();
            this.menuVmSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuVmWarmReset = new System.Windows.Forms.ToolStripMenuItem();
            this.menuVmColdReset = new System.Windows.Forms.ToolStripMenuItem();
            this.menuVmNmi = new System.Windows.Forms.ToolStripMenuItem();
            this.menuVmSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuVmSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpViewHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpKeyboardHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tbrStrip = new ZXMAK2.Host.WinForms.Controls.ToolStripEx();
            this.tbrButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.tbrButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbrButtonPause = new System.Windows.Forms.ToolStripButton();
            this.tbrButtonMaxSpeed = new System.Windows.Forms.ToolStripButton();
            this.tbrButtonWarmReset = new System.Windows.Forms.ToolStripButton();
            this.tbrButtonColdReset = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbrButtonFullScreen = new System.Windows.Forms.ToolStripButton();
            this.tbrButtonQuickLoad = new System.Windows.Forms.ToolStripButton();
            this.tbrButtonSettings = new System.Windows.Forms.ToolStripButton();
            this.sbrStrip = new System.Windows.Forms.StatusStrip();
            this.renderVideo = new ZXMAK2.Host.WinForms.Controls.RenderVideo();
            this.mnuStrip.SuspendLayout();
            this.tbrStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuStrip
            // 
            this.mnuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuView,
            this.menuVm,
            this.menuTools,
            this.menuHelp});
            this.mnuStrip.Location = new System.Drawing.Point(0, 0);
            this.mnuStrip.Name = "mnuStrip";
            this.mnuStrip.Size = new System.Drawing.Size(704, 24);
            this.mnuStrip.TabIndex = 0;
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFileOpen,
            this.menuFileSaveAs,
            this.menuFileSeparator,
            this.menuFileExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(37, 20);
            this.menuFile.Text = "File";
            // 
            // menuFileOpen
            // 
            this.menuFileOpen.Name = "menuFileOpen";
            this.menuFileOpen.Size = new System.Drawing.Size(123, 22);
            this.menuFileOpen.Text = "Open...";
            // 
            // menuFileSaveAs
            // 
            this.menuFileSaveAs.Name = "menuFileSaveAs";
            this.menuFileSaveAs.Size = new System.Drawing.Size(123, 22);
            this.menuFileSaveAs.Text = "Save As...";
            // 
            // menuFileSeparator
            // 
            this.menuFileSeparator.Name = "menuFileSeparator";
            this.menuFileSeparator.Size = new System.Drawing.Size(120, 6);
            // 
            // menuFileExit
            // 
            this.menuFileExit.Name = "menuFileExit";
            this.menuFileExit.Size = new System.Drawing.Size(123, 22);
            this.menuFileExit.Text = "Exit";
            // 
            // menuView
            // 
            this.menuView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.menuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuViewCustomize,
            this.menuViewSeparator1,
            this.menuViewSize,
            this.menuViewFullScreen,
            this.menuViewSeparator2,
            this.menuViewScaleMode,
            this.menuViewVideoFilter,
            this.menuViewSeparator3,
            this.menuViewFrameSync,
            this.menuViewSeparator4,
            this.menuViewSmoothing,
            this.menuViewMimicTv,
            this.menuViewDisplayIcon,
            this.menuViewDebugInfo});
            this.menuView.Name = "menuView";
            this.menuView.Size = new System.Drawing.Size(44, 20);
            this.menuView.Text = "View";
            // 
            // menuViewCustomize
            // 
            this.menuViewCustomize.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuViewCustomizeShowToolBar,
            this.menuViewCustomizeShowStatusBar});
            this.menuViewCustomize.Name = "menuViewCustomize";
            this.menuViewCustomize.Size = new System.Drawing.Size(188, 22);
            this.menuViewCustomize.Text = "Customize";
            // 
            // menuViewCustomizeShowToolBar
            // 
            this.menuViewCustomizeShowToolBar.CheckOnClick = true;
            this.menuViewCustomizeShowToolBar.Name = "menuViewCustomizeShowToolBar";
            this.menuViewCustomizeShowToolBar.Size = new System.Drawing.Size(126, 22);
            this.menuViewCustomizeShowToolBar.Text = "Tool Bar";
            // 
            // menuViewCustomizeShowStatusBar
            // 
            this.menuViewCustomizeShowStatusBar.CheckOnClick = true;
            this.menuViewCustomizeShowStatusBar.Name = "menuViewCustomizeShowStatusBar";
            this.menuViewCustomizeShowStatusBar.Size = new System.Drawing.Size(126, 22);
            this.menuViewCustomizeShowStatusBar.Text = "Status Bar";
            // 
            // menuViewSeparator1
            // 
            this.menuViewSeparator1.Name = "menuViewSeparator1";
            this.menuViewSeparator1.Size = new System.Drawing.Size(185, 6);
            // 
            // menuViewSize
            // 
            this.menuViewSize.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuViewSizeX1,
            this.menuViewSizeX2,
            this.menuViewSizeX3,
            this.menuViewSizeX4});
            this.menuViewSize.Name = "menuViewSize";
            this.menuViewSize.Size = new System.Drawing.Size(188, 22);
            this.menuViewSize.Text = "Size";
            // 
            // menuViewSizeX1
            // 
            this.menuViewSizeX1.Name = "menuViewSizeX1";
            this.menuViewSizeX1.Size = new System.Drawing.Size(102, 22);
            this.menuViewSizeX1.Text = "100%";
            // 
            // menuViewSizeX2
            // 
            this.menuViewSizeX2.Name = "menuViewSizeX2";
            this.menuViewSizeX2.Size = new System.Drawing.Size(102, 22);
            this.menuViewSizeX2.Text = "200%";
            // 
            // menuViewSizeX3
            // 
            this.menuViewSizeX3.Name = "menuViewSizeX3";
            this.menuViewSizeX3.Size = new System.Drawing.Size(102, 22);
            this.menuViewSizeX3.Text = "300%";
            // 
            // menuViewSizeX4
            // 
            this.menuViewSizeX4.Name = "menuViewSizeX4";
            this.menuViewSizeX4.Size = new System.Drawing.Size(102, 22);
            this.menuViewSizeX4.Text = "400%";
            // 
            // menuViewFullScreen
            // 
            this.menuViewFullScreen.Name = "menuViewFullScreen";
            this.menuViewFullScreen.ShortcutKeyDisplayString = "Alt+Enter";
            this.menuViewFullScreen.Size = new System.Drawing.Size(188, 22);
            this.menuViewFullScreen.Text = "Full Screen";
            // 
            // menuViewSeparator2
            // 
            this.menuViewSeparator2.Name = "menuViewSeparator2";
            this.menuViewSeparator2.Size = new System.Drawing.Size(185, 6);
            // 
            // menuViewScaleMode
            // 
            this.menuViewScaleMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuViewScaleModeStretch,
            this.menuViewScaleModeKeepProportion,
            this.menuViewScaleModeFixedPixelSize,
            this.menuViewScaleModeSquarePixelSize});
            this.menuViewScaleMode.Name = "menuViewScaleMode";
            this.menuViewScaleMode.Size = new System.Drawing.Size(188, 22);
            this.menuViewScaleMode.Text = "Scale Mode";
            // 
            // menuViewScaleModeStretch
            // 
            this.menuViewScaleModeStretch.Name = "menuViewScaleModeStretch";
            this.menuViewScaleModeStretch.Size = new System.Drawing.Size(160, 22);
            this.menuViewScaleModeStretch.Text = "Stretch";
            // 
            // menuViewScaleModeKeepProportion
            // 
            this.menuViewScaleModeKeepProportion.Name = "menuViewScaleModeKeepProportion";
            this.menuViewScaleModeKeepProportion.Size = new System.Drawing.Size(160, 22);
            this.menuViewScaleModeKeepProportion.Text = "Keep Proportion";
            // 
            // menuViewScaleModeFixedPixelSize
            // 
            this.menuViewScaleModeFixedPixelSize.Name = "menuViewScaleModeFixedPixelSize";
            this.menuViewScaleModeFixedPixelSize.Size = new System.Drawing.Size(160, 22);
            this.menuViewScaleModeFixedPixelSize.Text = "Fixed Pixel Size";
            // 
            // menuViewScaleModeSquarePixelSize
            // 
            this.menuViewScaleModeSquarePixelSize.Name = "menuViewScaleModeSquarePixelSize";
            this.menuViewScaleModeSquarePixelSize.Size = new System.Drawing.Size(160, 22);
            this.menuViewScaleModeSquarePixelSize.Text = "Square Pixel Size";
            // 
            // menuViewVideoFilter
            // 
            this.menuViewVideoFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuViewVideoFilterNone,
            this.menuViewVideoFilterNoFlick});
            this.menuViewVideoFilter.Name = "menuViewVideoFilter";
            this.menuViewVideoFilter.Size = new System.Drawing.Size(188, 22);
            this.menuViewVideoFilter.Text = "Video Filter";
            // 
            // menuViewVideoFilterNone
            // 
            this.menuViewVideoFilterNone.Name = "menuViewVideoFilterNone";
            this.menuViewVideoFilterNone.Size = new System.Drawing.Size(152, 22);
            this.menuViewVideoFilterNone.Text = "None";
            // 
            // menuViewVideoFilterNoFlick
            // 
            this.menuViewVideoFilterNoFlick.Name = "menuViewVideoFilterNoFlick";
            this.menuViewVideoFilterNoFlick.Size = new System.Drawing.Size(152, 22);
            this.menuViewVideoFilterNoFlick.Text = "No Flick";
            // 
            // menuViewSeparator3
            // 
            this.menuViewSeparator3.Name = "menuViewSeparator3";
            this.menuViewSeparator3.Size = new System.Drawing.Size(185, 6);
            // 
            // menuViewFrameSync
            // 
            this.menuViewFrameSync.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuViewFrameSyncTime,
            this.menuViewFrameSyncSound,
            this.menuViewFrameSyncVideo});
            this.menuViewFrameSync.Name = "menuViewFrameSync";
            this.menuViewFrameSync.Size = new System.Drawing.Size(188, 22);
            this.menuViewFrameSync.Text = "Frame Sync Source";
            // 
            // menuViewFrameSyncTime
            // 
            this.menuViewFrameSyncTime.Name = "menuViewFrameSyncTime";
            this.menuViewFrameSyncTime.Size = new System.Drawing.Size(108, 22);
            this.menuViewFrameSyncTime.Text = "Time";
            // 
            // menuViewFrameSyncSound
            // 
            this.menuViewFrameSyncSound.Name = "menuViewFrameSyncSound";
            this.menuViewFrameSyncSound.Size = new System.Drawing.Size(108, 22);
            this.menuViewFrameSyncSound.Text = "Sound";
            // 
            // menuViewFrameSyncVideo
            // 
            this.menuViewFrameSyncVideo.Name = "menuViewFrameSyncVideo";
            this.menuViewFrameSyncVideo.Size = new System.Drawing.Size(108, 22);
            this.menuViewFrameSyncVideo.Text = "Video";
            // 
            // menuViewSeparator4
            // 
            this.menuViewSeparator4.Name = "menuViewSeparator4";
            this.menuViewSeparator4.Size = new System.Drawing.Size(185, 6);
            // 
            // menuViewSmoothing
            // 
            this.menuViewSmoothing.Name = "menuViewSmoothing";
            this.menuViewSmoothing.Size = new System.Drawing.Size(188, 22);
            this.menuViewSmoothing.Text = "Smoothing";
            // 
            // menuViewMimicTv
            // 
            this.menuViewMimicTv.Name = "menuViewMimicTv";
            this.menuViewMimicTv.Size = new System.Drawing.Size(188, 22);
            this.menuViewMimicTv.Text = "Mimic TV";
            // 
            // menuViewDisplayIcon
            // 
            this.menuViewDisplayIcon.Name = "menuViewDisplayIcon";
            this.menuViewDisplayIcon.Size = new System.Drawing.Size(188, 22);
            this.menuViewDisplayIcon.Text = "Display Icons";
            // 
            // menuViewDebugInfo
            // 
            this.menuViewDebugInfo.Name = "menuViewDebugInfo";
            this.menuViewDebugInfo.Size = new System.Drawing.Size(188, 22);
            this.menuViewDebugInfo.Text = "Debug Info";
            // 
            // menuVm
            // 
            this.menuVm.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuVmPause,
            this.menuVmMaximumSpeed,
            this.menuVmSeparator1,
            this.menuVmWarmReset,
            this.menuVmColdReset,
            this.menuVmNmi,
            this.menuVmSeparator2,
            this.menuVmSettings});
            this.menuVm.Name = "menuVm";
            this.menuVm.Size = new System.Drawing.Size(37, 20);
            this.menuVm.Text = "VM";
            // 
            // menuVmPause
            // 
            this.menuVmPause.Name = "menuVmPause";
            this.menuVmPause.ShortcutKeyDisplayString = "Pause";
            this.menuVmPause.Size = new System.Drawing.Size(226, 22);
            this.menuVmPause.Text = "Pause";
            // 
            // menuVmMaximumSpeed
            // 
            this.menuVmMaximumSpeed.Name = "menuVmMaximumSpeed";
            this.menuVmMaximumSpeed.ShortcutKeyDisplayString = "Ctrl+Scroll";
            this.menuVmMaximumSpeed.Size = new System.Drawing.Size(226, 22);
            this.menuVmMaximumSpeed.Text = "Maximum Speed";
            // 
            // menuVmSeparator1
            // 
            this.menuVmSeparator1.Name = "menuVmSeparator1";
            this.menuVmSeparator1.Size = new System.Drawing.Size(223, 6);
            // 
            // menuVmWarmReset
            // 
            this.menuVmWarmReset.Name = "menuVmWarmReset";
            this.menuVmWarmReset.ShortcutKeyDisplayString = "Alt+Ctrl+Insert";
            this.menuVmWarmReset.Size = new System.Drawing.Size(226, 22);
            this.menuVmWarmReset.Text = "Warm Reset";
            // 
            // menuVmColdReset
            // 
            this.menuVmColdReset.Name = "menuVmColdReset";
            this.menuVmColdReset.Size = new System.Drawing.Size(226, 22);
            this.menuVmColdReset.Text = "Cold Reset";
            // 
            // menuVmNmi
            // 
            this.menuVmNmi.Name = "menuVmNmi";
            this.menuVmNmi.Size = new System.Drawing.Size(226, 22);
            this.menuVmNmi.Text = "NMI";
            // 
            // menuVmSeparator2
            // 
            this.menuVmSeparator2.Name = "menuVmSeparator2";
            this.menuVmSeparator2.Size = new System.Drawing.Size(223, 6);
            // 
            // menuVmSettings
            // 
            this.menuVmSettings.Name = "menuVmSettings";
            this.menuVmSettings.Size = new System.Drawing.Size(226, 22);
            this.menuVmSettings.Text = "Settings";
            // 
            // menuTools
            // 
            this.menuTools.Name = "menuTools";
            this.menuTools.Size = new System.Drawing.Size(48, 20);
            this.menuTools.Text = "Tools";
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelpViewHelp,
            this.menuHelpKeyboardHelp,
            this.menuHelpSeparator,
            this.menuHelpAbout});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(44, 20);
            this.menuHelp.Text = "Help";
            // 
            // menuHelpViewHelp
            // 
            this.menuHelpViewHelp.Name = "menuHelpViewHelp";
            this.menuHelpViewHelp.Size = new System.Drawing.Size(152, 22);
            this.menuHelpViewHelp.Text = "View Help";
            // 
            // menuHelpKeyboardHelp
            // 
            this.menuHelpKeyboardHelp.Name = "menuHelpKeyboardHelp";
            this.menuHelpKeyboardHelp.Size = new System.Drawing.Size(152, 22);
            this.menuHelpKeyboardHelp.Text = "Keyboard Help";
            // 
            // menuHelpSeparator
            // 
            this.menuHelpSeparator.Name = "menuHelpSeparator";
            this.menuHelpSeparator.Size = new System.Drawing.Size(149, 6);
            // 
            // menuHelpAbout
            // 
            this.menuHelpAbout.Name = "menuHelpAbout";
            this.menuHelpAbout.Size = new System.Drawing.Size(152, 22);
            this.menuHelpAbout.Text = "About";
            // 
            // tbrStrip
            // 
            this.tbrStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tbrStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.tbrStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbrButtonOpen,
            this.tbrButtonSave,
            this.toolStripSeparator2,
            this.tbrButtonPause,
            this.tbrButtonMaxSpeed,
            this.tbrButtonWarmReset,
            this.tbrButtonColdReset,
            this.toolStripSeparator1,
            this.tbrButtonFullScreen,
            this.tbrButtonQuickLoad,
            this.tbrButtonSettings});
            this.tbrStrip.Location = new System.Drawing.Point(0, 24);
            this.tbrStrip.Name = "tbrStrip";
            this.tbrStrip.Padding = new System.Windows.Forms.Padding(0);
            this.tbrStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tbrStrip.Size = new System.Drawing.Size(704, 39);
            this.tbrStrip.TabIndex = 2;
            this.tbrStrip.MouseMove += new System.Windows.Forms.MouseEventHandler(this.renderVideo_MouseMove);
            // 
            // tbrButtonOpen
            // 
            this.tbrButtonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrButtonOpen.Image = global::ZXMAK2.Host.WinForms.Properties.Resources.EmuFileOpen_32x32;
            this.tbrButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrButtonOpen.Name = "tbrButtonOpen";
            this.tbrButtonOpen.Size = new System.Drawing.Size(36, 36);
            // 
            // tbrButtonSave
            // 
            this.tbrButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrButtonSave.Image = global::ZXMAK2.Host.WinForms.Properties.Resources.EmuFileSave_32x32;
            this.tbrButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrButtonSave.Name = "tbrButtonSave";
            this.tbrButtonSave.Size = new System.Drawing.Size(36, 36);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // tbrButtonPause
            // 
            this.tbrButtonPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrButtonPause.Image = global::ZXMAK2.Host.WinForms.Properties.Resources.EmuResume_32x32;
            this.tbrButtonPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrButtonPause.Name = "tbrButtonPause";
            this.tbrButtonPause.Size = new System.Drawing.Size(36, 36);
            // 
            // tbrButtonMaxSpeed
            // 
            this.tbrButtonMaxSpeed.CheckOnClick = true;
            this.tbrButtonMaxSpeed.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrButtonMaxSpeed.Image = global::ZXMAK2.Host.WinForms.Properties.Resources.EmuMaxSpeed_32x32;
            this.tbrButtonMaxSpeed.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrButtonMaxSpeed.Name = "tbrButtonMaxSpeed";
            this.tbrButtonMaxSpeed.Size = new System.Drawing.Size(36, 36);
            // 
            // tbrButtonWarmReset
            // 
            this.tbrButtonWarmReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrButtonWarmReset.Image = global::ZXMAK2.Host.WinForms.Properties.Resources.EmuWarmReset_32x32;
            this.tbrButtonWarmReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrButtonWarmReset.Name = "tbrButtonWarmReset";
            this.tbrButtonWarmReset.Size = new System.Drawing.Size(36, 36);
            // 
            // tbrButtonColdReset
            // 
            this.tbrButtonColdReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrButtonColdReset.Image = global::ZXMAK2.Host.WinForms.Properties.Resources.EmuColdReset_32x32;
            this.tbrButtonColdReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrButtonColdReset.Name = "tbrButtonColdReset";
            this.tbrButtonColdReset.Size = new System.Drawing.Size(36, 36);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // tbrButtonFullScreen
            // 
            this.tbrButtonFullScreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrButtonFullScreen.Image = global::ZXMAK2.Host.WinForms.Properties.Resources.EmuFullScreen_32x32;
            this.tbrButtonFullScreen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrButtonFullScreen.Name = "tbrButtonFullScreen";
            this.tbrButtonFullScreen.Size = new System.Drawing.Size(36, 36);
            // 
            // tbrButtonQuickLoad
            // 
            this.tbrButtonQuickLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrButtonQuickLoad.Image = global::ZXMAK2.Host.WinForms.Properties.Resources.EmuQuickLoad_32x32;
            this.tbrButtonQuickLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrButtonQuickLoad.Name = "tbrButtonQuickLoad";
            this.tbrButtonQuickLoad.Size = new System.Drawing.Size(36, 36);
            this.tbrButtonQuickLoad.Text = "Quick Boot";
            // 
            // tbrButtonSettings
            // 
            this.tbrButtonSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrButtonSettings.Image = global::ZXMAK2.Host.WinForms.Properties.Resources.EmuSettings_32x32;
            this.tbrButtonSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrButtonSettings.Name = "tbrButtonSettings";
            this.tbrButtonSettings.Size = new System.Drawing.Size(36, 36);
            // 
            // sbrStrip
            // 
            this.sbrStrip.Location = new System.Drawing.Point(0, 527);
            this.sbrStrip.Name = "sbrStrip";
            this.sbrStrip.Size = new System.Drawing.Size(704, 22);
            this.sbrStrip.TabIndex = 4;
            // 
            // renderVideo
            // 
            this.renderVideo.Location = new System.Drawing.Point(0, 63);
            this.renderVideo.Name = "renderVideo";
            this.renderVideo.Size = new System.Drawing.Size(526, 385);
            this.renderVideo.TabIndex = 3;
            this.renderVideo.Text = "renderVideo";
            this.renderVideo.DoubleClick += new System.EventHandler(this.renderVideo_DoubleClick);
            this.renderVideo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.renderVideo_MouseMove);
            this.renderVideo.Resize += new System.EventHandler(this.renderVideo_Resize);
            // 
            // MainView
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 549);
            this.Controls.Add(this.sbrStrip);
            this.Controls.Add(this.tbrStrip);
            this.Controls.Add(this.mnuStrip);
            this.Controls.Add(this.renderVideo);
            this.KeyPreview = true;
            this.MainMenuStrip = this.mnuStrip;
            this.Name = "MainView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ZXMAK2";
            this.mnuStrip.ResumeLayout(false);
            this.mnuStrip.PerformLayout();
            this.tbrStrip.ResumeLayout(false);
            this.tbrStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuView;
        private System.Windows.Forms.ToolStripMenuItem menuVm;
        private System.Windows.Forms.ToolStripMenuItem menuTools;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripButton tbrButtonOpen;
        private System.Windows.Forms.ToolStripButton tbrButtonSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tbrButtonPause;
        private System.Windows.Forms.ToolStripButton tbrButtonWarmReset;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tbrButtonFullScreen;
        private System.Windows.Forms.ToolStripButton tbrButtonSettings;
        private ZXMAK2.Host.WinForms.Controls.RenderVideo renderVideo;
        private System.Windows.Forms.ToolStripMenuItem menuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem menuFileSaveAs;
        private System.Windows.Forms.ToolStripSeparator menuFileSeparator;
        private System.Windows.Forms.ToolStripMenuItem menuFileExit;
        private System.Windows.Forms.ToolStripMenuItem menuViewSize;
        private System.Windows.Forms.ToolStripMenuItem menuViewSizeX1;
        private System.Windows.Forms.ToolStripMenuItem menuViewSizeX2;
        private System.Windows.Forms.ToolStripMenuItem menuViewSizeX3;
        private System.Windows.Forms.ToolStripMenuItem menuViewSizeX4;
        private System.Windows.Forms.ToolStripMenuItem menuViewFullScreen;
        private System.Windows.Forms.ToolStripSeparator menuViewSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuViewScaleMode;
        private System.Windows.Forms.ToolStripMenuItem menuViewScaleModeStretch;
        private System.Windows.Forms.ToolStripMenuItem menuViewScaleModeKeepProportion;
        private System.Windows.Forms.ToolStripMenuItem menuViewScaleModeFixedPixelSize;
        private System.Windows.Forms.ToolStripSeparator menuViewSeparator4;
        private System.Windows.Forms.ToolStripMenuItem menuViewSmoothing;
        private System.Windows.Forms.ToolStripMenuItem menuViewDisplayIcon;
        private System.Windows.Forms.ToolStripMenuItem menuViewDebugInfo;
        private System.Windows.Forms.ToolStripMenuItem menuVmPause;
        private System.Windows.Forms.ToolStripMenuItem menuVmMaximumSpeed;
        private System.Windows.Forms.ToolStripSeparator menuVmSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuVmWarmReset;
        private System.Windows.Forms.ToolStripMenuItem menuVmNmi;
        private System.Windows.Forms.ToolStripSeparator menuVmSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuVmSettings;
        private System.Windows.Forms.ToolStripMenuItem menuHelpViewHelp;
        private System.Windows.Forms.ToolStripMenuItem menuHelpKeyboardHelp;
        private System.Windows.Forms.ToolStripSeparator menuHelpSeparator;
        private System.Windows.Forms.ToolStripMenuItem menuHelpAbout;
        private System.Windows.Forms.ToolStripMenuItem menuViewCustomize;
        private System.Windows.Forms.ToolStripSeparator menuViewSeparator1;
        private System.Windows.Forms.ToolStripButton tbrButtonMaxSpeed;
        private System.Windows.Forms.ToolStripMenuItem menuViewCustomizeShowToolBar;
        private System.Windows.Forms.ToolStripMenuItem menuViewCustomizeShowStatusBar;
        private System.Windows.Forms.StatusStrip sbrStrip;
        private System.Windows.Forms.ToolStripButton tbrButtonColdReset;
        private System.Windows.Forms.ToolStripMenuItem menuVmColdReset;
        private ZXMAK2.Host.WinForms.Controls.MenuStripEx mnuStrip;
        private ZXMAK2.Host.WinForms.Controls.ToolStripEx tbrStrip;
        private System.Windows.Forms.ToolStripButton tbrButtonQuickLoad;
        private System.Windows.Forms.ToolStripMenuItem menuViewMimicTv;
        private System.Windows.Forms.ToolStripMenuItem menuViewScaleModeSquarePixelSize;
        private System.Windows.Forms.ToolStripSeparator menuViewSeparator3;
        private System.Windows.Forms.ToolStripMenuItem menuViewFrameSync;
        private System.Windows.Forms.ToolStripMenuItem menuViewFrameSyncTime;
        private System.Windows.Forms.ToolStripMenuItem menuViewFrameSyncSound;
        private System.Windows.Forms.ToolStripMenuItem menuViewFrameSyncVideo;
        private System.Windows.Forms.ToolStripMenuItem menuViewVideoFilter;
        private System.Windows.Forms.ToolStripMenuItem menuViewVideoFilterNone;
        private System.Windows.Forms.ToolStripMenuItem menuViewVideoFilterNoFlick;
    }
}