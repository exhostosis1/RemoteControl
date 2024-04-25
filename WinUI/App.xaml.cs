using System.Collections.Generic;
using System.Linq;
using H.NotifyIcon;
using MainApp;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.Win32;
using WinUI.ViewModels;
using WinUI.Windows;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>

        public Window MainWindow { get; private set; }

        public bool HandleClosedEvents { get; set; } = true;

        public AppHost Host { get; init; }

        public App()
        {
            this.InitializeComponent();

            Host = new AppHostBuilder().Build();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            MainWindow = new MainWindow
            {
                ExtendsContentIntoTitleBar = true,
                SystemBackdrop = new MicaBackdrop()
            };

            MainWindow.Closed += (_, eventArgs) =>
            {
                if (!HandleClosedEvents) return;

                eventArgs.Handled = true;
                MainWindow.Hide(true);
            };

            _vm = ServerCollectionViewModelProvider.Get();
            SetSystemEvents();
        }

        private void SetSystemEvents()
        {
            SystemEvents.SessionSwitch += SessionSwitchHandler;
        }

        private List<ServerViewModel> _runningServers = [];
        private ServerCollectionViewModel _vm;

        private void SessionSwitchHandler(object sender, SessionSwitchEventArgs args)
        {
            switch (args.Reason)
            {
                case SessionSwitchReason.SessionLock:
                {
                    Host.Logger.LogInformation("Stopping servers due to logout");

                    _runningServers = _vm.Servers.Where(x => x.Status).ToList();
                    _runningServers.ForEach(x => x.StopCommand.Execute(null));

                    break;
                }
                case SessionSwitchReason.SessionUnlock:
                {
                    Host.Logger.LogInformation("Restoring servers");

                    _runningServers.ForEach(x => x.StartCommand.Execute(null));
                    break;
                }
                case SessionSwitchReason.ConsoleConnect:
                case SessionSwitchReason.ConsoleDisconnect:
                case SessionSwitchReason.RemoteConnect:
                case SessionSwitchReason.RemoteDisconnect:
                case SessionSwitchReason.SessionLogon:
                case SessionSwitchReason.SessionLogoff:
                case SessionSwitchReason.SessionRemoteControl:
                default:
                    break;

            }
        }
    }
}
