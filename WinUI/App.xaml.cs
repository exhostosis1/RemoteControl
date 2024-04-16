using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;


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

        public App()
        {
            this.InitializeComponent();
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
                MainWindow.Hide();
            };

            MainWindow.Activate();
        }
    }
}
