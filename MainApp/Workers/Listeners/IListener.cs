using MainApp.Workers.DataObjects;
using System.ComponentModel;

namespace MainApp.Workers.Listeners;

internal interface IListener : INotifyPropertyChanged
{
    public bool IsListening { get; }
    public void StartListen(StartParameters param);
    public void StopListen();
    public Task<RequestContext> GetContextAsync(CancellationToken token = default);
}