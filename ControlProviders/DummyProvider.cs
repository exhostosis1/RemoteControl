using ControlProviders.Abstract;
using ControlProviders.Devices;
using Shared.ControlProviders;
using Shared.ControlProviders.Devices;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace ControlProviders
{
    public class DummyProvider: BaseProvider, IKeyboardControlProvider, IMouseControlProvider, IDisplayControlProvider, IAudioControlProvider
    {
        private readonly IAudioDevice[] _dummyDevices;

        public DummyProvider(ILogger logger) : base(logger)
        {
            _dummyDevices = new IAudioDevice[]
            {
                new AudioDevice()
                {
                    Id = new Guid(),
                    IsCurrentControlDevice = true,
                    Name = "device 1"
                },
                new AudioDevice()
                {
                    Id = new Guid(),
                    IsCurrentControlDevice = false,
                    Name = "device 2"
                }
            };
        }

        public void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click)
        {
            Logger.LogInfo($"Dummy KeyPress {key}");
        }

        public void TextInput(string text)
        {
            Logger.LogInfo($"Dummy Text '{text}'");
        }

        public void Move(int x, int y)
        {
            Logger.LogInfo($"Dummy MouseMove {x} {y}");
        }

        public void ButtonPress(MouseButtons key = MouseButtons.Left, KeyPressMode mode = KeyPressMode.Click)
        {
            Logger.LogInfo($"Dummy MouseButtonPress {key}");
        }

        public void Wheel(bool up)
        {
            Logger.LogInfo($"Dummy MouseWheel {(up ? "up" : "down")}");
        }

        public void Darken()
        {
            Logger.LogInfo($"Dummy DisplayDarken");
        }
        
        public int GetVolume()
        {
            Logger.LogInfo($"Dummy GetVolume");
            return 56;
        }

        public void SetVolume(int volume)
        {
            Logger.LogInfo($"Dummy SetVolume {volume}");
        }

        public void Mute()
        {
            Logger.LogInfo($"Dummy Mute");
        }

        public void Unmute()
        {
            Logger.LogInfo("Dummy Unmute");
        }

        public IReadOnlyCollection<IAudioDevice> GetDevices()
        {
            Logger.LogInfo($"Dummy GetDevices");
            return _dummyDevices;
        }

        public IReadOnlyCollection<IAudioDevice> SetCurrentControlDevice(Guid id)
        {
            Logger.LogInfo($"Dummy SetDevice {id}");
            return _dummyDevices;
        }
    }
}
