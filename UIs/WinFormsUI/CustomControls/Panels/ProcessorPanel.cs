using Shared;
using Shared.Config;
using Shared.Server;

namespace WinFormsUI.CustomControls.Panels;

internal abstract class ProcessorPanel : Panel
{
    public int Id { get; init; }
    protected readonly IServer ControlProcessor;
    protected readonly SynchronizationContext? SynchronizationContext;
    protected readonly IDisposable Unsubscriber;
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

    protected readonly CheckBox AutostartBox = new()
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
    public event EventHandler<(int, CommonConfig)>? UpdateButtonClicked;

    protected ProcessorPanel(IServer processor)
    {
        Width = 508;
        Height = 90;
        Left = 12;
        BorderStyle = BorderStyle.FixedSingle;

        Id = processor.Id;
        ControlProcessor = processor;
        NameTextBox.Text = processor.Config.Name;

        SynchronizationContext = SynchronizationContext.Current;

        if (processor.Status.Working)
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

        AutostartBox.Checked = processor.Config.Autostart;

        NameTextBox.TextChanged += EnableUpdateButton;
        AutostartBox.CheckedChanged += EnableUpdateButton;

        Controls.AddRange(new Control[]
        {
            NameLabel,
            NameTextBox,
            AutostartBox,
            StartButton,
            StopButton,
            UpdateButton
        });

        Unsubscriber = ControlProcessor.Status.Subscribe(new Observer<bool>(SetButtons));
        Disposed += (_, _) => Unsubscriber.Dispose();
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

    protected void RaiseUpdateButtonClickedEvent(CommonConfig config) =>
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

    private void SetButtons(bool working)
    {
        var buttonToDisable = working ? StartButton : StopButton;
        var buttonToEnable = working ? StopButton : StartButton;

        SynchronizationContext?.Post(_ =>
        {
            buttonToDisable.Visible = false;
            buttonToEnable.Visible = true;
            buttonToEnable.Enabled = true;
        }, null);
    }
}