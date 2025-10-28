using H.NotifyIcon;
using MainApp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System.Runtime.Versioning;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    [SupportedOSPlatform("windows10.0.26100.0")]
    public partial class App
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>

        public Window MainWindow { get; private set; } = null!;

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
                SystemBackdrop = new MicaBackdrop(),
                Title = "Remote Control"
            };

            MainWindow.Closed += (_, eventArgs) =>
            {
                if (!HandleClosedEvents) return;

                eventArgs.Handled = true;
                MainWindow.Hide();
            };

            MainWindow.Hide();
        }
    }
}
