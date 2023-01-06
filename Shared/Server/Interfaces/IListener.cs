using System;
using System.Collections.Generic;

namespace Shared.Server.Interfaces;

public interface IListener
{
    public bool IsListening { get; }
    public IReadOnlyCollection<Uri> ListeningUris { get; }

    public event HttpEventHandler? OnRequest;
    public event BoolEventHandler? OnStatusChange;

    public void StartListen(Uri url);

    public void StopListen();
}