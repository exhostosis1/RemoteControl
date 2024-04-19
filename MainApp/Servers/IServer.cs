using System.ComponentModel;

namespace MainApp.Servers;

public interface IServer: INotifyPropertyChanged
{
    Guid Id { get; }
    bool Status { get; }
    ServerConfig Config { get; set; }
    void Start();
    void Restart();
    void Stop();

    event EventHandler<string> Error;
}