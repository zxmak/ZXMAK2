using System;
using System.IO;
using System.Text;

using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Engine.Tools;


namespace ZXMAK2.Serializers.SnapshotSerializers
{
    public class SzxSerializer : SnapshotSerializerBase
    {
        public SzxSerializer(Spectrum spec)
            : base(spec)
        {
        }

        #region FormatSerializer

        public override string FormatExtension { get { return "SZX"; } }

        public override bool CanDeserialize { get { return true; } }
        public override bool CanSerialize { get { return true; } }

        public override void Deserialize(Stream stream)
        {
            loadFromStream(stream);
            UpdateState();
        }

        public override void Serialize(Stream stream)
        {
            saveToStream(stream);
        }

        #endregion

        #region Load

        private void loadFromStream(Stream stream)
        {
            ZXST_Header header = new ZXST_Header();
            header.Deserialize(stream);

            if (header.MajorVersion != 1)
                throw new NotSupportedException(string.Format("Sorry, but MajorVersion={0} is not supported", header.MajorVersion));

            //int num = 0;

            InitStd128K();
            bool eof = false;
            do
            {
                byte[] data = new byte[8];
                eof |= stream.Read(data, 0, data.Length) != data.Length;

                UInt32 id = BitConverter.ToUInt32(data, 0);
                UInt32 size = BitConverter.ToUInt32(data, 4);
                data = new byte[size];
                eof |= stream.Read(data, 0, data.Length) != data.Length;

                if (!eof)
                {
                    string strId = Encoding.ASCII.GetString(BitConverter.GetBytes(id));
                    //using (FileStream fs = new FileStream((num++).ToString("D2")+"_" + strId.Replace('\0', '_') + ".block", FileMode.Create, FileAccess.Write, FileShare.Read))
                    //    fs.Write(data, 0, data.Length);

                    switch (strId)
                    {
                        case "Z80R":
                            apply_Z80R(data);
                            break;
                        case "SPCR":
                            apply_SPCR(data, header);
                            break;
                        case "AY\0\0":
                            apply_AY(data);
                            break;
                        case "RAMP":
                            apply_RAMP(data);
                            break;
                        case "B128":
                            applyB128(data);
                            break;
                        //case "KEYB":
                        //    break;
                        default:
                            //LogAgent.Info("SzxSerializer: skip block '{0}' (unsupported)", strId);
                            break;
                    }
                }
            }
            while (!eof);
        }

        #region Z80R

        private const int ZXSTZF_EILAST = 1;
        private const int ZXSTZF_HALTED = 2;

        #region Comment
        /// <summary>
        /// ZXSTZ80REGS
        /// </summary>
        #endregion
        private void apply_Z80R(byte[] data)
        {
            int offset = 0;
            _spec.CPU.regs.AF = BitConverter.ToUInt16(data, offset); offset += 2;
            _spec.CPU.regs.BC = BitConverter.ToUInt16(data, offset); offset += 2;
            _spec.CPU.regs.DE = BitConverter.ToUInt16(data, offset); offset += 2;
            _spec.CPU.regs.HL = BitConverter.ToUInt16(data, offset); offset += 2;
            _spec.CPU.regs._AF = BitConverter.ToUInt16(data, offset); offset += 2;
            _spec.CPU.regs._BC = BitConverter.ToUInt16(data, offset); offset += 2;
            _spec.CPU.regs._DE = BitConverter.ToUInt16(data, offset); offset += 2;
            _spec.CPU.regs._HL = BitConverter.ToUInt16(data, offset); offset += 2;
            _spec.CPU.regs.IX = BitConverter.ToUInt16(data, offset); offset += 2;
            _spec.CPU.regs.IY = BitConverter.ToUInt16(data, offset); offset += 2;
            _spec.CPU.regs.SP = BitConverter.ToUInt16(data, offset); offset += 2;
            _spec.CPU.regs.PC = BitConverter.ToUInt16(data, offset); offset += 2;
            _spec.CPU.regs.I = data[offset]; offset += 1;
            _spec.CPU.regs.R = data[offset]; offset += 1;
            _spec.CPU.IFF1 = data[offset] != 0; offset += 1;
            _spec.CPU.IFF2 = data[offset] != 0; offset += 1;
            _spec.CPU.IM = data[offset]; offset += 1;
            SetFrameTact(BitConverter.ToInt32(data, offset)); offset += 4;
            byte holdIntReqCycles = data[offset]; offset += 1;	// interrupt active tact counter
            byte flags = data[offset]; offset += 1;
            _spec.CPU.regs.MW = BitConverter.ToUInt16(data, offset); offset += 1;	   // appears in v1.4!

            _spec.CPU.BINT = (flags & ZXSTZF_EILAST) != 0;
            _spec.CPU.HALTED = (flags & ZXSTZF_HALTED) != 0;
        }

