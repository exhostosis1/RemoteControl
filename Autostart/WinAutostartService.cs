using Microsoft.Win32.TaskScheduler;
using Shared;

namespace Autostart
{
    public class WinAutostartService : IAutostartService
    {
        private readonly TaskService _ts = new();
        private const string TaskName = "RemoteControl";
        private readonly TaskDefinition _td;
        public WinAutostartService()
        {
            _td = _ts.NewTask();
            _td.Actions.Add(AppContext.BaseDirectory + "run.bat", null, AppContext.BaseDirectory);
            _td.Triggers.Add(new LogonTrigger());
            _td.Principal.RunLevel = TaskRunLevel.Highest;
        }

        public bool CheckAutostart()
        {
            return _ts.FindTask(TaskName)?.Enabled ?? false;
        }

        public void SetAutostart(bool value)
        {
            var task = _ts.FindTask(TaskName);

            if (task != null)
            {
                _ts.RootFolder.DeleteTask(TaskName);
            }

            if (!value) return;

            _ts.RootFolder.RegisterTaskDefinition(TaskName, _td);
        }
    }
}
