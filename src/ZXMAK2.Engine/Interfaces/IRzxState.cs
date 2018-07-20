

namespace ZXMAK2.Engine.Interfaces
{
	public interface IRzxState
	{
		bool IsPlayback { get; }
		bool IsRecording { get; }
		int Frame { get; }
		int Fetch { get; }
		int Input { get; }
		int FrameCount { get; }
		int FetchCount { get; }
		int InputCount { get; }
	}
}
