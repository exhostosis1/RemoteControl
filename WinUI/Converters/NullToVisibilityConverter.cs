using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using MainApp.Servers;

namespace WinUI.Converters;

internal class NullToVisibilityConverter: IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
    {
        if (parameter == null)
            return value == null ? Visibility.Collapsed : Visibility.Visible;

        if (value is not IServer server || parameter is not string s) return Visibility.Collapsed;

        return server.Config.Type.ToString() == s ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}