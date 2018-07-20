using System;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Engine.Interfaces
{
    #region Comment
    /// <summary>
    /// Used to save/load state of AY8910 devices
    /// </summary>
    #endregion
    public interface IPsgDevice
    {
        event Action<IPsgDevice, PsgPortState> IraHandler;
        event Action<IPsgDevice, PsgPortState> IrbHandler;
        
        byte RegAddr { get; set; }
        byte GetReg(int index);
        void SetReg(int index, byte value);
    }
}
