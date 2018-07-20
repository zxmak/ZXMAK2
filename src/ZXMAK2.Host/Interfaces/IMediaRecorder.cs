using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZXMAK2.Host.Interfaces
{
    public interface IMediaRecorder : IDisposable
    {
        void PushFrame(
            IFrameInfo infoFrame,
            IFrameVideo videoFrame, 
            IFrameSound soundFrame);
    }
}
