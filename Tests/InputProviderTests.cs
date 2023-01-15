using ControlProviders;
using Moq;
using Shared;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Tests;

public class InputProviderTests: IDisposable
{
    private readonly InputProvider _provider;
    private readonly ILogger<InputProvider> _logger;
    private readonly IInput _input;

    public InputProviderTests()
    {
        _logger = Mock.Of<ILogger<InputProvider>>();
        _input = Mock.Of<IInput>();
        _provider = new InputProvider(_input, _logger);
    }

    [Fact]
    public void DarkenTest()
    {
        _provider.Darken();

        Mock.Get(_input).Verify(x => x.SetMonitorInState(MonitorState.MonitorStateOff), Times.Once);
    }

    [Fact]
    public void KeyPressTest()
    {
        _provider.KeyPress(KeysEnum.Enter, KeyPressMode.Click);

        Mock.Get(_input).Verify(x => x.SendKeyInput(KeysEnum.Enter, false), Times.Once);
        Mock.Get(_input).Verify(x => x.SendKeyInput(KeysEnum.Enter, true), Times.Once);
    }

    [Fact]
    public void MouseKeyTest()
    {
        _provider.ButtonPress(MouseButtons.Middle, KeyPressMode.Click);

        Mock.Get(_input).Verify(x => x.SendMouseInput(MouseButtons.Middle, false), Times.Once);
        Mock.Get(_input).Verify(x => x.SendMouseInput(MouseButtons.Middle, true), Times.Once);
    }

    [Fact]
    public void MouseMoveTest()
    {
        var x = 34;
        var y = 18;

        _provider.Move(34, 18);

        Mock.Get(_input).Verify(input => input.SendMouseInput(x, y), Times.Once);
    }

    [Fact]
    public void WheelTest()
    {
        _provider.Wheel(false);
        
        Mock.Get(_input).Verify(x => x.SendScrollInput(-120), Times.Once);
    }

    [Fact]
    public void TextInput()
    {
        var text = "abcdefg";
        _provider.TextInput(text);

        Mock.Get(_input).Verify(x => x.SendCharInput(It.IsAny<char>(), It.IsAny<bool>()), Times.Exactly(text.Length * 2));
    }

    public void Dispose()
    {

    }
}