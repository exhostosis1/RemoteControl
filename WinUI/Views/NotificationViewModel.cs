using CommunityToolkit.Mvvm.ComponentModel;
using MainApp.ViewModels;
using System.Runtime.Versioning;

namespace WinUI.Views
{
    [SupportedOSPlatform("windows10.0.26100.0")]
    public partial class NotificationViewModel : ObservableObject
    {
        private ServerViewModel? _firstServerViewModel;
        public ServerViewModel? FirstServerViewModel
        {
            get => _firstServerViewModel;
            set => SetProperty(ref _firstServerViewModel, value);
        }

        private ServerViewModel? _secondServerViewModel;
        public ServerViewModel? SecondServerViewModel
        {
            get => _secondServerViewModel;
            set => SetProperty(ref _secondServerViewModel, value);
        }

        private bool _isAutorun;
        public bool IsAutorun
        {
            get => _isAutorun;
            set => SetProperty(ref _isAutorun, value);
        }
    }
}
