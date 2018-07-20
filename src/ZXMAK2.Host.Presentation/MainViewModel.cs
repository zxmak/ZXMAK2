using System;
using System.IO;
using System.ComponentModel;

using ZXMAK2.Dependency;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;
using ZXMAK2.Host.Presentation.Interfaces;
using ZXMAK2.Host.Presentation.Tools;
using ZXMAK2.Mvvm;
using System.Drawing;
using ZXMAK2.Mvvm.Attributes;



namespace ZXMAK2.Host.Presentation
{
    public class MainViewModel : BaseViewModel, IMainViewModel
    {
        private readonly IResolver m_resolver;
        private readonly ISettingService m_settingService;
        private readonly IUserMessage m_userMessage;
        private readonly IMainView m_view;
        private readonly string m_startupImage;
        private ISynchronizeInvoke m_synchronizeInvoke;
        private VirtualMachine m_vm;
        
        
        public MainViewModel(
            IResolver resolver,
            ISettingService settingService,
            IUserMessage userMessage,
            IMainView view, 
            params string[] args)
        {
            m_resolver = resolver;
            m_settingService = settingService;
            m_userMessage = userMessage;
            m_view = view;
            if (args.Length > 0 && File.Exists(args[0]))
            {
                m_startupImage = Path.GetFullPath(args[0]);
            }
            m_view.ViewOpened += MainView_OnViewOpened;
            m_view.ViewClosed += MainView_OnViewClosed;
            m_view.RequestFrame += MainView_OnRequestFrame;
            CreateCommands();

            _syncSource = m_settingService.SyncSource;
            _renderScaleMode = m_settingService.RenderScaleMode;
            _renderVideoFilter = m_settingService.RenderVideoFilter;
            _renderSize = new Size(m_settingService.WindowWidth, m_settingService.WindowHeight);
            CommandViewToolBar.Checked = m_settingService.IsToolBarVisible;
            CommandViewStatusBar.Checked = m_settingService.IsStatusBarVisible;
            CommandViewSmooth.Checked = m_settingService.RenderSmooth;
            CommandViewMimicTv.Checked = m_settingService.RenderMimicTv;
            CommandViewDisplayIcon.Checked = m_settingService.RenderDisplayIcon;
            CommandViewDebugInfo.Checked = m_settingService.RenderDebugInfo;
        }

        public void Dispose()
        {
            if (m_vm != null)
            {
                m_vm.Dispose();
                m_vm = null;
            }
        }

        public void Run()
        {
            m_view.Run();
        }

        public void Attach(ISynchronizeInvoke synchronizeInvoke)
        {
            m_synchronizeInvoke = synchronizeInvoke;
        }
        
        #region Commands

        public ICommand CommandFileOpen { get; private set; }
        public ICommand CommandFileSave { get; private set; }
        public ICommand CommandFileExit { get; private set; }
        public ICommand CommandViewFullScreen { get; private set; }
        public ICommand CommandViewSyncSource { get; private set; }
        public ICommand CommandViewScaleMode { get; private set; }
        public ICommand CommandViewVideoFilter { get; private set; }
        public ICommand CommandViewSmooth { get; private set; }
        public ICommand CommandViewMimicTv { get; private set; }
        public ICommand CommandViewDisplayIcon { get; private set; }
        public ICommand CommandViewDebugInfo { get; private set; }
        public ICommand CommandViewToolBar { get; private set; }
        public ICommand CommandViewStatusBar { get; private set; }
        public ICommand CommandViewScaleRatio { get; private set; }
        public ICommand CommandVmPause { get; private set; }
        public ICommand CommandVmMaxSpeed { get; private set; }
        public ICommand CommandVmWarmReset { get; private set; }
        public ICommand CommandVmColdReset { get; private set; }
        public ICommand CommandVmNmi { get; private set; }
        public ICommand CommandVmSettings { get; private set; }
        public ICommand CommandHelpViewHelp { get; private set; }
        public ICommand CommandHelpKeyboardHelp { get; private set; }
        public ICommand CommandHelpAbout { get; private set; }
        public ICommand CommandTapePause { get; private set; }
        public ICommand CommandQuickLoad { get; private set; }
        public ICommand CommandOpenUri { get; private set; }

