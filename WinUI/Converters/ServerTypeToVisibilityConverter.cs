using MainApp.Servers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace WinUI.Converters;

internal partial class ServerTypeToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not ServerType left || parameter is not string right) return Visibility.Collapsed;

        return left.ToString() == right ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}