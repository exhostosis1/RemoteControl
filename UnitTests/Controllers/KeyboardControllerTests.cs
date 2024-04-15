using ControlProviders.Enums;
using ControlProviders.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Servers.ApiControllers;
using Servers.Middleware;
using Servers.Results;

namespace UnitTests.Controllers;

public class KeyboardControllerTests : IDisposable
{
    private readonly KeyboardController _keyboardController;
    private readonly Mock<IKeyboardControl> _keyboardControlProvider;

    public KeyboardControllerTests()
    {
        var logger = Mock.Of<ILogger>();
        _keyboardControlProvider = new Mock<IKeyboardControl>(MockBehavior.Strict);
        _keyboardController = new KeyboardController(_keyboardControlProvider.Object, logger);
    }

    [Fact]
    public void BackTest()
    {
        _keyboardControlProvider.Setup(x => x.KeyboardKeyPress(KeysEnum.ArrowLeft, KeyPressMode.Click));

        var result = _keyboardController.Back();
        Assert.True(result is OkResult);

        _keyboardControlProvider.Verify(x => x.KeyboardKeyPress(KeysEnum.ArrowLeft, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void ForthTest()
    {
        _keyboardControlProvider.Setup(x => x.KeyboardKeyPress(KeysEnum.ArrowRight, KeyPressMode.Click));

        var result = _keyboardController.Forth();
        Assert.True(result is OkResult);

        _keyboardControlProvider.Verify(x => x.KeyboardKeyPress(KeysEnum.ArrowRight, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void MediaPrevTest()
    {
        _keyboardControlProvider.Setup(x => x.KeyboardKeyPress(KeysEnum.MediaPrev, KeyPressMode.Click));

        var result = _keyboardController.MediaBack();
        Assert.True(result is OkResult);
        _keyboardControlProvider.Verify(x => x.KeyboardKeyPress(KeysEnum.MediaPrev, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void MediaNextTest()
    {
        _keyboardControlProvider.Setup(x => x.KeyboardKeyPress(KeysEnum.MediaNext, KeyPressMode.Click));

        var result = _keyboardController.MediaForth();
        Assert.True(result is OkResult);
        _keyboardControlProvider.Verify(x => x.KeyboardKeyPress(KeysEnum.MediaNext, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void MediaPauseTest()
    {
        _keyboardControlProvider.Setup(x => x.KeyboardKeyPress(KeysEnum.MediaPlayPause, KeyPressMode.Click));

        var result = _keyboardController.Pause();
        Assert.True(result is OkResult);
        _keyboardControlProvider.Verify(x => x.KeyboardKeyPress(KeysEnum.MediaPlayPause, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void VolumeUpTest()
    {
        _keyboardControlProvider.Setup(x => x.KeyboardKeyPress(KeysEnum.VolumeUp, KeyPressMode.Click));

        var result = _keyboardController.MediaVolumeUp();
        Assert.True(result is OkResult);
        _keyboardControlProvider.Verify(x => x.KeyboardKeyPress(KeysEnum.VolumeUp, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void VolumeDownTest()
    {
        _keyboardControlProvider.Setup(x => x.KeyboardKeyPress(KeysEnum.VolumeDown, KeyPressMode.Click));

        var result = _keyboardController.MediaVolumeDown();
        Assert.True(result is OkResult);
        _keyboardControlProvider.Verify(x => x.KeyboardKeyPress(KeysEnum.VolumeDown, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void MuteTest()
    {
        _keyboardControlProvider.Setup(x => x.KeyboardKeyPress(KeysEnum.Mute, KeyPressMode.Click));

        var result = _keyboardController.MediaMute();
        Assert.True(result is OkResult);
        _keyboardControlProvider.Verify(x => x.KeyboardKeyPress(KeysEnum.Mute, KeyPressMode.Click), Times.Once);
    }

    [Theory]
    [InlineData("data1")]
    [InlineData("a asdf,sdjf asdf")]
    [InlineData("input 3")]
    public void TextTest(string input)
    {
        _keyboardControlProvider.Setup(x => x.TextInput(input));
        _keyboardControlProvider.Setup(x => x.KeyboardKeyPress(KeysEnum.Enter, KeyPressMode.Click));

        var result = _keyboardController.Text(input);
        Assert.True(result is OkResult);
        
        _keyboardControlProvider.Verify(x => x.TextInput(input), Times.Once);
        _keyboardControlProvider.Verify(x => x.KeyboardKeyPress(KeysEnum.Enter, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void TextErrorTest()
    {
        var result = _keyboardController.Text(null!);
        Assert.True(result is ErrorResult);
    }

    [Fact]
    public void GetMethodsTest()
    {
        var methodNames = new[]
        {
            "back",
            "forth",
            "pause",
            "mediaback",
            "mediaforth",
            "mediavolumeup",
            "mediavolumedown",
            "mediamute",
            "text",
            "browserback",
            "browserforward"
        };

        var methods = _keyboardController.GetActions();
        Assert.True(methods.Count == methodNames.Length && methods.All(x => methodNames.Contains(x.Key)) && methods.All(
            x => x.Value.Method.ReturnType == typeof(IActionResult)));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}