namespace Shared.DataObjects.Bot;

public abstract class BotContextResponse: IResponse
{
    public string Message { get; set; }
    public ButtonsMarkup? Buttons { get; set; }

    public abstract void Close();
}