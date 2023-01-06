using Shared.Config;
using Shared.ControlProcessor;

namespace WinFormsUI.CustomControls;

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
        Size = new Size(155, 23),
        BorderStyle = BorderStyle.FixedSingle
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
        Size = new Size(344, 23),
        BorderStyle = BorderStyle.FixedSingle
    };

    private readonly Label _userIdsLabel = new()
    {
        Text = @"User IDs",
        Location = new Point(221, 3),
        Size = new Size(50, 15)
    };

    private readonly TextBox _userIdsListBox = new()
    {
        Location = new Point(276, 3),
        Size = new Size(128, 49),
        Multiline = true,
        BorderStyle = BorderStyle.FixedSingle
    };

    public BotPanel(IBotProcessor processor, int index) : base(processor, index)
    {
        _apiUrlTextBox.Text = processor.CurrentConfig.ApiUri;
        _apiKeyTextBox.Text = processor.CurrentConfig.ApiKey;
        _userIdsListBox.Text = string.Join(Environment.NewLine, processor.CurrentConfig.Usernames);

        _apiKeyTextBox.TextChanged += EnableUpdateButton;
        _apiUrlTextBox.TextChanged += EnableUpdateButton;
        _userIdsListBox.TextChanged += EnableUpdateButton;

        Controls.AddRange(new Control[]
        {
            _apiUrlLabel,
            _apiUrlTextBox,
            _apiKeyLabel,
            _apiKeyTextBox,
            _userIdsLabel,
            _userIdsListBox
        });
    }

    protected override void UpdateButtonClick(object? sender, EventArgs e)
    {
        _apiKeyTextBox.ResetBackColor();
        _apiUrlTextBox.ResetBackColor();
        NameTextBox.ResetBackColor();
        _userIdsListBox.ResetBackColor();

        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            NameTextBox.BackColor = Color.OrangeRed;
            return;
        }

        if (string.IsNullOrWhiteSpace(_apiUrlTextBox.Text))
        {
            _apiUrlTextBox.BackColor = Color.OrangeRed;
            return;
        }

        if (string.IsNullOrWhiteSpace(_apiKeyTextBox.Text))
        {
            _apiKeyTextBox.BackColor = Color.OrangeRed;
            return;
        }

        if (string.IsNullOrWhiteSpace(_userIdsListBox.Text))
        {
            _userIdsListBox.BackColor = Color.OrangeRed;
        }

        var config = new BotConfig
        {
            Autostart = AutostartBox.Checked,
            ApiUri = _apiUrlTextBox.Text,
            ApiKey = _apiKeyTextBox.Text,
            Name = NameTextBox.Text,
            Usernames = _userIdsListBox.Text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
        };

        RaiseUpdateButtonClickedEvent(config);

        UpdateButton.Enabled = false;
    }
}