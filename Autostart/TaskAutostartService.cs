using Shared;
using Shared.Logging.Interfaces;
using Shared.Wrappers.TaskServiceWrapper;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Autostart;

public class TaskAutostartService : IAutostartService
{
    private readonly ITaskService _taskServiceWrapper;
    private readonly string _taskName;
    private readonly ITaskDefinition _td;
    private readonly string _filename = Path.Combine(AppContext.BaseDirectory, "run.bat");
    private readonly ILogger<TaskAutostartService> _logger;

    public TaskAutostartService(ITaskService taskService, ILogger<TaskAutostartService> logger)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");

        _logger = logger;
        _taskServiceWrapper = taskService;

        var userName = WindowsIdentity.GetCurrent().Name;
        _taskName =
            $"RemoteControl{(userName.LastIndexOf("\\", StringComparison.Ordinal) != -1 ? userName[(userName.LastIndexOf("\\", StringComparison.Ordinal) + 1)..] : userName)}";

        _td = _taskServiceWrapper.NewTask(_taskName, userName);
        _td.Actions.Add(new TaskAction(_filename, null, AppContext.BaseDirectory));
        _td.Triggers.Add(new TaskLogonTrigger(userName));
    }

    public bool CheckAutostart()
    {
        _logger.LogInfo("Checking win task autostart");
        return (_taskServiceWrapper.FindTask(_taskName)?.Enabled ?? false) && File.Exists(_filename);
    }

    public void SetAutostart(bool value)
    {
        _logger.LogInfo($"Setting win task autostart to {value}");

        _taskServiceWrapper.DeleteTask(_taskName, false);

        if (!value) return;

        try
        {
            File.WriteAllText(_filename, $"start {Process.GetCurrentProcess().MainModule?.FileName}");
            _taskServiceWrapper.RegisterNewTask(_td);

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