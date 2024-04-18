using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Data;

namespace WinUI.Converters;

internal class ListToStringConverter: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is not List<string> list ? "" : string.Join(",", list);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value is not string s ? Enumerable.Empty<string>() : s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }
}