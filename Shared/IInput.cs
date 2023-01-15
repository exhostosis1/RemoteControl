using Shared.Enums;

namespace Shared;

public interface IInput
{
    int SetMonitorInState(MonitorState state);
    void SendKeyInput(KeysEnum keyCode, bool up);
    void SendCharInput(char character, bool up);
    void SendMouseInput(int x, int y);
    void SendMouseInput(MouseButtons button, bool up);
    void SendScrollInput(int scrollAmount);
}