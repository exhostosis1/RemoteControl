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

    protected readonly ToolStripMenuItem StartStopItem = new();
    protected readonly ToolStripMenuItem DescriptionItem = new();

    protected readonly List<ToolStripItem> Items = new();

    public ToolStripItem[] ItemsArray => Items.ToArray();
    
    protected readonly IDisposable ConfigUnsubscriber;
    protected readonly IDisposable StatusUnsubscriber;

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

        SetDescriptionText(processor.Config);
        DescriptionItem.Enabled = processor.Working;
        Items.Add(DescriptionItem);

        StartStopItem.Text = !processor.Working ? @"Start" : @"Stop";
        StartStopItem.Click += StartStopClicked;
        Items.Add(StartStopItem);

        Items.Add(new ToolStripSeparator());

        ConfigUnsubscriber = processor.Subscribe(new Observer<CommonConfig>(ConfigChanged));
        StatusUnsubscriber = processor.Subscribe(new Observer<bool>(working =>
        {
            StartStopItem.Text = working ? @"Stop" : @"Start";
            DescriptionItem.Enabled = working;
        }));
    }

    protected void SetDescriptionText(CommonConfig config)
    {
        DescriptionItem.Text = config switch
        {
            ServerConfig s => s.Uri?.ToString(),
            BotConfig b => b.UsernamesString,
            _ => throw new NotSupportedException()
        };
    }

    private void StartStopClicked(object? _, EventArgs args)
    {
        if (StartStopItem.Text == @"Start")
        {
            OnStartClick?.Invoke(Id);
        }
        else
        {
            OnStopClick?.Invoke(Id);
        }
    }
    
    protected void DescriptionClickInvoke(string value) => OnDescriptionClick?.Invoke(value);

    private void ConfigChanged(CommonConfig config)
    {
        NameItem.Text = config.Name;
        SetDescriptionText(config);
    }

    public void Dispose()
    {
        ConfigUnsubscriber.Dispose();
        StatusUnsubscriber.Dispose();

        NameItem.Dispose();
        DescriptionItem.Dispose();

        Disposed?.Invoke(this, EventArgs.Empty);
    }
}