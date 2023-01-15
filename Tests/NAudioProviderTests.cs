using ControlProviders;
using Moq;
using Shared.AudioWrapper;
using Shared.Logging.Interfaces;

namespace Tests;

public class NAudioProviderTests: IDisposable
{
    private readonly AudioProvider _provider;
    private readonly ILogger<AudioProvider> _logger;
    private readonly MockAudioEnumerator _enumerator;

    private class MockMMDevice: IMMDevice
    {
        public string ID { get; init;  }
        public string DeviceFriendlyName { get; init; }
        public IAudioEndpointVolume AudioEndpointVolume { get; init; }
    }

    private class MockAudioEndpointVolume: IAudioEndpointVolume
    {
        public float MasterVolumeLevelScalar { get; set; }
        public bool Mute { get; set; }
    }

    private class MockAudioEnumerator: IAudioDeviceEnumerator
    {
        private readonly List<MockMMDevice> _devices;

        public MockAudioEnumerator(List<MockMMDevice> devices)
        {
            _devices = devices;
        }

        public IEnumerable<IMMDevice> EnumerateAudioEndPoints(DataFlow flow, DeviceState state)
        {
            return _devices;
        }

        public IMMDevice GetDefaultAudioEndpoint(DataFlow flow, Role role)
        {
            return _devices.First();
        }
    }

    private readonly List<MockMMDevice> Devices = new()
    {
        new MockMMDevice
        {
            ID = Guid.NewGuid().ToString(),
            DeviceFriendlyName = "Device1",
            AudioEndpointVolume = new MockAudioEndpointVolume
            {
                MasterVolumeLevelScalar = 0.25f,
                Mute = true
            }
        },
        new MockMMDevice
        {
            ID = Guid.NewGuid().ToString(),
            DeviceFriendlyName = "Device2",
            AudioEndpointVolume = new MockAudioEndpointVolume
            {
                MasterVolumeLevelScalar = 0.75f,
                Mute = false
            }
        }
    };

    public NAudioProviderTests()
    {
        _logger = Mock.Of<ILogger<AudioProvider>>();
        _enumerator = new MockAudioEnumerator(Devices);
        _provider = new AudioProvider(_enumerator, _logger);
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

    [Fact]
    public void MuteTest()
    {
        _provider.Mute();

        Assert.True(_provider.IsMuted);
    }

    [Fact]
    public void UnmuteTest()
    {
        _provider.Unmute();

        Assert.False(_provider.IsMuted);
    }

    [Fact]
    public void GetDevicesTest()
    {
        var devices = _provider.GetDevices().ToList();

        Assert.True(devices.Count == Devices.Count);

        for (var i = 0; i < devices.Count; i++)
        {
            Assert.True(devices[i].Name == Devices[i].DeviceFriendlyName);
            Assert.True(devices[i].Id == new Guid(Devices[i].ID));
        }
    }

    [Fact]
    public void SetCurrentDeviceTest()
    {
        var deviceId = new Guid(Devices[1].ID);

        _provider.SetCurrentControlDevice(deviceId);
        var volume = _provider.GetVolume();

        Assert.True(volume == 75);
    }

    [Fact]
    public void IsMuteTest()
    {
        var result = _provider.IsMuted;
        Assert.True(result);
    }
    

    public void Dispose()
    {

    }
}