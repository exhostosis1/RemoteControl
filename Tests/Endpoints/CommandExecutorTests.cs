using Bots;
using Moq;
using Shared.ControlProviders;
using Shared.DataObjects.Bot;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Tests.Endpoints;

public class CommandExecutorTests : IDisposable
{
    private readonly ILogger<CommandsExecutor> _logger;
    private readonly IControlProvider _provider;

    public CommandExecutorTests()
    {
        _logger = Mock.Of<ILogger<CommandsExecutor>>();
        _provider = Mock.Of<IControlProvider>();
    }

    [Fact]
    public void ExecuteTest()
    {
        var executor = new CommandsExecutor(_provider, _logger);

        var context = new BotContext
        {
            Request = new BotContextRequest
            {
                Command = BotButtons.Darken
            }
        };
        executor.ProcessRequest(context);
        Assert.True(context.Response.Result == "done");
        Mock.Get(_provider).Verify(x => x.DisplayOff());

        result = executor.Execute(BotButtons.MediaBack);
        Assert.True(result == "done");
        Mock.Get(_provider).Verify(x => x.KeyboardKeyPress(KeysEnum.MediaPrev, KeyPressMode.Click));

        result = executor.Execute(BotButtons.MediaForth);
        Assert.True(result == "done");
        Mock.Get(_provider).Verify(x => x.KeyboardKeyPress(KeysEnum.MediaNext, KeyPressMode.Click));

        result = executor.Execute(BotButtons.Pause);
        Assert.True(result == "done");
        Mock.Get(_provider).Verify(x => x.KeyboardKeyPress(KeysEnum.MediaPlayPause, KeyPressMode.Click));

        result = executor.Execute(BotButtons.VolumeUp);
        var intResult = int.Parse(result);
        Assert.True(intResult is >= 0 and <= 100);
        var result1 = intResult;
        Mock.Get(_provider).Verify(x => x.SetVolume(result1));

        result = executor.Execute(BotButtons.VolumeDown);
        intResult = int.Parse(result);
        Assert.True(intResult is >= 0 and <= 100);
        Mock.Get(_provider).Verify(x => x.SetVolume(intResult));
    }

    public void Dispose()
    {

    }
}