using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Host.Presentation.Interfaces;
using ZXMAK2.Host.WinForms.Mdx;
using ZXMAK2.Host.WinForms.Controls;
using ZXMAK2.Host.WinForms.Tools;
using ZXMAK2.Host.WinForms.Services;
using ZXMAK2.Host.Entities;
using ZXMAK2.Host.Services;
using ZXMAK2.Resources;
using ZXMAK2.Mvvm;
using ZXMAK2.Mvvm.BindingTools;
using ZXMAK2.Host.WinForms.BindingTools;


namespace ZXMAK2.Host.WinForms.Views
{
    public partial class MainView : Form, IMainView, ICommandManager, INotifyPropertyChanged
    {
        #region Fields

        private readonly IResolver _resolver;
        private readonly BindingService _binding; 

        private IHostService _host;

        //private Size _size;
        private Point _location;
        private FormBorderStyle _style;
        private bool _isToolBarPopupActive;
        private bool _isFullScreenChanging;
        private bool _isFormShown;

        #endregion Fields


        static MainView()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += (s, e) => Logger.Error(e.Exception, "Application.ThreadException");
        }

        public MainView(IResolver resolver)
        {
            _resolver = resolver;
            _binding = new BindingService();
            _binding.RegisterAdapterFactory<Control>(
                arg => new ControlBindingAdapter(arg));
            _binding.RegisterAdapterFactory<ToolStripItem>(
                arg => new ToolStripItemBindingAdapter(arg));

            SetStyle(ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
            Icon = ResourceImages.IconApp;

            Bind();
        }


        private void Bind()
        {
            _binding.Bind(this, "IsToolBarEnabled", "IsToolBarEnabled");
            _binding.Bind(this, "IsStatusBarEnabled", "IsStatusBarEnabled");
            _binding.Bind(this, "RenderSize", "RenderSize");
            BindCommandLight(menuViewSizeX1, "CommandViewScaleRatio", 1);
            BindCommandLight(menuViewSizeX2, "CommandViewScaleRatio", 2);
            BindCommandLight(menuViewSizeX3, "CommandViewScaleRatio", 3);
            BindCommandLight(menuViewSizeX4, "CommandViewScaleRatio", 4);
            _binding.Bind(this, "RenderScaleRatio", "RenderScaleRatio");

            BindCommand(menuFileOpen, "CommandFileOpen", this);
            BindCommand(menuFileSaveAs, "CommandFileSave", this);
            BindCommand(menuFileExit, "CommandFileExit");
            BindCommand(menuViewFullScreen, "CommandViewFullScreen");
            BindCommand(menuVmPause, "CommandVmPause");
            BindCommand(menuVmMaximumSpeed, "CommandVmMaxSpeed");
            BindCommand(menuVmWarmReset, "CommandVmWarmReset");
            BindCommand(menuVmColdReset, "CommandVmColdReset");
            BindCommand(menuVmNmi, "CommandVmNmi");
            BindCommand(menuVmSettings, "CommandVmSettings", this);
            BindCommand(menuHelpViewHelp, "CommandHelpViewHelp", this);
            BindCommand(menuHelpKeyboardHelp, "CommandHelpKeyboardHelp", this);
            BindCommand(menuHelpAbout, "CommandHelpAbout", this);

            BindCommand(tbrButtonOpen, "CommandFileOpen", this);
            BindCommand(tbrButtonSave, "CommandFileSave", this);
            BindCommand(tbrButtonPause, "CommandVmPause");
            BindCommand(tbrButtonMaxSpeed, "CommandVmMaxSpeed");
            BindCommand(tbrButtonWarmReset, "CommandVmWarmReset");
            BindCommand(tbrButtonColdReset, "CommandVmColdReset");
            BindCommand(tbrButtonFullScreen, "CommandViewFullScreen");
            BindCommand(tbrButtonQuickLoad, "CommandQuickLoad");
            BindCommand(tbrButtonSettings, "CommandVmSettings", this);

            BindCommand(menuViewCustomizeShowToolBar, "CommandViewToolBar");
            BindCommand(menuViewCustomizeShowStatusBar, "CommandViewStatusBar");
            _binding.Bind(this, "IsToolBarEnabled", "CommandViewToolBar.Checked");
            _binding.Bind(this, "IsStatusBarEnabled", "CommandViewStatusBar.Checked");

            BindCommandLight(menuViewFrameSyncTime, "CommandViewSyncSource", SyncSource.Time);
            BindCommandLight(menuViewFrameSyncSound, "CommandViewSyncSource", SyncSource.Sound);
            BindCommandLight(menuViewFrameSyncVideo, "CommandViewSyncSource", SyncSource.Video);
            _binding.Bind(this, "SelectedSyncSource", "SyncSource");

            BindCommandLight(menuViewScaleModeStretch, "CommandViewScaleMode", ScaleMode.Stretch);
            BindCommandLight(menuViewScaleModeKeepProportion, "CommandViewScaleMode", ScaleMode.KeepProportion);
            BindCommandLight(menuViewScaleModeFixedPixelSize, "CommandViewScaleMode", ScaleMode.FixedPixelSize);
            BindCommandLight(menuViewScaleModeSquarePixelSize, "CommandViewScaleMode", ScaleMode.SquarePixelSize);
            _binding.Bind(this, "SelectedScaleMode", "RenderScaleMode");

            BindCommandLight(menuViewVideoFilterNone, "CommandViewVideoFilter", VideoFilter.None);
            BindCommandLight(menuViewVideoFilterNoFlick, "CommandViewVideoFilter", VideoFilter.NoFlick);
            _binding.Bind(this, "SelectedVideoFilter", "RenderVideoFilter");

            BindCommand(menuViewSmoothing, "CommandViewSmooth");
            BindCommand(menuViewMimicTv, "CommandViewMimicTv");
            BindCommand(menuViewDisplayIcon, "CommandViewDisplayIcon");
            BindCommand(menuViewDebugInfo, "CommandViewDebugInfo");

            _binding.Bind(renderVideo, "AntiAlias", "CommandViewSmooth.Checked");
            _binding.Bind(renderVideo, "MimicTv", "CommandViewMimicTv.Checked");
            _binding.Bind(renderVideo, "DisplayIcon", "CommandViewDisplayIcon.Checked");
            _binding.Bind(renderVideo, "DebugInfo", "CommandViewDebugInfo.Checked");

            _binding.Bind(this, "Title", "Title");
            _binding.Bind(this, "IsFullScreen", "IsFullScreen");
            _binding.Bind(this, "IsRunning", "IsRunning");
            _binding.Bind(renderVideo, "IsRunning", "IsRunning");

            var imagePause = global::ZXMAK2.Host.WinForms.Properties.Resources.EmuPause_32x32;
            var imageResume = global::ZXMAK2.Host.WinForms.Properties.Resources.EmuResume_32x32;
            var imageWindowed = global::ZXMAK2.Host.WinForms.Properties.Resources.EmuWindowed_32x32;
            var imageFullScreen = global::ZXMAK2.Host.WinForms.Properties.Resources.EmuFullScreen_32x32;
            _binding.Bind(
                tbrButtonPause,
                "Image",
                "IsRunning",
                arg => (bool)arg ? imagePause : imageResume);
            _binding.Bind(
                tbrButtonFullScreen,
                "Image",
                "IsFullScreen",
                arg => (bool)arg ? imageWindowed : imageFullScreen);

            _binding.Bind(this, "CommandViewFullScreen", "CommandViewFullScreen");
            _binding.Bind(this, "CommandVmPause", "CommandVmPause");
            _binding.Bind(this, "CommandVmMaxSpeed", "CommandVmMaxSpeed");
            _binding.Bind(this, "CommandVmWarmReset", "CommandVmWarmReset");
            _binding.Bind(this, "CommandTapePause", "CommandTapePause");
            _binding.Bind(this, "CommandQuickLoad", "CommandQuickLoad");
            _binding.Bind(this, "CommandOpenUri", "CommandOpenUri");
        }


        #region Commands

        public ICommand CommandViewFullScreen { get; set; }
        public ICommand CommandVmPause { get; set; }
        public ICommand CommandVmMaxSpeed { get; set; }
        public ICommand CommandVmWarmReset { get; set; }
        public ICommand CommandTapePause { get; set; }
        public ICommand CommandQuickLoad { get; set; }
        public ICommand CommandOpenUri { get; set; }

        #endregion Commands


        #region IMainView

        public object DataContext
        {
            get { return _binding.DataContext; }
            set { _binding.DataContext = value; }
        }

        private string _title;
        
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                UpdateTitle();
            }
        }

        private bool _isRunning;

        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                UpdateTitle();
            }
        }

        public IHostService Host
        {
            get { return _host; }
        }

        public ICommandManager CommandManager
        {
            get { return this; }
        }

        public event EventHandler ViewOpened;
        public event EventHandler ViewClosed;
        public event EventHandler RequestFrame;

        public void Run()
        {
            Application.Run(this);
        }

        #endregion IMainView


        #region IHostUi

        private List<ToolStripItemBindingAdapter> _deviceItemAdapters = new List<ToolStripItemBindingAdapter>();

        public void Clear()
        {
            _deviceItemAdapters
                .ForEach(arg => arg.Dispose());
            _deviceItemAdapters.Clear();
            menuTools.DropDownItems.Clear();
        }

        public void Add(ICommand command)
        {
            var subMenu = menuTools.DropDownItems.Add(command.Text) as ToolStripMenuItem;
            if (subMenu == null)
            {
                return;
            }
            var adapter = new ToolStripItemBindingAdapter(subMenu);
            adapter.CommandParameter = this;
            adapter.Command = command;
            _deviceItemAdapters.Add(adapter);
            SortMenuTools();
        }

        #endregion IHostUi


        #region IMainView Events

        private void OnViewOpened()
        {
            var handler = ViewOpened;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnViewClosed()
        {
            var handler = ViewClosed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnRequestFrame()
        {
            var handler = RequestFrame;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion IMainView Events


        #region Form Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                NativeMethods.TimeBeginPeriod(1);
                renderVideo.InitWnd();
                _host = CreateHost();
                OnViewOpened();
                //_host.MediaRecorder = new ZXMAK2.Host.Media.MediaRecorder("C:\\test.mp4", 320*2, 256*2, _host.SampleRate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private IHostService CreateHost()
        {
            var viewResolver = Locator.TryResolve<IResolver>("View");
            if (viewResolver == null)
            {
                return new HostService(renderVideo, null, null, null, null);
            }
            var arg = new Argument("form", this);
            var sound = viewResolver.TryResolve<IHostSound>(arg);
            var keyboard = viewResolver.TryResolve<IHostKeyboard>(arg);
            var mouse = viewResolver.TryResolve<IHostMouse>(arg);
            var joystick = viewResolver.TryResolve<IHostJoystick>(arg);
            return new HostService(renderVideo, sound, keyboard, mouse, joystick);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _isFormShown = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _isFormShown = false;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            try
            {
                OnViewClosed();
                if (_host != null)
                {
                    _host.Dispose();
                    _host = null;
                }
                renderVideo.FreeWnd();
                NativeMethods.TimeEndPeriod(1);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                _resolver.Resolve<IUserMessage>().ErrorDetails(ex);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Relayout(true);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (_host.IsCaptured)
            {
                e.SuppressKeyPress = true;
            }
            if (e.Alt && e.Control)
            {
                _host.Uncapture();
            }
            // FULLSCREEN
            if (e.Alt && e.KeyCode == Keys.Enter)
            {
                if (e.Alt)
                {
                    OnCommand(CommandViewFullScreen);
                }
                e.Handled = true;
                return;
            }
            //RESET
            if (e.Alt && e.Control && e.KeyCode == Keys.Insert)
            {
                OnCommand(CommandVmWarmReset, true);
                e.Handled = true;
                return;
            }
            // STOP/RUN
            if (e.KeyCode == Keys.Pause)
            {
                OnCommand(CommandVmPause);
                e.Handled = true;
                return;
            }
            // Max Speed
            if (e.Alt && e.KeyCode == Keys.Scroll)
            {
                OnCommand(CommandVmMaxSpeed);
                e.Handled = true;
                return;
            }
            if (e.Alt && e.Control && e.KeyCode == Keys.F1)
            {
                OnCommand(CommandQuickLoad);
                e.Handled = true;
                return;
            }
            if (e.Alt && e.Control && e.KeyCode == Keys.F8)
            {
                OnCommand(CommandTapePause);
                e.Handled = true;
                return;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            //RESET
            if (e.Alt && e.Control && e.KeyCode == Keys.Insert)
            {
                OnCommand(CommandVmWarmReset, false);
                e.Handled = true;
                return;
            }
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            if (!Created)
            {
                return;
            }
            OnCommand(CommandVmWarmReset, false);
        }


        #endregion Form Event Handlers


        #region Drag-n-Drop

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            try
            {
                if (!CanFocus)
                {
                    e.Effect = DragDropEffects.None;
                    return;
                }
                var ddw = new DragDataWrapper(e.Data);
                var allowOpen = false;
                if (ddw.IsFileDrop)
                {
                    var uri = new Uri(Path.GetFullPath(ddw.GetFilePath()));
                    allowOpen = CanExecute(CommandOpenUri, uri);
                }
                else if (ddw.IsLinkDrop)
                {
                    var uri = new Uri(ddw.GetLinkUri());
                    allowOpen = CanExecute(CommandOpenUri, uri);
                }
                e.Effect = allowOpen ? DragDropEffects.Link : DragDropEffects.None;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);
            try
            {
                if (!CanFocus)
                {
                    return;
                }
                var ddw = new DragDataWrapper(e.Data);
                if (ddw.IsFileDrop)
                {
                    var uri = new Uri(Path.GetFullPath(ddw.GetFilePath()));
                    this.Activate();
                    this.BeginInvoke(new Action(() => OnCommand(CommandOpenUri, uri)));

                    //string fileName = ddw.GetFilePath();
                    //if (fileName != string.Empty)
                    //{
                    //    this.Activate();
                    //    this.BeginInvoke(new OpenFileHandler(OpenFile), fileName, true);
                    //}
                }
                else if (ddw.IsLinkDrop)
                {
                    var uri = new Uri(ddw.GetLinkUri());
                    this.Activate();
                    this.BeginInvoke(new Action(() => OnCommand(CommandOpenUri, uri)));

                    //string linkUrl = ddw.GetLinkUri();
                    //if (linkUrl != string.Empty)
                    //{
                    //    Uri fileUri = new Uri(linkUrl);
                    //    this.Activate();
                    //    this.BeginInvoke(new OpenUriHandler(OpenUri), fileUri);
                    //}
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion Drag-n-Drop


        #region Layout

        private bool _isToolBarEnabled;

        public bool IsToolBarEnabled
        {
            get { return _isToolBarEnabled; }
            set
            {
                _isToolBarEnabled = value;
                Relayout(false);
            }
        }

        private bool _isStatusBarEnabled;

        public bool IsStatusBarEnabled
        {
            get { return _isStatusBarEnabled; }
            set
            {
                _isStatusBarEnabled = value;
                Relayout(false);
            }
        }

        private bool _renderSizeInitialized;

        private Size _renderSize;

        public Size RenderSize
        {
            get { return _renderSize; }
            set
            {
                if (_renderSize == value && _renderSizeInitialized)
                {
                    return;
                }
                _renderSizeInitialized = true;
                if (value.Width < 0)
                    value = new Size(0, value.Height);
                if (value.Height < 0)
                    value = new Size(value.Width, 0);
                _renderSize = value;
                Relayout(false);
            }
        }

        private int? _renderScaleRatio;

        public int? RenderScaleRatio
        {
            get { return _renderScaleRatio; }
            set
            {
                menuViewSizeX1.Checked = value.HasValue && value.Value == 1;
                menuViewSizeX2.Checked = value.HasValue && value.Value == 2;
                menuViewSizeX3.Checked = value.HasValue && value.Value == 3;
                menuViewSizeX4.Checked = value.HasValue && value.Value == 4;
                _renderScaleRatio = value;
            }
        }

        private void Relayout(bool srcUserResizing)
        {
            if (!_renderSizeInitialized || _isFullScreenChanging)
            {
                return;
            }
            if (IsFullScreen)
            {
                var menuEnabled = _isToolBarPopupActive;
                var toolEnabled = IsToolBarEnabled && _isToolBarPopupActive;
                var statEnabled = IsStatusBarEnabled;

                sbrStrip.SizingGrip = false;
                mnuStrip.Visible = menuEnabled;
                tbrStrip.Visible = toolEnabled;
                sbrStrip.Visible = statEnabled;

                var sbarHeight = statEnabled ? sbrStrip.Height : 0;
                var size = new Size(ClientSize.Width, ClientSize.Height - sbarHeight);
                var pos = new Point(0, 0);
                if (renderVideo.Location != pos)
                {
                    renderVideo.Location = pos;
                }
                if (renderVideo.Size != size)
                {
                    renderVideo.Size = size;
                }
            }
            else
            {
                var menuEnabled = true;
                var toolEnabled = IsToolBarEnabled;
                var statEnabled = IsStatusBarEnabled;

                sbrStrip.SizingGrip = true;
                mnuStrip.Visible = menuEnabled;
                tbrStrip.Visible = toolEnabled;
                sbrStrip.Visible = statEnabled;

                var menuHeight = menuEnabled ? mnuStrip.Height : 0;
                var tbarHeight = toolEnabled ? tbrStrip.Height : 0;
                var sbarHeight = statEnabled ? sbrStrip.Height : 0;
                var shift = menuHeight + tbarHeight + sbarHeight;
                var notifyNeeded = false;
                if (srcUserResizing)
                {
                    var size = new Size(ClientSize.Width, ClientSize.Height - shift);
                    if (_renderSize != size && _isFormShown)
                    {
                        _renderSize = size;
                        notifyNeeded = true;
                    }
                }
                else
                {
                    var size = new Size(RenderSize.Width, RenderSize.Height + shift);
                    if (ClientSize != size)
                    {
                        ClientSize = size;
                    }
                }
                var pos = new Point(0, menuHeight+tbarHeight);
                if (renderVideo.Location != pos)
                {
                    renderVideo.Location = pos;
                }
                if (renderVideo.Size != RenderSize)
                {
                    renderVideo.Size = RenderSize;
                }
                if (notifyNeeded)
                {
                    OnPropertyChanged("RenderSize");
                }
            }
        }

        private void ScanPopup()
        {
            if (!IsFullScreen)
            {
                _isToolBarPopupActive = false;
                return;
            }
            var pos = PointToClient(Cursor.Position);
            var menuHeight = mnuStrip.Height;
            var tbarHeight = IsToolBarEnabled ? tbrStrip.Height : 0;
            var sbarHeight = IsStatusBarEnabled ? sbrStrip.Height : 0;
            var toolHeight = menuHeight + tbarHeight;
            var isTopArea = pos.X >= 0 && pos.X < ClientSize.Width &&
                pos.Y >= 0 && pos.Y < toolHeight + 2;
            if (_isToolBarPopupActive == isTopArea)
            {
                return;
            }
            _isToolBarPopupActive = isTopArea;
            Relayout(true);
        }

        private bool _isFullScreen;

        public bool IsFullScreen
        {
            get { return _isFullScreen; }
            set
            {
                if (value == _isFullScreen)
                {
                    return;
                }
                _isFullScreen = value;
                if (_isFullScreen)
                {
                    _style = FormBorderStyle;
                    _location = Location;
                    //_size = ClientSize;

                    _isFullScreenChanging = true;
                    FormBorderStyle = FormBorderStyle.None;
                    var screen = Screen.FromControl(this);
                    Location = screen.Bounds.Location;
                    _isFullScreenChanging = false;
                    Size = screen.Bounds.Size;

                    //_host.StartInputCapture();

                    Focus();
                }
                else
                {
                    _isFullScreenChanging = true;
                    Location = _location;
                    FormBorderStyle = _style;
                    _isFullScreenChanging = false;
                    Relayout(false);
                    //ClientSize = _size;

                    _host.Uncapture();
                }
            }
        }

        private void renderVideo_MouseMove(object sender, MouseEventArgs e)
        {
            ScanPopup();
        }

        private void renderVideo_DoubleClick(object sender, EventArgs e)
        {
            if (!renderVideo.Focused)
            {
                return;
            }
            _host.Capture();
        }

        private void renderVideo_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Maximized)
            {
                return;
            }
            WindowState = FormWindowState.Normal;
            OnCommand(CommandViewFullScreen, true);
        }

        private void UpdateTitle()
        {
            var tail = IsRunning ? "ZXMAK2" : "ZXMAK2 [paused]";
            Text = string.IsNullOrEmpty(_title) ?
                tail :
                string.Format("[{0}] - {1}", Title, tail);
        }

        #endregion Layout


        #region View Settings

        private SyncSource _selectedSyncSource;
        
        public SyncSource SelectedSyncSource
        {
            get { return _selectedSyncSource; }
            set
            {
                menuViewFrameSyncTime.Checked = value == SyncSource.Time;
                menuViewFrameSyncSound.Checked = value == SyncSource.Sound;
                menuViewFrameSyncVideo.Checked = value == SyncSource.Video;
                _selectedSyncSource = value;
            }
        }

        private ScaleMode _selectedScaleMode;
        
        public ScaleMode SelectedScaleMode
        {
            get { return _selectedScaleMode; }
            set
            {
                menuViewScaleModeStretch.Checked = value == ScaleMode.Stretch;
                menuViewScaleModeKeepProportion.Checked = value == ScaleMode.KeepProportion;
                menuViewScaleModeFixedPixelSize.Checked = value == ScaleMode.FixedPixelSize;
                menuViewScaleModeSquarePixelSize.Checked = value == ScaleMode.SquarePixelSize;
                renderVideo.ScaleMode = value;
                _selectedScaleMode = value;
            }
        }

        private VideoFilter _selectedVideoFilter;

        public VideoFilter SelectedVideoFilter
        {
            get { return _selectedVideoFilter; }
            set
            {
                menuViewVideoFilterNoFlick.Checked = value == VideoFilter.NoFlick;
                menuViewVideoFilterNone.Checked = value == VideoFilter.None;
                renderVideo.VideoFilter = value;
                _selectedVideoFilter = value;
            }
        }

        #endregion View Settings


        #region Menu Handlers

        private void SortMenuTools()
        {
            var list = new List<ToolStripItem>();
            foreach (ToolStripItem item in menuTools.DropDownItems)
            {
                list.Add(item);
            }
            list.Sort(SortMenuToolsComparison);
            menuTools.DropDownItems.Clear();
            foreach (var item in list)
            {
                menuTools.DropDownItems.Add(item);
            }
        }

        private static int SortMenuToolsComparison(ToolStripItem x, ToolStripItem y)
        {
            if (x.Text == y.Text) return 0;
            if (string.Compare(x.Text, "Debugger", true) == 0) return -1;
            if (string.Compare(y.Text, "Debugger", true) == 0) return 1;
            return x.Text.CompareTo(y.Text);
        }

        #endregion Menu Handlers


        #region Bind Helpers

        private void BindCommand(ToolStripItem target, string path, object parameter = null)
        {
            target.Tag = parameter; // CommandParameter
            _binding.Bind(target, "Command", path);
            _binding.Bind(target, "Text", path + ".Text");
            _binding.Bind(target, "Checked", path + ".Checked");
            _binding.Bind(target, "Visible", path, arg => arg != null);
        }

        private void BindCommandLight(ToolStripItem target, string path, object parameter = null)
        {
            target.Tag = parameter; // CommandParameter
            _binding.Bind(target, "Command", path);
            _binding.Bind(target, "Visible", path, arg => arg != null);
        }

        private static bool CanExecute(ICommand command, object arg)
        {
            return command != null && command.CanExecute(arg);
        }

        private static void OnCommand(ICommand command, object arg = null)
        {
            if (command == null || !command.CanExecute(arg))
            {
                return;
            }
            command.Execute(arg);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Bind Helpers
    }
}
