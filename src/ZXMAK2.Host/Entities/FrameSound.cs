using System;
using System.Linq;
using System.Collections.Generic;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Host.Entities
{
    public class FrameSound : IFrameSound
    {
        private readonly uint[] _mixBuffer;
        private uint[] _buffer;
        private uint[][] _sources;

        public FrameSound(int sampleRate, IEnumerable<uint[]> sources)
        {
            _mixBuffer = new uint[(int)(sampleRate / 50D + 0.5D)];
            SampleRate = sampleRate;
            _sources = sources.ToArray();
        }

        #region ISoundFrame

        public int SampleRate { get; private set; }

        public void Refresh()
        {
            _buffer = null;
        }

        public uint[] GetBuffer()
        {
            if (_buffer != null)
            {
                return _buffer;
            }
            _buffer = _mixBuffer;
            Mix(_buffer, _sources);
            return _buffer;
        }

        #endregion ISoundFrame


        #region Private

        private unsafe static void Mix(uint[] dst, uint[][] sources)
        {
            fixed (uint* puidst = dst)
            {
                var pdst = (short*)puidst;
                for (var i = 0; i < dst.Length; i++)
                {
                    var index = i * 2;
                    var left = 0;
                    var right = 0;
                    foreach (var src in sources)
                    {
                        fixed (uint* puisrc = src)
                        {
                            var psrc = (short*)puisrc;
                            left += psrc[index];
                            right += psrc[index + 1];
                        }
                    }
                    if (sources.Length > 1)
                    {
                        left /= sources.Length;
                        right /= sources.Length;
                    }
                    pdst[index] = (short)left;
                    pdst[index + 1] = (short)right;
                }
            }
        }

        #endregion Private
    }
}
