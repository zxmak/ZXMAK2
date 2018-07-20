using ZXMAK2.Host.Interfaces;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Hardware.General
{
    public class AyMouseDevice : BusDeviceBase, IMouseDevice
    {
        public AyMouseDevice()
        {
            Category = BusDeviceCategory.Mouse;
            Name = "MOUSE AY";
            Description = "AY mouse based on V.M.G. extension";
        }
        

        #region IBusDevice Members

        public override void BusInit(IBusManager bmgr)
        {
            m_psgDevice = bmgr.FindDevice<IPsgDevice>();
        }

        public override void BusConnect()
        {
            if (m_psgDevice != null)
            {
                m_psgDevice.IraHandler += PsgDevice_OnUpdateIra;
            }
        }

        public override void BusDisconnect()
        {
            if (m_psgDevice != null)
            {
                m_psgDevice.IraHandler -= PsgDevice_OnUpdateIra;
            }
        }

        #endregion

        #region IMouseDevice Members

		public IMouseState MouseState
		{
			get { return m_mouseState; }
			set { m_mouseState = value; }
		}

        #endregion

        private IPsgDevice m_psgDevice;
		private IMouseState m_mouseState = null;
        private int _lastAyMouseX = 0;
        private int _lastAyMouseY = 0;

        private void PsgDevice_OnUpdateIra(IPsgDevice sender, PsgPortState state)
        {
            //
            // Emulation AY-Mouse (V.M.G. schema)
            //
			int x = 0;
			int y = 0;
			int b = 0;
			if (MouseState != null)
			{
				x = MouseState.X;
				y = MouseState.Y;
				b = MouseState.Buttons;
			}
			
			int pcDelta;
            if ((state.OutState & 0x40) != 0) // selected V counter
                pcDelta = _lastAyMouseY - y / 4;
            else							  // selected H counter
                pcDelta = x / 4 - _lastAyMouseX;
            // make signed 4 bit integer...
            pcDelta = pcDelta + 8;

            // prevent overflow (this feature not present in original schema)...
            if (pcDelta < 0) pcDelta = 0;
            if (pcDelta > 15) pcDelta = 15;

            // buttons 0 and 1...
            state.InState = (byte)((pcDelta & 0x0F) | ((b & 3) ^ 3) << 4 | (state.OutState & 0xC0));

            if (state.DirOut && ((state.OutState ^ state.OldOutState) & 0x40) != 0 && (state.OutState & 0x40) == 0)
            {
                // reset H and V counters
                _lastAyMouseX = x / 4;
                _lastAyMouseY = y / 4;
            }
        }
    }
}
