using Microsoft.Win32;
using Shared.Logging.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Shared.Config;

namespace ConfigProviders;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Planform check in constructor is sufficient")]
public class WinRegistryConfigProvider: BaseConfigProvider
{
    private readonly RegistryKey _serverKey;
    private readonly RegistryKey _botKey;

    public WinRegistryConfigProvider(ILogger logger): base(logger)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");

        var regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE")!.OpenSubKey("RemoteControl", true) ?? 
                  Registry.CurrentUser.OpenSubKey("SOFTWARE", true)!.CreateSubKey("RemoteControl", true);
        _serverKey = regKey.OpenSubKey(ServerSectionName) ?? regKey.CreateSubKey(ServerSectionName);
        _botKey = regKey.OpenSubKey(BotSectionName) ?? regKey.CreateSubKey(BotSectionName);
    }

    protected override ConfigItem GetConfigInternal()
    {
        var names = _serverKey.GetValueNames();

        foreach (var name in names)
        {
            switch (name)
            {
                case HostName:
                    Host = _serverKey.GetValue(name) as string ?? Host;
                    break;
                case PortName:
                    Port = _serverKey.GetValue(name) as int? ?? Port;
                    break;
                case SchemeName:
                    Scheme = _serverKey.GetValue(name) as string ?? Scheme;
                    break;
                case ServerAutostartName:
                    ServerAutostart = _serverKey.GetValue(name) as bool? ?? ServerAutostart;
                    break;
            }
        }

        names = _botKey.GetValueNames();

        foreach (var name in names)
        {
            switch (name)
            {
                case ChatIdsName:
                    ChatIds = (_botKey.GetValue(name) as string)?.Split(';',
                        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(
                        x =>
                        {
                            if (int.TryParse(x, out var res))
                                return res;
                            return -1;
                        }).Where(x => x != -1).ToList() ?? ChatIds;
                    break;
                case BotAutostartName:
                    BotAutostart = _serverKey.GetValue(name) as bool? ?? BotAutostart;
                    break;
            }
        }

        try
        {
            return new ConfigItem
            {
                BotConfig = new BotConfig
                {
                    ChatIds = ChatIds,
                    StartListening = BotAutostart
                },
                ServerConfig = new ServerConfig
                {
                    Scheme = Scheme,
                    Host = Host,
                    Port = Port,
                    StartListening = ServerAutostart
                }
            };
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
            throw;
        }
    }

    protected override void SetConfigInternal(ConfigItem config)
    {
        _serverKey.SetValue(HostName, config.ServerConfig.Host, RegistryValueKind.String);
        _serverKey.SetValue(SchemeName, config.ServerConfig.Scheme, RegistryValueKind.String);
        _serverKey.SetValue(PortName, config.ServerConfig.Port, RegistryValueKind.DWord);
        _serverKey.SetValue(ServerAutostartName, config.ServerConfig.StartListening, RegistryValueKind.Binary);

        _botKey.SetValue(ChatIdsName, string.Join(';', config.BotConfig.ChatIds), RegistryValueKind.String);
        _botKey.SetValue(BotAutostartName, config.BotConfig.StartListening, RegistryValueKind.Binary);
    }
}