using System;
using System.IO;


namespace ZXMAK2.Engine.Tools
{
    public static class StreamHelper
    {
        public static void Write(Stream stream, Int32 value)
        {
            var data = BitConverter.GetBytes(value);
            stream.Write(data, 0, data.Length);
        }

        public static void Write(Stream stream, UInt32 value)
        {
            var data = BitConverter.GetBytes(value);
            stream.Write(data, 0, data.Length);
        }

        public static void Write(Stream stream, Int16 value)
        {
            var data = BitConverter.GetBytes(value);
            stream.Write(data, 0, data.Length);
        }

        public static void Write(Stream stream, UInt16 value)
        {
            var data = BitConverter.GetBytes(value);
            stream.Write(data, 0, data.Length);
        }

        public static void Write(Stream stream, Byte value)
        {
            var data = new byte[1];
            data[0] = value;
            stream.Write(data, 0, data.Length);
        }

        public static void Write(Stream stream, SByte value)
        {
            var data = new byte[1];
            data[0] = (byte)value;
            stream.Write(data, 0, data.Length);
        }

        public static void Write(Stream stream, byte[] value)
        {
            stream.Write(value, 0, value.Length);
        }


        public static void Read(Stream stream, out Int32 value)
        {
            var data = new byte[4];
            stream.Read(data, 0, data.Length);
            //if (!BitConverter.IsLittleEndian)
            //    Array.Reverse(data);
            value = BitConverter.ToInt32(data, 0);
        }

        public static void Read(Stream stream, out UInt32 value)
        {
            var data = new byte[4];
            stream.Read(data, 0, data.Length);
            value = BitConverter.ToUInt32(data, 0);
        }

        public static void Read(Stream stream, out Int16 value)
        {
            var data = new byte[2];
            stream.Read(data, 0, data.Length);
            value = BitConverter.ToInt16(data, 0);
        }

        public static void Read(Stream stream, out UInt16 value)
        {
            var data = new byte[2];
            stream.Read(data, 0, data.Length);
            value = BitConverter.ToUInt16(data, 0);
        }

        public static void Read(Stream stream, out Byte value)
        {
            var data = new byte[1];
            stream.Read(data, 0, data.Length);
            value = data[0];
        }

        public static void Read(Stream stream, out SByte value)
        {
            var data = new byte[1];
            stream.Read(data, 0, data.Length);
            value = (sbyte)data[0];
        }

        public static void Read(Stream stream, byte[] value)
        {
            stream.Read(value, 0, value.Length);
        }
    }
}
