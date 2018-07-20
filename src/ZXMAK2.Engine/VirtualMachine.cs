using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using ZXMAK2.Engine.Cpu;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Mvvm;
using System.Drawing;



namespace ZXMAK2.Engine
{
    public sealed class VirtualMachine : IVirtualMachine, IDebuggable
    {
        #region Fields

        private readonly object m_sync = new object();
        private readonly AutoResetEvent m_startedEvent = new AutoResetEvent(false);
        private Thread m_thread = null;
        private IFrameVideo m_blankData = new FrameVideo(320, 240, 1F);

        private string m_name = "ZX Spectrum Clone";
        private string m_description = "N/A";
        private bool m_isConfigUpdate;
        private string m_configFileName = string.Empty;


        private IHostService m_host;

        public Spectrum Spectrum { get; private set; }
        public IBus Bus { get { return Spectrum.BusManager; } }

        
        public int DebugFrameStartTact 
        { 
            get { return Spectrum.FrameStartTact; } 
        }

        #endregion Fields


        #region .ctor

        public unsafe VirtualMachine(IHostService host, ICommandManager commandManager)
        {
            m_host = host;
            Spectrum = new Spectrum();
            Spectrum.UpdateState += OnUpdateState;
            Spectrum.Breakpoint += OnBreakpoint;
            Spectrum.BusManager.FrameReady += Bus_OnFrameReady;
            Spectrum.BusManager.CommandManager = commandManager;
            Spectrum.BusManager.ConfigChanged += BusManager_OnConfigChanged;
            Spectrum.BusManager.SampleRate = host.SampleRate;
        }

        public void Dispose()
        {
            DoStop();
            if (m_host != null)
            {
                m_host.CancelPush();
            }
            Spectrum.BusManager.Disconnect();
            m_startedEvent.Dispose();
        }

        #endregion .ctor


        #region Config

        public void LoadConfigXml(XmlNode parent)
        {
            var infoNode = parent["Info"];
            var busNode = parent["Bus"];
            if (busNode == null)
            {
                Logger.Error("Machine bus configuration not found!");
                throw new ArgumentException("Machine bus configuration not found!");
            }

            m_name = "ZX Spectrum Clone";
            m_description = "N/A";
            if (infoNode != null)
            {
                if (infoNode.Attributes["name"] != null)
                {
                    m_name = infoNode.Attributes["name"].InnerText;
                }
                if (infoNode.Attributes["description"] != null)
                {
                    m_description = infoNode.Attributes["description"].InnerText;
                }
            }
            m_isConfigUpdate = true;
            try
            {
                Spectrum.BusManager.LoadConfigXml(busNode);
            }
            finally
            {
                m_isConfigUpdate = false;
            }
            DoReset();
        }

        public void SaveConfigXml(XmlNode parent)
        {
            var xeInfo = parent.OwnerDocument.CreateElement("Info");
            if (m_name != "ZX Spectrum Clone")
            {
                xeInfo.SetAttribute("name", m_name);
            }
            if (m_description != "N/A")
            {
                xeInfo.SetAttribute("description", m_description);
            }
            parent.AppendChild(xeInfo);
            var xeBus = parent.OwnerDocument.CreateElement("Bus");
            var busNode = parent.AppendChild(xeBus);
            m_isConfigUpdate = true;
            try
            {
                Spectrum.BusManager.SaveConfigXml(busNode);
            }
            finally
            {
                m_isConfigUpdate = false;
            }
        }

        private void BusManager_OnConfigChanged(object sender, EventArgs e)
        {
            if (!m_isConfigUpdate)
            {
                SaveConfig();
            }
        }

        public void OpenConfig(string fileName)
        {
            fileName = Path.GetFullPath(fileName);
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                m_configFileName = fileName;
                Spectrum.BusManager.MachineFile = m_configFileName;
                OpenConfig(stream);
            }
        }

        public void SaveConfig()
        {
            if (!string.IsNullOrEmpty(m_configFileName))
            {
                using (var stream = new FileStream(m_configFileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    SaveConfig(stream);
                }
            }
        }

        public void SaveConfigAs(string fileName)
        {
            fileName = Path.GetFullPath(fileName);
            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                m_configFileName = fileName;
                Spectrum.BusManager.MachineFile = m_configFileName;
                SaveConfig(stream);
            }
        }

