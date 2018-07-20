using System;
using System.IO;

using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Serializers.ScreenshotSerializers
{
	public class ScrSerializer : FormatSerializer
	{
		protected IUlaDevice m_ulaDevice;
	
		
		public ScrSerializer(IUlaDevice ulaDevice)
		{
            m_ulaDevice = ulaDevice;
		}



		#region FormatSerializer

        public override string FormatGroup { get { return "Screenshots"; } }
        public override string FormatName { get { return "SCR screenshot"; } }
		public override string FormatExtension { get { return "SCR"; } }

		public override bool CanDeserialize { get { return true; } }
		public override bool CanSerialize { get { return true; } }

		public override void Deserialize(Stream stream)
		{
            m_ulaDevice.LoadScreenData(stream);
            m_ulaDevice.ForceRedrawFrame();
		}

		public override void Serialize(Stream stream)
		{
            m_ulaDevice.SaveScreenData(stream);
		}

		#endregion
	}
}
