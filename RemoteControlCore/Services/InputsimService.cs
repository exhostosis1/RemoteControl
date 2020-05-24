using RemoteControlCore.Enums;
using RemoteControlCore.Interfaces;

namespace RemoteControlCore.Services
{
    internal class InputsimService : IInputService
    {
        private readonly IInputProvider _inputSim;

        public InputsimService()
        {
            _inputSim = DependencyFactory.Factory.GetInstance<IInputProvider>();
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