        #endregion

        #region SPCR

        #region Comment
        /// <summary>
        /// ZXSTSPECREGS - The Spectrum's ULA state specifying the current border colour, memory paging status etc.
        /// </summary>
        #endregion
        private void apply_SPCR(byte[] data, ZXST_Header header)
        {
            int offset = 0;
            byte chBorder = data[offset]; offset += 1;
            byte ch7ffd = data[offset]; offset += 1;
            byte ch1ffd = data[offset]; offset += 1;		   // #1ffd/#effd
            byte chFe = data[offset]; offset += 1;			   // Only bits 3 and 4 (the MIC and EAR bits) are guaranteed to be valid.
            //byte[] chReserved = new byte[4];
            //for (int i = 0; i < 4; i++)
            //    chReserved[i] = data[i + offset];

            byte pFE = (byte)((chFe & 0xF8) | (chBorder & 7));

            var ula = _spec.BusManager.FindDevice<IUlaDevice>();
            var memory = _spec.BusManager.FindDevice<IMemoryDevice>();
            ula.PortFE = pFE;
            switch (header.MachineId)
            {
                case SzxModelId.ZXSTMID_16K:
                case SzxModelId.ZXSTMID_48K:
                    memory.CMR0 = 0x30;
                    memory.CMR1 = 0;
                    break;
                default:
                    memory.CMR0 = ch7ffd;
                    memory.CMR1 = ch1ffd;
                    break;
            }
        }

        #endregion

        #region RAMP

        private const int ZXSTRF_COMPRESSED = 1;

        #region Comment
        /// <summary>
        /// ZXSTRAMPAGE - 16KB RAM page blocks, depending on the specific Spectrum model.
        /// </summary>
        #endregion
        private void apply_RAMP(byte[] data)
        {
            int offset = 0;
            UInt16 wFlags = BitConverter.ToUInt16(data, offset); offset += 2;
            byte chPageNo = data[offset]; offset += 1;

            var memory = _spec.BusManager.FindDevice<IMemoryDevice>();
            if (chPageNo >= memory.RamPages.Length)
            {
                Logger.Warn(
                    "SzxSerializer: skip block 'RAMP' chPageNo={0} (incompatible)",
                    chPageNo);
                return;
            }
            byte[] chData = new byte[data.Length - offset];
            for (int i = 0; i < chData.Length; i++)
            {
                chData[i] = data[i + offset];
            }

            if ((wFlags & ZXSTRF_COMPRESSED) != 0)
            {
                byte[] dataDecompressed = new byte[0x4000];
                using (MemoryStream ms = new MemoryStream(chData))
                {
                    ZipLib.Zip.Compression.Streams.InflaterInputStream inflater = new ZipLib.Zip.Compression.Streams.InflaterInputStream(ms);
                    inflater.Read(dataDecompressed, 0, dataDecompressed.Length);
                }
                chData = dataDecompressed;
            }

            //using (FileStream fs = new FileStream("_RAMP" + chPageNo.ToString("D2") + ".bin", FileMode.Create, FileAccess.Write, FileShare.Read))
            //    fs.Write(chData, 0, chData.Length);

            for (int i = 0; i < 0x4000; i++)
            {
                memory.RamPages[chPageNo][i] = chData[i];
            }
        }

