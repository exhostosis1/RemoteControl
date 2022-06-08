using Microsoft.Win32.TaskScheduler;

namespace RemoteControl.Autostart
{
    internal static class AutostartService
    {
        private static readonly TaskService _ts = new();
        private const string TaskName = "RemoteControl";
        private static readonly TaskDefinition _td;
        static AutostartService()
        {
            _td = _ts.NewTask();
            _td.Actions.Add(AppContext.BaseDirectory + "run.bat", null, AppContext.BaseDirectory);
            _td.Triggers.Add(new LogonTrigger());
            _td.Principal.RunLevel = TaskRunLevel.Highest;
        }

        public static bool CheckAutostart()
        {
            return _ts.FindTask(TaskName)?.Enabled ?? false;
        }

        public static void SetAutostart(bool value)
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
