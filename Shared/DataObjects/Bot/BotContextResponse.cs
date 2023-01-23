namespace Shared.DataObjects.Bot;

public abstract class BotContextResponse : IResponse
{
    public string Message { get; set; } = string.Empty;
    public ButtonsMarkup? Buttons { get; set; }

    public abstract void Close();
}