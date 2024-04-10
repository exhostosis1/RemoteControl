using Servers;
using System.ComponentModel;

namespace MainUI.CustomControls.Panels;

internal abstract class ServerPanel : Panel
{
    public int Id { get; init; }
    protected readonly Server Server;
    protected readonly SynchronizationContext? SynchronizationContext;
    protected Theme Theme { get; set; } = new();

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

    protected readonly Button StartButton = new()
    {
        Text = @"Start",
        Location = new Point(419, 5),
        Size = new Size(75, 23),
        Visible = false,
        FlatStyle = FlatStyle.Flat,

    };

    protected readonly Button StopButton = new()
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

    public event EventHandler<int>? StartButtonClicked;
    public event EventHandler<int>? StopButtonClicked;
    public event EventHandler<(int, ServerConfig)>? UpdateButtonClicked;

    protected ServerPanel(Server server)
    {
        Width = 508;
        Height = 90;
        Left = 12;
        BorderStyle = BorderStyle.FixedSingle;

        Id = server.Id;
        Server = server;
        NameTextBox.Text = server.Config.Name;

        SynchronizationContext = SynchronizationContext.Current;

        if (server.Status)
        {
            StopButton.Visible = true;
        }
        else
        {
            StartButton.Visible = true;
        }

        StartButton.Click += StartButtonClick;
        StopButton.Click += StopButtonClick;
        UpdateButton.Click += UpdateButtonClick;

        AutoStartBox.Checked = server.Config.AutoStart;

        NameTextBox.TextChanged += EnableUpdateButton;
        AutoStartBox.CheckedChanged += EnableUpdateButton;

        Controls.AddRange(
        [
            NameLabel,
            NameTextBox,
            AutoStartBox,
            StartButton,
            StopButton,
            UpdateButton
        ]);

        Server.PropertyChanged += SetButtons;
        Disposed += (_, _) => Server.PropertyChanged -= SetButtons;
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

    protected void RaiseUpdateButtonClickedEvent(ServerConfig config) =>
        UpdateButtonClicked?.Invoke(null, (Id, config));

    protected abstract void UpdateButtonClick(object? sender, EventArgs e);

    protected void StartButtonClick(object? sender, EventArgs args)
    {
        StartButtonClicked?.Invoke(null, Id);
        StartButton.Enabled = false;
    }

    protected void StopButtonClick(object? sender, EventArgs args)
    {
        StopButtonClicked?.Invoke(null, Id);
        StopButton.Enabled = false;
    }

    private void SetButtons(object? _, PropertyChangedEventArgs args)
    {
        if(args.PropertyName != nameof(Server.Status)) return;

        var buttonToDisable = Server.Status ? StartButton : StopButton;
        var buttonToEnable = Server.Status ? StopButton : StartButton;

        SynchronizationContext?.Post(_ =>
        {
            buttonToDisable.Visible = false;
            buttonToEnable.Visible = true;
            buttonToEnable.Enabled = true;
        }, null);
    }
}