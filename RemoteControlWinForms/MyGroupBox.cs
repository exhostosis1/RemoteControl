using Shared.Enums;

namespace RemoteControlWinForms;

internal sealed class MyGroupBox: GroupBox
{
    public string ProcessorName { get; set; }
    public ControlProcessorType ProcessorType { get; set; }
    public string ProcessorText { get => _textBox.Text; set => _textBox.Text = value; }

    private readonly TextBox _textBox = new()
    {
        Width = 238,
        Height = 23,
        Top = 22,
        Left = 6,
    };

    public MyGroupBox(string processorName, ControlProcessorType processorType, string processorText)
    {
        this.Width = 252;
        this.Height = 55;
        this.Left = 12;
        this.Text = ProcessorName;

        ProcessorName = processorName;
        ProcessorType = processorType;
        ProcessorText = processorText;

        this.Controls.Add(_textBox);
    }
}