        #endregion Commands

        
        #region Properties

        private string _title;

        public string Title
        {
            get { return _title; }
            set { PropertyChangeRef("Title", ref _title, value); }
        }

        private bool _isRunning;

        public bool IsRunning
        {
            get { return _isRunning; }
            set 
            {
                if (!PropertyChangeVal("IsRunning", ref _isRunning, value))
                {
                    return;
                }
                CommandVmPause.Text = IsRunning ? "Pause" : "Resume";
            } 
        }

        private bool _isFullScreen;

        public bool IsFullScreen
        {
            get { return _isFullScreen; }
            set 
            {
                if (!PropertyChangeVal("IsFullScreen", ref _isFullScreen, value))
                {
                    return;
                }
                CommandViewFullScreen.Update();
                CommandViewFullScreen.Text = IsFullScreen ? "Windowed" : "Full Screen";
            }
        }

        private SyncSource _syncSource;

        public SyncSource SyncSource
        {
            get { return _syncSource; }
            set 
            {
                if (!PropertyChangeVal("SyncSource", ref _syncSource, value))
                {
                    return;
                }
                CommandViewSyncSource.Update();
                if (m_view.Host != null)
                {
                    m_view.Host.SyncSource = CommandVmMaxSpeed.Checked ? SyncSource.None : SyncSource;
                }
                m_settingService.SyncSource = value;
            }
        }

        private bool _isMaxSpeed;

        public bool IsMaxSpeed
        {
            get { return _isMaxSpeed; }
            set 
            {
                if (!PropertyChangeVal("IsMaxSpeed", ref _isMaxSpeed, value))
                {
                    return;
                }
                CommandVmMaxSpeed.Checked = value;
                CommandVmMaxSpeed.Update();
                if (m_view.Host != null)
                {
                    m_view.Host.SyncSource = CommandVmMaxSpeed.Checked ? SyncSource.None : SyncSource;
                }
            }
        }

        private ScaleMode _renderScaleMode;

        public ScaleMode RenderScaleMode
        {
            get { return _renderScaleMode; }
            set 
            {
                if (!PropertyChangeVal("RenderScaleMode", ref _renderScaleMode, value))
                {
                    return;
                }
                CommandViewScaleMode.Update();
                m_settingService.RenderScaleMode = value;
            }
        }

        private VideoFilter _renderVideoFilter;

        public VideoFilter RenderVideoFilter
        {
            get { return _renderVideoFilter; }
            set
            {
                if (!PropertyChangeVal("RenderVideoFilter", ref _renderVideoFilter, value))
                {
                    return;
                }
                CommandViewVideoFilter.Update();
                m_settingService.RenderVideoFilter = value;
            }
        }

        private Size _renderSize;

        public Size RenderSize
        {
            get { return _renderSize; }
            set 
            {
                if (!PropertyChangeVal("RenderSize", ref _renderSize, value))
                {
                    return;
                }
                m_settingService.WindowWidth = value.Width;
                m_settingService.WindowHeight = value.Height;
            }
        }

        private Size _frameSize;

        public Size FrameSize
        {
            get { return _frameSize; }
            set { PropertyChangeVal("FrameSize", ref _frameSize, value); }
        }

        [DependsOnProperty("RenderSize")]
        [DependsOnProperty("FrameSize")]
        public int? RenderScaleRatio
        {
            get 
            {
                if (RenderSize.Width == 0 ||
                    RenderSize.Height == 0 ||
                    FrameSize.Width == 0 ||
                    FrameSize.Height == 0 ||
                    RenderSize.Width % FrameSize.Width != 0 ||
                    RenderSize.Height % FrameSize.Height != 0 ||
                    RenderSize.Width % FrameSize.Width != RenderSize.Height % FrameSize.Height)
                {
                    return null;
                }
                return RenderSize.Width / FrameSize.Width;
            }
            set
            {
                if (!value.HasValue || RenderScaleRatio == value)
                {
                    return;
                }
                RenderSize = new Size(
                    FrameSize.Width * value.Value,
                    FrameSize.Height * value.Value);
            }
        }

