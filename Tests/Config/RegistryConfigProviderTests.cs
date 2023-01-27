using System.Text.Json;
using System.Text.Json.Serialization;
using ConfigProviders;
using Moq;
using Shared.Config;
using Shared.Logging.Interfaces;
using Shared.Wrappers.RegistryWrapper;

namespace UnitTests.Config;

public class RegistryConfigProviderTests : IDisposable
{
    private readonly RegistryConfigProvider _provider;
    private static Mock<IRegistryKey> _currentKey;
    private readonly ILogger<RegistryConfigProvider> _logger;
    private const string KeyName = "RemoteControl";
    private const string ValueName = "Config";

    public RegistryConfigProviderTests()
    {
        _logger = Mock.Of<ILogger<RegistryConfigProvider>>();
        var registry = new Mock<IRegistry>(MockBehavior.Strict);
        var registryKey = new Mock<IRegistryKey>(MockBehavior.Strict);
        _currentKey = new Mock<IRegistryKey>(MockBehavior.Strict);

        registry.SetupGet(x => x.CurrentUser).Returns(registryKey.Object);
        registryKey.Setup(x => x.OpenSubKey(It.IsAny<string>())).Returns(registryKey.Object);
        registryKey.Setup(x => x.OpenSubKey(It.IsAny<string>(), It.IsAny<bool>())).Returns(registryKey.Object);
        registryKey.Setup(x => x.OpenSubKey(KeyName, true)).Returns(_currentKey.Object);
        registryKey.Setup(x => x.CreateSubKey(KeyName, true)).Returns(_currentKey.Object);

        _provider = new RegistryConfigProvider(registry.Object, _logger);
    }

    [Fact]
    public void NoRegistryKeyExceptionTest()
    {
        var registry = new Mock<IRegistry>(MockBehavior.Strict);
        var registryKey = new Mock<IRegistryKey>(MockBehavior.Strict);
        registry.SetupGet(x => x.CurrentUser).Returns(registryKey.Object);
        registryKey.Setup(x => x.OpenSubKey(It.IsAny<string>())).Returns((IRegistryKey?)null);
        registryKey.Setup(x => x.OpenSubKey(It.IsAny<string>(), It.IsAny<bool>())).Returns((IRegistryKey?)null);

        Assert.Throws<Exception>(() => new RegistryConfigProvider(registry.Object, _logger));
    }

    [Fact]
    public void GetDefaultConfigTest()
    {
        _currentKey.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<object?>())).Returns(null);

        var config = _provider.GetConfig();
        var defaultConfig = new AppConfig();

        Assert.Equal(defaultConfig, config);
    }

    [Fact]
    public void FailedJsonTest()
    {
        _currentKey.Setup(x => x.GetValue(ValueName, null)).Returns("bla bla bla");

        _provider.GetConfig();

        Mock.Get(_logger).Verify(x => x.LogError(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void GetConfigTest()
    {
        var testConfig = new AppConfig
        {
            ServerConfigs = new List<CommonConfig>
            {
                new BotConfig
                {
                    Name = "newBot",
                    ApiKey = "newApiKey",
                    ApiUri = "newApiUrl",
                    Autostart = false,
                    Usernames = new List<string> { "newuser1", "newuser2" }
                },
                new WebConfig
                {
                    Name = "newServer",
                    Autostart = false,
                    Host = "newhost",
                    Scheme = "newscheme",
                    Port = 12344
                }
            }
        };

        var serializedConfig = JsonSerializer.Serialize(testConfig);

        _currentKey.Setup(x => x.GetValue(ValueName, null)).Returns(serializedConfig);

        var config = _provider.GetConfig();

        Assert.Equal(testConfig, config);
    }

    [Fact]
    public void SetConfigTest()
    {
        var testConfig = new AppConfig
        {
            ServerConfigs = new List<CommonConfig>
            {
                new BotConfig
                {
                    Name = "newBot",
                    ApiKey = "newApiKey",
                    ApiUri = "newApiUrl",
                    Autostart = false,
                    Usernames = new List<string> { "newuser1", "newuser2" }
                },
                new WebConfig
                {
                    Name = "newServer",
                    Autostart = false,
                    Host = "newhost",
                    Scheme = "newscheme",
                    Port = 12344
                }
            }
        };
        var serializedConfig = JsonSerializer.Serialize(testConfig,
            new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

        _currentKey.Setup(x => x.SetValue(ValueName, It.IsAny<object>(), RegValueType.String));

        _provider.SetConfig(testConfig);

        _currentKey.Verify(x => x.SetValue(ValueName, serializedConfig, RegValueType.String), Times.Once);
    }

    public void Dispose()
    {
    }
}