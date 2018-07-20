using ZXMAK2.Host.Entities;


namespace ZXMAK2.Host.Interfaces
{
    public interface IUserQuery
    {
        DlgResult Show(
            string message,
            string caption,
            DlgButtonSet buttonSet,
            DlgIcon icon);
        
        object ObjectSelector(object[] objArray, string caption);

        bool QueryText(
            string caption,
            string text,
            ref string value);

        bool QueryValue(
            string caption,
            string text,
            string format,
            ref int value,
            int min,
            int max);
    }
}
