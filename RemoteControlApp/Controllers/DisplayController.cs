using Shared.Control;
using Shared.Controllers;
using Shared.Controllers.Attributes;
using Shared.Logging.Interfaces;

namespace RemoteControlApp.Controllers
{
    [Controller("display")]
    public class DisplayController: BaseController
    {
        private readonly IDisplayControl _display;

        public DisplayController(IDisplayControl display, ILogger logger) : base(logger)
        {
            _display = display;
        }

        [Action("darken")]
        public string? DisplayControl(string _)
        {
            _display.Darken();

            return "done";
        }
    }
}
