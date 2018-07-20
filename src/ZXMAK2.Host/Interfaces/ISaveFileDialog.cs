using System;
using System.Linq;
using ZXMAK2.Host.Entities;


namespace ZXMAK2.Host.Interfaces
{
    public interface ISaveFileDialog : IDisposable
    {
        string Title { get; set; }
        string Filter { get; set; }
        string DefaultExt { get; set; }
        string FileName { get; set; }
        bool OverwritePrompt { get; set; }

        DlgResult ShowDialog(object owner);
    }
}
