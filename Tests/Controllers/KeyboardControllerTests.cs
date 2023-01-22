﻿using ApiControllers;
using Moq;
using Shared;
using Shared.ApiControllers.Results;
using Shared.ControlProviders.Provider;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Tests.Controllers;

public class KeyboardControllerTests : IDisposable
{
    private readonly KeyboardController _keyboardController;
    private readonly IKeyboardControlProvider _keyboardControlProvider;

    public KeyboardControllerTests()
    {
        var logger = Mock.Of<ILogger<KeyboardController>>();
        _keyboardControlProvider = Mock.Of<IKeyboardControlProvider>();
        _keyboardController = new KeyboardController(_keyboardControlProvider, logger);
    }

    [Fact]
    public void BackTest()
    {
        var result = _keyboardController.Back(null);
        Assert.True(result is OkResult);
        Mock.Get(_keyboardControlProvider).Verify(x => x.KeyboardKeyPress(KeysEnum.ArrowLeft, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void ForthTest()
    {
        var result = _keyboardController.Forth(null);
        Assert.True(result is OkResult);
        Mock.Get(_keyboardControlProvider).Verify(x => x.KeyboardKeyPress(KeysEnum.ArrowRight, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void MediaPrevTest()
    {
        var result = _keyboardController.MediaBack(null);
        Assert.True(result is OkResult);
        Mock.Get(_keyboardControlProvider).Verify(x => x.KeyboardKeyPress(KeysEnum.MediaPrev, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void MediaNextTest()
    {
        var result = _keyboardController.MediaForth(null);
        Assert.True(result is OkResult);
        Mock.Get(_keyboardControlProvider).Verify(x => x.KeyboardKeyPress(KeysEnum.MediaNext, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void MediaPauseTest()
    {
        var result = _keyboardController.Pause(null);
        Assert.True(result is OkResult);
        Mock.Get(_keyboardControlProvider).Verify(x => x.KeyboardKeyPress(KeysEnum.MediaPlayPause, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void VolumeUpTest()
    {
        var result = _keyboardController.MediaVolumeUp(null);
        Assert.True(result is OkResult);
        Mock.Get(_keyboardControlProvider).Verify(x => x.KeyboardKeyPress(KeysEnum.VolumeUp, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void VolumeDownTest()
    {
        var result = _keyboardController.MediaVolumeDown(null);
        Assert.True(result is OkResult);
        Mock.Get(_keyboardControlProvider).Verify(x => x.KeyboardKeyPress(KeysEnum.VolumeDown, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void MuteTest()
    {
        var result = _keyboardController.MediaMute(null);
        Assert.True(result is OkResult);
        Mock.Get(_keyboardControlProvider).Verify(x => x.KeyboardKeyPress(KeysEnum.Mute, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void TextTest()
    {
        var result = _keyboardController.Text(null);
        Assert.True(result is ErrorResult);

        result = _keyboardController.Text("param");
        Assert.True(result is OkResult);
        Mock.Get(_keyboardControlProvider).Verify(x => x.TextInput("param"), Times.Once);
        Mock.Get(_keyboardControlProvider).Verify(x => x.KeyboardKeyPress(KeysEnum.Enter, KeyPressMode.Click), Times.Once);
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
            "text"
        };

        var methods = _keyboardController.GetMethods();
        Assert.True(methods.Count == methodNames.Length && methods.All(x => methodNames.Contains(x.Key)) && methods.All(
            x => x.Value.Target == _keyboardController && x.Value.Method.ReturnType == typeof(IActionResult) &&
                 x.Value.Method.GetParameters().Length == 1 &&
                 x.Value.Method.GetParameters()[0].ParameterType == typeof(string)));
    }

    public void Dispose()
    {

    }
}