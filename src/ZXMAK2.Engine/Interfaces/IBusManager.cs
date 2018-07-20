using System;
using ZXMAK2.Engine.Cpu;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Mvvm;


namespace ZXMAK2.Engine.Interfaces
{
	public interface IBusManager
	{
        IEventManager Events { get; }

		void AddSerializer(IFormatSerializer serializer);
		void RegisterIcon(IIconDescriptor iconDesc);

        void AddCommandUi(ICommand command);

		CpuUnit CPU { get; }
		bool IsSandbox { get; }
		string GetSatelliteFileName(string extension);

        T FindDevice<T>() where T : class;

		RzxHandler RzxHandler { get; }
	}

	public delegate void BusReadProc(ushort addr, ref byte value);
	public delegate void BusWriteProc(ushort addr, byte value);
	public delegate void BusReadIoProc(ushort addr, ref byte value, ref bool handled);
	public delegate void BusWriteIoProc(ushort addr, byte value, ref bool handled);
    public delegate void BusRqProc(BusCancelArgs e);

    public class BusCancelArgs
    {
        private bool m_cancel;

        public BusCancelArgs()
        {
            m_cancel = false;
        }

        public bool Cancel
        {
            get { return m_cancel; }
            set { m_cancel = m_cancel | value; }
        }
    }
}