        #endregion Properties


        #region MainView Event Handlers

        private void MainView_OnViewOpened(object sender, EventArgs e)
        {
            if (m_vm != null)
            {
                Logger.Warn("IMainView.ViewOpened event raised twice!");
                return;
            }
            m_vm = new VirtualMachine(m_view.Host, m_view.CommandManager);
            m_vm.Init();
            m_vm.UpdateState += VirtualMachine_OnUpdateState;
            m_vm.FrameSizeChanged += VirtualMachine_OnFrameSizeChanged;
            var host = m_view != null ? m_view.Host : null;
            if (host != null)
            {
                host.SyncSource = SyncSource;
            }
            Title = string.Empty;
            var fileName = Path.Combine(
                Utils.GetAppDataFolder(),
                "ZXMAK2.vmz");
            if (File.Exists(fileName))
            {
                m_vm.OpenConfig(fileName);
            }
            else
            {
                m_vm.SaveConfigAs(fileName);
            }
            if (m_startupImage != null)
            {
                Title = m_vm.Spectrum.BusManager.LoadManager.OpenFileName(m_startupImage, true);
            }
            m_vm.DoRun();
            UpdateAllCommands();
        }

        private void MainView_OnViewClosed(object sender, EventArgs e)
        {
            if (m_vm == null)
            {
                Logger.Warn("IMainView.ViewClosed: object is not initialized!");
            }
            Dispose();
        }

        private void MainView_OnRequestFrame(object sender, EventArgs e)
        {
            if (m_view == null || m_view.Host == null)
            {
                return;
            }
            m_vm.RequestFrame();
        }

        #endregion MainView Event Handlers


        #region Command Implementation

