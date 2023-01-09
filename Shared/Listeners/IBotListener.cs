using System.Collections.Generic;

namespace Shared.Listeners;

public interface IBotListener
{
    public bool IsListening { get; }
    public List<string> Usernames { get; }
    
    public event BoolEventHandler? OnStatusChange;

    public void StartListen(string apiUrl, string apiKey, List<string> usernames);
    public void StopListen();
}