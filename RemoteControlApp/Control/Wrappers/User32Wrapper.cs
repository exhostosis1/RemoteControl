using System.Runtime.InteropServices;
using Shared.Interfaces.Control;

namespace RemoteControlApp.Control.Wrappers
{
    public class User32Wrapper: IDisplayControl
    {
        private enum MonitorState
        {
            MonitorStateOn = -1,
            MonitorStateOff = 2,
            MonitorStateStandBy = 1
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        private static void SetMonitorInState(MonitorState state) => _ = SendMessage(0xFFFF, 0x112, 0xF170, (int)state);

        public void Darken() => SetMonitorInState(MonitorState.MonitorStateOff);
    }
}
