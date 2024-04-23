using System.ComponentModel;

namespace MainApp.Servers;

public interface IServer
{
    Guid Id { get; }
    bool Status { get; }
    ServerConfig Config { get; set; }
    bool Start();
    bool Restart();
    bool Stop();

    event EventHandler<string> Error;
}