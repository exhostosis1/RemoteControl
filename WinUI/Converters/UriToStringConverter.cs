using Microsoft.UI.Xaml.Data;
using System;

namespace WinUI.Converters;

internal class UriToStringConverter: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not Uri uri) throw new ArgumentException("Value must be of Uri type");

        return uri.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is not string uriString) throw new ArgumentException("Value must be string");

        return new Uri(uriString);
    }
}