using Config;
using Control.Wrappers;
using Http.Listeners;
using Logging;
using RemoteControlApp;
using RemoteControlApp.Middleware;
using RemoteControlConsole;
using Shared.Interfaces.Control;
using Shared.Interfaces.Logging;
using Shared.Interfaces.Web;
using System.Runtime.InteropServices;

var container = new Container()
    .Register<ILogger, ConsoleLogger>();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    container
        .Register<IKeyboardControl, User32Wrapper>()
        .Register<IMouseControl, User32Wrapper>()
        .Register<IDisplayControl, User32Wrapper>()
        .Register<IAudioControl, NAudioWrapper>();
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    container
        .Register<IKeyboardControl, YdotoolWrapper>()
        .Register<IMouseControl, YdotoolWrapper>()
        .Register<IDisplayControl, DummyWrapper>()
        .Register<IAudioControl, DummyWrapper>();
}
else
{
    Console.WriteLine("OS not supported");
    return;
}

container.Register<IListener, GenericListener>();

var app = new RemoteControl(container.Get<IListener>(), container)
    .UseLoggingMiddleware()
    .UseStaticFilesMiddleware()
    .UseApiV1Enpoint()
    .Build();

var config = new LocalFileConfigService(container.Get<ILogger>());

app.Start(config.GetConfig().UriConfig.Uri);

Console.ReadLine();