using Shared;
using Shared.Config;
using Shared.ControlProcessor;

namespace WinFormsUI.CustomControls.MenuItems;

internal abstract class ProcessorMenuItemGroup : IDisposable
{
    public int Id { get; init; }
    protected readonly AbstractControlProcessor Processor;

    protected readonly ToolStripMenuItem NameItem = new()
    {
        Enabled = false
    };

    protected readonly List<ToolStripItem> Items = new();

    public ToolStripItem[] ItemsArray => Items.ToArray();
    
    protected readonly IDisposable ConfigUnsubscriber;

    public event StringEventHandler? OnDescriptionClick;
    public event IntEventHandler? OnStartClick;
    public event IntEventHandler? OnStopClick;

    protected event EventHandler? Disposed;

    protected ProcessorMenuItemGroup(AbstractControlProcessor processor)
    {
        Id = processor.Id;
        Processor = processor;

        NameItem.Text = processor.Config.Name;

        Items.Add(NameItem);

        ConfigUnsubscriber = processor.Subscribe(new Observer<CommonConfig>(ConfigChanged));
    }

    protected void StartClickInvoke(int id) => OnStartClick?.Invoke(id);
    protected void StopClickInvoke(int id) => OnStopClick?.Invoke(id);
    protected void DescriptionClickInvoke(string value) => OnDescriptionClick?.Invoke(value);

    private void ConfigChanged(CommonConfig config)
    {
        NameItem.Text = config.Name;
    }

    public void Dispose()
    {
        ConfigUnsubscriber.Dispose();
        NameItem.Dispose();
        
        Disposed?.Invoke(this, EventArgs.Empty);
    }
}