using Shared;
using Shared.Config;
using Shared.ControlProcessor;

namespace WinFormsUI.CustomControls;

internal abstract class MyPanel : Panel
{
    public int ProcessorIndex { get; init; }
    protected readonly IControlProcessor ControlProcessor;
    protected readonly SynchronizationContext? SynchronizationContext;

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

    public event IntEventHandler? StartButtonClicked;
    public event IntEventHandler? StopButtonClicked;
    public event ConfigEventHandler? UpdateButtonClicked;

    protected MyPanel(IControlProcessor processor, int index)
    {
        Width = 508;
        Height = 90;
        Left = 12;
        BorderStyle = BorderStyle.FixedSingle;

        ProcessorIndex = index;
        ControlProcessor = processor;
        NameTextBox.Text = processor.CurrentConfig.Name;

        SynchronizationContext = SynchronizationContext.Current;

        if (processor.Working)
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

        AutostartBox.Checked = processor.CurrentConfig.Autostart;

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
    }

    protected void EnableUpdateButton(object? sender, EventArgs e) => UpdateButton.Enabled = true;

    protected void RaiseUpdateButtonClickedEvent(CommonConfig config) =>
        UpdateButtonClicked?.Invoke(ProcessorIndex, config);

    protected abstract void UpdateButtonClick(object? sender, EventArgs e);

    protected async void StartButtonClick(object? sender, EventArgs args)
    {
        StartButtonClicked?.Invoke(ProcessorIndex);

        StartButton.Enabled = false;

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var token = cts.Token;

        await WaitForWork(true, token);
    }

    protected async void StopButtonClick(object? sender, EventArgs args)
    {
        StopButtonClicked?.Invoke(ProcessorIndex);

        StopButton.Enabled = false;

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var token = cts.Token;

        await WaitForWork(false, token);
    }

    private async Task WaitForWork(bool started, CancellationToken token)
    {
        var buttonToDisable = started ? StartButton : StopButton;
        var buttonToEnable = started ? StopButton : StartButton;

        while (!token.IsCancellationRequested)
        {
            if (started ? !ControlProcessor.Working : ControlProcessor.Working)
            {
                try
                {
                    await Task.Delay(100, token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }

            SynchronizationContext?.Post(_ =>
            {
                buttonToDisable.Visible = false;
                buttonToEnable.Visible = true;
                buttonToEnable.Enabled = true;
            }, null);

            return;
        }
    }
}