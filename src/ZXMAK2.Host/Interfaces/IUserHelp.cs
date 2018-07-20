

namespace ZXMAK2.Host.Interfaces
{
    public interface IUserHelp
    {
        bool CanShow(object uiControl);
        void ShowHelp(object uiControl);
        void ShowHelp(object uiControl, string keyword);
    }
}
