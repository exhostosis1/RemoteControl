using System.Collections.Generic;

namespace Shared.Listeners;

public interface IBotListener
{
    public bool IsListening { get; }
    
    public event BoolEventHandler? OnStatusChange;
    public event BotEventHandler? OnRequest;

    public void StartListen(string apiUrl, string apiKey, List<string> usernames);
    public void StopListen();
}