using MainApp.Workers;
using System.ComponentModel;

namespace MainApp.Interfaces;

public interface IWorker : INotifyPropertyChanged
{
    bool Status { get; }
    WorkerType Type => Config.Type;
    string Name => Config.Name;
    string Uri => Config.Uri.ToString();
    string ApiUri => Config.ApiUri;
    string ApiKey => Config.ApiKey;
    string UsernamesString => Config.UsernamesString;
    bool AutoStart => Config.AutoStart;
    WorkerConfig Config { get; set; }

    bool Start();
    bool Stop();
}
