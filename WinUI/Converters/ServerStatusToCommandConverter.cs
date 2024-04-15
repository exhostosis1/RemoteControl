using Microsoft.UI.Xaml.Data;
using System;

namespace WinUI.Converters;

internal class ServerStatusToCommandConverter : IValueConverter
{
    private readonly ViewModel _viewModel = ViewModelProvider.GetViewModel();

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not bool left) return new object();

        return left ? _viewModel.StartCommand : _viewModel.StopCommand;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}