        public void OpenConfig(Stream stream)
        {
            var xml = new XmlDocument();
            xml.Load(stream);
            var root = xml.DocumentElement;
            if (root == null || string.Compare(root.Name, "VirtualMachine", true)!=0)
            {
                Logger.Error("Invalid Machine Configuration File");
                throw new ArgumentException("Invalid Machine Configuration File");
            }
            LoadConfigXml(root);
        }

        public void SaveConfig(Stream stream)
        {
            var xml = new XmlDocument();
            var root = xml.AppendChild(xml.CreateElement("VirtualMachine"));
            SaveConfigXml(root);
            xml.Save(stream);
        }

        #endregion Config

        private IUlaDevice m_ula;

        public void RequestFrame()
        {
            PushFrame();
        }

        private void PushFrame(bool isRequested = true)
        {
            var host = m_host;
            if (host == null)
            {
                return;
            }
            var infoFrame = new FrameInfo(
                Spectrum.BusManager.IconDescriptorArray,
                DebugFrameStartTact,
                m_instantUpdateTime,
                Spectrum.BusManager.SoundFrame.SampleRate,
                isRequested);
            var ula = m_ula ?? Spectrum.BusManager.FindDevice<IUlaDevice>();
            var videoFrame = ula != null && ula.VideoData != null ? ula.VideoData : m_blankData;
            FrameSize = videoFrame.Size;
            if (isRequested)
            {
                m_host.PushFrame(infoFrame, videoFrame, null);
                return;
            }
            var soundFrame = Spectrum.BusManager.SoundFrame;
            m_host.PushFrame(infoFrame, videoFrame, soundFrame);
        }

        public event EventHandler FrameSizeChanged;
        
        private Size _frameSize;

