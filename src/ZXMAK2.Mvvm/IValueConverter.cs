using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace ZXMAK2.Mvvm
{
    public interface IValueConverter
    {
        object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}
