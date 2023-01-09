using System;
using System.Collections.Generic;
using Shared.Server;

namespace Shared.Listeners;

public interface IHttpListener
{
    public bool IsListening { get; }
    public IReadOnlyCollection<Uri> ListeningUris { get; }

    public event HttpEventHandler? OnRequest;
    public event BoolEventHandler? OnStatusChange;

    public void StartListen(Uri url);
    public void StopListen();
}