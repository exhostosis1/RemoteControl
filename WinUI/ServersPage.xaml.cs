using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Servers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ServersPage : Page
{
    public ViewModel ViewModel { get; set; } = ViewModelProvider.GetViewModel();

    public ServersPage()
    {
        this.InitializeComponent();
    }

    private void ToggleButton_OnChecked(object sender, RoutedEventArgs _)
    {
        if (sender is not CheckBox { CommandParameter: Server server }) return;

        ViewModel.ToggleCommand.Execute(server);
    }

    private void ButtonRemove_OnClick(object sender, RoutedEventArgs _)
    {
        if (sender is not Button { CommandParameter: Server server }) return;

        ViewModel.RemoveServerCommand.Execute(server);
    }

    private void ButtonUpdate_OnClick(object _, RoutedEventArgs __)
    {
        ViewModel.App.SaveConfig();
    }
}