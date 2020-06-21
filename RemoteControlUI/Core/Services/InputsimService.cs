using System;
using RemoteControl.Core.Enums;
using RemoteControl.Core.Interfaces;
using RemoteControl.Core.Providers;

namespace RemoteControl.Core.Services
{
    internal class InputsimService : IInputService
    {
        private readonly IInputProvider _inputSim;

        public InputsimService()
        {
            _inputSim = new MyInputProvider();
        }

        public void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click)
        {
            switch (mode)
            {
                case KeyPressMode.Click:
                    _inputSim.Keyboard.KeyPress(key);
                    break;
                case KeyPressMode.Up:
                    _inputSim.Keyboard.KeyUp(key);
                    break;
                case KeyPressMode.Down:
                    _inputSim.Keyboard.KeyDown(key);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public void MouseKeyPress(MouseKeysEnum key = MouseKeysEnum.Left, KeyPressMode mode = KeyPressMode.Click)
        { 
            switch (mode)
            {
                case KeyPressMode.Click:
                    _inputSim.Mouse.MouseButtonClick(key);
                    break;
                case KeyPressMode.Down:
                    _inputSim.Mouse.MouseButtonDown(key);
                    break;
                case KeyPressMode.Up:
                    _inputSim.Mouse.MouseButtonUp(key);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public void MouseMove(ICoordinates coords)
        {
            _inputSim.Mouse.MoveMouseBy(coords.X, coords.Y);
        }

        public void MouseWheel(bool up)
        {
            _inputSim.Mouse.VerticalScroll(up ? 1 : -1);
        }

        public void TextInput(string text)
        {
            _inputSim.Keyboard.TextEntry(text);
        }
    }
}
