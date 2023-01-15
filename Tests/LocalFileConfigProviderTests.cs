using ConfigProviders;
using Moq;
using Shared.Config;
using Shared.Logging.Interfaces;
using System.Text.Json;

namespace Tests;

public class LocalFileConfigProviderTests: IDisposable
{
    private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "testconfig.ini");
    private readonly LocalFileConfigProvider _provider;
    private readonly ILogger<LocalFileConfigProvider> _logger;

    public LocalFileConfigProviderTests()
    {
        _logger = Mock.Of<ILogger<LocalFileConfigProvider>>();
        _provider = new LocalFileConfigProvider(_filePath, _logger);
        if(File.Exists(_filePath))
            File.Delete(_filePath);
    }

    [Fact]
    public void GetConfigTest()
    {
        var config = _provider.GetConfig();

        Mock.Get(_logger).Verify(x => x.LogWarn("No config file"), Times.Once);
        Assert.True(config.ProcessorConfigs.Count == 0);

        var configText = @"
{
  ""ProcessorConfigs"": [
    {
      ""$type"": ""server"",
      ""Scheme"": ""http"",
      ""Host"": ""192.168.31.12"",
      ""Port"": 1488,
      ""Name"": ""localhost"",
      ""Autostart"": true
    },
    {
      ""$type"": ""bot"",
      ""ApiUri"": ""apiUrl"",
      ""ApiKey"": ""apiKey"",
      ""Usernames"": [
        ""exhostosis""
      ],
      ""Name"": ""telegram"",
      ""Autostart"": false
    }
  ]
}
";
        var savedConfig = JsonSerializer.Deserialize<AppConfig>(configText);

        File.WriteAllText(_filePath, configText);

        config = _provider.GetConfig();
        Assert.True(config.Equals(savedConfig));

        config = _provider.GetConfig();
        Assert.True(config.Equals(savedConfig));

        Mock.Get(_logger).Verify(x => x.LogInfo(It.IsAny<string>()), Times.Exactly(3));

        File.Delete(_filePath);
    }

    [Fact]
    public void SetConfigTest()
    {
        var config = new AppConfig()
        {
            ProcessorConfigs = new List<CommonConfig>
            {
                new BotConfig
                {
                    Name = "bot1",
                    ApiKey = "key1",
                    ApiUri = "uri1",
                    Autostart = true,
                    Usernames = new List<string>{"user1", "user2"}
                },
                new ServerConfig
                {
                    Name = "server1",
                    Autostart = false,
                    Host = "host1",
                    Scheme = "scheme1",
                    Port = 1234
                },
                new ServerConfig
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

        Mock.Get(_logger).Verify(x => x.LogInfo($"Writing config to file {_filePath}"), Times.Once);

        var savedConfig = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(_filePath));

        Assert.True(savedConfig?.Equals(config));

        File.Delete(_filePath);
    }

    public void Dispose()
    {
    }
}