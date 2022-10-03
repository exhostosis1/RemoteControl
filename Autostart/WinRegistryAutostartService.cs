using Microsoft.Win32;
using Shared;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Autostart
{
    public class WinRegistryAutostartService : IAutostartService
    {
        private readonly RegistryKey _regKey;
        private readonly string _regName = "Remote Control";
        private readonly string _regValue = $"\"{Process.GetCurrentProcess().MainModule?.FileName ?? throw new NullReferenceException()}\"";

        public WinRegistryAutostartService()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new Exception("OS not supported");

            _regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE")?.OpenSubKey("Microsoft")?.OpenSubKey("Windows")
                ?.OpenSubKey("CurrentVersion")?.OpenSubKey("Run", true) ?? throw new NullReferenceException();
        }

        public bool CheckAutostart()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new Exception("OS not supported");

            return _regKey.GetValue(_regName, "") as string == _regValue;
        }

        public void SetAutostart(bool value)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new Exception("OS not supported");

            _regKey.DeleteValue(_regName, false);

            if(value)
                _regKey.SetValue(_regName, _regValue, RegistryValueKind.String);
        }
    }
}