        #endregion

        #region AY

        private const int ZXSTAYF_FULLERBOX = 1;
        private const int ZXSTAYF_128AY = 2;

        #region Comment
        /// <summary>
        /// ZXSTAYBLOCK - The state of the AY chip
        /// </summary>
        #endregion
        private void apply_AY(byte[] data)
        {
            int offset = 0;
            byte chFlags = data[offset]; offset += 1;
            byte chCurrentRegister = data[offset]; offset += 1;

            var aydev = _spec.BusManager.FindDevice<IPsgDevice>();
            if (aydev == null)
            {
                return;
            }
            for (var i = 0; i < 16; i++)
            {
                aydev.SetReg(i, data[i + offset]);
            }
            aydev.RegAddr = chCurrentRegister;
        }

        #endregion

        #region B128

        private const int ZXSTBETAF_CONNECTED = 1;
        private const int ZXSTBETAF_CUSTOMROM = 2;
        private const int ZXSTBETAF_PAGED = 4;
        private const int ZXSTBETAF_AUTOBOOT = 8;
        private const int ZXSTBETAF_SEEKLOWER = 16;
        private const int ZXSTBETAF_COMPRESSED = 32;

        #region Comment
        /// <summary>
        /// ZXSTBETA128 - Beta 128 disk interface from Technology Research UK Ltd.
        /// </summary>
        #endregion
        private void applyB128(byte[] data)
        {
            var betaDisk = _spec.BusManager.FindDevice<IBetaDiskDevice>();
            if (betaDisk != null)
            {
                int offset = 0;
                int dwFlags = BitConverter.ToInt32(data, offset); offset += 4;
                byte chNumDrives = data[offset]; offset += 1;
                byte chSysReg = data[offset]; offset += 1;
                byte chTrackReg = data[offset]; offset += 1;
                byte chSectorReg = data[offset]; offset += 1;
                byte chDataReg = data[offset]; offset += 1;
                byte chStatusReg = data[offset]; offset += 1;
                byte[] chRomData = new byte[data.Length - offset];
                for (int i = 0; i < chRomData.Length; i++)
                    chRomData[i] = data[i + offset];

                if (chRomData.Length > 0 && (dwFlags & ZXSTBETAF_COMPRESSED) != 0)
                {
                    byte[] dataDecompressed = new byte[0x4000];
                    using (MemoryStream ms = new MemoryStream(chRomData))
                    {
                        ZipLib.Zip.Compression.Streams.InflaterInputStream inflater = new ZipLib.Zip.Compression.Streams.InflaterInputStream(ms);
                        inflater.Read(dataDecompressed, 0, dataDecompressed.Length);
                    }
                    chRomData = dataDecompressed;
                }

                betaDisk.DOSEN = (dwFlags & ZXSTBETAF_PAGED) != 0;
            }
        }

        #endregion

        #endregion

        #region Save

