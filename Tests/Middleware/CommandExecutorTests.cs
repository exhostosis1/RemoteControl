using Moq;
using Servers.Middleware;
using Shared.ControlProviders.Provider;
using Shared.DataObjects.Bot;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace UnitTests.Middleware;

public class CommandExecutorTests : IDisposable
{
    private readonly ILogger<CommandsExecutor> _logger;
    private readonly Mock<IGeneralControlProvider> _provider;
    private readonly CommandsExecutor _executor;

    public CommandExecutorTests()
    {
        _logger = Mock.Of<ILogger<CommandsExecutor>>();
        _provider = new Mock<IGeneralControlProvider>(MockBehavior.Strict);

        _executor = new CommandsExecutor(_provider.Object, _logger);
    }

    [Theory]
    [InlineData(BotButtons.Pause, KeysEnum.MediaPlayPause)]
    [InlineData(BotButtons.MediaBack, KeysEnum.MediaPrev)]
    [InlineData(BotButtons.MediaForth, KeysEnum.MediaNext)]
    public void ExecuteKeyboardKeyTest(string button, KeysEnum key)
    {
        _provider.Setup(x => x.KeyboardKeyPress(It.IsAny<KeysEnum>(), It.IsAny<KeyPressMode>()));

        var context = new BotContext(new BotContextRequest("", "", 0, button, DateTime.Now),
            Mock.Of<BotContextResponse>());

        _executor.ProcessRequest(context);

        _provider.Verify(x => x.KeyboardKeyPress(key, KeyPressMode.Click), Times.Once);
        Assert.True(context.BotResponse.Message == "done");
        Assert.True(context.BotResponse.Buttons is ReplyButtonsMarkup { OneTime: false, Persistent: true, Resize: true } s && s.Items.Count() == 2);
    }

    [Theory]
    [InlineData(BotButtons.VolumeUp, 45, 50)]
    [InlineData(BotButtons.VolumeDown, 45, 40)]
    [InlineData(BotButtons.VolumeUp, 0, 5)]
    [InlineData(BotButtons.VolumeDown, 0, 0)]
    [InlineData(BotButtons.VolumeUp, 100, 100)]
    [InlineData(BotButtons.VolumeDown, 100, 95)]
    public void ExecuteVolumeKeyTest(string button, int actual, int expected)
    {
        _provider.Setup(x => x.GetVolume()).Returns(actual);
        _provider.Setup(x => x.SetVolume(It.IsInRange(0, 100, Moq.Range.Inclusive)));
        
        var context = new BotContext(new BotContextRequest("", "", 0, button, DateTime.Now),
            Mock.Of<BotContextResponse>());

        _executor.ProcessRequest(context);

        Assert.True(context.BotResponse.Message == expected.ToString());
        Assert.True(context.BotResponse.Buttons is ReplyButtonsMarkup { OneTime: false, Persistent: true, Resize: true } s && s.Items.Count() == 2);

        _provider.Verify(x => x.GetVolume(), Times.Once);
        _provider.Verify(x => x.SetVolume(expected), Times.Once);
    }

    [Fact]
    public void DarkenTest()
    {
        _provider.Setup(x => x.DisplayOff());

        var context = new BotContext(new BotContextRequest("", "", 0, BotButtons.Darken, DateTime.Now),
            Mock.Of<BotContextResponse>());

        _executor.ProcessRequest(context);

        Assert.True(context.BotResponse.Message == "done");
        Assert.True(context.BotResponse.Buttons is ReplyButtonsMarkup { OneTime: false, Persistent: true, Resize: true } s && s.Items.Count() == 2);

        _provider.Verify(x => x.DisplayOff(), Times.Once);
    }

    [Theory]
    [InlineData(45)]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(-5)]
    [InlineData(105)]
    public void SetVolumeTest(int value)
    {
        _provider.Setup(x => x.SetVolume(It.IsInRange(0, 100, Moq.Range.Inclusive)));

        var context = new BotContext(new BotContextRequest("", "", 0, value.ToString(), DateTime.Now),
            Mock.Of<BotContextResponse>());

        _executor.ProcessRequest(context);

        Assert.True(context.BotResponse.Message == "done");
        Assert.True(context.BotResponse.Buttons is ReplyButtonsMarkup { OneTime: false, Persistent: true, Resize: true } s && s.Items.Count() == 2);

        var actual = value > 100 ? 100 : value < 0 ? 0 : value;

        _provider.Verify(x => x.SetVolume(actual), Times.Once);
    }

    [Fact]
    public void ParsingErrorTest()
    {
        var context = new BotContext(new BotContextRequest("", "", 0, "absdf", DateTime.Now),
            Mock.Of<BotContextResponse>());

        _executor.ProcessRequest(context);

        Assert.True(context.BotResponse.Message == "done");
        Assert.True(context.BotResponse.Buttons is ReplyButtonsMarkup { OneTime: false, Persistent: true, Resize: true } s && s.Items.Count() == 2);
    }

    public void Dispose()
    {

    }
}