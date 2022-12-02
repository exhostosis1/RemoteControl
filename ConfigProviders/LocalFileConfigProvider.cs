using Shared;
using Shared.Config;
using Shared.Logging.Interfaces;

namespace ConfigProviders;

public class LocalFileConfigProvider : BaseConfigProvider
{
    private static readonly string ConfigPath = AppContext.BaseDirectory + "config.ini";

    public LocalFileConfigProvider(ILogger logger): base(logger)
    {
    }

    protected override AppConfig GetConfigInternal()
    {
        var result = new AppConfig();

        var lines = File.ReadAllLines(ConfigPath)
            .Select(x => x.Contains('#') ? x[..x.IndexOf('#')].Trim() : x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x));

        object? configItem = null;

        foreach (var line in lines)
        {
            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                var name = line[1..^1];

                var configItemProp = result.GetPropertyByDisplayName(name);

                configItem = configItemProp?.GetValue(result);
            }
            else if (line.Contains('=') && configItem != null)
            {
                try
                {
                    if (!line.TryParseConfig(out var param, out var value))
                        continue;

                    var prop = configItem.GetPropertyByDisplayName(param);

                    if (prop == null) continue;
                    
                    var convertedValue = Convert.ChangeType(value, prop.PropertyType);

                    prop.SetValue(configItem, convertedValue);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                }
            }
        }

        return result;
    }

    protected override void SetConfigInternal(AppConfig appConfig)
    {
        var result = new List<string>();

        var appConifigProps = appConfig.GetPropertiesWithDisplayName();

        foreach (var appConfigProp in appConifigProps)
        {
            var appConfigItem = appConfigProp.GetValue(appConfig);
            if (appConfigItem == null) continue;

            result.Add($"[{appConfigProp.PropertyType.GetDisplayName()}]");

            foreach (var configItemProp in appConfigItem.GetPropertiesWithDisplayName())
            {
                var name = configItemProp.GetDisplayName();
                var value = configItemProp.GetValue(appConfigItem)?.ToString() ?? string.Empty;

                result.Add($"{name} = {value}");
            }
            result.Add("");
        }

        File.WriteAllLines(ConfigPath, result);
    }
}