        private void saveToStream(Stream stream)
        {
            var header = new ZXST_Header();

            // very ugly, because there is no option for custom configuration machine
            var memory = _spec.BusManager.FindDevice<IMemoryDevice>();

            header.MachineId = GetModelId(_spec.BusManager.ModelId);
            if (header.MachineId == SzxModelId.ZXSTMID_CUSTOM)
            {
                if (memory.RamPages.Length == 8)
                {
                    header.MachineId = SzxModelId.ZXSTMID_PENTAGON128;
                }
                else if (memory.IsMap48)
                {
                    header.MachineId = SzxModelId.ZXSTMID_48K;
                }
            }
            
            var ula = _spec.BusManager.FindDevice<IUlaDevice>();
            if (ula.IsEarlyTimings)
            {
                header.Flags = (byte)(header.Flags & ~ZXSTMF_ALTERNATETIMINGS);
            }
            header.Serialize(stream);

            save_CRTR(stream);
            save_Z80R(stream);
            save_SPCR(stream, header);
            switch (header.MachineId)
            {
                case SzxModelId.ZXSTMID_16K:
                    save_RAMP(stream, 5, memory.RamPages[memory.Map48[1]]);
                    break;
                case SzxModelId.ZXSTMID_48K:
                    save_RAMP(stream, 5, memory.RamPages[memory.Map48[1]]);
                    save_RAMP(stream, 2, memory.RamPages[memory.Map48[2]]);
                    save_RAMP(stream, 0, memory.RamPages[memory.Map48[3]]);
                    break;
                default:
                    for (int i = 0; i < memory.RamPages.Length; i++)
                        save_RAMP(stream, (byte)i, memory.RamPages[i]);
                    break;
            }
            save_AY(stream);
            save_B128(stream);
        }

        private SzxModelId GetModelId(ModelId modelId)
        {
            switch (modelId)
            {
                case ModelId.Pentagon128: return SzxModelId.ZXSTMID_PENTAGON128;
                case ModelId.Pentagon512: return SzxModelId.ZXSTMID_PENTAGON512;
                case ModelId.Pentagon1024: return SzxModelId.ZXSTMID_PENTAGON1024;
                case ModelId.Sinclair48: return SzxModelId.ZXSTMID_48K;
                case ModelId.Sinclair128: return SzxModelId.ZXSTMID_128K;
                case ModelId.SinclairPlus3: return SzxModelId.ZXSTMID_PLUS3;
                case ModelId.Scorpion: return SzxModelId.ZXSTMID_SCORPION;
            }
            return SzxModelId.ZXSTMID_CUSTOM;
        }

        private void save_CRTR(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                byte[] szCreator = Encoding.ASCII.GetBytes("ZXMAK2");
                UInt16 chMajorVersion = 2;
                UInt16 chMinorVersion = 2;
                byte[] chData = new byte[0];

                ms.Write(szCreator, 0, szCreator.Length);
                for (int i = szCreator.Length; i < 32; i++)
                    ms.WriteByte(0);
                StreamHelper.Write(ms, chMajorVersion);
                StreamHelper.Write(ms, chMinorVersion);
                StreamHelper.Write(ms, chData);

                byte[] blockData = ms.ToArray();

                StreamHelper.Write(stream, Encoding.ASCII.GetBytes("CRTR"));
                int size = blockData.Length;
                StreamHelper.Write(stream, size);
                StreamHelper.Write(stream, blockData);
            }
        }

        private void save_Z80R(Stream stream)
        {
            StreamHelper.Write(stream, Encoding.ASCII.GetBytes("Z80R"));
            int size = 37;
            StreamHelper.Write(stream, size);

            StreamHelper.Write(stream, _spec.CPU.regs.AF);
            StreamHelper.Write(stream, _spec.CPU.regs.BC);
            StreamHelper.Write(stream, _spec.CPU.regs.DE);
            StreamHelper.Write(stream, _spec.CPU.regs.HL);
            StreamHelper.Write(stream, _spec.CPU.regs._AF);
            StreamHelper.Write(stream, _spec.CPU.regs._BC);
            StreamHelper.Write(stream, _spec.CPU.regs._DE);
            StreamHelper.Write(stream, _spec.CPU.regs._HL);
            StreamHelper.Write(stream, _spec.CPU.regs.IX);
            StreamHelper.Write(stream, _spec.CPU.regs.IY);
            StreamHelper.Write(stream, _spec.CPU.regs.SP);
            StreamHelper.Write(stream, _spec.CPU.regs.PC);
            StreamHelper.Write(stream, _spec.CPU.regs.I);
            StreamHelper.Write(stream, _spec.CPU.regs.R);
            StreamHelper.Write(stream, (byte)(_spec.CPU.IFF1 ? 1 : 0));
            StreamHelper.Write(stream, (byte)(_spec.CPU.IFF2 ? 1 : 0));
            StreamHelper.Write(stream, _spec.CPU.IM);
            StreamHelper.Write(stream, (int)base.GetFrameTact());
            byte holdIntReqCycles = 0;
            StreamHelper.Write(stream, holdIntReqCycles);
            byte flags = (byte)(
                (_spec.CPU.BINT ? ZXSTZF_EILAST : 0) |
                (_spec.CPU.HALTED ? ZXSTZF_HALTED : 0));
            StreamHelper.Write(stream, flags);
            StreamHelper.Write(stream, _spec.CPU.regs.MW);
        }

