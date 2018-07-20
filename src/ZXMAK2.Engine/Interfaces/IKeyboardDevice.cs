using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Engine.Interfaces
{
    public interface IKeyboardDevice
    {
        IKeyboardState KeyboardState { get; set; }
    }
}
