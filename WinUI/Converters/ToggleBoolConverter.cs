using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Servers;
using System;

namespace WinUI.Converters;

internal class ToggleBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not bool val) return false;

        return !val;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}