        private void save_SPCR(Stream stream, ZXST_Header header)
        {
            StreamHelper.Write(stream, Encoding.ASCII.GetBytes("SPCR"));
            int size = 8;
            StreamHelper.Write(stream, size);

            var ula = _spec.BusManager.FindDevice<IUlaDevice>();
            var memory = _spec.BusManager.FindDevice<IMemoryDevice>();

            byte cmr0 = memory.CMR0;
            byte cmr1 = memory.CMR1;
            switch (header.MachineId)
            {
                case SzxModelId.ZXSTMID_16K:
                case SzxModelId.ZXSTMID_48K:
                    cmr0 = 0x30;
                    cmr1 = 0x00;
                    break;
            }
            StreamHelper.Write(stream, (byte)(ula.PortFE & 7));				// chBorder
            StreamHelper.Write(stream, (byte)(cmr0));			            // ch7ffd
            StreamHelper.Write(stream, (byte)(cmr1));				        // ch1ffd  TODO: port 1ffd/effd
            StreamHelper.Write(stream, (byte)(ula.PortFE & 0xF8));			// chFe
            StreamHelper.Write(stream, new byte[4]);						// chReserved[4]
        }

        private void save_RAMP(Stream stream, byte chPageNo, byte[] chData)
        {
            var memory = _spec.BusManager.FindDevice<IMemoryDevice>();

            UInt16 wFlags = 0;

            using (var ms = new MemoryStream())
            {
                ZipLib.Zip.Compression.Streams.DeflaterOutputStream deflater = new ZipLib.Zip.Compression.Streams.DeflaterOutputStream(ms);
                deflater.Write(chData, 0, chData.Length);
                deflater.Finish();
                deflater.Flush();
                byte[] dataCompressed = ms.ToArray();
                if (dataCompressed.Length < chData.Length)
                {
                    chData = dataCompressed;
                    wFlags |= ZXSTRF_COMPRESSED;
                }
            }

            StreamHelper.Write(stream, Encoding.ASCII.GetBytes("RAMP"));
            int size = 3 + chData.Length;
            StreamHelper.Write(stream, size);

            StreamHelper.Write(stream, wFlags);
            StreamHelper.Write(stream, chPageNo);
            StreamHelper.Write(stream, chData);
        }

        private void save_AY(Stream stream)
        {
            var aydev = _spec.BusManager.FindDevice<IPsgDevice>();
            if (aydev == null)
            {
                return;
            }
            StreamHelper.Write(stream, Encoding.ASCII.GetBytes("AY\0\0"));
            var size = 18;
            StreamHelper.Write(stream, (int)size);
            var chFlags = 0;
            StreamHelper.Write(stream, (byte)chFlags);
            StreamHelper.Write(stream, (byte)aydev.RegAddr);
            for (var i = 0; i < 16; i++)
            {
                StreamHelper.Write(stream, aydev.GetReg(i));
            }
        }

