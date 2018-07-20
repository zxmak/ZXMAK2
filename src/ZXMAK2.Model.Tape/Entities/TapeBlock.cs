using System.Collections.Generic;
using ZXMAK2.Model.Tape.Interfaces;


namespace ZXMAK2.Model.Tape.Entities
{
	public class TapeBlock : ITapeBlock
    {
        #region ITapeBlock

        public string Description { get; set; }
        public int TzxId { get; set; }
        public TapeCommand Command { get; set; }

        public int Count 
        { 
            get { return Periods.Count; } 
        }

        public int GetPeriod(int index, int cpuFrequency)
        {
            return Periods[index];
        }

        public byte[] GetData()
        {
            return TapData;
        }

        #endregion ITapeBlock


        public List<int> Periods;
        public byte[] TapData = null;


        public TapeBlock()
        {
            TzxId = -1;
            Command = TapeCommand.NONE;
            Periods = new List<int>();
            TapData = null;
        }
	}
}
