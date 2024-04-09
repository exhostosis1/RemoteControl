using ControlProviders.Enums;
using ControlProviders.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Servers.DataObjects.Bot;
using Servers.Middleware;
using Servers.Middleware.Enums;

namespace UnitTests.Middleware;

public class CommandExecutorTests : IDisposable
{
    private readonly Mock<IKeyboardControlProvider> _keyboard;
    private readonly Mock<IDisplayControlProvider> _display;
    private readonly Mock<IAudioControlProvider> _audio;
    private readonly CommandsExecutor _executor;

    public CommandExecutorTests()
    {
        var logger = Mock.Of<ILogger>();
        _keyboard = new Mock<IKeyboardControlProvider>(MockBehavior.Strict);
        _display = new Mock<IDisplayControlProvider>(MockBehavior.Strict);
        _audio = new Mock<IAudioControlProvider>(MockBehavior.Strict);

        _executor = new CommandsExecutor(_keyboard.Object, _display.Object, _audio.Object, logger);
    }

    [Theory]
    [InlineData(BotButtons.Pause, KeysEnum.MediaPlayPause)]
    [InlineData(BotButtons.MediaBack, KeysEnum.MediaPrev)]
    [InlineData(BotButtons.MediaForth, KeysEnum.MediaNext)]
    public async Task ExecuteKeyboardKeyTest(string button, KeysEnum key)
    {
        _keyboard.Setup(x => x.KeyboardKeyPress(It.IsAny<KeysEnum>(), It.IsAny<KeyPressMode>()));

        var context = new BotContext(new BotContextRequest("", "", 0, button, DateTime.Now),
            Mock.Of<BotContextResponse>());

        await _executor.ProcessRequestAsync(context, null!);

        _keyboard.Verify(x => x.KeyboardKeyPress(key, KeyPressMode.Click), Times.Once);
        Assert.Equal("done", context.BotResponse.Message);
        Assert.True(context.BotResponse.Buttons is ReplyButtonsMarkup { OneTime: false, Persistent: true, Resize: true } s && s.Items.Count() == 2);
    }

    [Theory]
    [InlineData(BotButtons.VolumeUp, 45, 50)]
    [InlineData(BotButtons.VolumeDown, 45, 40)]
    [InlineData(BotButtons.VolumeUp, 0, 5)]
    [InlineData(BotButtons.VolumeDown, 0, 0)]
    [InlineData(BotButtons.VolumeUp, 100, 100)]
    [InlineData(BotButtons.VolumeDown, 100, 95)]
    public async Task ExecuteVolumeKeyTest(string button, int actual, int expected)
    {
        _audio.Setup(x => x.GetVolume()).Returns(actual);
        _audio.Setup(x => x.SetVolume(It.IsInRange(0, 100, Moq.Range.Inclusive)));
        
        var context = new BotContext(new BotContextRequest("", "", 0, button, DateTime.Now),
            Mock.Of<BotContextResponse>());

        await _executor.ProcessRequestAsync(context, null!);

        Assert.True(context.BotResponse.Message == expected.ToString());
        Assert.True(context.BotResponse.Buttons is ReplyButtonsMarkup { OneTime: false, Persistent: true, Resize: true } s && s.Items.Count() == 2);

        _audio.Verify(x => x.GetVolume(), Times.Once);
        _audio.Verify(x => x.SetVolume(expected), Times.Once);
    }

    [Fact]
    public async Task DarkenTest()
    {
        _display.Setup(x => x.DisplayOff());

        var context = new BotContext(new BotContextRequest("", "", 0, BotButtons.Darken, DateTime.Now),
            Mock.Of<BotContextResponse>());

        await _executor.ProcessRequestAsync(context, null!);

        Assert.Equal("done", context.BotResponse.Message);
        Assert.True(context.BotResponse.Buttons is ReplyButtonsMarkup { OneTime: false, Persistent: true, Resize: true } s && s.Items.Count() == 2);

        _display.Verify(x => x.DisplayOff(), Times.Once);
    }

    [Theory]
    [InlineData(45)]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(-5)]
    [InlineData(105)]
    public async Task SetVolumeTest(int value)
    {
        _audio.Setup(x => x.SetVolume(It.IsInRange(0, 100, Moq.Range.Inclusive)));

        var context = new BotContext(new BotContextRequest("", "", 0, value.ToString(), DateTime.Now),
            Mock.Of<BotContextResponse>());

        await _executor.ProcessRequestAsync(context, null!);

        Assert.Equal("done", context.BotResponse.Message);
        Assert.True(context.BotResponse.Buttons is ReplyButtonsMarkup { OneTime: false, Persistent: true, Resize: true } s && s.Items.Count() == 2);

        var actual = value > 100 ? 100 : value < 0 ? 0 : value;

        _audio.Verify(x => x.SetVolume(actual), Times.Once);
    }

    [Fact]
    public async Task ParsingErrorTest()
    {
        var context = new BotContext(new BotContextRequest("", "", 0, "absdf", DateTime.Now),
            Mock.Of<BotContextResponse>());

        await _executor.ProcessRequestAsync(context, null!);

        Assert.Equal("done", context.BotResponse.Message);
        Assert.True(context.BotResponse.Buttons is ReplyButtonsMarkup { OneTime: false, Persistent: true, Resize: true } s && s.Items.Count() == 2);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}