        private void CreateCommands()
        {
            CommandFileOpen = new CommandDelegate(CommandFileOpen_OnExecute, CommandFileOpen_OnCanExecute);
            CommandFileSave = new CommandDelegate(CommandFileSave_OnExecute, CommandFileSave_OnCanExecute);
            CommandFileExit = new CommandDelegate(CommandFileExit_OnExecute);
            CommandViewFullScreen = new CommandDelegate(CommandViewFullScreen_OnExecute);
            CommandViewSyncSource = new CommandDelegate(CommandViewSyncSource_OnExecute, CommandViewSyncSource_OnCanExecute);
            CommandViewScaleMode = new CommandDelegate(CommandViewScaleMode_OnExecute, CommandViewScaleMode_OnCanExecute);
            CommandViewVideoFilter = new CommandDelegate(CommandViewVideoFilter_OnExecute, CommandViewVideoFilter_OnCanExecute);
            CommandViewSmooth = new CommandDelegate(CommandViewSmooth_OnExecute, CommandViewSmooth_OnCanExecute);
            CommandViewMimicTv = new CommandDelegate(CommandViewMimicTv_OnExecute, CommandViewMimicTv_OnCanExecute);
            CommandViewDisplayIcon = new CommandDelegate(CommandViewDisplayIcon_OnExecute, CommandViewDisplayIcon_OnCanExecute);
            CommandViewDebugInfo = new CommandDelegate(CommandViewDebugInfo_OnExecute, CommandViewDebugInfo_OnCanExecute);
            CommandViewToolBar = new CommandDelegate(CommandViewToolBar_OnExecute, CommandViewToolBar_OnCanExecute);
            CommandViewStatusBar = new CommandDelegate(CommandViewStatusBar_OnExecute, CommandViewStatusBar_OnCanExecute);
            CommandViewScaleRatio = new CommandDelegate(CommandViewScaleRatio_OnExecute, CommandViewScaleRatio_OnCanExecute);
            CommandVmPause = new CommandDelegate(CommandVmPause_OnExecute);
            CommandVmMaxSpeed = new CommandDelegate(CommandVmMaxSpeed_OnExecute, CommandVmMaxSpeed_OnCanExecute);
            CommandVmWarmReset = new CommandDelegate(CommandVmWarmReset_OnExecute);
            //CommandVmColdReset = new CommandDelegate(() => { });
            CommandVmNmi = new CommandDelegate(CommandVmNmi_OnExecute);
            CommandVmSettings = new CommandDelegate(CommandVmSettings_OnExecute, CommandVmSettings_OnCanExecute);
            CommandHelpViewHelp = new CommandDelegate(CommandHelpViewHelp_OnExecute, CommandHelpViewHelp_OnCanExecute);
            CommandHelpKeyboardHelp = CreateViewHolderCommand<IKeyboardView>();
            CommandHelpAbout = CreateViewHolderCommand<IAboutView>();
            CommandTapePause = new CommandDelegate(CommandTapePause_OnExecute, CommandTapePause_CanExecute);
            CommandQuickLoad = new CommandDelegate(CommandQuickLoad_OnExecute, CommandQuickLoad_OnCanExecute);
            CommandOpenUri = new CommandDelegate(CommandOpenUri_OnExecute, CommandOpenUri_OnCanExecute);

            CommandFileOpen.Text = "Open...";
            CommandFileSave.Text = "Save As...";
            CommandFileExit.Text = "Exit";
            CommandViewFullScreen.Text = "Full Screen";
            CommandViewSmooth.Text = "Antialias";
            CommandViewMimicTv.Text = "Mimic TV";
            CommandViewDisplayIcon.Text = "Show Icons";
            CommandViewDebugInfo.Text = "Debug Info";
            CommandViewToolBar.Text = "Tool Bar";
            CommandViewStatusBar.Text = "Status Bar";
            CommandVmPause.Text = "Resume";
            CommandVmMaxSpeed.Text = "Maximum Speed";
            CommandVmWarmReset.Text = "Warm Reset    Alt+Ctrl+Ins";
            //CommandVmColdReset.Text = "Cold Reset";
            CommandVmNmi.Text = "NMI";
            CommandVmSettings.Text = "Settings";
            CommandHelpViewHelp.Text = "View Help";
            CommandHelpKeyboardHelp.Text = "Keyboard Help";
            CommandHelpAbout.Text = "About...";
            CommandTapePause.Text = "Pause Tape";
            CommandQuickLoad.Text = "Quick Boot";
            CommandOpenUri.Text = "Open Url";
        }

        private void UpdateAllCommands()
        {
            CommandFileOpen.Update();
            CommandFileSave.Update();
            CommandFileExit.Update();
            CommandViewFullScreen.Update();
            CommandViewSyncSource.Update();
            CommandViewScaleMode.Update();
            CommandViewVideoFilter.Update();
            CommandViewSmooth.Update();
            CommandViewMimicTv.Update();
            CommandViewDisplayIcon.Update();
            CommandViewDebugInfo.Update();
            CommandViewToolBar.Update();
            CommandViewStatusBar.Update();
            CommandViewScaleRatio.Update();
            CommandVmPause.Update();
            CommandVmMaxSpeed.Update();
            CommandVmWarmReset.Update();
            //CommandVmColdReset.Update();
            CommandVmNmi.Update();
            CommandVmSettings.Update();
            CommandHelpViewHelp.Update();
            CommandHelpKeyboardHelp.Update();
            CommandHelpAbout.Update();
            CommandTapePause.Update();
            CommandQuickLoad.Update();
            CommandOpenUri.Update();
        }

        private ICommand CreateViewHolderCommand<T>()
            where T : IView
        {
            var viewHolder = new ViewHolder<T>(null);
            return viewHolder.CommandOpen;
        }

        private bool CommandFileOpen_OnCanExecute()
        {
            return CheckViewAvailable<IOpenFileDialog>() &&
                m_vm != null;
        }

