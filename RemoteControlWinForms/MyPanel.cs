using Shared.ControlProcessor;

namespace RemoteControlWinForms;

internal abstract class MyPanel: Panel
{
    public int ProcessorIndex { get; init; }

    private readonly Label _nameLabel = new()
    {
        Text = @"Name",
        Location = new Point(6, 6),
        Size = new Size(39, 15)
    };

    private readonly TextBox _nameTextBox = new()
    {
        Location = new Point(60, 3),
        Size = new Size(155, 23)
    };

    private readonly CheckBox _autostartBox = new()
    {
        Text = @"Autostart",
        Location = new Point(419, 28),
        Size = new Size(75, 19)
    };

    private readonly Button _startButton = new()
    {
        Text = @"Start",
        Location = new Point(419, 2),
        Size = new Size(75, 23),
        Visible = false
    };

    private readonly Button _stopButton = new()
    {
        Text = @"Stop",
        Location = new Point(419, 2),
        Size = new Size(75, 23),
        Visible = false
    };

    private readonly Button _updateButton = new()
    {
        Text = @"Update",
        Location = new Point(419, 53),
        Size = new Size(75, 23),
        Enabled = false
    };

    protected MyPanel(IControlProcessor processor, int index)
    {
        this.Width = 508;
        this.Height = 90;
        this.Left = 12;
        this.BorderStyle = BorderStyle.FixedSingle;

        ProcessorIndex = index;
        _nameTextBox.Text = processor.CurrentConfig.Name;

        if (processor.Working)
        {
            _stopButton.Visible = true;
        }
        else
        {
            _startButton.Visible = true;
        }

        _autostartBox.Checked = processor.CurrentConfig.Autostart;

        this.Controls.AddRange(new Control[]
        {
            _nameLabel,
            _nameTextBox,
            _autostartBox,
            _startButton,
            _stopButton,
            _updateButton
        });
    }
}