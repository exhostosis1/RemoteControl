using System;

namespace Shared.Server.Interfaces;

public interface IServer
{
    public void Start(Uri uri);
    public void Stop();

    public Uri? GetListeningUri();
    public bool IsListening { get; }
}