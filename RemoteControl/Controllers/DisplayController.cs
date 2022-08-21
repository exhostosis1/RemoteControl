using Shared.Controllers;
using Shared.Controllers.Attributes;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;

namespace RemoteControlApp.Controllers
{
    [Controller("display")]
    public class DisplayController: BaseController
    {
        private readonly IDisplayControlProvider _display;

        public DisplayController(IDisplayControlProvider display, ILogger logger) : base(logger)
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
