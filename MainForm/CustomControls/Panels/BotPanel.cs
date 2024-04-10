using Servers;

namespace MainUI.CustomControls.Panels;

internal sealed class BotPanel : ServerPanel
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
        Location = new Point(221, 6),
        Size = new Size(50, 15)
    };

    private readonly TextBox _userIdsListBox = new()
    {
        Location = new Point(276, 6),
        Size = new Size(128, 49),
        Multiline = true,
        BorderStyle = BorderStyle.FixedSingle
    };

    public BotPanel(Server server) : base(server)
    {
        _apiUrlTextBox.Text = server.Config.ApiUri;
        _apiKeyTextBox.Text = server.Config.ApiKey;
        _userIdsListBox.Text = string.Join(Environment.NewLine, server.Config.Usernames);

        _apiKeyTextBox.TextChanged += EnableUpdateButton;
        _apiUrlTextBox.TextChanged += EnableUpdateButton;
        _userIdsListBox.TextChanged += EnableUpdateButton;

        Controls.AddRange(
        [
            _apiUrlLabel,
            _apiUrlTextBox,
            _apiKeyLabel,
            _apiKeyTextBox,
            _userIdsLabel,
            _userIdsListBox
        ]);

        server.PropertyChanged += (_, args) =>
        {
            if(args.PropertyName != nameof(server.Config)) return;

            NameTextBox.Text = server.Config.Name;
            AutoStartBox.Checked = server.Config.AutoStart;
            _apiUrlTextBox.Text = server.Config.ApiUri;
            _apiKeyTextBox.Text = server.Config.ApiKey;
            _userIdsListBox.Text = string.Join(Environment.NewLine, server.Config.Usernames);
        };
        Disposed += LocalDispose;
    }

    protected override void UpdateButtonClick(object? sender, EventArgs e)
    {
        ApplyTheme(Theme);

        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            NameLabel.ForeColor = Color.OrangeRed;
            return;
        }

        if (string.IsNullOrWhiteSpace(_apiUrlTextBox.Text))
        {
            _apiUrlLabel.ForeColor = Color.OrangeRed;
            return;
        }

        if (string.IsNullOrWhiteSpace(_apiKeyTextBox.Text))
        {
            _apiKeyLabel.ForeColor = Color.OrangeRed;
            return;
        }

        if (string.IsNullOrWhiteSpace(_userIdsListBox.Text))
        {
            _userIdsLabel.ForeColor = Color.OrangeRed;
        }

        var config = new ServerConfig(ServerType.Web)
        {
            AutoStart = AutoStartBox.Checked,
            ApiUri = _apiUrlTextBox.Text,
            ApiKey = _apiKeyTextBox.Text,
            Name = NameTextBox.Text,
            Usernames = [.. _userIdsListBox.Text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)]
        };

        RaiseUpdateButtonClickedEvent(config);

        UpdateButton.Enabled = false;
    }

    private void LocalDispose(object? sender, EventArgs args)
    {
        _apiKeyTextBox.TextChanged -= EnableUpdateButton;
        _apiUrlTextBox.TextChanged -= EnableUpdateButton;
        _userIdsListBox.TextChanged -= EnableUpdateButton;

        Disposed -= LocalDispose;
    }
}