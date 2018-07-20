using ZXMAK2.Host.Entities;


namespace ZXMAK2.Host.Presentation.Interfaces
{
    public interface ISettingService
    {
        int WindowWidth { get; set; }
        int WindowHeight { get; set; }
        bool IsToolBarVisible { get; set; }
        bool IsStatusBarVisible { get; set; }

        SyncSource SyncSource { get; set; }
        
        ScaleMode RenderScaleMode { get; set; }
        VideoFilter RenderVideoFilter { get; set; }
        bool RenderSmooth { get; set; }
        bool RenderMimicTv { get; set; }
        bool RenderDisplayIcon { get; set; }
        bool RenderDebugInfo { get; set; }
    }
}
