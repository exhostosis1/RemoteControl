using System;

namespace Shared.Server;

public interface IServer: IControlProcessor
{
    public Uri? GetListeningUri();
    public bool IsListening { get; }
}