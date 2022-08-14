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
        public string? Left(string _)
        {
            _input.KeyPress();

            return "done";
        }

        [Action("right")]
        public string? Right(string _)
        {
            _input.KeyPress(MouseKeysEnum.Right);

            return "done";
        }

        [Action("middle")]
        public string? Middle(string _)
        {
            _input.KeyPress(MouseKeysEnum.Middle);

            return "done";
        }

        [Action("up")]
        public string? DragUp(string _)
        {
            _input.Wheel(true);

            return "done";
        }

        [Action("down")]
        public string? DragDown(string _)
        {
            _input.Wheel(false);

            return "done";
        }

        [Action("dragstart")]
        public string? DragStart(string _)
        {
            _input.KeyPress(MouseKeysEnum.Left, KeyPressMode.Down);
            Task.Run(async () =>
            {
                await Task.Delay(5_000);
                _input.KeyPress(MouseKeysEnum.Left, KeyPressMode.Up);
            });

            return "done";
        }

        [Action("dragstop")]
        public string? DragStop(string _)
        {
            _input.KeyPress(MouseKeysEnum.Left, KeyPressMode.Up);

            return "done";
        }

        [Action("move")]
        public string? Move(string param)
        {
            if (Utils.TryGetCoords(param, out var x, out var y))
            {
                _input.Move(x, y);
            }

            return null;
        }
    }
}
