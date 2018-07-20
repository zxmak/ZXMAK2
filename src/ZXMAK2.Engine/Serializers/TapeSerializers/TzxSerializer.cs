using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Model.Tape.Entities;
using ZXMAK2.Model.Tape.Interfaces;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Serializers.TapeSerializers
{
    public class TzxSerializer : FormatSerializer
    {
        private ITapeDevice _tape;


        public TzxSerializer(ITapeDevice tape)
        {
            _tape = tape;
        }


        #region FormatSerializer

        public override string FormatGroup { get { return "Tape images"; } }
        public override string FormatName { get { return "TZX image"; } }
        public override string FormatExtension { get { return "TZX"; } }

        public override bool CanDeserialize { get { return true; } }

        public override void Deserialize(Stream stream)
        {
            _tape.Blocks.Clear();
            var blocks = Load(stream, _tape.TactsPerSecond);
            if (blocks != null)
            {
                _tape.Blocks.AddRange(blocks);
            }
            _tape.Reset();
        }

        private static IEnumerable<ITapeBlock> Load(Stream stream, int frequency)
        {
            byte[] snbuf = new byte[stream.Length];
            stream.Read(snbuf, 0, snbuf.Length);

            if (Encoding.ASCII.GetString(snbuf, 0, 7) != "ZXTape!" || snbuf[7] != 0x1A)
            {
                Locator.Resolve<IUserMessage>()
                    .Error("TZX loader\n\nInvalid TZX file, identifier not found!");
                return null;
            }
            //int verMajor = snbuf[0x08];
            //int verMinor = snbuf[0x09];


            int ptr = 0;
            int size, pause, i, n, t, t0, j;
            int pl, p, last;//, *end; char *p;
            //int loop_n = 0, loop_p = 0;
            //byte[] nm = new byte[512];

            //TapeBlock tb = null;
            //bool grouping = false;

            //int newOffset;

            List<TapeBlock> tzxBlocks = new List<TapeBlock>();
            Stack<KeyValuePair<int, int>> loopStack = new Stack<KeyValuePair<int, int>>();
            
            while (ptr < snbuf.Length)
            {
                int blockId = snbuf[ptr++];

                TapeBlock tb = new TapeBlock();
                tb.TzxId = blockId;

                switch (blockId)
                {
                    #region ID 10 - Standard Speed Data Block
                    case 0x10: // Standard Speed Data Block
                        size = getUInt16(snbuf, ptr + 2);
                        pause = getUInt16(snbuf, ptr);
                        ptr += 4;
                        tb.Description = TapSerializer.getBlockDescription(snbuf, ptr, size);
                        tb.Periods = TapSerializer.getBlockPeriods(snbuf, ptr, size, 2168, 667, 735, 855, 1710, (snbuf[ptr] < 4) ? 8064 : 3220, pause, 8);
                        tb.TapData = TapSerializer.getBlockTapData(snbuf, ptr, size);
                        ptr += size;
                        break;
                    #endregion
                    
                    #region ID 11 - Turbo Speed Data Block
                    case 0x11: // Turbo Speed Data Block
                        size = 0xFFFFFF & getInt32(snbuf, ptr + 0x0F);
                        tb.Description = TapSerializer.getBlockDescription(snbuf, ptr + 0x12, size);
                        tb.Periods = TapSerializer.getBlockPeriods(snbuf, ptr + 0x12, size,
                           getUInt16(snbuf, ptr), getUInt16(snbuf, ptr + 2),
                           getUInt16(snbuf, ptr + 4), getUInt16(snbuf, ptr + 6),
                           getUInt16(snbuf, ptr + 8), getUInt16(snbuf, ptr + 10),
                           getUInt16(snbuf, ptr + 13), snbuf[ptr + 12]);
                        //tb.TapData = TapSerializer.getBlockTapData(snbuf, ptr+0x12, size);
                        // todo: test used bits - ptr+12
                        ptr += size + 0x12;
                        break;
                    #endregion

                    #region ID 12 - Pure Tone
                    case 0x12: // Pure Tone
                        tb.Description = "Pure Tone";
                        pl = getUInt16(snbuf, ptr);
                        n = getUInt16(snbuf, ptr + 2);
                        tb.Periods = new List<int>(n);
                        for (i = 0; i < n; i++)
                            tb.Periods.Add(pl);
                        ptr += 4;
                        break;
                    #endregion
                    
                    #region ID 13 - Pulse sequence
                    case 0x13: // Pulse sequence
                        tb.Description = "Pulse sequence";
                        n = snbuf[ptr++];
                        tb.Periods = new List<int>(n);
                        for (i = 0; i < n; i++, ptr += 2)
                            tb.Periods.Add(getUInt16(snbuf, ptr));
                        break;
                    #endregion
                    
                    #region ID 14 - Pure Data Block
                    case 0x14: // Pure Data Block
                        tb.Description = "Pure Data Block";
                        size = 0xFFFFFF & getInt32(snbuf, ptr + 7);
                        tb.Periods = TapSerializer.getBlockPeriods(snbuf, ptr + 0x0A, size,
                           0, 0, 0, getUInt16(snbuf, ptr),
                           getUInt16(snbuf, ptr + 2),
                           -1,
                           getUInt16(snbuf, ptr + 5), snbuf[ptr + 4]);
                        tb.TapData = TapSerializer.getBlockTapData(snbuf, ptr + 0x0A, size);
                        ptr += size + 0x0A;
                        break;
                    #endregion
                    
                    #region ID 15 - Direct Recording
                    case 0x15: // Direct Recording
                        tb.Description = "Direct Recording";
                        size = 0xFFFFFF & getInt32(snbuf, ptr + 5);
                        t0 = getUInt16(snbuf, ptr);
                        pause = getUInt16(snbuf, ptr + 2);
                        last = snbuf[ptr + 4];
                        ptr += 8;
                        pl = 0; n = 0;
                        for (i = 0; i < size; i++) // count number of pulses
                            for (j = 0x80; j != 0; j >>= 1)
                                if (((snbuf[ptr + i] ^ pl) & j) != 0)
                                {
                                    n++;
                                    pl ^= -1;
                                }
                        t = 0; pl = 0;
                        tb.Periods = new List<int>(n + 2);
                        for (i = 1; i < size; i++, ptr++) // find pulses
                            for (j = 0x80; j != 0; j >>= 1)
                            {
                                t += t0;
                                if (((snbuf[ptr] ^ pl) & j) != 0)
                                {
                                    tb.Periods.Add(t);
                                    pl ^= -1;
                                    t = 0;
                                }
                            }
                        // find pulses - last byte
                        for (j = 0x80; j != (byte)(0x80 >> last); j >>= 1)
                        {
                            t += t0;
                            if (((snbuf[ptr] ^ pl) & j) != 0)
                            {
                                tb.Periods.Add(t);
                                pl ^= -1;
                                t = 0;
                            }
                        }
                        ptr++;
                        tb.Periods.Add(t); // last pulse ???
                        if (pause != 0)
                            tb.Periods.Add(pause * 3500);
                        break;
                    #endregion

                    //#region ID 18
                    //case 0x18:
                    //    tb.Description = "CSW Recording";
                    //    break;
                    //#endregion

                    #region ID 19 - Generalized Data Block
                    case 0x19:
                        ptr += ReadGeneralizedDataBlock(snbuf, ptr, ref tb);
                        break;
                    #endregion

                    #region ID 20 - Pause (silence) or 'Stop the Tape' command
                    case 0x20: // Pause (silence) or 'Stop the Tape' command
                        pause = getUInt16(snbuf, ptr);
                        tb.Description = (pause != 0 ? "[Pause " + pause + " ms]" : "[Stop the Tape]");
                        tb.Periods = new List<int>(2);
                        ptr += 2;
                        if (pause == 0) // at least 1ms pulse as specified in TZX 1.13
                        {
                            tb.Command = TapeCommand.STOP_THE_TAPE;
                            tb.Periods.Add(3500);
                            pause = -1;
                        }
                        else pause *= 3500;
                        tb.Periods.Add(pause);
                        break;
                    #endregion

                    #region ID 21 - Group start
                    case 0x21: // Group start
                        n = snbuf[ptr++];
                        tb.Description = "[GROUP: " + Encoding.ASCII.GetString(snbuf, ptr, n) + "]";
                        tb.Command = TapeCommand.BEGIN_GROUP;
                        ptr += n;
                        tb.Periods = new List<int>();
                        //grouping = true;
                        break;
                    #endregion
                    
                    #region ID 22 - Group end
                    case 0x22: // Group end
                        tb.Description = "[END GROUP]";
                        tb.Command = TapeCommand.END_GROUP;
                        tb.Periods = new List<int>();
                        //grouping = false;
                        break;
                    #endregion

                    #region ID 23 - Jump to block
                    case 0x23: // Jump to block
                        tb.Description = "[JUMP TO BLOCK " + getUInt16(snbuf, ptr) + "]";  // 1=next block, 2=skip 1 block, -1=prev block
                        tb.Periods = new List<int>();
                        ptr += 2;
                        break;
                    #endregion

                    #region ID 24 - Loop start
                    case 0x24: // Loop start
                        {
                            int loop_p = tzxBlocks.Count + 1; // from next
                            int loop_n = getUInt16(snbuf, ptr);  // TODO: as Command
                            loopStack.Push(new KeyValuePair<int, int>(loop_p, loop_n));
                            ptr += 2;
                            tb.Description = string.Format("[LOOP {0}]", loop_n);
                            tb.Periods = new List<int>();
                            break;
                        }
                    #endregion
                    
                    #region ID 25 - Loop end
                    case 0x25: // Loop end
                        {
                            tb.Description = string.Format("[END LOOP]");
                            tb.Periods = new List<int>();

                            KeyValuePair<int, int> loopInfo = loopStack.Pop();
                            int loop_p = loopInfo.Key;
                            int loop_n = loopInfo.Value;

                            if (loop_n == 0)
                                break;  // loop_n is repeat count, =1 for mem economy
                            size = tzxBlocks.Count - loop_p;
                            for (i = 0; i < loop_n; i++)
                            {
                                TapeBlock repeatBlock = new TapeBlock();
                                repeatBlock.Description = string.Format("[LOOP REPEAT {0}]", i+1);
                                repeatBlock.Periods = new List<int>();
                                tzxBlocks.Add(repeatBlock);
                                for (int z = 0; z < size; z++)
                                {
                                    tzxBlocks.Add(tzxBlocks[loop_p + z]);
                                }
                            }
                            break;
                        }
                    #endregion

                    #region ID 26 - Call sequence
                    case 0x26: // Call sequence
                        tb.Description = "[CALL SEQUENCE]";
                        tb.Periods = new List<int>();
                        ptr += 2 + 2 * getUInt16(snbuf, ptr);
                        break;
                    #endregion
                    
                    #region ID 27 - Return from sequence
                    case 0x27: // Return from sequence
                        tb.Description = "[RETURN SEQUENCE]";
                        tb.Periods = new List<int>();
                        break;
                    #endregion

                    #region ID 28 - Select block
                    case 0x28: // Select block
                        tb.Description = "[SELECT BLOCK]";
                        tb.Periods = new List<int>();
                        ptr += 2 + getUInt16(snbuf, ptr);
                        break;
                    #endregion
                    
                    #region ID 2A - Stop tape if in 48K mode
                    case 0x2A: // Stop tape if in 48K mode
                        tb.Description = "[Stop tape if in 48K mode]";
                        tb.Command = TapeCommand.STOP_THE_TAPE_48K;
                        tb.Periods = new List<int>();
                        ptr += 4 + getUInt16(snbuf, ptr);
                        break;
                    #endregion


                    #region ID 30 - Text description
                    case 0x30: // Text description
                        n = snbuf[ptr++];
                        tb.Description = "[" + Encoding.ASCII.GetString(snbuf, ptr, n) + "]";
                        tb.Periods = new List<int>();
                        ptr += n;
                        break;
                    #endregion
                    
                    #region ID 31 - Message block
                    case 0x31: // Message block
                        ptr++;//n = snbuf[ptr++]; // msg display time
                        n = snbuf[ptr++];
                        tb.Description = "[Message: " + Encoding.ASCII.GetString(snbuf, ptr, n) + "]";
                        tb.Command = TapeCommand.SHOW_MESSAGE;
                        tb.Periods = new List<int>();
                        ptr += n;
                        break;
                    #endregion
                    
                    #region ID 32 - Archive info+
                    case 0x32: // Archive info
                        {
                            tb.Description = "Archive info";
                            tb.Periods = new List<int>();
                            p = ptr + 3;
                            StringBuilder builderInfo = new StringBuilder();
                            for (i = 0; i < snbuf[ptr + 2]; i++)
                            {
                                string info;
                                int infoType = snbuf[p++];
                                switch (infoType)
                                {
                                    case 0: info = "Full title"; break;
                                    case 1: info = "Publisher"; break;
                                    case 2: info = "Author"; break;
                                    case 3: info = "Year"; break;
                                    case 4: info = "Language"; break;
                                    case 5: info = "Type"; break;
                                    case 6: info = "Price"; break;
                                    case 7: info = "Protection"; break;
                                    case 8: info = "Origin"; break;
                                    case 0xFF: info = "Comment"; break;
                                    default: info = "info"; break;
                                }
                                size = snbuf[p++];
                                string infoValue = Encoding.ASCII.GetString(snbuf, p, size);
                                p += size;

                                if (infoType == 0 || infoType == 7 || infoType == 0xFF)
                                {
                                    if (builderInfo.Length > 0 && infoValue.Length > 0) // insert separator
                                        builderInfo.Append("; ");
                                    builderInfo.Append(string.Format("{0}: {1}", info, infoValue));
                                }
                            }
                            tb.Description = builderInfo.ToString();
                            ptr += 2 + getUInt16(snbuf, ptr);
                            break;
                        }
                    #endregion
                    
                    #region ID 33 - Hardware type
                    case 0x33: // Hardware type
                        n = snbuf[ptr++];    // hwinfo count
                        tb.Description = "[HARDWARE TYPE]";
                        tb.Periods = new List<int>();
                        ptr += 3 * n;
                        break;
                    #endregion
                    
                    #region ID 34 - Emulation info
                    case 0x34: // Emulation info
                        tb.Description = "[EMULATION INFO]";
                        tb.Periods = new List<int>();
                        ptr += 8;
                        break;
                    #endregion
                    
                    #region ID 35 - Custom info block+
                    case 0x35: // Custom info block
                        {
                            string infoValue = Encoding.ASCII.GetString(snbuf, ptr, 0x10);
                            tb.Description = string.Format("[CUSTOM INFO - {0}]", infoValue);
                            ptr += 0x10;
                            tb.Periods = new List<int>();
                            int customLength = BitConverter.ToInt32(snbuf, ptr);
                            ptr += 4 + customLength;
                            break;
                        }
                    #endregion
                    
                    #region ID 40 - Snapshot block
                    case 0x40: // Snapshot block
                        tb.Description = "[SNAPSHOT - ";
                        if (snbuf[ptr] == 0)
                            tb.Description += ".Z80]";
                        else if (snbuf[ptr] == 1)
                            tb.Description += ".SNA]";
                        else
                            tb.Description += "???]";
                        ptr++;
                        size = snbuf[ptr] | snbuf[ptr + 1] << 8 | snbuf[ptr + 2] << 16;
                        ptr += 3;
                        tb.Periods = new List<int>();
                        ptr += size;
                        break;
                    #endregion

                    #region ID 5A - "Glue" block+
                    case 0x5A: // "Glue" block
                        tb.Description = string.Format("[GLUE]");
                        tb.Periods = new List<int>();
                        ptr += 9;
                        break;
                    #endregion

                    default:
                        tb.Description = string.Format(
                            "[NOT SUPPORTED BLOCK 0x{0:X2}]",
                            snbuf[ptr - 1]);
                        tb.Periods = new List<int>();
                        ptr += getInt32(snbuf, ptr) & 0xFFFFFF;
                        ptr += 4;
                        break;
                }
                tzxBlocks.Add(tb);
            }
            var ltb = tzxBlocks.Count > 0 ?
                tzxBlocks[tzxBlocks.Count-1] :
                null;
            if (ltb != null)
            {
                ltb.Periods.Add(frequency / 220);
                ltb.Periods.Add(frequency / 220);
            }
            return tzxBlocks;
        }

        private static int ReadGeneralizedDataBlock(byte[] buf, int index, ref TapeBlock tb)
        {
            tb.Description = "CSW Recording";
            int size = getInt32(buf, index); index += 4;
            int pause = getUInt16(buf, index); index += 2;
            int totp = getInt32(buf, index); index += 4;
            int npp = buf[index]; index+=1;
            int asp = buf[index]; index += 1;
            int totd = getInt32(buf, index); index += 4;
            int npd = buf[index]; index += 1;
            int asd = buf[index]; index += 1;


            return size;
        }

        #endregion

    }
}
