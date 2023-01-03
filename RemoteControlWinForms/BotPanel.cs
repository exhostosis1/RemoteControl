using Shared.ControlProcessor;

namespace RemoteControlWinForms;

internal sealed class BotPanel : MyPanel
{
    private readonly Label _apiUrlLabel = new()
    {
        Text = @"Api Url",
        Location = new Point(6, 35),
        Size = new Size(43, 15)
    };

    private readonly TextBox _apiUrlTextBox = new()
    {
        Location = new Point(60, 32),
        Size = new Size(155, 23)
    };

    private readonly Label _apiKeyLabel = new()
    {
        Text = @"Api Key",
        Location = new Point(6, 61),
        Size = new Size(47, 15)
    };

    private readonly TextBox _apiKeyTextBox = new()
    {
        Location = new Point(60, 58),
        Size = new Size(344, 23)
    };

    private readonly Label _userIdsLabel = new()
    {
        Text = @"User IDs",
        Location = new Point(221, 3),
        Size = new Size(50, 15)
    };

    private readonly ListBox _userIdsListBox = new()
    {
        Location = new Point(276, 3),
        Size = new Size(128, 49)
    };

    public BotPanel(IBotProcessor processor, int index) : base(processor, index)
    {
        _apiUrlTextBox.Text = processor.CurrentConfig.ApiUri;
        _apiKeyTextBox.Text = processor.CurrentConfig.ApiKey;
        _userIdsListBox.DataSource = processor.CurrentConfig.Usernames;

        this.Controls.AddRange(new Control[]
        {
            _apiUrlLabel,
            _apiUrlTextBox,
            _apiKeyLabel,
            _apiKeyTextBox,
            _userIdsLabel,
            _userIdsListBox
        });
    }
}