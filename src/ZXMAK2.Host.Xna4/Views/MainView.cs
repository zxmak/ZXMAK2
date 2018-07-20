using System;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Host.Xna4.Properties;
using ZXMAK2.Host.Xna4.Xna;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Presentation.Interfaces;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Services;
using ZXMAK2.Mvvm;
using ZXMAK2.Mvvm.BindingTools;


namespace ZXMAK2.Host.Xna4.Views
{
    public unsafe class MainView : Game, IMainView, IHostVideo
    {
        #region Fields

        private readonly GraphicsDeviceManager m_deviceManager;
        private readonly object m_syncTexture = new object();
        private readonly AutoResetEvent m_frameEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent m_cancelEvent = new AutoResetEvent(false);
        private readonly BindingService m_binding = new BindingService();
        private Texture2D[] m_texture = new Texture2D[2];
        private SpriteBatch m_sprite;
        private SpriteFont m_font;
        private readonly GraphMonitor m_graphRender = new GraphMonitor(150);
        private readonly GraphMonitor m_graphUpdate = new GraphMonitor(150);
        private int m_textureIndex;

        private string m_title;
        private IHostService m_host;
        private int[] m_translateBuffer;
        private int m_debugFrameStart;
        private IFrameVideo m_videoData;

        private ICommand CommandViewFullScreen { get; set; }
        private ICommand CommandVmWarmReset { get; set; }

        #endregion Fields


        public MainView()
        {
            m_deviceManager = new GraphicsDeviceManager(this);

            m_binding.Bind(this, "Title", "Title");
        }


        #region IMainView

        public object DataContext
        {
            get { return m_binding.DataContext; }
            set { m_binding.DataContext = value; }
        }

        public string Title
        {
            get { return m_title; }
            set 
            { 
                m_title = value; 
                if (string.IsNullOrEmpty(m_title))
                {
                    Window.Title = "ZXMAK2-XNA4";
                }
                else
                {
                    Window.Title = string.Format(
                        "[{0}] - ZXMAK2-XNA4", 
                        m_title); 
                }
            }
        }

        public bool IsFullScreen
        {
            get { return m_deviceManager.IsFullScreen; }
            set { m_deviceManager.IsFullScreen = value; }
        }

        public IHostService Host
        {
            get { return m_host; }
        }

        public ICommandManager CommandManager
        {
            get { return null; }
        }

        public event EventHandler ViewOpened;
        public event EventHandler ViewClosed;
        public event EventHandler RequestFrame;

        public void Bind()
        {
            //presenter.CommandViewSyncSource.Execute(false);
            m_binding.Bind(this, "CommandViewFullScreen", "CommandViewFullScreen");
            m_binding.Bind(this, "CommandVmWarmReset", "CommandVmWarmReset");
        }

        public void Close()
        {
            m_binding.Dispose();
            Exit();
        }

        public void Activate()
        {
        }

        #endregion IMainView

        
        #region IHostVideo

        public bool IsSyncSupported
        {
            get { return true; }
        }

        public bool IsSynchronized { get; set; }
        
        public void WaitFrame()
        {
            m_cancelEvent.Reset();
            WaitHandle.WaitAny(new[] { m_frameEvent, m_cancelEvent });
        }

        public void CancelWait()
        {
            m_cancelEvent.Set();
        }

        public void PushFrame(IFrameInfo info, IFrameVideo frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame");
            }
            if (!info.IsRefresh)
            {
                m_graphUpdate.PushPeriod();
            }
            if (IsSynchronized && !info.IsRefresh)
            {
                WaitFrame();
            }
            m_debugFrameStart = info.StartTact;
            m_videoData = frame;
            
            var videoLen = m_videoData.Size.Width * m_videoData.Size.Height;
            
            // we need to translate bgra colors to rgba
            // because brga color support was removed from XNA4
            if (m_translateBuffer == null ||
                m_translateBuffer.Length < videoLen)
            {
                m_translateBuffer = new int[videoLen];
            }
            fixed (int* pBuffer = m_videoData.Buffer)
            {
                Marshal.Copy(
                    (IntPtr)pBuffer,
                    m_translateBuffer,
                    0,
                    videoLen);
            }
            fixed (int* pBuffer = m_translateBuffer)
            {
                var puBuffer = (uint*)pBuffer;
                // bgra -> rgba
                for (var i = 0; i < videoLen; i++)
                {
                    puBuffer[i] = 
                        (puBuffer[i] & 0x000000ff) << 16 |
                        (puBuffer[i] & 0xFF00FF00) |
                        (puBuffer[i] & 0x00FF0000) >> 16;
                }
            }
            // copy translated image to output texture
            lock (m_syncTexture)
            {
                var texture = m_texture[m_textureIndex];
                if (texture == null)
                {
                    return;
                }
                texture.SetData<int>(
                    m_translateBuffer,
                    0,
                    videoLen);
            }
        }

        #endregion IHostVideo


        #region Event Raise

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

        #endregion Event Raise


        #region XNA

        protected override void Initialize()
        {
            m_deviceManager.GraphicsProfile = GraphicsProfile.HiDef;
            m_deviceManager.ApplyChanges();
            base.Initialize();

            m_host = CreateHost();
            OnViewOpened();

            OnRequestFrame();
            m_deviceManager.PreferredBackBufferWidth = m_videoData.Size.Width * 2;
            m_deviceManager.PreferredBackBufferHeight = m_videoData.Size.Height * 2;
            m_deviceManager.PreferredBackBufferFormat = SurfaceFormat.Color;
            m_deviceManager.ApplyChanges();
            m_sprite = new SpriteBatch(m_deviceManager.GraphicsDevice);
            //m_deviceManager.SynchronizeWithVerticalRetrace = false;
        }

