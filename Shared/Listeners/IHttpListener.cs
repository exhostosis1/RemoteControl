using System;

namespace Shared.Listeners;

public interface IHttpListener
{
    public bool IsListening { get; }

    public event HttpEventHandler? OnRequest;
    public event BoolEventHandler? OnStatusChange;

    public void StartListen(Uri url);
    public void StopListen();
}