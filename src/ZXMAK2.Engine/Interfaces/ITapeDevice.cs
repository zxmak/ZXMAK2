using System;
using System.Collections.Generic;
using ZXMAK2.Model.Tape.Interfaces;


namespace ZXMAK2.Engine.Interfaces
{
	public interface ITapeDevice
	{
		void Play();
		void Stop();
		void Rewind();
		void Reset();       // loaded new image
		bool IsPlay { get; }
		int TactsPerSecond { get; }
		List<ITapeBlock> Blocks { get; }
		event EventHandler TapeStateChanged;
		int CurrentBlock { get; set; }
		int Position { get; }
		bool UseTraps { get; set; }
		bool UseAutoPlay { get; set; }
	}
}
