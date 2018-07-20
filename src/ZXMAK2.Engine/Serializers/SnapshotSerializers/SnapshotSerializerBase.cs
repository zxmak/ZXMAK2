using System;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Serializers.SnapshotSerializers
{
    public abstract class SnapshotSerializerBase : FormatSerializer
    {
        protected Spectrum _spec;


        public SnapshotSerializerBase(Spectrum spec)
        {
            _spec = spec;
        }

        public override string FormatGroup { get { return "Snapshots"; } }
        public override string FormatName { get { return string.Format("{0} snapshot", FormatExtension); } }


        protected void UpdateState()
        {
            var ula = _spec.BusManager.FindDevice<IUlaDevice>();
            ula.ForceRedrawFrame();
            _spec.RaiseUpdateState();
        }

        protected byte ReadMemory(ushort addr)
        {
            //IMemory memory = _spec.BusManager.FindDevice(typeof(IMemory)) as IMemory;
            //return memory.RDMEM_DBG(addr);
            return _spec.DebugReadMemory(addr);
        }

        public void WriteMemory(ushort addr, byte value)
        {
            //IMemory memory = _spec.BusManager.FindDevice(typeof(IMemory)) as IMemory;
            //memory.WRMEM_DBG(addr, value);
            _spec.DebugWriteMemory(addr, value);
        }

        public int GetFrameTact()
        {
            var ula = _spec.BusManager.FindDevice<IUlaDevice>();
            return (int)(_spec.CPU.Tact % ula.FrameTactCount);
        }

        public void SetFrameTact(int frameTact)
        {
            var ula = _spec.BusManager.FindDevice<IUlaDevice>();

            if (frameTact < 0)
                frameTact = 0;
            frameTact %= ula.FrameTactCount;

            int delta = frameTact - GetFrameTact();
            if (delta < 0)
                delta += ula.FrameTactCount;
            _spec.CPU.Tact += delta;
        }

        public void InitStd128K()
        {
            _spec.DebugReset();
            foreach (var device in _spec.BusManager.FindDevices<BusDeviceBase>())
            {
                device.ResetState();
            }
            var memory = _spec.BusManager.FindDevice<IMemoryDevice>();
            memory.SYSEN = false;
            memory.DOSEN = false;
            SetFrameTact(0);
        }
    }
}