        private void CommandFileOpen_OnExecute()
        {
            if (!CommandFileOpen_OnCanExecute())
            {
                return;
            }
            var loadDialog = GetView<IOpenFileDialog>();
            if (loadDialog == null)
            {
                return;
            }
            using (loadDialog)
            {
                loadDialog.Title = "Open...";
                loadDialog.Filter = m_vm.Spectrum.BusManager.LoadManager.GetOpenExtFilter();
                loadDialog.FileName = "";
                loadDialog.ShowReadOnly = true;
                loadDialog.ReadOnlyChecked = true;
                loadDialog.CheckFileExists = true;
                loadDialog.FileOk += LoadDialog_FileOk;
                if (loadDialog.ShowDialog(m_view) != DlgResult.OK)
                {
                    return;
                }
                OpenFile(loadDialog.FileName, loadDialog.ReadOnlyChecked);
            }
        }

        private bool CommandFileSave_OnCanExecute()
        {
            return CheckViewAvailable<ISaveFileDialog>() &&
                m_vm != null;
        }

        private void CommandFileSave_OnExecute()
        {
            if (!CommandFileSave_OnCanExecute())
            {
                return;
            }
            var saveDialog = GetView<ISaveFileDialog>();
            if (saveDialog == null)
            {
                return;
            }
            using (saveDialog)
            {
                saveDialog.Title = "Save...";
                saveDialog.Filter = m_vm.Spectrum.BusManager.LoadManager.GetSaveExtFilter();
                saveDialog.DefaultExt = m_vm.Spectrum.BusManager.LoadManager.GetDefaultExtension();
                saveDialog.FileName = string.Empty;
                saveDialog.OverwritePrompt = true;
                if (saveDialog.ShowDialog(m_view) != DlgResult.OK)
                {
                    return;
                }
                SaveFile(saveDialog.FileName);
            }
        }

        private void CommandFileExit_OnExecute()
        {
            m_view.Close();
        }

        private bool CheckViewAvailable<T>()
        {
            var viewResolver = m_resolver.Resolve<IResolver>("View");
            return viewResolver.CheckAvailable<T>();
        }
        
        private T GetView<T>()
        {
            var viewResolver = m_resolver.Resolve<IResolver>("View");
            return viewResolver.TryResolve<T>();
        }

        private void CommandViewFullScreen_OnExecute(object objState)
        {
            var state = objState as bool?;
            var value = IsFullScreen;
            value = state.HasValue ? state.Value : !value;
            IsFullScreen = value;
        }

        private bool CommandViewSyncSource_OnCanExecute(object objState)
        {
            return objState is SyncSource && 
                m_view != null &&
                m_view.Host != null &&
                m_view.Host.CheckSyncSourceSupported((SyncSource)objState);
        }

        private void CommandViewSyncSource_OnExecute(object objState)
        {
            if (!CommandViewSyncSource_OnCanExecute(objState))
            {
                return;
            }
            SyncSource = (SyncSource)objState;
        }

        private bool CommandViewScaleMode_OnCanExecute(object objState)
        {
            return objState is ScaleMode;
        }

        private void CommandViewScaleMode_OnExecute(object objState)
        {
            if (!CommandViewScaleMode_OnCanExecute(objState))
            {
                return;
            }
            RenderScaleMode = (ScaleMode)objState;
        }

        private bool CommandViewVideoFilter_OnCanExecute(object objState)
        {
            return objState is VideoFilter;
        }

        private void CommandViewVideoFilter_OnExecute(object objState)
        {
            if (!CommandViewVideoFilter_OnCanExecute(objState))
            {
                return;
            }
            RenderVideoFilter = (VideoFilter)objState;
        }

        private bool CommandViewSmooth_OnCanExecute(object objState)
        {
            return objState == null || objState is bool;
        }

