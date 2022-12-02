using Microsoft.Win32;
using Shared;
using Shared.Config;
using Shared.Logging.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ConfigProviders;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Planform check in constructor is sufficient")]
public class WinRegistryConfigProvider: BaseConfigProvider
{
    private RegistryKey _regKey;

    public WinRegistryConfigProvider(ILogger logger): base(logger)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");

        _regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE")!.OpenSubKey("RemoteControl", true) ?? 
                  Registry.CurrentUser.OpenSubKey("SOFTWARE", true)!.CreateSubKey("RemoteControl", true);
    }

    protected override AppConfig GetConfigInternal()
    {
        var result = new AppConfig();

        var configItemsProps = result.GetPropertiesWithDisplayName();

        foreach (var prop in configItemsProps)
        {
            var key = _regKey.OpenSubKey(prop.GetDisplayName()!);
            if(key == null)
                continue;

            var valueNames = key.GetValueNames();
            var configItem = prop.GetValue(result);

            if(configItem == null)
                continue;

            foreach (var valueName in valueNames)
            {
                var itemProp = configItem.GetPropertyByDisplayName(valueName);
                
                if (itemProp == null)
                    continue;

                var convertedValue = Convert.ChangeType(key.GetValue(valueName), itemProp.PropertyType);

                itemProp.SetValue(configItem, convertedValue);
            }
        }

        return result;
    }

    protected override void SetConfigInternal(AppConfig config)
    {
        var props = config.GetPropertiesWithDisplayName();

        foreach (var prop in props)
        {
            var key = _regKey.OpenSubKey(prop.GetDisplayName()!, true) ??
                      _regKey.CreateSubKey(prop.GetDisplayName()!, true);

            var item = prop.GetValue(config);

            if(item == null)
                continue;

            foreach (var itemProp in item.GetPropertiesWithDisplayName())
            {
                key.SetValue(itemProp.GetDisplayName(), itemProp.GetValue(item)?.ToString() ?? string.Empty);
            }
        }
    }
}