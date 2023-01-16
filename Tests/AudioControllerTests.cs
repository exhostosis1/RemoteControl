using ApiControllers;
using Moq;
using Shared.ApiControllers.Results;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;

namespace Tests;

public class AudioControllerTests: IDisposable
{
    private readonly AudioController _audioController;
    private readonly IControlProvider _audioControlProvider;

    public AudioControllerTests()
    {
        var logger = Mock.Of<ILogger<AudioController>>();
        _audioControlProvider = Mock.Of<IControlProvider>();
        _audioController = new AudioController(_audioControlProvider, logger);
    }

    [Fact]
    public void GetDeviceTest()
    {
        var result = _audioController.GetDevices(null);
        Assert.True(result is JsonResult);
        Mock.Get(_audioControlProvider).Verify(x => x.GetAudioDevices(), Times.Once);
    }

    [Fact]
    public void SetDeviceTest()
    {
        var result = _audioController.SetDevice("param");
        Assert.True(result is ErrorResult);

        result = _audioController.SetDevice(Guid.NewGuid().ToString());
        Assert.True(result is OkResult);
        Mock.Get(_audioControlProvider).Verify(x => x.SetAudioDevice(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public void GetVolumeTest()
    {
        var result = _audioController.GetVolume(null);
        Assert.True(result is StringResult);
        Mock.Get(_audioControlProvider).Verify(x => x.GetVolume(), Times.Once);
    }

    [Fact]
    public void SetVolumeTest()
    {
        var result = _audioController.SetVolume("15");
        Assert.True(result is StringResult { Result: "15" });

        result = _audioController.SetVolume("abdsd");
        Assert.True(result is ErrorResult);

        result = _audioController.SetVolume("140");
        Assert.True(result is StringResult { Result: "100" });

        result = _audioController.SetVolume("-45");
        Assert.True(result is StringResult { Result: "0" });

        Mock.Get(_audioControlProvider).Verify(x => x.SetVolume(It.IsAny<int>()), Times.Exactly(3));

    }

    [Fact]
    public void IncreaseBy5Test()
    {
        var result = _audioController.IncreaseBy5(null);
        Assert.True(result is StringResult { Result: "5" });

        Mock.Get(_audioControlProvider).Verify(x => x.GetVolume(), Times.Once);
        Mock.Get(_audioControlProvider).Verify(x => x.SetVolume(5), Times.Once);
    }

    [Fact]
    public void DecreaseBy5Test()
    {
        var result = _audioController.DecreaseBy5(null);
        Assert.True(result is StringResult { Result: "0" });

        Mock.Get(_audioControlProvider).Verify(x => x.GetVolume(), Times.Once);
        Mock.Get(_audioControlProvider).Verify(x => x.SetVolume(0), Times.Once);
    }

    [Fact]
    public void MuteTest()
    {
        var result = _audioController.Mute(null);

        Mock.Get(_audioControlProvider).VerifyGet(x => x.IsMuted, Times.Once);
        Mock.Get(_audioControlProvider).Verify(x => x.Mute(), Times.Once);
    }


    public void Dispose()
    {

    }
}