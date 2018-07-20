using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Engine.Interfaces
{
    public interface IJoystickDevice
    {
        string HostId { get; set; }
        IJoystickState JoystickState { get; set; }
    }
}
