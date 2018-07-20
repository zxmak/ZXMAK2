using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ZXMAK2.Mvvm;
using ZXMAK2.Mvvm.BindingTools;

namespace ZXMAK2.Hardware.WinForms.General.Views
{
    public static class Converters
    {
        public static readonly IValueConverter RegPairToString = 
            new IntegerToStringConverter() { IsHex = true, DigitCount = 4, };


        public static readonly IValueConverter BoolToStatusBackColor =
            new FuncConverter() { Function = BoolToStatusBackColor_OnConvert, };

        public static readonly IValueConverter BoolToStatusForeColor =
            new FuncConverter() { Function = BoolToStatusForeColor_OnConvert, };

        
        private static object BoolToStatusBackColor_OnConvert(object isRunning)
        {
            return (bool)isRunning ? ColorTranslator.FromHtml("#cc6600") :
                ColorTranslator.FromHtml("#0077cc");
        }

        private static object BoolToStatusForeColor_OnConvert(object isRunning)
        {
            return (bool)isRunning ? ColorTranslator.FromHtml("#ffffff") :
                ColorTranslator.FromHtml("#ffffff");
        }
    }
}
