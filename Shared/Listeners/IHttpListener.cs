using System;
using Shared.DataObjects.Http;

namespace Shared.Listeners;

public interface IHttpListener
{
    public bool IsListening { get; }

    public event EventHandler<Context>? OnRequest;
    public event EventHandler<bool>? OnStatusChange;

    public void StartListen(Uri url);
    public void StopListen();
}