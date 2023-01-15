using ControlProviders;
using Moq;
using Shared.AudioWrapper;
using Shared.Logging.Interfaces;

namespace Tests;

public class NAudioProviderTests: IDisposable
{
    private readonly NAudioProvider _provider;
    private readonly ILogger<NAudioProvider> _logger;
    private readonly MockAudioEnumerator _enumerator;

    private class MockMMDevice: IMMDevice
    {
        public string ID { get; init;  }
        public string DeviceFriendlyName { get; init; }
        public IAudioEndpointVolume AudioEndpointVolume { get; init; }
    }

    private class MockAudioEndpointVolume: IAudioEndpointVolume
    {
        public float MasterVolumeLevelScalar { get; set; } = 0.25f;
        public bool Mute { get; set; }
    }

    private class MockAudioEnumerator: IAudioDeviceEnumerator
    {
        public IEnumerable<IMMDevice> EnumerateAudioEndPoints(DataFlow flow, DeviceState state)
        {
            return Devices;
        }

        public IMMDevice GetDefaultAudioEndpoint(DataFlow flow, Role role)
        {
            return Devices.First();
        }
    }

    private static readonly List<MockMMDevice> Devices = new()
    {
        new MockMMDevice
        {
            ID = Guid.NewGuid().ToString(),
            DeviceFriendlyName = "Device1",
            AudioEndpointVolume = new MockAudioEndpointVolume()
        },
        new MockMMDevice
        {
            ID = Guid.NewGuid().ToString(),
            DeviceFriendlyName = "Device2",
            AudioEndpointVolume = new MockAudioEndpointVolume()
        }
    };

    public NAudioProviderTests()
    {
        _logger = Mock.Of<ILogger<NAudioProvider>>();
        _enumerator = new MockAudioEnumerator();
        _provider = new NAudioProvider(_enumerator, _logger);
    }

    [Fact]

    public void GetVolumeTest()
    {
        var volume = _provider.GetVolume();

        Assert.True(volume == 25);
        Mock.Get(_logger).Verify(x => x.LogInfo("Getting volume"), Times.Once);
    }

    [Fact]
    public void SetVolumeTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _provider.SetVolume(-5));
        Assert.Throws<ArgumentOutOfRangeException>(() => _provider.SetVolume(105));

        var volume = 59;
        _provider.SetVolume(volume);

        Assert.True(Math.Abs(Devices.First().AudioEndpointVolume.MasterVolumeLevelScalar * 100 - volume) < 0.01);
        Mock.Get(_logger).Verify(x => x.LogInfo($"Setting volume to {volume}"), Times.Once);
    }
    

    public void Dispose()
    {

    }
}