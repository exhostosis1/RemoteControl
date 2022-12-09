namespace RemoteControlWinForms;

internal class ToolStripItemWithIntEvent: ToolStripMenuItem
{
    public string? ClickStringValue { get; set; }

    protected override void OnClick(EventArgs e)
    {
        var args = new StringArgs()
        {
            Value = ClickStringValue
        };

        base.OnClick(args);
    }
}