using System;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Engine.Cpu;


namespace ZXMAK2.Hardware.Quorum
{
    // Base memory layer for Quorum-64 and Quorum-128 and higher
    public abstract class MemoryQuorum : MemoryBase
    {
        #region Constants

        protected const int Q_F_RAM = 0x01;
        protected const int Q_RAM_8 = 0x08;
        protected const int Q_B_ROM = 0x20;
        protected const int Q_BLK_WR = 0x40;
        protected const int Q_TR_DOS = 0x80;

        #endregion Constants

        #region Fields

        protected CpuUnit Cpu;

        #endregion Fields

        protected MemoryQuorum(
            string romSetName,
            int romPageCount,
            int ramPageCount)
            : base(romSetName, romPageCount, ramPageCount)
        {
        }

        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            Cpu = bmgr.CPU;

            bmgr.Events.SubscribeWrIo(0x0099, 0x0000 & 0x0099, BusWritePort0000);

            bmgr.Events.SubscribeRdMemM1(0xFF00, 0x3D00, BusReadMem3DXX_M1);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x4000, BusReadMemRam);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x8000, BusReadMemRam);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0xC000, BusReadMemRam);

            bmgr.Events.SubscribeReset(BusReset);
            bmgr.Events.SubscribeNmiRq(BusNmiRq);
            bmgr.Events.SubscribeNmiAck(BusNmiAck);

            // Subscribe before MemoryBase.BusInit 
            // to handle memory switches before read
            base.BusInit(bmgr);
        }

        #endregion

        #region Private

        protected virtual void BusWritePort0000(ushort addr, byte value, ref bool handled)
        {
            //LogAgent.Info("PC: #{0:X4}  CMR1 <- #{1:X2}", m_cpu.regs.PC, value);
            CMR1 = value;
        }

        protected virtual void BusReadMem3DXX_M1(ushort addr, ref byte value)
        {
            if (IsRom48)
            {
                DOSEN = true;
            }
        }

        protected virtual void BusReadMemRam(ushort addr, ref byte value)
        {
            if (DOSEN)
            {
                DOSEN = false;
            }
        }

        protected virtual void BusReset()
        {
            DOSEN = false;
            CMR0 = 0;
            CMR1 = 0;
        }

        protected virtual void BusNmiRq(BusCancelArgs e)
        {
            //e.Cancel = DOSEN;
        }

        protected virtual void BusNmiAck()
        {
            CMR1 = 0x00;
        }

        #endregion
    }
}
