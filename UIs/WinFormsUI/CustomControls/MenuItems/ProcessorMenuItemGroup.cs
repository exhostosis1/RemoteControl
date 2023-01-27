using Shared;
using Shared.Server;

namespace WinFormsUI.CustomControls.MenuItems;

internal abstract class ServerMenuItemGroup : IDisposable
{
    public int Id { get; init; }
    protected readonly IServer Server;

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

    protected ServerMenuItemGroup(IServer server)
    {
        Id = server.Id;
        Server = server;

        NameItem.Text = server.Config.Name;
        Items.Add(NameItem);

        DescriptionItem.Enabled = server.Status.Working;
        Items.Add(DescriptionItem);

        StartStopItem.Text = !server.Status.Working ? @"Start" : @"Stop";
        StartStopItem.Click += StartStopClicked;
        Items.Add(StartStopItem);

        Items.Add(new ToolStripSeparator());

        StatusUnsubscriber = server.Status.Subscribe(new Observer<bool>(working =>
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