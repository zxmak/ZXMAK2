using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZXMAK2.Engine.Interfaces
{
    public interface IEventManager
    {
        void SubscribeRdMemM1(int addrMask, int maskedValue, BusReadProc proc);
        void SubscribeRdMem(int addrMask, int maskedValue, BusReadProc proc);
        void SubscribeWrMem(int addrMask, int maskedValue, BusWriteProc proc);
        void SubscribeRdIo(int addrMask, int maskedValue, BusReadIoProc proc);
        void SubscribeWrIo(int addrMask, int maskedValue, BusWriteIoProc proc);
        void SubscribeRdNoMreq(int addrMask, int maskedValue, Action<ushort> proc);
        void SubscribeWrNoMreq(int addrMask, int maskedValue, Action<ushort> proc);
        void SubscribeReset(Action proc);
        void SubscribeNmiRq(BusRqProc proc);
        void SubscribeNmiAck(Action proc);
        void SubscribeIntAck(Action proc);
        void SubscribeScanInt(Action<int> handler);

        void SubscribePreCycle(Action proc);
        void SubscribeBeginFrame(Action handler);
        void SubscribeEndFrame(Action handler);

        void RequestNmi(int timeOut);
    }
}
