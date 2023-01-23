using Shared;
using Shared.Server;

namespace WinFormsUI.CustomControls.MenuItems;

internal abstract class ProcessorMenuItemGroup : IDisposable
{
    public int Id { get; init; }
    protected readonly IServer Processor;

    protected readonly ToolStripMenuItem NameItem = new()
    {
        Enabled = false
    };

    protected readonly ToolStripMenuItem StartStopItem = new();
    protected readonly ToolStripMenuItem DescriptionItem = new();

    protected readonly List<ToolStripItem> Items = new();

    public ToolStripItem[] ItemsArray => Items.ToArray();

    protected readonly IDisposable StatusUnsubscriber;

    public event EventHandler<string>? OnDescriptionClick;
    public event EventHandler<int>? OnStartClick;
    public event EventHandler<int>? OnStopClick;

    protected event EventHandler? Disposed;

    protected ProcessorMenuItemGroup(IServer processor)
    {
        Id = processor.Id;
        Processor = processor;

        NameItem.Text = processor.Config.Name;
        Items.Add(NameItem);

        DescriptionItem.Enabled = processor.Status.Working;
        Items.Add(DescriptionItem);

        StartStopItem.Text = !processor.Status.Working ? @"Start" : @"Stop";
        StartStopItem.Click += StartStopClicked;
        Items.Add(StartStopItem);

        Items.Add(new ToolStripSeparator());

        StatusUnsubscriber = processor.Status.Subscribe(new Observer<bool>(working =>
        {
            StartStopItem.Text = working ? @"Stop" : @"Start";
            DescriptionItem.Enabled = working;
        }));
    }

    private void StartStopClicked(object? _, EventArgs args)
    {
        if (StartStopItem.Text == @"Start")
        {
            OnStartClick?.Invoke(null, Id);
        }
        else
        {
            OnStopClick?.Invoke(null, Id);
        }
    }

    protected void DescriptionClickInvoke(string value) => OnDescriptionClick?.Invoke(null, value);



    public void Dispose()
    {
        StatusUnsubscriber.Dispose();

        NameItem.Dispose();
        DescriptionItem.Dispose();

        Disposed?.Invoke(this, EventArgs.Empty);
    }
}