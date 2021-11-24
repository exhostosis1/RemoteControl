using RemoteControl.App.Control.Interfaces;
using RemoteControl.App.Control.Wrappers;
using RemoteControl.App.Enums;

namespace RemoteControl.App.Control
{
    internal static class ControlFacade
    {
        private static readonly IControlAudio _audio = new AudioSwitchWrapper();
        private static readonly IControlInput _input = new WindowsInputLibWrapper();
        private static readonly IControlDisplay _display = new User32Wrapper();

        public static int GetVolume() => _audio.Volume;
        public static int SetVolume(int volume) => _audio.Volume = volume;
        public static void Mute(bool mute) => _audio.Mute(mute);
        public static void KeyboardKeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click) => _input.KeyPress(key, mode);
        public static void MouseKeyPress(MouseKeysEnum key, KeyPressMode mode = KeyPressMode.Click) => _input.MouseKeyPress(key, mode);
        public static void TextInput(string text) => _input.TextInput(text);
        public static void MouseWheel(bool up) => _input.MouseWheel(up);
        public static void MouseMove(int x, int y) => _input.MouseMove(x, y);
        public static void DisplayDarken() => _display.Darken();
    }
}
