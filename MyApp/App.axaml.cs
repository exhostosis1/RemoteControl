using Avalonia;
using Avalonia.Markup.Xaml;
using RemoteControlContainer;
using Shared;

namespace RemoteControlAvalonia
{
    public class App : Application
    {
        private IServer _server;
        private IConfigProvider _configProvider;
        private IAutostartService _autostartService;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            _server = Container.Server;
            _configProvider = Container.Config;
            _autostartService = Container.AutostartService;
        }
    }
}