using Microsoft.Win32.TaskScheduler;
using Shared;
using System.Diagnostics;

namespace Autostart
{
    public class WinAutostartService : IAutostartService
    {
        private readonly TaskService _ts = new();
        private const string TaskName = "RemoteControl";
        private readonly TaskDefinition _td;
        private const string Filename = "run.bat";
        private const string Admins = "S-1-5-32-544";
        private const string Users = "S-1-5-32-545";
        private const string Author = "exhostosis";

        public WinAutostartService()
        {
            _td = _ts.NewTask();
            _td.Actions.Add(AppContext.BaseDirectory + Filename, null, AppContext.BaseDirectory);
            _td.Triggers.Add(new LogonTrigger());
            _td.Principal.RunLevel = TaskRunLevel.Highest;
            _td.Settings.Compatibility = TaskCompatibility.V2_3;
            _td.Principal.GroupId = Admins;
            _td.RegistrationInfo.Author = Author;
        }

        public bool CheckAutostart()
        {
            return _ts.FindTask(TaskName)?.Enabled ?? false;
        }

        public void SetAutostart(bool value)
        {
            _ts.RootFolder.DeleteTask(TaskName, false);

            if (value)
            {
                File.WriteAllText(Filename, $"start {Process.GetCurrentProcess().MainModule?.FileName}");
                _ts.RootFolder.RegisterTaskDefinition(TaskName, _td);
            }
        }
    }
}
