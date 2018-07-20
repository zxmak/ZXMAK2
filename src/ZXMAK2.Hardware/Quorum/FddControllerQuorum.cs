using System;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Hardware.General;
using ZXMAK2.Hardware.Circuits.Fdd;


namespace ZXMAK2.Hardware.Quorum
{
    public class FddControllerQuorum : FddController
    {
        #region Fields

        private static readonly int[] s_drvDecode = new int[] { 3, 0, 1, 3 };

        #endregion


        public FddControllerQuorum()
        {
            Name = "FDD QUORUM";
            Description = "FDD controller WD1793 with QUORUM port activation";
        }

        
        #region BetaDiskInterface

        protected override void OnSubscribeIo(IBusManager bmgr)
        {
            // mask - #9F
            // #80 - CMD
            // #81 - TRK
            // #82 - SEC
            // #83 - DAT
            // #85 - SYS
            bmgr.Events.SubscribeWrIo(0x9C, 0x80 & 0x9C, BusWriteFdc);
            bmgr.Events.SubscribeRdIo(0x9C, 0x80 & 0x9C, BusReadFdc);
            bmgr.Events.SubscribeWrIo(0x9F, 0x85 & 0x9F, BusWriteSys);
            bmgr.Events.SubscribeRdIo(0x9F, 0x85 & 0x9F, BusReadSys);
        }

        public override bool IsActive
        {
            get 
            {
                return true;//m_memory.CMR1 & 0x80; // Q_TR_DOS
            }
        }

        protected override void BusWriteFdc(ushort addr, byte value, ref bool handled)
        {
            if (handled || !IsActive)
                return;
            handled = true;

            var fdcReg = addr & 0x03;
            if (LogIo)
            {
                LogIoWrite(m_cpu.Tact, (WD93REG)fdcReg, value);
            }
            m_wd.Write(m_cpu.Tact, (WD93REG)fdcReg, value);
        }

        protected override void BusReadFdc(ushort addr, ref byte value, ref bool handled)
        {
            if (handled || !IsActive)
                return;
            handled = true;

            var fdcReg = addr & 0x03;
            value = m_wd.Read(m_cpu.Tact, (WD93REG)fdcReg);
            if (LogIo)
            {
                LogIoRead(m_cpu.Tact, (WD93REG)fdcReg, value);
            }
        }

        protected override void BusWriteSys(ushort addr, byte value, ref bool handled)
        {
            if (handled || !IsActive)
                return;
            handled = true;

            var drv = s_drvDecode[value & 3];
            drv = ((value & ~3) ^ 0x10) | drv;
            if (LogIo)
            {
                LogIoWrite(m_cpu.Tact, WD93REG.SYS, (byte)drv);
            }
            m_wd.Write(m_cpu.Tact, WD93REG.SYS, (byte)drv);
        }

        protected override void BusReadSys(ushort addr, ref byte value, ref bool handled)
        {
            if (handled || !IsActive)
                return;
            handled = true;

            value = m_wd.Read(m_cpu.Tact, WD93REG.SYS);
            if (LogIo)
            {
                LogIoRead(m_cpu.Tact, WD93REG.SYS, value);
            }
        }

        #endregion
    }
}
