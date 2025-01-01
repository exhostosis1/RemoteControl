using MainApp.Servers.DataObjects;
using System.ComponentModel;

namespace MainApp.Servers.Listeners;

internal interface IListener : INotifyPropertyChanged
{
    public bool IsListening { get; }
    public void StartListen(StartParameters param);
    public void StopListen();
    public Task<RequestContext> GetContextAsync(CancellationToken token = default);
}