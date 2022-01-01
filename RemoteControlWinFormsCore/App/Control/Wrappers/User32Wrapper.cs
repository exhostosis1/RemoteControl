﻿using RemoteControl.App.Control.Interfaces;
using System.Runtime.InteropServices;

namespace RemoteControl.App.Control.Wrappers
{
    internal class User32Wrapper: IControlDisplay
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