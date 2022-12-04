﻿namespace Shared;

public interface IBot
{
    public void Start(int chatId);
    public void Stop();

    public bool IsRunning { get; }
    
    public int GetChatId();
}