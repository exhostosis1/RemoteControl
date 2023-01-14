﻿using Shared;
using Shared.ControlProcessor;
using Shared.UI;

namespace ConsoleUI;

// ReSharper disable once InconsistentNaming
public class MainConsole: IUserInterface
{
    public event NullableIntEventHandler? StartEvent;
    public event NullableIntEventHandler? StopEvent;
    public event EmptyEventHandler? CloseEvent;
    public event BoolEventHandler? AutostartChangedEvent;
    public event ConfigWithIdEventHandler? ConfigChangedEvent;
    public event StringEventHandler? ProcessorAddedEvent;
    public event IntEventHandler? ProcessorRemovedEvent;

    public void SetAutostartValue(bool value)
    {
        IsAutostart = value;
    }

    private List<AbstractControlProcessor> Model { get; set; } = new();

    private bool IsAutostart { get; set; }

    // ReSharper disable once InconsistentNaming
    public void RunUI(List<AbstractControlProcessor> processors)
    {
        Model = processors;
        DisplayInfo(Model);

        while (true)
        {
            var key = Console.ReadLine();

            if (key == "x")
            {
                CloseEvent?.Invoke();
                return;
            }

            switch (key)
            {
                case "s":
                    if(Model.Any(x => x.Working))
                        StopEvent?.Invoke(null);
                    else
                        StartEvent?.Invoke(null);
                    break;
                case "a":
                    AutostartChangedEvent?.Invoke(!IsAutostart);
                    break;
                default:
                    continue;
            }

            DisplayInfo(Model);
        }
    }

    public void ShowError(string message)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
            
        Console.WriteLine(message);
            
        Console.ForegroundColor = color;
    }

    public void AddProcessor(AbstractControlProcessor processor)
    {
        throw new NotImplementedException();
    }

    private void DisplayInfo(List<AbstractControlProcessor> dtos)
    {
        foreach (var dto in dtos)
        {
            switch (dto)
            {
                case ServerProcessor s:
                    Console.WriteLine(s.Working ? $"Server {s.CurrentConfig.Name} listening on {s.CurrentConfig.Uri}" : $"Server {s.CurrentConfig.Name} stopped");
                    break;
                case BotProcessor b:
                    Console.WriteLine(b.Working ? $"Bot {b.CurrentConfig.Name} responds to {b.CurrentConfig.UsernamesString}" : $"Bot {b.CurrentConfig.Name} stopped");
                    break;
            }

            Console.WriteLine($"Autostart {(IsAutostart ? "enabled" : "disabled")}");
            Console.WriteLine();
        }

        Console.Write($"{(dtos.Any(x => x.Working) ? "[s]top" : "[s]tart")}, [a]utostart, e[x]it:");
    }
}