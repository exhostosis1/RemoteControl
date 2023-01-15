using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigProviders;
using Microsoft.Win32;
using Moq;
using Shared.Config;
using Shared.Logging.Interfaces;
using Shared.RegistryWrapper;
using System.Text.Json;

namespace Tests;

public class WinRegistryConfigProviderTests: IDisposable
{
    private readonly WinRegistryConfigProvider _provider;
    private readonly ILogger<WinRegistryConfigProvider> _logger;
    private readonly MockRegistry _registry;
    private static MockRegistryKey _currentKey;

    private class MockRegistryKey: IRegistryKey
    {
        public string Name { get; set; } = string.Empty;
        public bool Writable { get; set; } = false;

        public string Value = JsonSerializer.Serialize(new AppConfig
        {
            ProcessorConfigs = new List<CommonConfig>
            {
                new BotConfig
                {
                    Name = "bot",
                    ApiKey = "apiKey",
                    ApiUri = "apiUrl",
                    Autostart = true,
                    Usernames = new List<string>{"user1", "user2"}
                },
                new ServerConfig
                {
                    Name = "server",
                    Autostart = true,
                    Host = "host",
                    Scheme = "scheme",
                    Port = 1234
                }
            }
        });

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void DeleteValue(string name, bool throwOnMissingValue)
        {
            throw new NotImplementedException();
        }

        public object? GetValue(string? name, object? defaultValue)
        {
            if (Name == "RemoteControl" && name == "Config")
                return Value;
            else
                return null;
        }

        public IRegistryKey? OpenSubKey(string name)
        {
            var result = new MockRegistryKey { Name = name };
            _currentKey = result;
            return result;
        }

        public IRegistryKey? OpenSubKey(string name, bool writable)
        {
            var result = new MockRegistryKey
            {
                Name = name,
                Writable = writable
            };
            _currentKey = result;
            return result;
        }

        public IRegistryKey CreateSubKey(string name, bool writable)
        {
            var result = new MockRegistryKey
            {
                Name = name,
                Writable = writable
            };
            _currentKey = result;
            return result;
        }

        public void SetValue(string? name, object value, RegistryValueKind valueKind)
        {
            if (Name == "RemoteControl" && name == "Config" && value is string s)
                Value = s;
        }
    }

    private class MockRegistry: IRegistry
    {
        public IRegistryKey CurrentUser { get; } = new MockRegistryKey();
    }

    public WinRegistryConfigProviderTests()
    {
        _logger = Mock.Of<ILogger<WinRegistryConfigProvider>>();
        _registry = new MockRegistry();
        _provider = new WinRegistryConfigProvider(_registry, _logger);
    }

    [Fact]
    public void GetConfigTest()
    {
        var config = _provider.GetConfig();
        var testConfig = new AppConfig
        {
            ProcessorConfigs = new List<CommonConfig>
            {
                new BotConfig
                {
                    Name = "bot",
                    ApiKey = "apiKey",
                    ApiUri = "apiUrl",
                    Autostart = true,
                    Usernames = new List<string> { "user1", "user2" }
                },
                new ServerConfig
                {
                    Name = "server",
                    Autostart = true,
                    Host = "host",
                    Scheme = "scheme",
                    Port = 1234
                }
            }
        };

        Assert.Equal(testConfig, config);
    }

    [Fact]
    public void SetConfigTest()
    {
        var testConfig = new AppConfig
        {
            ProcessorConfigs = new List<CommonConfig>
            {
                new BotConfig
                {
                    Name = "newBot",
                    ApiKey = "newApiKey",
                    ApiUri = "newApiUrl",
                    Autostart = false,
                    Usernames = new List<string> { "newuser1", "newuser2" }
                },
                new ServerConfig
                {
                    Name = "newServer",
                    Autostart = false,
                    Host = "newhost",
                    Scheme = "newscheme",
                    Port = 12344
                }
            }
        };

        _provider.SetConfig(testConfig);

        var registryConfig = JsonSerializer.Deserialize<AppConfig>(_currentKey.Value);

        Assert.Equal(testConfig, registryConfig);
    }

    public void Dispose()
    {
    }
}