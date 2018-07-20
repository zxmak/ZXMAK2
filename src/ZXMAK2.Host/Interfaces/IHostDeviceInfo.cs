using System;


namespace ZXMAK2.Host.Interfaces
{
    public interface IHostDeviceInfo : IComparable
    {
        string Name { get; }
        string HostId { get; }
    }
}
