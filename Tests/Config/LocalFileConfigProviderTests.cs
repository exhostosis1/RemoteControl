using System.Text.Json;
using ConfigProviders;
using Moq;
using Shared.Config;
using Shared.Logging.Interfaces;

namespace UnitTests.Config;

public class LocalFileConfigProviderTests : IDisposable
{
    private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "testconfig.ini");
    private readonly LocalFileConfigProvider _provider;
    private readonly ILogger<LocalFileConfigProvider> _logger;

    public LocalFileConfigProviderTests()
    {
        _logger = Mock.Of<ILogger<LocalFileConfigProvider>>();
        _provider = new LocalFileConfigProvider(_logger, _filePath);

        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }

    [Fact]
    public void GetDefaultConfigTest()
    {
        var config = _provider.GetConfig();
        var defaultConfig = new AppConfig();

        Mock.Get(_logger).Verify(x => x.LogWarn("No config file"), Times.Once);
        Assert.Equal(config, defaultConfig);
    }

    [Fact]
    public void ReadConfigFromFileTest()
    {
        var configText = """
{
  "ServerConfigs": [
    {
      "$type": "web",
      "Scheme": "http",
      "Host": "192.168.31.12",
      "Port": 1488,
      "Name": "localhost",
      "Autostart": true
    },
    {
      "$type": "bot",
      "ApiUri": "apiUrl",
      "ApiKey": "apiKey",
      "Usernames": [
        "exhostosis"
      ],
      "Name": "telegram",
      "Autostart": false
    }
  ]
}
""";
        var savedConfig = JsonSerializer.Deserialize<AppConfig>(configText);

        File.WriteAllText(_filePath, configText);

        var config = _provider.GetConfig();
        Assert.Equal(config, savedConfig);

        File.Delete(_filePath);
    }

    [Fact]
    public void SetConfigTest()
    {
        var config = new AppConfig
        {
            ServerConfigs = new List<CommonConfig>
            {
                new BotConfig
                {
                    Name = "bot1",
                    ApiKey = "key1",
                    ApiUri = "uri1",
                    Autostart = true,
                    Usernames = new List<string>{"user1", "user2"}
                },
                new WebConfig
                {
                    Name = "server1",
                    Autostart = false,
                    Host = "host1",
                    Scheme = "scheme1",
                    Port = 1234
                },
                new WebConfig
                {
                    Name = "server2",
                    Autostart = false,
                    Host = "host2",
                    Scheme = "scheme2",
                    Port = 3456
                }
            }
        };

        _provider.SetConfig(config);

        var savedConfig = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(_filePath));

        Assert.Equal(savedConfig, config);

        File.Delete(_filePath);
    }

    public void Dispose()
    {
    }
}