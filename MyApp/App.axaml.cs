using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RemoteControlContainer;
using Shared;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RemoteControlAvalonia
{
    public class App : Application
    {
        public class ViewModel
        {
            public bool IsListening { get; set; }
            public string Uri { get; set; } 
            public bool Autostart { get; set; }

            public string Start => IsListening ? "Stop" : "Start";
        }

        private Uri _prefUri;

        public ViewModel MyDataContext { get; set; }

        private IServer _server;
        private IAutostartService _autostartService;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            _server = Container.Server;
            _autostartService = Container.AutostartService;

            _prefUri = Container.Config.GetConfig().UriConfig.Uri;

            _server.Start(_prefUri);

            MyDataContext = new ViewModel
            {
                IsListening = _server.IsListening,
                Autostart = _autostartService.CheckAutostart(),
                Uri = _server.GetListeningUri() ?? "stopped"
            };

            DataContext = this;
        }

        public void Exit()
        {
            (ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Shutdown(0);
        }

        public void Start()
        {
            if(_server.IsListening) 
                _server.Stop();
            else
                _server.Start(_prefUri);

            MyDataContext.Uri = _server.GetListeningUri() ?? "stopped";
            MyDataContext.IsListening = _server.IsListening;
        }

        public void SetAutostart(bool value)
        {
            _autostartService.SetAutostart(value);
            MyDataContext.Autostart = value;
        }

        public void Open()
        {
            var address = _server.GetListeningUri() ?? throw new Exception("No listening uri");

            try
            {
                Process.Start(address);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    address = address.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {address}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", address);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}