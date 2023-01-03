using Shared.ControlProcessor;

namespace RemoteControlWinForms;

internal sealed class ServerPanel : MyPanel
{
    private readonly Label _schemeLabel = new()
    {
        Text = @"Scheme",
        Location = new Point(3, 36),
        Size = new Size(43, 15)
    };

    private readonly TextBox _schemeTextBox = new()
    {
        Location = new Point(61, 32),
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
}