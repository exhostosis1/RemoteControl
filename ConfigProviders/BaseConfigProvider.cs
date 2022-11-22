using Shared;
using Shared.Logging.Interfaces;

namespace ConfigProviders
{
    public abstract class BaseConfigProvider: IConfigProvider
    {
        public Uri ConfigUri
        {
            get => _cachedConfig ?? GetConfig();
            set => SetConfig(value);
        }

        private Uri? _cachedConfig;

        protected readonly ILogger Logger;

        protected const string SchemeName = "scheme";
        protected const string HostName = "host";
        protected const string PortName = "port";

        protected string Host = "localhost";
        protected string Scheme = "http";
        protected int Port = 80;

        protected BaseConfigProvider(ILogger logger)
        {
            Logger = logger;
        }

        private Uri GetConfig()
        {
            var result = GetConfigInternal();
            _cachedConfig = result;
            return result;
        }

        private void SetConfig(Uri config)
        {
            SetConfigInternal(config);
            _cachedConfig = config;
        }

        protected abstract Uri GetConfigInternal();
        protected abstract void SetConfigInternal(Uri config);
    }
}
