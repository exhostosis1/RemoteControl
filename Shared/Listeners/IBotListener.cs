using System;
using System.Collections.Generic;
using Shared.DataObjects.Bot;

namespace Shared.Listeners;

public interface IBotListener
{
    public bool IsListening { get; }
    
    public event EventHandler<bool>? OnStatusChange;
    public event EventHandler<BotContext>? OnRequest;

    public void StartListen(string apiUrl, string apiKey, List<string> usernames);
    public void StopListen();
}