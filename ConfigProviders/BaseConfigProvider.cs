using Shared;
using Shared.Config;
using Shared.Logging.Interfaces;

namespace ConfigProviders
{
    public abstract class BaseConfigProvider: IConfigProvider
    {
        public AppConfig Config
        {
            get => _cachedConfig ?? GetConfig();
            set => SetConfig(value);
        }

        private AppConfig? _cachedConfig;

        protected readonly ILogger Logger;

        protected const string SchemeName = "scheme";
        protected const string HostName = "host";
        protected const string PortName = "port";

        public BaseConfigProvider(ILogger logger)
        {
            Logger = logger;
        }

        private AppConfig GetConfig()
        {
            var result = GetConfigInternal();
            _cachedConfig = result;
            return result;
        }

        private void SetConfig(AppConfig config)
        {
            SetConfigInternal(config);
            _cachedConfig = config;
        }

        protected abstract AppConfig GetConfigInternal();
        protected abstract void SetConfigInternal(AppConfig config);
    }
}
