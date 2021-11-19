using RemoteControl.App.Enums;
using RemoteControl.App.Control.Interfaces;
using RemoteControl.App.Control.Wrappers;
using RemoteControl.App.Utility;

namespace RemoteControl.App.Control
{
    internal class ControlFacade
    {
        private readonly IControlAudio _audio = new AudioSwitchWrapper();
        private readonly IControlInput _input = new WindowsInputLibWrapper();

        public int GetVolume() => _audio.Volume;
        public int SetVolume(int volume) => _audio.Volume = volume;
        public void Mute(bool mute) => _audio.Mute(mute);
        public void KeyboardKeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click) => _input.KeyPress(key, mode);
        public void MouseKeyPress(MouseKeysEnum key, KeyPressMode mode = KeyPressMode.Click) => _input.MouseKeyPress(key, mode);
        public void TextInput(string text) => _input.TextInput(text);
        public void MouseWheel(bool up) => _input.MouseWheel(up);
        public void MouseMove(MyPoint point) => _input.MouseMove(point);
    }
}
