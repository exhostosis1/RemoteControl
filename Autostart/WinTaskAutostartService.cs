using Autostart.Task;
using Shared;
using Shared.Logging.Interfaces;
using Shared.TaskServiceWrapper;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Autostart;

public class WinTaskAutostartService : IAutostartService
{
    private readonly ITaskService _ts;
    private readonly string _taskName;
    private readonly ITaskDefinition _td;
    private readonly string _filename = AppContext.BaseDirectory + "run.bat";
    private readonly ILogger<WinTaskAutostartService> _logger;

    public WinTaskAutostartService(ITaskService taskService, ILogger<WinTaskAutostartService> logger)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");

        _logger = logger;
        _ts = taskService;

        var userName = WindowsIdentity.GetCurrent().Name;
        _taskName =
            $"RemoteControl{(userName.LastIndexOf("\\", StringComparison.Ordinal) != -1 ? userName[(userName.LastIndexOf("\\", StringComparison.Ordinal) + 1)..] : userName)}";

        _td = _ts.NewTask();
        _td.Actions.Add(_filename, null, AppContext.BaseDirectory);
        _td.Triggers.Add(new LogonTriggerWrapper { UserId = userName });
        _td.Principal.UserId = userName;
    }

    public bool CheckAutostart()
    {
        _logger.LogInfo("Checking win task autostart");
        return (_ts.FindTask(_taskName)?.Enabled ?? false) && File.Exists(_filename);
    }

    public void SetAutostart(bool value)
    {
        _logger.LogInfo($"Setting win task autostart to {value}");

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
                _logger.LogError($"Cannot write {_filename} due to access restrictions");
            }
            catch (DirectoryNotFoundException)
            {
                _logger.LogError($"Cannot find directory to write {_filename}");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}