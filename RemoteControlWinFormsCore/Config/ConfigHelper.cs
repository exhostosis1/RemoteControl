using System.Reflection;

namespace RemoteControl.Config
{
    internal static class ConfigHelper
    {
        private static readonly string ConfigPath = AppContext.BaseDirectory + "config.ini";
        
        public static AppConfig GetAppConfigFromFile()
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
                    var types = result.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(x => x.PropertyType.GetInterface(nameof(IConfigItem)) != null);

                    var configItemProp = types.SingleOrDefault(x => x.PropertyType.GetDisplayName() == name);

                    configItem = configItemProp?.PropertyType.GetConstructor(Array.Empty<Type>())?.Invoke(null) as IConfigItem;

                    if (configItem == null) continue;

                    configItemProp?.SetValue(result, configItem);
                }

                if (line.Contains('=') && configItem != null)
                {
                    string param;
                    string value;

                    try
                    {
                        (param, value) = line.Split('=',
                            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e.Message);
                        continue;
                    }

                    var props = configItem.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);

                    var prop = configItem.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                        .SingleOrDefault(x => x.GetDisplayName() == param);

                    if (prop == null) continue;

                    var propType = prop.PropertyType;
                    var convertedValue = Convert.ChangeType(value, propType);

                    prop.SetValue(configItem, convertedValue);
                }
            }

            return result;
        }

        public static void SaveConfigToFile(AppConfig appConfig)
        {
            var result = new List<string>();

            var appConifigProps = appConfig.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.PropertyType.GetInterface(nameof(IConfigItem)) != null);

            foreach (var appConfigProp in appConifigProps)
            {
                var appConfigItem = appConfigProp.GetValue(appConfig);
                if (appConfigItem == null) continue;

                result.Add($"[{appConfigProp.PropertyType.GetDisplayName()}]\n");

                foreach (var configItemProp in appConfigItem.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    var name = configItemProp.GetDisplayName();
                    var value = configItemProp.GetValue(appConfigItem)?.ToString() ?? string.Empty;

                    if (string.IsNullOrEmpty(name)) continue;

                    result.Add($"{name} = {value}");
                }
            }

            File.WriteAllLines(ConfigPath, result);
        }

        public static void Deconstruct(this string[] input, out string param, out string value)
        {
            param = input[0];
            value = input[1];
        }
    }
}
