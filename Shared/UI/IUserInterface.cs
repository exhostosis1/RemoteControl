using System;
using System.Collections.Generic;
using Shared.Config;
using Shared.ControlProcessor;

namespace Shared.UI;

public interface IUserInterface
{
    public event EventHandler<int?>? StartEvent;
    public event EventHandler<int?>? StopEvent;
    public event EventHandler? CloseEvent;
    public event EventHandler<bool>? AutostartChangedEvent;
    public event EventHandler<(int, CommonConfig)>? ConfigChangedEvent;
    public event EventHandler<string>? ProcessorAddedEvent;
    public event EventHandler<int>? ProcessorRemovedEvent;

    public void SetAutostartValue(bool value);

    // ReSharper disable once InconsistentNaming
    public void RunUI(List<AbstractControlProcessor> processors);
    public void ShowError(string message);
    public void AddProcessor(AbstractControlProcessor processor);
}