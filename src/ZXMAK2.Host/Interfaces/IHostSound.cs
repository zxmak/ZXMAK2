using System;


namespace ZXMAK2.Host.Interfaces
{
    public interface IHostSound : IDisposable
    {
        /// <summary>
        /// Returns sample rate (50Hz step)
        /// </summary>
        int SampleRate { get; }
        
        /// <summary>
        /// Indicates if IsSynchronized=true is supported
        /// </summary>
        bool IsSyncSupported { get; }

        /// <summary>
        /// True for blocking PushFrame (50Hz wait),
        /// False for non-blocking (asynchronous) PushFrame
        /// </summary>
        bool IsSynchronized { get; set; }
        
        /// <summary>
        /// Push new frame
        /// </summary>
        void PushFrame(IFrameInfo info, IFrameSound frame);
        
        /// <summary>
        /// Cancel wait for blocking PushFrame
        /// </summary>
        void CancelWait();
    }
}
