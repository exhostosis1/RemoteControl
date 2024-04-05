
using Microsoft.Extensions.Logging;
using Shared.Config;

#if DEBUG
using Microsoft.Extensions.Logging.Debug;
#else
using NReco.Logging.File;
#endif

namespace AppHost;

public class AppHostBuilder
{
    private ILoggerProvider? _loggerProvider = null;
    private ServerFactory? _serverFactory = null;
    private RegistryAutoStartService? _autoStartService = null;
    private IConfigProvider? _configProvider = null;

    public AppHostBuilder UseLogger(ILoggerProvider loggerProvider)
    {
        _loggerProvider = loggerProvider;
        return this;
    }

    public AppHostBuilder UseConfiguration(IConfigProvider configProvider)
    {
        _configProvider = configProvider;
        return this;
    }

    public AppHost Build()
    {
#if DEBUG
        _loggerProvider ??= new DebugLoggerProvider();
#else
        _loggerProvider ??= new FileLoggerProvider(Path.Combine(Environment.CurrentDirectory, "error.log"), new FileLoggerOptions
        {
            Append = true,
            MinLevel = LogLevel.Error
        });
#endif
        _serverFactory ??= new ServerFactory(_loggerProvider);
        _autoStartService ??=
            new RegistryAutoStartService(_loggerProvider.CreateLogger(nameof(RegistryAutoStartService)));
        _configProvider ??= new JsonConfigurationProvider(
            _loggerProvider.CreateLogger(nameof(JsonConfigurationProvider)),
            Path.Combine(Environment.CurrentDirectory, "appsettings.json"));

        return new AppHost(_loggerProvider, _serverFactory, _autoStartService, _configProvider);
    }
}