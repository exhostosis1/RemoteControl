using Microsoft.Extensions.Logging;
using Moq;
using Servers.ApiControllers;
using Servers.Middleware;
using Shared.ControlProviders.Devices;
using Shared.ControlProviders.Provider;
using System.Text.Json;
using Servers.Results;

namespace UnitTests.Controllers;

public class AudioControllerTests : IDisposable
{
    private readonly AudioController _audioController;
    private readonly Mock<IAudioControlProvider> _audioControlProvider;
    private readonly ILogger _logger;

    public AudioControllerTests()
    {
        _logger = Mock.Of<ILogger>();
        _audioControlProvider = new Mock<IAudioControlProvider>(MockBehavior.Strict);
        _audioController = new AudioController(_audioControlProvider.Object, _logger);
    }

    private class MockAudioDevice : IAudioDevice
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsCurrentControlDevice { get; set; }
    }

    [Fact]
    public void GetDeviceTest()
    {
        var devices = new List<IAudioDevice>
        {
            new MockAudioDevice
            {
                Id = Guid.NewGuid(),
                IsCurrentControlDevice = true,
                Name = "device1"
            },
            new MockAudioDevice
            {
                Id = Guid.NewGuid(),
                IsCurrentControlDevice = false,
                Name = "device2"
            }
        };

        _audioControlProvider.Setup(x => x.GetAudioDevices()).Returns(devices);

        var result = _audioController.GetDevices();

        var expectedJson = JsonSerializer.Serialize(devices);

        Assert.True(result is JsonResult && result.Result == expectedJson);

        _audioControlProvider.Verify(x => x.GetAudioDevices(), Times.Once);
    }

    [Fact]
    public void SetDeviceTest()
    {
        var validGuid1 = Guid.NewGuid().ToString();
        var validGuid2 = Guid.NewGuid().ToString();

        var invalidGuid = "guid";

        _audioControlProvider.Setup(x => x.SetAudioDevice(It.IsAny<Guid>()));

        var result = _audioController.SetDevice(validGuid1);
        Assert.True(result is OkResult);

        result = _audioController.SetDevice(validGuid2);
        Assert.True(result is OkResult);

        result = _audioController.SetDevice(invalidGuid);
        Assert.True(result is ErrorResult {Result: "No such device"});

        _audioControlProvider.Verify(x => x.SetAudioDevice(It.IsAny<Guid>()), Times.Exactly(2));
    }

    [Theory]
    [InlineData(23)]
    [InlineData(45)]
    [InlineData(100)]
    public void GetVolumeTest(int value)
    {
        _audioControlProvider.Setup(x => x.GetVolume()).Returns(value);

        var result = _audioController.GetVolume();
        Assert.True(result is TextResult && result.Result == value.ToString());

        _audioControlProvider.Verify(x => x.GetVolume(), Times.Once);
    }

    [Theory]
    [InlineData("15")]
    [InlineData("140")]
    [InlineData("-45")]
    public void SetVolumeTest(string input)
    {
        var expectedInput = int.Parse(input);
        expectedInput = expectedInput > 100 ? 100 : expectedInput < 0 ? 0 : expectedInput;

        _audioControlProvider.Setup(x => x.SetVolume(It.IsInRange(0, 100, Moq.Range.Inclusive)));

        var result = _audioController.SetVolume(input);

        Assert.True(result is TextResult && result.Result == expectedInput.ToString());
        _audioControlProvider.Verify(x => x.SetVolume(expectedInput), Times.Once);
    }

    [Fact]
    public void SetVolumeFailTest()
    {
        const string input = "abasd";

        var result = _audioController.SetVolume(input);
        Assert.True(result is ErrorResult{Result: "Wrong volume format" });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(15)]
    [InlineData(90)]
    [InlineData(100)]
    public void IncreaseBy5Test(int volume)
    {
        _audioControlProvider.Setup(x => x.GetVolume()).Returns(volume);
        _audioControlProvider.Setup(x => x.SetVolume(It.IsInRange(0, 100, Moq.Range.Inclusive)));

        var expectedVolume = volume + 5;
        expectedVolume = expectedVolume > 100 ? 100 : expectedVolume < 0 ? 0 : expectedVolume;

        var result = _audioController.IncreaseBy5();
        Assert.True(result is TextResult && result.Result == expectedVolume.ToString());

        _audioControlProvider.Verify(x => x.GetVolume(), Times.Once);
        _audioControlProvider.Verify(x => x.SetVolume(It.IsInRange(0, 100, Moq.Range.Inclusive)), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(15)]
    [InlineData(90)]
    [InlineData(100)]
    public void DecreaseBy5Test(int volume)
    {
        _audioControlProvider.Setup(x => x.GetVolume()).Returns(volume);
        _audioControlProvider.Setup(x => x.SetVolume(It.IsInRange(0, 100, Moq.Range.Inclusive)));

        var expectedVolume = volume - 5;
        expectedVolume = expectedVolume > 100 ? 100 : expectedVolume < 0 ? 0 : expectedVolume;

        var result = _audioController.DecreaseBy5();
        Assert.True(result is TextResult && result.Result == expectedVolume.ToString());

        _audioControlProvider.Verify(x => x.GetVolume(), Times.Once);
        _audioControlProvider.Verify(x => x.SetVolume(It.IsInRange(0, 100, Moq.Range.Inclusive)), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MuteTest(bool value)
    {
        _audioControlProvider.SetupGet(x => x.IsMuted).Returns(value);
        _audioControlProvider.Setup(x => x.Mute());
        _audioControlProvider.Setup(x => x.Unmute());

        var result = _audioController.Mute();
        Assert.True(result is OkResult);

        _audioControlProvider.Verify(x => x.Mute(), value ? Times.Never : Times.Once);
        _audioControlProvider.Verify(x => x.Unmute(), value ? Times.Once : Times.Never);
    }

    [Fact]
    public void GetMethodsTest()
    {
        var methodNames = new[]
        {
            "getvolume",
            "setvolume",
            "getdevices",
            "setdevice",
            "increaseby5",
            "decreaseby5",
            "mute"
        };

        var methods = _audioController.GetActions();
        Assert.True(methods.Count == methodNames.Length && methods.All(x => methodNames.Contains(x.Key)) && methods.All(
            x => x.Value.Method.ReturnType == typeof(IActionResult)));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}