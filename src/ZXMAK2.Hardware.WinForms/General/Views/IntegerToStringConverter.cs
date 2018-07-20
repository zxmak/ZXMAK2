using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using ZXMAK2.Mvvm;

namespace ZXMAK2.Hardware.WinForms.General.Views
{
    public class IntegerToStringConverter : IValueConverter
    {
        public bool IsHex { get; set; }
        public int DigitCount { get; set; }
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var iValue = System.Convert.ChangeType(value, typeof(int));
            var format = IsHex ? string.Format("#{{0:X{0}}}", DigitCount) :
                string.Format("{{0:D{0}}}", DigitCount);
            return string.Format(culture, format, iValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sValue = (string)value;
            var radix = 10;
            if (sValue.StartsWith("#") ||
                sValue.StartsWith("$"))
            {
                radix = 16;
                sValue = sValue.Substring(1);
            }
            else if (sValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                radix = 16;
                sValue = sValue.Substring(2);
            }
            else if (sValue.EndsWith("h", StringComparison.OrdinalIgnoreCase))
            {
                radix = 16;
                sValue = sValue.Substring(0, sValue.Length - 1);
            }
            var iValue = System.Convert.ToInt32(sValue, radix);
            return System.Convert.ChangeType(iValue, targetType);
        }
    }
}