        private void CommandViewSmooth_OnExecute(object objState)
        {
            if (!CommandViewSmooth_OnCanExecute(objState))
            {
                return;
            }
            CommandViewSmooth.Checked = objState is bool ? (bool)objState : 
                !CommandViewSmooth.Checked;
            m_settingService.RenderSmooth = CommandViewSmooth.Checked;
        }

        private bool CommandViewMimicTv_OnCanExecute(object objState)
        {
            return objState == null || objState is bool;
        }

        private void CommandViewMimicTv_OnExecute(object objState)
        {
            if (!CommandViewMimicTv_OnCanExecute(objState))
            {
                return;
            }
            CommandViewMimicTv.Checked = objState is bool ? (bool)objState :
                !CommandViewMimicTv.Checked;
            m_settingService.RenderMimicTv = CommandViewMimicTv.Checked;
        }

        private bool CommandViewDisplayIcon_OnCanExecute(object objState)
        {
            return objState == null || objState is bool;
        }

        private void CommandViewDisplayIcon_OnExecute(object objState)
        {
            if (!CommandViewDisplayIcon_OnCanExecute(objState))
            {
                return;
            }
            CommandViewDisplayIcon.Checked = objState is bool ? (bool)objState :
                !CommandViewDisplayIcon.Checked;
            m_settingService.RenderDisplayIcon = CommandViewDisplayIcon.Checked;
        }

        private bool CommandViewDebugInfo_OnCanExecute(object objState)
        {
            return objState == null || objState is bool;
        }

        private void CommandViewDebugInfo_OnExecute(object objState)
        {
            if (!CommandViewDebugInfo_OnCanExecute(objState))
            {
                return;
            }
            CommandViewDebugInfo.Checked = objState is bool ? (bool)objState :
                !CommandViewDebugInfo.Checked;
            m_settingService.RenderDebugInfo = CommandViewDebugInfo.Checked;
        }

        private bool CommandViewToolBar_OnCanExecute(object objState)
        {
            return objState == null || objState is bool;
        }

        private void CommandViewToolBar_OnExecute(object objState)
        {
            if (!CommandViewToolBar_OnCanExecute(objState))
            {
                return;
            }
            CommandViewToolBar.Checked = objState is bool ? (bool)objState :
                !CommandViewToolBar.Checked;
            m_settingService.IsToolBarVisible = CommandViewToolBar.Checked;
        }

        private bool CommandViewStatusBar_OnCanExecute(object objState)
        {
            return objState == null || objState is bool;
        }

        private void CommandViewStatusBar_OnExecute(object objState)
        {
            if (!CommandViewStatusBar_OnCanExecute(objState))
            {
                return;
            }
            CommandViewStatusBar.Checked = objState is bool ? (bool)objState :
                !CommandViewStatusBar.Checked;
            m_settingService.IsStatusBarVisible = CommandViewStatusBar.Checked;
        }

        private bool CommandViewScaleRatio_OnCanExecute(object objState)
        {
            return objState is int;
        }

        private void CommandViewScaleRatio_OnExecute(object objState)
        {
            if (!CommandViewScaleRatio_OnCanExecute(objState))
            {
                return;
            }
            RenderScaleRatio = (int)objState;
            IsFullScreen = false;
        }

        private void CommandVmPause_OnExecute()
        {
            if (m_vm == null)
            {
                return;
            }
            if (m_vm.IsRunning)
            {
                m_vm.DoStop();
            }
            else
            {
                m_vm.DoRun();
            }
        }

        private bool CommandVmMaxSpeed_OnCanExecute(object objState)
        {
            return (objState is bool? || objState==null) &&
                m_view != null &&
                m_view.Host != null;
        }

        private void CommandVmMaxSpeed_OnExecute(object objState)
        {
            if (!CommandVmMaxSpeed_OnCanExecute(objState))
            {
                return;
            }
            var state = (bool?)objState;
            var value = CommandVmMaxSpeed.Checked;
            value = state.HasValue ? state.Value : !value;
            IsMaxSpeed = value;
        }

