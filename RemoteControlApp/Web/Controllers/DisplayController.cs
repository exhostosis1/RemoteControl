using RemoteControlApp.Web.Attributes;
using Shared.Interfaces.Control;
using Shared.Interfaces.Logging;

namespace RemoteControlApp.Web.Controllers
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
