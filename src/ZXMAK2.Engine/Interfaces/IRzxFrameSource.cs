using System;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Engine.Interfaces
{
    public interface IRzxFrameSource : IDisposable
    {
        RzxFrame[] GetNextFrameArray();
    }
}
