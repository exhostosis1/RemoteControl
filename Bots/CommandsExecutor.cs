using Shared;
using Shared.Controllers;

namespace Bots;

public class CommandsExecutor
{
    private readonly ControllersWithMethods _methods;

    public CommandsExecutor(IEnumerable<BaseController> controllers)
    {
        _methods = Utils.GetControllersMethods(controllers);
    }

    public IEnumerable<string?> Execute(IEnumerable<string?> commands)
    {
        foreach (var command in commands)
        {
            switch (command)
            {
                case Buttons.MediaBack:
                    yield return _methods[MethodNames.KeyboardControllerName][MethodNames.KeyboardMediaBack](null);
                    break;
                case Buttons.MediaForth:
                    yield return _methods[MethodNames.KeyboardControllerName][MethodNames.KeyboardMediaForth](null);
                    break;
                case Buttons.Pause:
                    yield return _methods[MethodNames.KeyboardControllerName][MethodNames.KeyboardMediaPause](null);
                    break;
                case Buttons.Darken:
                    yield return _methods[MethodNames.DisplayControllerName][MethodNames.DisplayDarken](null);
                    break;
                case Buttons.VolumeUp:
                    var volumeAdd = _methods[MethodNames.AudioControllerName][MethodNames.AudioGetVolume](null);
                    if (int.TryParse(volumeAdd, out var volumeInt))
                    {
                        volumeInt += 5;
                        volumeInt = volumeInt > 100 ? 100 : volumeInt;

                        yield return _methods[MethodNames.AudioControllerName][MethodNames.AudioSetVolume](volumeInt.ToString());
                    }

                    break;
                case Buttons.VolumeDown:
                    var volumeSub = _methods[MethodNames.AudioControllerName][MethodNames.AudioGetVolume](null);
                    if (int.TryParse(volumeSub, out var volumeSubInt))
                    {
                        volumeSubInt -= 5;
                        volumeSubInt = volumeSubInt < 0 ? 0 : volumeSubInt;

                        yield return _methods[MethodNames.AudioControllerName][MethodNames.AudioSetVolume](volumeSubInt.ToString());
                    }

                    break;
                default:
                    yield return _methods[MethodNames.AudioControllerName][MethodNames.AudioGetVolume](null);
                    break;
            }
        }
    }
}