using MainApp.Servers;
using System.ComponentModel;

namespace MainApp.Interfaces;

public interface IWorker : INotifyPropertyChanged
{
    bool Status { get; }
    ServerConfig Config { get; set; }

    bool Start();
    bool Stop();
}
