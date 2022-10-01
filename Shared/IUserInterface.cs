using System;

namespace Shared;

public interface IUserInterface
{
    public event EmptyEventHandler? StartEvent;
    public event EmptyEventHandler? StopEvent;
    public event BoolEventHandler? AutostartChangeEvent;
    public event EmptyEventHandler? CloseEvent;

    public Uri? Uri { get; set; }
    public bool IsListening { get; set; }
    public bool IsAutostart { get; set; }

    // ReSharper disable once InconsistentNaming
    public void RunUI();
}