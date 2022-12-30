using Shared.Enums;

namespace RemoteControlWinForms;

internal sealed class MyToolStripMenuItem: ToolStripMenuItem
{
    public string ProcessorName { get; set; } = string.Empty;
    public ControlProcessorType ProcessorType { get; set; } = ControlProcessorType.Common;

    protected override void OnClick(EventArgs e)
    {
        var args = new MyEventArgs
        {
            ProcessorName = ProcessorName,
            ProcessorType = ProcessorType
        };

        base.OnClick(args);
    }
}