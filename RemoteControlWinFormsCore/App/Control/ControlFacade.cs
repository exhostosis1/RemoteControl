using RemoteControl.App.Control.Interfaces;
using RemoteControl.App.Control.Wrappers;
using RemoteControl.App.Enums;

namespace RemoteControl.App.Control
{
    internal static class ControlFacade
    {
        private static readonly IControlAudio Audio = new AudioSwitchWrapper();
        private static readonly IControlInput Input = new WindowsInputLibWrapper();
        private static readonly IControlDisplay Display = new User32Wrapper();

        public static IEnumerable<IAudioDevice> GetDevices() => Audio.GetDevices();
        public static void SetDevice(Guid id) => Audio.SetDevice(id);
        public static int GetVolume() => Audio.Volume;
        public static void SetVolume(int volume) => Audio.Volume = volume;
        public static void Mute(bool mute) => Audio.Mute(mute);
        public static void KeyboardKeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click) => Input.KeyPress(key, mode);
        public static void MouseKeyPress(MouseKeysEnum key, KeyPressMode mode = KeyPressMode.Click) => Input.MouseKeyPress(key, mode);
        public static void TextInput(string text) => Input.TextInput(text);
        public static void MouseWheel(bool up) => Input.MouseWheel(up);
        public static void MouseMove(int x, int y) => Input.MouseMove(x, y);
        public static void DisplayDarken() => Display.Darken();
    }
}
