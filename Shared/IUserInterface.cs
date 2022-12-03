using System;

namespace Shared;

public interface IUserInterface
{
    public event EmptyEventHandler? StartEvent;
    public event EmptyEventHandler? StopEvent;
    public event BoolEventHandler? AutostartChangeEvent;
    public event EmptyEventHandler? CloseEvent;
    public bool IsAutostart { get; set; }

    // ReSharper disable once InconsistentNaming
    public void RunUI();
    public void ShowError(string message);
}