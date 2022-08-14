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
        public void Back()
        {
            _input.KeyPress(KeysEnum.ArrowLeft);
        }

        [Action("forth")]
        public void Forth()
        {
            _input.KeyPress(KeysEnum.ArrowRight);
        }

        [Action("pause")]
        public void Pause()
        {
            _input.KeyPress(KeysEnum.MediaPlayPause);
        }

        [Action("mediaback")]
        public void MediaBack()
        {
            _input.KeyPress(KeysEnum.MediaBack);
        }

        [Action("mediaforth")]
        public void MediaForth()
        {
            _input.KeyPress(KeysEnum.MediaForth);
        }

        [Action("text")]
        public void TextInput(string param)
        {
            _input.TextInput(WebUtility.UrlDecode(param));
            _input.KeyPress(KeysEnum.Enter);
        }
    }
}
