using Microsoft.Win32.TaskScheduler;
using Shared.Logging.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Autostart;

public class WinTaskAutostartService : BaseAutostartService
{
    private readonly TaskService _ts = new();
    private readonly string _taskName;
    private readonly TaskDefinition _td;
    private readonly string _filename = AppContext.BaseDirectory + "run.bat";

    public WinTaskAutostartService(ILogger logger): base(logger)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");

        var userName = WindowsIdentity.GetCurrent().Name;
        _taskName =
            $"RemoteControl{(userName.LastIndexOf("\\", StringComparison.Ordinal) != -1 ? userName[(userName.LastIndexOf("\\", StringComparison.Ordinal) + 1)..] : userName)}";

        _td = _ts.NewTask();
        _td.Actions.Add(_filename, null, AppContext.BaseDirectory);
        _td.Triggers.Add(new LogonTrigger { UserId = userName });
        _td.Principal.UserId = userName;
    }

    public override bool CheckAutostart()
    {
        Logger.LogInfo("Checking win task autostart");
        return (_ts.FindTask(_taskName)?.Enabled ?? false) && File.Exists(_filename);
    }

    public override void SetAutostart(bool value)
    {
        Logger.LogInfo($"Setting win task autostart to {value}");

        _ts.RootFolder.DeleteTask(_taskName, false);

        if (File.Exists(_filename))
        {
            try
            {
                File.Delete(_filename);

                if (value)
                {
                    File.WriteAllText(_filename, $"start {Process.GetCurrentProcess().MainModule?.FileName}");
                    _ts.RootFolder.RegisterTaskDefinition(_taskName, _td);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Logger.LogError($"Cannot write {_filename} due to access restrictions");
            }
            catch (DirectoryNotFoundException)
            {
                Logger.LogError($"Cannot find directory to write {_filename}");
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }
    }
}