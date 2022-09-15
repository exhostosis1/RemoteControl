using System.Reflection;
using Shared;
using Shared.Config;
using Shared.Config.Interfaces;
using Shared.Logging.Interfaces;

namespace ConfigProviders
{
    public class LocalFileConfigProvider: IConfigProvider
    {
        private static readonly string ConfigPath = AppContext.BaseDirectory + "config.ini";
        private readonly ILogger _logger;

        public LocalFileConfigProvider(ILogger logger)
        {
            _logger = logger;
        }

        public AppConfig GetConfig()
        {
            var result = new AppConfig();

            var lines = File.ReadAllLines(ConfigPath)
                .Select(x => x.Contains('#') ? x[..x.IndexOf('#')].Trim() : x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x));

            IConfigItem? configItem = null;

            foreach (var line in lines)
            {
                if(line.StartsWith('[') && line.EndsWith(']'))
                {
                    var name = line[1..^1];
                    var types = result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(x => x.PropertyType.GetInterface(nameof(IConfigItem)) != null);

                    var configItemProp = types.SingleOrDefault(x => x.PropertyType.GetDisplayName() == name);

                    configItem = configItemProp?.PropertyType.GetConstructor(Array.Empty<Type>())?.Invoke(null) as IConfigItem;

                    if (configItem == null) continue;

                    configItemProp?.SetValue(result, configItem);
                }

                if (line.Contains('=') && configItem != null)
                {
                    try
                    {
                        if (!line.TryParseConfig(out var param, out var value))
                            continue;

                        var prop = configItem.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .SingleOrDefault(x => x.GetDisplayName() == param);

                        if (prop == null) continue;

                        var propType = prop.PropertyType;
                        var convertedValue = Convert.ChangeType(value, propType);

                        prop.SetValue(configItem, convertedValue);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                    }
                }
            }

            return result;
        }

        public bool SetConfig(AppConfig appConfig)
        {
            var result = new List<string>();

            var appConifigProps = appConfig.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.PropertyType.GetInterface(nameof(IConfigItem)) != null);

            foreach (var appConfigProp in appConifigProps)
            {
                var appConfigItem = appConfigProp.GetValue(appConfig);
                if (appConfigItem == null) continue;

                result.Add($"[{appConfigProp.PropertyType.GetDisplayName()}]\n");

                foreach (var configItemProp in appConfigItem.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var name = configItemProp.GetDisplayName();
                    var value = configItemProp.GetValue(appConfigItem)?.ToString() ?? string.Empty;

                    if (string.IsNullOrEmpty(name)) continue;

                    result.Add($"{name} = {value}");
                }
            }

            File.WriteAllLines(ConfigPath, result);

            return true;
        }
    }
}
