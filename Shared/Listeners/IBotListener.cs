using System.Collections.Generic;

namespace Shared.Listeners;

public interface IBotListener
{
    public bool IsListening { get; }
    public IReadOnlyCollection<string> Usernames { get; }
    
    public event BoolEventHandler? OnStatusChange;

    public void StartListen(string apiUrl, string apiKey, IEnumerable<string> usernames);
    public void StopListen();
}