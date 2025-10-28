namespace MainApp.Servers.Listeners.Telegram.BotButtons;

internal class SingleButton(string Text = "")
{
    public string Text { get; set; } = Text;
}