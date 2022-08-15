using RemoteControlApp.Web.Attributes;
using Shared;
using Shared.Enums;
using Shared.Interfaces.Control;
using Shared.Interfaces.Logging;

namespace RemoteControlApp.Web.Controllers
{
    [Controller("mouse")]
    public class MouseController : BaseController
    {
        private readonly IMouseControl _input;

        public MouseController(IMouseControl input, ILogger logger) : base(logger)
        {
            _input = input;
        }

        [Action("left")]
        public string? Left(string _)
        {
            _input.ButtonPress();

            return "done";
        }

        [Action("right")]
        public string? Right(string _)
        {
            _input.ButtonPress(MouseKeysEnum.Right);

            return "done";
        }

        [Action("middle")]
        public string? Middle(string _)
        {
            _input.ButtonPress(MouseKeysEnum.Middle);

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
            _input.ButtonPress(MouseKeysEnum.Left, KeyPressMode.Down);
            Task.Run(async () =>
            {
                await Task.Delay(5_000);
                _input.ButtonPress(MouseKeysEnum.Left, KeyPressMode.Up);
            });

            return "done";
        }

        [Action("dragstop")]
        public string? DragStop(string _)
        {
            _input.ButtonPress(MouseKeysEnum.Left, KeyPressMode.Up);

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
