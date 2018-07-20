using System;
using System.Linq;
using System.Globalization;


namespace ZXMAK2.Mvvm.BindingTools
{
    public class FuncConverter : IValueConverter
    {
        public Func<object, object> Function { get; set; }


        public object Convert(
            object value, 
            Type targetType, 
            object parameter, 
            CultureInfo culture)
        {
            return Function(value);
        }

        public object ConvertBack(
            object value, 
            Type targetType, 
            object parameter, 
            CultureInfo culture)
        {
            return BindingInfo.DoNothing;
        }
    }
}
