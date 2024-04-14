using Servers;
using System.ComponentModel;

namespace MainUI.CustomControls.Panels;

internal abstract class ServerPanel : Panel
{
    public int Id { get; init; }
    private readonly Server _server;
    private readonly SynchronizationContext _synchronizationContext;
    protected Theme Theme { get; private set; } = new();

    public event EventHandler? UpdateButtonClicked;

    protected readonly Label NameLabel = new()
    {
        Text = @"Name",
        Location = new Point(6, 6),
        Size = new Size(39, 15)
    };

    protected readonly TextBox NameTextBox = new()
    {
        Location = new Point(60, 6),
        Size = new Size(155, 23),
        BorderStyle = BorderStyle.FixedSingle
    };

    protected readonly CheckBox AutoStartBox = new()
    {
        Text = @"Autostart",
        Location = new Point(419, 34),
        Size = new Size(75, 19),
    };

    private readonly Button _startButton = new()
    {
        Text = @"Start",
        Location = new Point(419, 5),
        Size = new Size(75, 23),
        Visible = false,
        FlatStyle = FlatStyle.Flat,

    };

    private readonly Button _stopButton = new()
    {
        Text = @"Stop",
        Location = new Point(419, 5),
        Size = new Size(75, 23),
        Visible = false,
        FlatStyle = FlatStyle.Flat,
    };

    protected readonly Button UpdateButton = new()
    {
        Text = @"Update",
        Location = new Point(419, 57),
        Size = new Size(75, 23),
        Enabled = false,
        FlatStyle = FlatStyle.Flat
    };

    protected ServerPanel(Server server)
    {
        Width = 508;
        Height = 90;
        Left = 12;
        BorderStyle = BorderStyle.FixedSingle;

        _server = server;
        NameTextBox.Text = server.Config.Name;

        _synchronizationContext = SynchronizationContext.Current ?? throw new Exception("No synchronization context");

        if (server.Status)
        {
            _stopButton.Visible = true;
        }
        else
        {
            _startButton.Visible = true;
        }

        _startButton.Click += StartButtonClick;
        _stopButton.Click += StopButtonClick;
        UpdateButton.Click += UpdateButtonClick;

        AutoStartBox.Checked = server.Config.AutoStart;

        NameTextBox.TextChanged += EnableUpdateButton;
        AutoStartBox.CheckedChanged += EnableUpdateButton;

        Controls.AddRange(
        [
            NameLabel,
            NameTextBox,
            AutoStartBox,
            _startButton,
            _stopButton,
            UpdateButton
        ]);

        _server.PropertyChanged += SetButtons;
        Disposed += (_, _) => _server.PropertyChanged -= SetButtons;
    }

    public void ApplyTheme(Theme theme)
    {
        Theme = theme;
        foreach (Control control in Controls)
        {
            theme.ApplyTheme(control);
        }

        if (ContextMenuStrip != null)
            theme.ApplyTheme(ContextMenuStrip);
    }

    protected void EnableUpdateButton(object? sender, EventArgs e) => UpdateButton.Enabled = true;

    protected void RaiseUpdateButtonClickedEvent(ServerConfig config)
    {
        _server.Config = config;
        if(_server.Status)
            _server.Restart();

        UpdateButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    protected abstract void UpdateButtonClick(object? sender, EventArgs e);

    private void StartButtonClick(object? sender, EventArgs args)
    {
        _server.Start();
        _startButton.Enabled = false;
    }

    private void StopButtonClick(object? sender, EventArgs args)
    {
        _server.Stop();
        _stopButton.Enabled = false;
    }

    private void SetButtons(object? _, PropertyChangedEventArgs args)
    {
        if(args.PropertyName != nameof(_server.Status)) return;

        var buttonToDisable = _server.Status ? _startButton : _stopButton;
        var buttonToEnable = _server.Status ? _stopButton : _startButton;

        _synchronizationContext.Post(_ =>
        {
            buttonToDisable.Visible = false;
            buttonToEnable.Visible = true;
            buttonToEnable.Enabled = true;
        }, null);
    }
}