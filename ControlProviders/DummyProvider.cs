using ControlProviders.Devices;
using Shared.ControlProviders;
using Shared.ControlProviders.Devices;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace ControlProviders;

public class DummyProvider: IKeyboardControlProvider, IMouseControlProvider, IDisplayControlProvider, IAudioControlProvider
{
    private readonly IAudioDevice[] _dummyDevices;
    private readonly ILogger<DummyProvider> _logger;

    public DummyProvider(ILogger<DummyProvider> logger)
    {
        _logger = logger;

        _dummyDevices = new IAudioDevice[]
        {
            new AudioDevice
            {
                Id = new Guid(),
                IsCurrentControlDevice = true,
                Name = "device 1"
            },
            new AudioDevice
            {
                Id = new Guid(),
                IsCurrentControlDevice = false,
                Name = "device 2"
            }
        };
    }

    public void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click)
    {
        _logger.LogInfo($"Dummy KeyPress {key}");
    }

    public void TextInput(string text)
    {
        _logger.LogInfo($"Dummy Text '{text}'");
    }

    public void Move(int x, int y)
    {
        _logger.LogInfo($"Dummy MouseMove {x} {y}");
    }

    public void ButtonPress(MouseButtons key = MouseButtons.Left, KeyPressMode mode = KeyPressMode.Click)
    {
        _logger.LogInfo($"Dummy MouseButtonPress {key}");
    }

    public void Wheel(bool up)
    {
        _logger.LogInfo($"Dummy MouseWheel {(up ? "up" : "down")}");
    }

    public void Darken()
    {
        _logger.LogInfo($"Dummy DisplayDarken");
    }
        
    public int GetVolume()
    {
        _logger.LogInfo($"Dummy GetVolume");
        return 56;
    }

    public void SetVolume(int volume)
    {
        _logger.LogInfo($"Dummy SetVolume {volume}");
    }

    public void Mute()
    {
        IsMuted = true;
        _logger.LogInfo($"Dummy Mute");
    }

    public void Unmute()
    {
        IsMuted = false;
        _logger.LogInfo("Dummy Unmute");
    }

    public bool IsMuted { get; private set; } = false;

    public IReadOnlyCollection<IAudioDevice> GetDevices()
    {
        _logger.LogInfo($"Dummy GetDevices");
        return _dummyDevices;
    }

    public IReadOnlyCollection<IAudioDevice> SetCurrentControlDevice(Guid id)
    {
        _logger.LogInfo($"Dummy SetDevice {id}");
        return _dummyDevices;
    }
}