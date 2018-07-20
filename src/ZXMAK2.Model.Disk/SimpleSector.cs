

namespace ZXMAK2.Model.Disk
{
	public class SimpleSector : Sector
	{
		private readonly bool _adPresent;
		private readonly bool _dataPresent;
		private readonly byte[] _adMark;
		private readonly byte[] _data;

		public override byte[] Data { get { return _data; } }
		public override bool DataPresent { get { return _dataPresent; } }

		public override bool AdPresent { get { return _adPresent; } }
		public override byte C { get { return _adMark[0]; } }
		public override byte H { get { return _adMark[1]; } }
		public override byte R { get { return _adMark[2]; } }
		public override byte N { get { return _adMark[3]; } }


		/// <summary>
		/// Make sector
		/// </summary>
		/// <param name="cc">Cylinder</param>
		/// <param name="hh">Head</param>
		/// <param name="rr">Sector Number</param>
		/// <param name="nn">Sector Size Code</param>
		public SimpleSector(int cc, int hh, int rr, int nn, byte[] data)
		{
			_adPresent = true;
            _dataPresent = data != null;
            _adMark = new byte[4]
            {
			    (byte)cc,
			    (byte)hh,
			    (byte)rr,
			    (byte)nn,
            };
			_data = data ?? new byte[0];
		}

		public SimpleSector(int cc, int hh, int rr, int nn) 
			: this(cc,hh,rr,nn,new byte[128 << (nn&7)]) 
        { 
        }

		public SimpleSector(byte[] data)
			: this(0,0,0,0, data)
		{
			_adPresent = false;		
		}
	}
}
