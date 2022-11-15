using System;
using System.Collections.Generic;

namespace Shared.Server.Interfaces;

public interface IListener
{
    public bool IsListening { get; }
    public IReadOnlyCollection<Uri> ListeningUris { get; }

    public event HttpEventHandler? OnRequest;

    public void StartListen(Uri url);

    public void StopListen();
}