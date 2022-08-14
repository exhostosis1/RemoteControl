using RemoteControl.App.Enums;
using RemoteControl.App.Interfaces.Control;
using RemoteControl.App.Web.Attributes;

namespace RemoteControl.App.Web.Controllers
{
    [Controller("mouse")]
    internal class MouseController : BaseController
    {
        private readonly IMouseControl _input;

        public MouseController(IMouseControl input)
        {
            _input = input;
        }

        [Action("left")]
        public void Left()
        {
            _input.KeyPress();
        }

        [Action("right")]
        public void Right()
        {
            _input.KeyPress(MouseKeysEnum.Right);
        }

        [Action("middle")]
        public void Middle()
        {
            _input.KeyPress(MouseKeysEnum.Middle);
        }

        [Action("up")]
        public void DragUp()
        {
            _input.Wheel(true);
        }

        [Action("down")]
        public void DragDown()
        {
            _input.Wheel(false);
        }

        [Action("dragstart")]
        public void DragStart()
        {
            _input.KeyPress(MouseKeysEnum.Left, KeyPressMode.Down);
            Task.Run(async () =>
            {
                await Task.Delay(5_000);
                _input.KeyPress(MouseKeysEnum.Left, KeyPressMode.Up);
            });
        }

        [Action("dragstop")]
        public void DragStop()
        {
            _input.KeyPress(MouseKeysEnum.Left, KeyPressMode.Up);
        }

        [Action("move")]
        public void Move(string param)
        {
            if (Utils.TryGetCoords(param, out var x, out var y))
            {
                _input.Move(x, y);
            }
        }
    }
}
