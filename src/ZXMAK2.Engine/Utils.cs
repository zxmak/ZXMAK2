using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Globalization;


namespace ZXMAK2.Engine
{
    public static class Utils
    {
        #region Xml Helpers

        public static void SetXmlAttribute(XmlNode node, string name, Int32 value)
        {
            var strValue = string.Format(CultureInfo.InvariantCulture, "{0}", value);
            SetXmlAttribute(node, name, strValue);
        }

        public static void SetXmlAttribute(XmlNode node, string name, UInt32 value)
        {
            var strValue = string.Format(CultureInfo.InvariantCulture, "{0}", value);
            SetXmlAttribute(node, name, strValue);
        }

        public static void SetXmlAttribute(XmlNode node, string name, string value)
        {
            var attr = node.OwnerDocument.CreateAttribute(name);
            attr.Value = value;
            node.Attributes.Append(attr);
        }

        public static void SetXmlAttribute(XmlNode node, string name, bool value)
        {
            var strValue = string.Format(CultureInfo.InvariantCulture, "{0}", value);
            SetXmlAttribute(node, name, strValue);
        }

        public static void SetXmlAttributeAsEnum<TEnum>(XmlNode node, string name, TEnum value)
            where TEnum : struct
        {
            var strValue = string.Format(CultureInfo.InvariantCulture, "{0}", value);
            SetXmlAttribute(node, name, strValue);
        }

        public static Int32 GetXmlAttributeAsInt32(XmlNode node, string name, int defValue)
        {
            var strValue = GetXmlAttributeAsString(node, name, null);
            if (strValue == null)
            {
                return defValue;
            }
            var value = TryParseSpectrumInt32(strValue);
            if (value.HasValue)
            {
                return value.Value;
            }            
            Logger.Warn("Unknown int value: {0}", strValue);
            return defValue;
        }

        public static uint GetXmlAttributeAsUInt32(XmlNode node, string name, uint defValue)
        {
            var strValue = GetXmlAttributeAsString(node, name, null);
            if (strValue == null)
            {
                return defValue;
            }
            var value = TryParseSpectrumUInt32(strValue);
            if (value.HasValue)
            {
                return value.Value;
            }            
            Logger.Warn("Unknown uint value: {0}", strValue);
            return defValue;
        }

        public static bool GetXmlAttributeAsBool(XmlNode node, string name, bool defValue)
        {
            var strValue = GetXmlAttributeAsString(node, name, null);
            if (strValue == null)
            {
                return defValue;
            }
            bool value;
            if (bool.TryParse(strValue, out value))
            {
                return value;
            }
            Logger.Warn("Unknown bool value: {0}", strValue);
            return defValue;
        }

        public static string GetXmlAttributeAsString(XmlNode itemNode, string name, string defValue)
        {
            var attr = itemNode.Attributes[name];
            if (attr == null)
            {
                return defValue;
            }
            return attr.InnerText;
        }

        public static TEnum GetXmlAttributeAsEnum<TEnum>(XmlNode node, string name, TEnum defValue)
            where TEnum : struct
        {
            var strValue = GetXmlAttributeAsString(node, name, null);
            if (strValue == null)
            {
                return defValue;
            }
            TEnum value;
            if (Enum.TryParse<TEnum>(strValue, true, out value))
            {
                return value;
            }
            Logger.Warn("Unknown enum {0} value: {1}", typeof(TEnum).FullName, strValue);
            return defValue;
        }

        #endregion

        private static int? TryParseSpectrumInt32(string value)
        {
            value = value.Trim().ToLowerInvariant();
            if (value.StartsWith("#"))
            {
                return TryParseInt32(value.Substring(1), 16);
            }
            else if (value.StartsWith("0x"))
            {
                return TryParseInt32(value.Substring(2), 16);
            }
            else if (value.StartsWith("%"))
            {
                return TryParseInt32(value.Substring(1), 2);
            }
            return TryParseInt32(value, 10);
        }

        private static uint? TryParseSpectrumUInt32(string value)
        {
            value = value.Trim().ToLowerInvariant();
            if (value.StartsWith("#"))
            {
                return TryParseUInt32(value.Substring(1), 16);
            }
            else if (value.StartsWith("0x"))
            {
                return TryParseUInt32(value.Substring(2), 16);
            }
            else if (value.StartsWith("%"))
            {
                return TryParseUInt32(value.Substring(1), 2);
            }
            return TryParseUInt32(value, 10);
        }

        private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static int? TryParseInt32(string text, int radix)
        {
            if (radix < 2 || radix > Alphabet.Length)
            {
                throw new ArgumentOutOfRangeException("radix");
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            text = text.Trim().ToUpperInvariant();

            var pos = 0;
            var result = 0L;
            var sign = 1L;
            if (text.StartsWith("-"))
            {
                sign = -1L;
                text = text.Substring(1);
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            // Use lookup table to parse string
            while (pos < text.Length && !Char.IsWhiteSpace(text[pos]))
            {
                var digit = text.Substring(pos, 1);
                var i = Alphabet.IndexOf(digit);
                if (i < 0 || i >= radix)
                {
                    return null;
                }
                result *= radix;
                result += i;
                pos++;
                if ((sign > 0 && result > (long)int.MaxValue) ||
                    (sign < 0 && result > -(long)int.MinValue))
                {
                    return null;
                }
            }
            // Return true if any characters processed
            if (pos < 1)
            {
                return null;
            }
            return (int)(result * sign);
        }

        public static uint? TryParseUInt32(string text, int radix)
        {
            if (radix < 2 || radix > Alphabet.Length)
            {
                throw new ArgumentOutOfRangeException("radix");
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            text = text.Trim().ToUpperInvariant();

            var pos = 0;
            var result = 0L;

            // Use lookup table to parse string
            while (pos < text.Length && !Char.IsWhiteSpace(text[pos]))
            {
                var digit = text.Substring(pos, 1);
                var i = Alphabet.IndexOf(digit);
                if (i < 0 || i >= radix)
                {
                    return null;
                }
                result *= radix;
                result += i;
                pos++;
                if (result > (long)uint.MaxValue)
                {
                    return null;
                }
            }
            // Return true if any characters processed
            if (pos < 1)
            {
                return null;
            }
            return (uint)result;
        }



        public static int ParseSpectrumInt(string strValue)
        {
            strValue = strValue.Trim().ToLower();
            int value;
            if ((strValue.Length > 0) && (strValue[0] == '#'))
            {
                strValue = strValue.Remove(0, 1);
                value = Convert.ToInt32(strValue, 16);
            }
            else if ((strValue.Length > 1) && ((strValue[1] == 'x') && (strValue[0] == '0')))
            {
                strValue = strValue.Remove(0, 2);
                value = Convert.ToInt32(strValue, 16);
            }
            else
            {
                value = Convert.ToInt32(strValue, 10);
            }
            return value;
        }

        public static String GetAppDataFolder()
        {
            var appName = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
            var appFolder = Path.GetDirectoryName(appName);
            return appFolder;
        }

        public static string GetAppFolder()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
