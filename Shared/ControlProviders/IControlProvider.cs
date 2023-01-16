using System;
using System.Collections.Generic;
using Shared.ControlProviders.Devices;
using Shared.Enums;

namespace Shared.ControlProviders;

public interface IControlProvider
{
    public int GetVolume();
    public void SetVolume(int volume);
    void Mute();
    void Unmute();
    bool IsMuted { get; }
    IEnumerable<IAudioDevice> GetAudioDevices();
    void SetAudioDevice(Guid id);
    void DisplayOff();
    void KeyboardKeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click);
    void TextInput(string text);
    void MouseMove(int x, int y);
    void MouseKeyPress(MouseButtons button = MouseButtons.Left, KeyPressMode mode = KeyPressMode.Click);
    void MouseWheel(bool up);
}