using Shared.Config;
using Shared.Enums;
using Shared.Server;
using System;
using System.Collections.Generic;

namespace Shared.UI;

public interface IUserInterface
{
    public IObservable<int?> ServerStart { get; }
    public IObservable<int?> ServerStop { get; }
    public IObservable<object?> AppClose { get; }
    public IObservable<bool> AutostartChange { get; }
    public IObservable<(int, CommonConfig)> ConfigChange { get; }
    public IObservable<ServerType> ServerAdd { get; }
    public IObservable<int> ServerRemove { get; }

    public void SetAutostartValue(bool value);

    // ReSharper disable once InconsistentNaming
    public void RunUI(List<IServer> servers);
    public void ShowError(string message);
    public void AddServer(IServer server);
}