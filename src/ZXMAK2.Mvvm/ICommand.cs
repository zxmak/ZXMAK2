using System;
using System.Linq;
using System.ComponentModel;


namespace ZXMAK2.Mvvm
{
    public interface ICommand : INotifyPropertyChanged
    {
        event EventHandler CanExecuteChanged;
        bool CanExecute(Object parameter);
        void Execute(Object parameter);
        void Update();

        string Text { get; set; }
        bool Checked { get; set; }
    }
}
