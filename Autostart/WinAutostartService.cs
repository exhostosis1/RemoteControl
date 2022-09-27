using Microsoft.Win32.TaskScheduler;
using Shared;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Autostart
{
    public class WinAutostartService : IAutostartService
    {
        private readonly TaskService _ts = new();
        private readonly string _taskName;
        private readonly TaskDefinition _td;
        private const string Filename = "run.bat";

        public WinAutostartService()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new Exception("OS not supported");

            var userName = WindowsIdentity.GetCurrent().Name;
            _taskName =
                $"RemoteControl{(userName.LastIndexOf("\\", StringComparison.Ordinal) != -1 ? userName[(userName.LastIndexOf("\\", StringComparison.Ordinal) + 1)..] : userName)}";

            _td = _ts.NewTask();
            _td.Actions.Add(AppContext.BaseDirectory + Filename, null, AppContext.BaseDirectory);
            _td.Triggers.Add(new LogonTrigger { UserId = userName });
            _td.Principal.UserId = userName;
        }

        public bool CheckAutostart()
        {
            return (_ts.FindTask(_taskName)?.Enabled ?? false) && File.Exists(Filename);
        }

        public void SetAutostart(bool value)
        {
            _ts.RootFolder.DeleteTask(_taskName, false);

            if(File.Exists(Filename))
                File.Delete(Filename);

            if (value)
            {
                File.WriteAllText(Filename, $"start {Process.GetCurrentProcess().MainModule?.FileName}");
                _ts.RootFolder.RegisterTaskDefinition(_taskName, _td);
            }
        }
    }
}