        private void save_B128(Stream stream)
        {
            var betaDisk = _spec.BusManager.FindDevice<IBetaDiskDevice>();
            if (betaDisk != null)
            {
                int dwFlags =
                    (betaDisk.DOSEN ? ZXSTBETAF_PAGED : 0);

                byte chNumDrives = 0;
                byte chSysReg = 0x3f;
                byte chTrackReg = 0;
                byte chSectorReg = 0;
                byte chDataReg = 0;
                byte chStatusReg = 0xe0;
                byte[] chRomData = new byte[0];

                if (chRomData.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        ZipLib.Zip.Compression.Streams.DeflaterOutputStream deflater = new ZipLib.Zip.Compression.Streams.DeflaterOutputStream(ms);
                        deflater.Write(chRomData, 0, chRomData.Length);
                        deflater.Finish();
                        deflater.Flush();
                        byte[] dataCompressed = ms.ToArray();
                        if (dataCompressed.Length < chRomData.Length)
                        {
                            chRomData = dataCompressed;
                            dwFlags |= ZXSTBETAF_COMPRESSED;
                        }
                    }
                }

                StreamHelper.Write(stream, Encoding.ASCII.GetBytes("B128"));
                int size = 10;
                StreamHelper.Write(stream, size);

                StreamHelper.Write(stream, dwFlags);
                StreamHelper.Write(stream, chNumDrives);
                StreamHelper.Write(stream, chSysReg);
                StreamHelper.Write(stream, chTrackReg);
                StreamHelper.Write(stream, chSectorReg);
                StreamHelper.Write(stream, chDataReg);
                StreamHelper.Write(stream, chStatusReg);
                StreamHelper.Write(stream, chRomData);
            }
        }

        #endregion

        #region Struct

        private enum SzxModelId : byte
        {
            ZXSTMID_16K = 0,
            ZXSTMID_48K = 1,
            ZXSTMID_128K = 2,
            ZXSTMID_PLUS2 = 3,
            ZXSTMID_PLUS2A = 4,
            ZXSTMID_PLUS3 = 5,
            ZXSTMID_PLUS3E = 6,
            ZXSTMID_PENTAGON128 = 7,
            ZXSTMID_TC2048 = 8,
            ZXSTMID_TC2068 = 9,
            ZXSTMID_SCORPION = 10,
            ZXSTMID_SE = 11,
            ZXSTMID_TS2068 = 12,
            ZXSTMID_PENTAGON512 = 13,
            ZXSTMID_PENTAGON1024 = 14,
            ZXSTMID_NTSC48K = 15,
            ZXSTMID_128KE = 16,
            ZXSTMID_CUSTOM = 255,
        }

        private const int ZXSTMF_ALTERNATETIMINGS = 1;

        private class ZXST_Header
        {
            public uint Magic = 0x5453585A;	// ZXST
            public byte MajorVersion = 1;
            public byte MinorVersion = 4;
            public SzxModelId MachineId = SzxModelId.ZXSTMID_PENTAGON128;
            public byte Flags = ZXSTMF_ALTERNATETIMINGS;					  // use late model

            public void Serialize(Stream stream)
            {
                StreamHelper.Write(stream, Magic);
                StreamHelper.Write(stream, MajorVersion);
                StreamHelper.Write(stream, MinorVersion);
                StreamHelper.Write(stream, (byte)MachineId);
                StreamHelper.Write(stream, Flags);
            }

            public void Deserialize(Stream stream)
            {
                StreamHelper.Read(stream, out Magic);
                StreamHelper.Read(stream, out MajorVersion);
                StreamHelper.Read(stream, out MinorVersion);
                byte mId;
                StreamHelper.Read(stream, out mId);
                MachineId = (SzxModelId)mId;
                StreamHelper.Read(stream, out Flags);
            }
        }

        #endregion

        #region Links

        //http://stackoverflow.com/questions/70347/zlib-compatible-compression-streams
        //http://weblogs.asp.net/astopford/archive/2004/10/31/250092.aspx


        #endregion
    }
}
