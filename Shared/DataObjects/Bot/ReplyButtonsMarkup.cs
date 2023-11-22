using System.Collections.Generic;

namespace Shared.DataObjects.Bot;

public class ReplyButtonsMarkup(IEnumerable<IEnumerable<SingleButton>> items) : ButtonsMarkup
{
    public IEnumerable<IEnumerable<SingleButton>> Items { get; set; } = items;

    public bool Resize { get; set; }
    public bool Persistent { get; set; }
    public bool OneTime { get; set; }
}