using Shared.Config;
using Shared.Server;
using System;
using System.Collections.Generic;
using Shared.Enums;

namespace Shared.UI;

public interface IUserInterface
{
    public event EventHandler<int?>? OnStart;
    public event EventHandler<int?>? OnStop;
    public event EventHandler? OnClose;
    public event EventHandler<bool>? OnAutostartChanged;
    public event EventHandler<(int, CommonConfig)>? OnConfigChanged;
    public event EventHandler<ServerType>? OnServerAdded;
    public event EventHandler<int>? OnServerRemoved;

    public void SetAutostartValue(bool value);

    // ReSharper disable once InconsistentNaming
    public void RunUI(List<IServer> servers);
    public void ShowError(string message);
    public void AddServer(IServer server);
}