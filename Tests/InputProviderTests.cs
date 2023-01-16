using ControlProviders;
using Moq;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Tests;

public class InputProviderTests: IDisposable
{
    private readonly InputProvider _provider;
    private readonly ILogger<InputProvider> _logger;
    private readonly IKeyboardInput _keyboard;
    private readonly IMouseInput _mouse;
    private readonly IDisplayInput _display;
    private readonly IAudioInput _audio;


    public InputProviderTests()
    {
        _logger = Mock.Of<ILogger<InputProvider>>();
        _keyboard = Mock.Of<IKeyboardInput>();
        _mouse = Mock.Of<IMouseInput>();
        _display = Mock.Of<IDisplayInput>();
        _audio = Mock.Of<IAudioInput>();

        _provider = new InputProvider(_keyboard, _mouse, _display, _audio, _logger);
    }

    [Fact]
    public void DarkenTest()
    {
        _provider.DisplayOff();

        Mock.Get(_display).Verify(x => x.SetState(MonitorState.MonitorStateOff), Times.Once);
    }

    [Fact]
    public void KeyPressTest()
    {
        _provider.KeyboardKeyPress(KeysEnum.Enter, KeyPressMode.Click);

        Mock.Get(_keyboard).Verify(x => x.SendKey(KeysEnum.Enter, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void MouseKeyTest()
    {
        _provider.MouseKeyPress(MouseButtons.Middle, KeyPressMode.Click);

        Mock.Get(_mouse).Verify(x => x.SendMouseKey(MouseButtons.Middle, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void MouseMoveTest()
    {
        var x = 34;
        var y = 18;

        _provider.MouseMove(34, 18);

        Mock.Get(_mouse).Verify(input => input.SendMouseMove(x, y), Times.Once);
    }

    [Fact]
    public void WheelTest()
    {
        _provider.MouseWheel(false);
        
        Mock.Get(_mouse).Verify(x => x.SendScroll(-120), Times.Once);
    }

    [Fact]
    public void TextInputTest()
    {
        var text = "abcdefg";
        _provider.TextInput(text);

        Mock.Get(_keyboard).Verify(x => x.SendText(text), Times.Once);
    }

    [Fact]
    public void GetVolumeTest()
    {
        var result = _provider.GetVolume();

        Mock.Get(_audio).Verify(x => x.GetVolume(null), Times.Once);
    }

    [Fact]
    public void SetVolumeTest()
    {
        var volume = 34;
        _provider.SetVolume(volume);

        Mock.Get(_audio).Verify(x => x.SetVolume(volume, null), Times.Once);
    }

    [Fact]
    public void MuteTest()
    {
        _provider.Mute();

        Mock.Get(_audio).VerifySet(x => x.IsMute = true, Times.Once);
    }

    [Fact]
    public void UnmuteTest()
    {
        _provider.Unmute();

        Mock.Get(_audio).VerifySet(x => x.IsMute = false, Times.Once);
    }

    [Fact]
    public void IsMuteTest()
    {
        var result = _provider.IsMuted;
        Assert.False(result);

        Mock.Get(_audio).VerifyGet(x => x.IsMute, Times.Once);
    }

    [Fact]
    public void GetDevicesTest()
    {
        var result = _provider.GetAudioDevices();
        Assert.NotNull(result);
    }

    [Fact]
    public void SetDevicesTest()
    {
        var guid = Guid.NewGuid();
        _provider.SetAudioDevice(guid);

        Mock.Get(_audio).Verify(x => x.SetCurrentDevice(guid), Times.Once);
    }

    public void Dispose()
    {

    }
}