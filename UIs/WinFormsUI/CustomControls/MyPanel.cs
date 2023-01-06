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
        Location = new Point(60, 3),
        Size = new Size(155, 23)
    };

    protected readonly CheckBox AutostartBox = new()
    {
        Text = @"Autostart",
        Location = new Point(419, 28),
        Size = new Size(75, 19)
    };

    protected readonly Button StartButton = new()
    {
        Text = @"Start",
        Location = new Point(419, 2),
        Size = new Size(75, 23),
        Visible = false
    };

    protected readonly Button StopButton = new()
    {
        Text = @"Stop",
        Location = new Point(419, 2),
        Size = new Size(75, 23),
        Visible = false
    };

    protected readonly Button UpdateButton = new()
    {
        Text = @"Update",
        Location = new Point(419, 53),
        Size = new Size(75, 23),
        Enabled = false
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

    protected void StartButtonClick(object? sender, EventArgs args)
    {
        StartButtonClicked?.Invoke(ProcessorIndex);

        StartButton.Enabled = false;

        Task.Run(async () =>
        {
            while (true)
            {
                if (!ControlProcessor.Working)
                {
                    await Task.Delay(100);
                    continue;
                }

                SynchronizationContext?.Post(_ =>
                {
                    StartButton.Visible = false;
                    StopButton.Visible = true;
                    StopButton.Enabled = true;
                }, null);

                return;
            }
        });
    }

    protected void StopButtonClick(object? sender, EventArgs args)
    {
        StopButtonClicked?.Invoke(ProcessorIndex);

        StopButton.Enabled = false;

        Task.Run(async () =>
        {
            while (true)
            {
                if (ControlProcessor.Working)
                {
                    await Task.Delay(100);
                    continue;
                }

                SynchronizationContext?.Post(_ =>
                {
                    StopButton.Visible = false;
                    StartButton.Visible = true;
                    StartButton.Enabled = true;
                }, null);

                return;
            }
        });
    }
}