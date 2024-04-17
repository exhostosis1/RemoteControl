using Microsoft.UI.Xaml.Data;
using System;

namespace WinUI.Converters;

internal class UriToStringConverter: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not Uri uri) return "";

        return uri.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is not string uriString) return new Uri("");

        return new Uri(uriString);
    }
}