        public Size FrameSize
        {
            get { return _frameSize; }
            private set
            {
                _frameSize = value;
                var handler = FrameSizeChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        
        public Size GetFrameSize()
        {
            var ula = m_ula ?? Spectrum.BusManager.FindDevice<IUlaDevice>();
            var videoFrame = ula != null && ula.VideoData != null ? ula.VideoData : m_blankData;
            return videoFrame.Size;
        }

        private long _renderTime;

        private void Bus_OnFrameReady()
        {
            if (m_host == null)
            {
                _renderTime = 0;
                return;
            }
            var startTime = Stopwatch.GetTimestamp();
            PushFrame(false);
            _renderTime = Stopwatch.GetTimestamp() - startTime;
        }

        /// <summary>
        /// Debugger Update State
        /// </summary>
        private void OnUpdateState(object sender, EventArgs e)
        {
            Spectrum.BusManager.IconPause.Visible = !Spectrum.IsRunning;
            var handler = UpdateState;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
            var ula = Spectrum.BusManager.FindDevice<IUlaDevice>();
            if (ula != null)
            {
                ula.Flush();
            }
            PushFrame();
        }

        private void OnBreakpoint(object sender, EventArgs e)
        {
            m_bpTriggered = true;
            var handler = Breakpoint;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }


        #region spectrum

        public void Init()
        {
            Spectrum.Init();
            Spectrum.DebugReset();
            Spectrum.BusManager.SetDebuggable(this);
        }

        private unsafe void runThreadProc()
        {
            try
            {
                m_startedEvent.Set();
                Spectrum.IsRunning = true;

                var bus = Spectrum.BusManager;
                var host = m_host;
                using (var input = new InputAggregator(
                    host,
                    bus.FindDevices<IKeyboardDevice>().ToArray(),
                    bus.FindDevices<IMouseDevice>().ToArray(),
                    bus.FindDevices<IJoystickDevice>().ToArray()))
                {
                    m_ula = Spectrum.BusManager.FindDevice<IUlaDevice>();

                    // main emulation loop
                    while (Spectrum.IsRunning)
                    {
                        input.Scan();
                        var startTime = Stopwatch.GetTimestamp();
                        Spectrum.ExecuteFrame();
                        var stopTime = Stopwatch.GetTimestamp();
                        m_instantUpdateTime = stopTime - startTime - _renderTime;
                        m_instantRenderTime = _renderTime;
                    }

                    m_ula = null;
                }
                OnUpdateState(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private double m_instantUpdateTime;
        private double m_instantRenderTime;

        #endregion

        #region IDebuggable

        private bool m_bpTriggered;

        public void DoReset()
        {
            lock (m_sync)
            {
                var run = IsRunning;
                DoStop();
                m_bpTriggered = false;
                Spectrum.DebugReset();
                if (run && !m_bpTriggered)
                {
                    DoRun();
                }
            }
            PushFrame();
        }

        public void DoNmi()
        {
            lock (m_sync)
            {
                var run = IsRunning;
                DoStop();
                m_bpTriggered = false;
                Spectrum.DebugNmi();
                if (run && !m_bpTriggered)
                {
                    DoRun();
                }
            }
            PushFrame();
        }

        public void DoStepInto()
        {
            lock (m_sync)
            {
                Spectrum.DebugStepInto();
            }
            PushFrame();
        }

        public void DoStepOver()
        {
            lock (m_sync)
            {
                Spectrum.DebugStepOver();
            }
            PushFrame();
        }

        public void DoRun()
        {
            lock (m_sync)
            {
                if (IsRunning)
                {
                    return;
                }
                m_thread = null;
                m_thread = new Thread(new ThreadStart(runThreadProc));
                m_thread.Name = "ZXVM";
                if (Environment.ProcessorCount > 1)
                {
                    m_thread.Priority = ThreadPriority.AboveNormal;
                }
                m_thread.Start();
            }
            m_startedEvent.WaitOne();
            //OnUpdateVideo();
        }

        public void DoStop()
        {
            var thread = default(Thread);
            lock (m_sync)
            {
                if (!IsRunning || m_thread == null)
                {
                    return;
                }
                Spectrum.IsRunning = false;
                var host = m_host;
                if (host != null)
                {
                    host.CancelPush();
                }
                thread = m_thread;
                m_thread = null;
            }
            if (thread != null)
            {
                thread.Join();
            }
            PushFrame();
        }

        public void RaiseUpdateState()
        {
            Spectrum.RaiseUpdateState();
        }

        public byte ReadMemory(ushort addr)
        {
            var data = new byte[1];
            ReadMemory(addr, data, 0, 1);
            return data[0];
        }

        public void WriteMemory(ushort addr, byte value)
        {
            var data = new byte[1];
            data[0] = value;
            WriteMemory(addr, data, 0, 1);
        }

        public void ReadMemory(ushort addr, byte[] data, int offset, int length)
        {
            lock (m_sync)
            {
                var memory = Spectrum.BusManager.FindDevice<IMemoryDevice>();
                ushort ptr = addr;
                for (int i = 0; i < length; i++, ptr++)
                {
                    data[offset + i] = memory.RDMEM_DBG(ptr);
                }
            }
        }

        public void WriteMemory(ushort addr, byte[] data, int offset, int length)
        {
            lock (m_sync)
            {
                var memory = Spectrum.BusManager.FindDevice<IMemoryDevice>();
                ushort ptr = addr;
                for (int i = 0; i < length; i++, ptr++)
                {
                    memory.WRMEM_DBG(ptr, data[offset + i]);
                }
            }
            PushFrame();
        }

        public void AddBreakpoint(Breakpoint bp)
        {
            lock (m_sync)
            {
                Spectrum.DebugAddBreakpoint(bp);
            }
        }

        public void RemoveBreakpoint(Breakpoint bp)
        {
            lock (m_sync)
            {
                Spectrum.DebugRemoveBreakpoint(bp);
            }
        }

        public Breakpoint[] GetBreakpointList()
        {
            lock (m_sync)
            {
                return Spectrum.DebugGetBreakpointList();
            }
        }

        public void ClearBreakpoints()
        {
            lock (m_sync)
            {
                Spectrum.DebugClearBreakpoints();
            }
        }

        public event EventHandler UpdateState;
        public event EventHandler Breakpoint;

        public bool IsRunning
        {
            get { return Spectrum.IsRunning; }
        }

        public CpuUnit CPU
        {
            get { return Spectrum.CPU; }
        }

        public int GetFrameTact()
        {
            return Spectrum.BusManager.GetFrameTact();
        }

        public int FrameTactCount
        {
            get { return Spectrum.BusManager.FrameTactCount; }
        }

        public IRzxState RzxState
        {
            get { return Spectrum.BusManager.RzxHandler; }
        }

        #endregion
    }
}
