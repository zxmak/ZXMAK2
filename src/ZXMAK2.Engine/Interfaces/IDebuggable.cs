using System;
using ZXMAK2.Engine.Cpu;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Engine.Interfaces
{
    public interface IDebuggable
    {
        void DoReset();
        void DoStepInto();
        void DoStepOver();
        void DoRun();
        void DoStop();
        void RaiseUpdateState();

        byte ReadMemory(ushort addr);
        void WriteMemory(ushort addr, byte value);
        void ReadMemory(ushort addr, byte[] data, int offset, int length);
        void WriteMemory(ushort addr, byte[] data, int offset, int length);

        void AddBreakpoint(Breakpoint bp);
        void RemoveBreakpoint(Breakpoint bp);
        Breakpoint[] GetBreakpointList();
        void ClearBreakpoints();

        event EventHandler UpdateState;
        event EventHandler Breakpoint;
        bool IsRunning { get; }
        CpuUnit CPU { get; }
        IBus Bus { get; }
        int GetFrameTact();
        int FrameTactCount { get; }

        IRzxState RzxState { get; }
    }
}
