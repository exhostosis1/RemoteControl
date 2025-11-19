namespace MainApp.Workers.Listeners.Telegram.BotButtons;

internal class SingleButton(string Text = "")
{
    public string Text { get; set; } = Text;
}