        private void CommandVmWarmReset_OnExecute(object objState)
        {
            if (m_vm == null)
            {
                return;
            }
            var state = objState as bool?;
            if (state.HasValue)
            {
                if (m_vm.IsRunning)
                {
                    // state-change command
                    if (state.Value != m_vm.CPU.RST)
                    {
                        m_vm.CPU.RST = (bool)objState;
                    }
                    CommandVmWarmReset.Checked = state.Value;
                }
                else if (!state.Value && state.Value != m_vm.CPU.RST)
                {
                    // if stopped then trigger reset on back front only once
                    // because false may be set on wnd.deactivate for breakpoint
                    m_vm.DoReset();
                    CommandVmWarmReset.Checked = false;
                }
            }
            else
            {
                // event command
                m_vm.DoReset();
                CommandVmWarmReset.Checked = false;
            }
        }

        private void CommandVmNmi_OnExecute()
        {
            if (m_vm == null)
            {
                return;
            }
            m_vm.DoNmi();
        }

        private bool CommandVmSettings_OnCanExecute(Object objArg)
        {
            return CheckViewAvailable<IMachineSettingsView>() &&
                m_vm != null;
        }

        private void CommandVmSettings_OnExecute(Object objArg)
        {
            try
            {
                if (m_vm == null)
                {
                    return;
                }
                var viewSettings = GetView<IMachineSettingsView>();
                if (viewSettings == null)
                {
                    return;
                }
                using (viewSettings)
                {
                    viewSettings.Init(m_view.Host, m_vm);
                    viewSettings.ShowDialog(m_view);
                    m_vm.RequestFrame();
                    
                    CommandTapePause.Update();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                m_userMessage.Error(ex);
            }
        }

        private bool CommandHelpViewHelp_OnCanExecute(object arg)
        {
            if (!CheckViewAvailable<IUserHelp>())
            {
                return false;
            }
            var service = GetView<IUserHelp>();
            return service != null && service.CanShow(arg);
        }

        private void CommandHelpViewHelp_OnExecute(object arg)
        {
            if (!CommandHelpViewHelp_OnCanExecute(arg))
            {
                return;
            }
            var service = GetView<IUserHelp>();
            service.ShowHelp(arg);
        }

        private bool CommandTapePause_CanExecute()
        {
            if (m_vm == null)
            {
                return false;
            }
            var tape = m_vm.Spectrum.BusManager.FindDevice<ITapeDevice>();
            return tape != null;
        }

        private void CommandTapePause_OnExecute()
        {
            if (m_vm == null)
            {
                return;
            }
            var tape = m_vm.Spectrum.BusManager.FindDevice<ITapeDevice>();
            if (tape == null)
            {
                return;
            }
            if (tape.IsPlay)
            {
                tape.Stop();
            }
            else
            {
                tape.Play();
            }
        }

        private bool CommandQuickLoad_OnCanExecute()
        {
            var fileName = Path.Combine(Utils.GetAppFolder(), "boot.zip");
            return m_vm != null &&
                File.Exists(fileName);
        }

        private void CommandQuickLoad_OnExecute()
        {
            if (!CommandQuickLoad_OnCanExecute())
            {
                return;
            }
            var fileName = Path.Combine(Utils.GetAppFolder(), "boot.zip");
            if (!File.Exists(fileName))
            {
                m_userMessage.Error("Quick snapshot boot.zip is missing!");
                return;
            }
            var running = m_vm.IsRunning;
            m_vm.DoStop();
            try
            {
                if (m_vm.Spectrum.BusManager.LoadManager.CheckCanOpenFileName(fileName))
                {
                    m_vm.Spectrum.BusManager.LoadManager.OpenFileName(fileName, true);
                }
                else
                {
                    m_userMessage.Error("Cannot open quick snapshot boot.zip!");
                }
            }
            finally
            {
                if (running)
                    m_vm.DoRun();
            }
        }

        private bool CommandOpenUri_OnCanExecute(object objUri)
        {
            try
            {
                if (m_vm == null)
                {
                    return false;
                }
                var uri = objUri as Uri;
                return uri != null && 
                    (!uri.IsLoopback ||
                    m_vm.Spectrum.BusManager.LoadManager.CheckCanOpenFileName(uri.LocalPath));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        private void CommandOpenUri_OnExecute(object objUri)
        {
            if (!CommandOpenUri_OnCanExecute(objUri))
            {
                return;
            }
            try
            {
                if (m_vm == null)
                {
                    return;
                }
                var uri = (Uri)objUri;
                if (uri.IsLoopback)
                {
                    OpenFile(uri.LocalPath, true);
                }
                else
                {
                    var downloader = new WebDownloader();
                    var webFile = downloader.Download(uri);
                    using (var ms = new MemoryStream(webFile.Content))
                    {
                        OpenStream(webFile.FileName, ms);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                m_userMessage.Error(ex);
            }
        }

        #endregion Command Implementation


        #region Private

        private void VirtualMachine_OnUpdateState(object sender, EventArgs e)
        {
            ExecuteSynchronizedAsync(() => IsRunning = m_vm != null && m_vm.IsRunning);
        }

        private void VirtualMachine_OnFrameSizeChanged(object sender, EventArgs e)
        {
            ExecuteSynchronizedAsync(() => FrameSize = m_vm != null ? m_vm.FrameSize : Size.Empty);
        }

        private void LoadDialog_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                var loadDialog = sender as IOpenFileDialog;
                if (loadDialog == null) return;
                e.Cancel = !m_vm.Spectrum.BusManager.LoadManager.CheckCanOpenFileName(loadDialog.FileName);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                e.Cancel = true;
                m_userMessage.Error(ex);
            }
        }

        private void OpenFile(string fileName, bool readOnly)
        {
            var running = m_vm.IsRunning;
            m_vm.DoStop();
            try
            {
                if (m_vm.Spectrum.BusManager.LoadManager.CheckCanOpenFileName(fileName))
                {
                    string imageName = m_vm.Spectrum.BusManager.LoadManager.OpenFileName(fileName, readOnly);
                    if (imageName != string.Empty)
                    {
                        Title = imageName;
                        m_vm.SaveConfig();
                    }
                }
                else
                {
                    m_userMessage.Error("Unrecognized file!");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                m_userMessage.Error(ex);
            }
            finally
            {
                if (running)
                    m_vm.DoRun();
            }
        }

        private void SaveFile(string fileName)
        {
            var running = m_vm.IsRunning;
            m_vm.DoStop();
            try
            {
                if (m_vm.Spectrum.BusManager.LoadManager.CheckCanSaveFileName(fileName))
                {
                    Title = m_vm.Spectrum.BusManager.LoadManager.SaveFileName(fileName);
                    m_vm.SaveConfig();
                }
                else
                {
                    m_userMessage.Error("Unrecognized file!");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                m_userMessage.Error(ex);
            }
            finally
            {
                if (running)
                    m_vm.DoRun();
            }
        }

        private void OpenStream(string fileName, Stream fileStream)
        {
            var running = m_vm.IsRunning;
            m_vm.DoStop();
            try
            {
                if (m_vm.Spectrum.BusManager.LoadManager.CheckCanOpenFileStream(fileName, fileStream))
                {
                    string imageName = m_vm.Spectrum.BusManager.LoadManager.OpenFileStream(fileName, fileStream);
                    if (imageName != string.Empty)
                    {
                        Title = imageName;
                        m_vm.SaveConfig();
                    }
                }
                else
                {
                    m_userMessage.Error(
                        "Unrecognized file!\n\n{0}", 
                        fileName);
                }
            }
            finally
            {
                if (running)
                    m_vm.DoRun();
            }
        }

        private void ExecuteSynchronizedAsync(Action action)
        {
            var synchronizer = m_synchronizeInvoke;
            if (synchronizer != null && synchronizer.InvokeRequired)
            {
                synchronizer.BeginInvoke(action, null);
                return;
            }
            action();
        }

        #endregion Private
    }
}
