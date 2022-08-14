using RemoteControl.App.Enums;
using RemoteControl.App.Interfaces.Control;

namespace RemoteControl.App.Control
{
    internal class ControlFacade
    {
        private readonly IControlAudio _audio;
        private readonly IControlInput _input;
        private readonly IControlDisplay _display;

        public ControlFacade(IControlAudio audio, IControlInput input, IControlDisplay display)
        {
            _audio = audio;
            _input = input;
            _display = display;
        }

        public IEnumerable<IAudioDevice> GetDevices() => _audio.GetDevices();
        public void SetDevice(Guid id) => _audio.SetDevice(id);
        public int GetVolume() => _audio.Volume;
        public void SetVolume(int volume) => _audio.Volume = volume;
        public void Mute(bool mute) => _audio.Mute(mute);
        public void KeyboardKeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click) => _input.KeyPress(key, mode);
        public void MouseKeyPress(MouseKeysEnum key, KeyPressMode mode = KeyPressMode.Click) => _input.MouseKeyPress(key, mode);
        public void TextInput(string text) => _input.TextInput(text);
        public void MouseWheel(bool up) => _input.MouseWheel(up);
        public void MouseMove(int x, int y) => _input.MouseMove(x, y);
        public void DisplayDarken() => _display.Darken();
    }
}
