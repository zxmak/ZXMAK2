using ZXMAK2.Mvvm;


namespace ZXMAK2.Engine.Interfaces
{
    public interface ICommandManager
    {
        void Clear();
        void Add(ICommand command);
    }
}
