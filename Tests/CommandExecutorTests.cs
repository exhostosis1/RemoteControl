using Bots;
using Moq;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Tests;

public class CommandExecutorTests: IDisposable
{
    private readonly ILogger<CommandsExecutor> _logger;
    private readonly ControlFacade _controlFacade;
    private readonly IAudioControlProvider _audio;
    private readonly IMouseControlProvider _mouse;
    private readonly IKeyboardControlProvider _keyboard;
    private readonly IDisplayControlProvider _display;

    public CommandExecutorTests()
    {
        _logger = Mock.Of<ILogger<CommandsExecutor>>();

        _audio = Mock.Of<IAudioControlProvider>();
        _keyboard = Mock.Of<IKeyboardControlProvider>();
        _display = Mock.Of<IDisplayControlProvider>();
        _mouse = Mock.Of<IMouseControlProvider>();

        _controlFacade = new ControlFacade(_audio, _keyboard, _mouse, _display);
    }

    [Fact]
    public void ExecuteTest()
    {
        var executor = new CommandsExecutor(_controlFacade, _logger);

        var result = executor.Execute(BotButtons.Darken);
        Assert.True(result == "done");
        Mock.Get(_display).Verify(x => x.Darken());

        result = executor.Execute(BotButtons.MediaBack);
        Assert.True(result == "done");
        Mock.Get(_keyboard).Verify(x => x.KeyPress(KeysEnum.MediaPrev, KeyPressMode.Click));

        result = executor.Execute(BotButtons.MediaForth);
        Assert.True(result == "done");
        Mock.Get(_keyboard).Verify(x => x.KeyPress(KeysEnum.MediaNext, KeyPressMode.Click));

        result = executor.Execute(BotButtons.Pause);
        Assert.True(result == "done");
        Mock.Get(_keyboard).Verify(x => x.KeyPress(KeysEnum.MediaPlayPause, KeyPressMode.Click));

        result = executor.Execute(BotButtons.VolumeUp);
        var intResult = int.Parse(result);
        Assert.True(intResult is >= 0 and <= 100);
        var result1 = intResult;
        Mock.Get(_audio).Verify(x => x.SetVolume(result1));

        result = executor.Execute(BotButtons.VolumeDown);
        intResult = int.Parse(result);
        Assert.True(intResult is >= 0 and <= 100);
        Mock.Get(_audio).Verify(x => x.SetVolume(intResult));
    }

    public void Dispose()
    {
        
    }
}