        private IHostService CreateHost()
        {
            var viewResolver = Locator.TryResolve<IResolver>("View");
            if (viewResolver == null)
            {
                return new HostService(this, null, null, null, null);
            }
            var sound = viewResolver.TryResolve<IHostSound>();
            var keyboard = viewResolver.TryResolve<IHostKeyboard>();
            var mouse = viewResolver.TryResolve<IHostMouse>();
            var joystick = viewResolver.TryResolve<IHostJoystick>();
            return new HostService(this, sound, keyboard, mouse, joystick);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            OnViewClosed();
            if (m_host != null)
            {
                m_host.Dispose();
                m_host = null;
            }
        }

        protected override bool ShowMissingRequirementMessage(Exception exception)
        {
            Logger.Error(exception);
            return base.ShowMissingRequirementMessage(exception);
        }

        protected override void LoadContent()
        {
            Content = new ResourceContentManager(Services, Resources.ResourceManager);
            base.LoadContent();
            m_font = Content.Load<SpriteFont>("SansSerifBold");
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var kbdState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            UpdateInput(kbdState, mouseState);
            
            var isAlt = kbdState[Keys.LeftAlt] == KeyState.Down ||
                kbdState[Keys.RightAlt] == KeyState.Down;
            var isCtrl = kbdState[Keys.LeftControl] == KeyState.Down ||
                kbdState[Keys.RightControl] == KeyState.Down;
            if (isAlt && isCtrl &&
                CommandVmWarmReset != null &&
                CommandVmWarmReset.CanExecute(null))
            {
                CommandVmWarmReset.Execute(kbdState[Keys.Insert] == KeyState.Down);
            }
        }

        private void UpdateInput(
            KeyboardState kbdState, 
            MouseState mouseState)
        {
            var keyboard = m_host.Keyboard as XnaKeyboard;
            if (keyboard != null)
            {
                keyboard.Update(kbdState);
            }
            var mouse = m_host.Mouse as XnaMouse;
            if (mouse != null)
            {
                mouse.Update(mouseState);
            }
        }

        private void CheckTexture()
        {
            var texture = m_texture[m_textureIndex];
            if (texture == null ||
                texture.Width != m_videoData.Size.Width ||
                texture.Height != m_videoData.Size.Height)
            {
                if (m_texture[m_textureIndex] != null)
                {
                    m_texture[m_textureIndex].Dispose();
                    m_texture[m_textureIndex] = null;
                }
                if (m_texture[m_textureIndex] != null)
                {
                    m_texture[m_textureIndex].Dispose();
                    m_texture[m_textureIndex] = null;
                }
                m_texture[m_textureIndex] = new Texture2D(
                    m_deviceManager.GraphicsDevice,
                    m_videoData.Size.Width,
                    m_videoData.Size.Height,
                    false,
                    SurfaceFormat.Color);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            m_frameEvent.Set();
            m_graphRender.PushPeriod();
            base.Draw(gameTime);

            m_deviceManager.GraphicsDevice.Clear(Color.Black);
            m_sprite.Begin(
                SpriteSortMode.Immediate, 
                BlendState.AlphaBlend,
                SamplerState.PointClamp, // disable anti-aliasing
                null,
                null);
            
            var texture = (Texture2D)null;
            lock (m_syncTexture)
            {
                CheckTexture();
                texture = m_texture[m_textureIndex];
                m_textureIndex++;
                m_textureIndex &= 1;
            }
            if (texture != null)
            {
                var rect = new Rectangle(
                    0,
                    0,
                    m_deviceManager.PreferredBackBufferWidth,
                    m_deviceManager.PreferredBackBufferHeight);
                m_sprite.Draw(
                    texture,
                    rect,
                    Color.White);
                
                m_writePos = 0F;
                
                var fpsRenderMin = GetFps(m_graphRender.Get().Max());
                var fpsRenderAvg = GetFps(m_graphRender.Get().Average());
                var fpsRenderMax = GetFps(m_graphRender.Get().Min());
                WriteLine("Render FPS: {0:F3} < {1:F3} < {2:F3}", fpsRenderMin, fpsRenderAvg, fpsRenderMax);

                var fpsUpdateMin = GetFps(m_graphUpdate.Get().Max());
                var fpsUpdateAvg = GetFps(m_graphUpdate.Get().Average());
                var fpsUpdateMax = GetFps(m_graphUpdate.Get().Min());
                WriteLine("Update FPS: {0:F3} < {1:F3} < {2:F3}", fpsUpdateMin, fpsUpdateAvg, fpsUpdateMax);

                WriteLine(
                    "Back: [{0}, {1}, {2}]", 
                    m_deviceManager.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    m_deviceManager.GraphicsDevice.PresentationParameters.BackBufferHeight,
                    m_deviceManager.GraphicsDevice.PresentationParameters.BackBufferFormat);
                WriteLine(
                    "Surface: [{0}, {1}]", 
                    m_videoData.Size.Width,
                    m_videoData.Size.Height);
                WriteLine("FrameStart: {0}T", m_debugFrameStart);
            }
            m_sprite.End();
        }

        private double? GetFps(double period)
        {
            if (period > 0D)
            {
                return GraphMonitor.Frequency / period;
            }
            return null;
        }

        #endregion XNA

        #region Frame Console Emulation

        private float m_writePos;

        private void WriteLine(string fmt, params object[] args)
        {
            var strText = string.Format(fmt, args);
            m_sprite.DrawString(
                m_font,
                strText,
                new Vector2(5, 5 + m_writePos),
                Color.Yellow);
            var strSize = m_font.MeasureString(strText);
            m_writePos += strSize.Y;
        }

        #endregion Frame Console Emulation
    }
}
