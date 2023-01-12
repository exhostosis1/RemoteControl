using System.Collections.Generic;

namespace Shared.DataObjects.Bot;

public class BotContext
{
    public int Id { get; set; }
    public string Message { get; set; }
    public string? Result { get; set; }
    public ButtonsMarkup? Buttons { get; set; }

    public BotContext(int id, string message)
    {
        Id = id;
        Message = message;
    }
}

public abstract class ButtonsMarkup
{
    
}

public class ReplyButtonsMarkup: ButtonsMarkup
{
    public IEnumerable<IEnumerable<SingleButton>> Items { get; set; }

    public bool Resize { get; set; }
    public bool Persistent { get; set; }
    public bool OneTime { get; set; }

    public ReplyButtonsMarkup(IEnumerable<IEnumerable<SingleButton>> items)
    {
        Items = items;
    }
}

public class RemoveButtonsMarkup: ButtonsMarkup
{
    public bool Remove = true;
}

public class SingleButton
{
    public string Text { get; set; }

    public SingleButton(string text)
    {
        Text = text;
    }
}