using System.Collections.Generic;

namespace Shared.DataObjects.Bot;

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