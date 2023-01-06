using System.Runtime.InteropServices;

namespace WinFormsUI;

internal static partial class DarkTitleBar
{
    [LibraryImport("dwmapi.dll")]
    private static partial int DwmSetWindowAttribute(nint hwnd, int attr, ref int attrValue, int attrSize);
    
    private const int DwmwaUseImmersiveDarkMode = 20;

    internal static bool UseImmersiveDarkMode(nint handle, bool enabled)
    {
        var useImmersiveDarkMode = enabled ? 1 : 0;
        return DwmSetWindowAttribute(handle, DwmwaUseImmersiveDarkMode, ref useImmersiveDarkMode, sizeof(int)) == 0;
    }
}