using RemoteControl.App.Interfaces.Control;
using RemoteControl.App.Web.Attributes;

namespace RemoteControl.App.Web.Controllers
{
    [Controller("display")]
    internal class DisplayController: BaseController
    {
        private readonly IDisplayControl _display;

        public DisplayController(IDisplayControl display)
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
