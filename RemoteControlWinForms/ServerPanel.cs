using Shared.Config;
using Shared.ControlProcessor;

namespace RemoteControlWinForms;

internal class ServerPanel : MyPanel
{
    private readonly Label _schemeLabel = new()
    {
        Text = @"Scheme",
        Location = new Point(6, 36),
        Size = new Size(50, 15)
    };

    private readonly TextBox _schemeTextBox = new()
    {
        Location = new Point(60, 32),
        Size = new Size(53, 23)
    };

    private readonly Label _hostLabel = new()
    {
        Text = @"Host",
        Location = new Point(120, 36),
        Size = new Size(32, 15)
    };

    private readonly TextBox _hostTextBox = new()
    {
        Location = new Point(155, 32),
        Size = new Size(143, 23)
    };

    private readonly Label _portLabel = new()
    {
        Text = @"Port",
        Location = new Point(304, 36),
        Size = new Size(29, 15)
    };

    private readonly TextBox _portTextBox = new()
    {
        Location = new Point(338, 32),
        Size = new Size(35, 23)
    };

    public ServerPanel(IServerProcessor processor, int index) : base(processor, index)
    {
        _schemeTextBox.Text = processor.CurrentConfig.Scheme;
        _hostTextBox.Text = processor.CurrentConfig.Host;
        _portTextBox.Text = processor.CurrentConfig.Port.ToString();

        _schemeTextBox.TextChanged += EnableUpdateButton;
        _hostTextBox.TextChanged += EnableUpdateButton;
        _portTextBox.TextChanged += EnableUpdateButton;

        this.Controls.AddRange(new Control[]
        {
            _schemeLabel,
            _schemeTextBox,
            _hostLabel,
            _hostTextBox,
            _portLabel,
            _portTextBox
        });
    }

    protected override void UpdateButtonClick(object? sender, EventArgs e)
    {
        _schemeTextBox.ResetBackColor();
        _hostTextBox.ResetBackColor();
        _portTextBox.ResetBackColor();
        NameTextBox.ResetBackColor();

        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            NameTextBox.BackColor = Color.OrangeRed;
            return;
        }

        if (!int.TryParse(_portTextBox.Text, out var port))
        {
            _portTextBox.BackColor = Color.OrangeRed;
            return;
        }

        Uri? uri;
        try
        {
            uri = new UriBuilder(_schemeTextBox.Text, _hostTextBox.Text, port).Uri;
        }
        catch
        {
            _schemeTextBox.BackColor = Color.OrangeRed;
            _portTextBox.BackColor = Color.OrangeRed;
            _hostTextBox.BackColor = Color.OrangeRed;

            return;
        }

        var config = new ServerConfig
        {
            Autostart = AutostartBox.Checked,
            Uri = uri,
            Name = NameTextBox.Text
        };

        RaiseUpdateButtonClickedEvent(config);

        UpdateButton.Enabled = false;
    }
}