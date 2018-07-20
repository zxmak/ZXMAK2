using System.IO;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Engine.Interfaces
{
	public interface IUlaDevice
	{
        IFrameVideo VideoData { get; }

        bool IsEarlyTimings { get; }

		void LoadScreenData(Stream stream);
		void SaveScreenData(Stream stream);
		void ForceRedrawFrame();

		void Flush();

		int FrameTactCount { get; }
		bool CheckInt(int frameTact);

		byte PortFE { get; set; }
	}
}
