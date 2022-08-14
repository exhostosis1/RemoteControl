using RemoteControl.App.Enums;
using RemoteControl.App.Interfaces.Control;
using System.Net;
using RemoteControl.App.Web.Attributes;

namespace RemoteControl.App.Web.Controllers
{
    [Controller("keyboard")]
    internal class KeyboardController: BaseController
    {
        private readonly IKeyboardControl _input;

        public KeyboardController(IKeyboardControl input)
        {
            _input = input;
        }

        [Action("back")]
        public string? Back(string _)
        {
            _input.KeyPress(KeysEnum.ArrowLeft);

            return "done";
        }

        [Action("forth")]
        public string? Forth(string _)
        {
            _input.KeyPress(KeysEnum.ArrowRight);

            return "done";
        }

        [Action("pause")]
        public string? Pause(string _)
        {
            _input.KeyPress(KeysEnum.MediaPlayPause);

            return "done";
        }

        [Action("mediaback")]
        public string? MediaBack(string _)
        {
            _input.KeyPress(KeysEnum.MediaBack);

            return "done";
        }

        [Action("mediaforth")]
        public string? MediaForth(string _)
        {
            _input.KeyPress(KeysEnum.MediaForth);

            return "done";
        }

        [Action("text")]
        public string? TextInput(string param)
        {
            _input.TextInput(WebUtility.UrlDecode(param));
            _input.KeyPress(KeysEnum.Enter);

            return "done";
        }
    }
}
