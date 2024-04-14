using Servers;

namespace MainUI.CustomControls.MenuItems;

internal abstract class ServerMenuItemGroup : IDisposable
{
    public int Id { get; init; }
    protected readonly Server Server;

    protected readonly ToolStripMenuItem NameItem = new()
    {
        Enabled = false
    };

    protected readonly ToolStripMenuItem StartStopItem = new();
    protected readonly ToolStripMenuItem DescriptionItem = new();

    protected readonly List<ToolStripItem> Items = [];

    public ToolStripItem[] ItemsArray => [.. Items];

    public event EventHandler<string>? OnDescriptionClick;

    protected ServerMenuItemGroup(Server server)
    {
        Server = server;

        NameItem.Text = server.Config.Name;
        Items.Add(NameItem);

        DescriptionItem.Enabled = server.Status;
        Items.Add(DescriptionItem);

        StartStopItem.Text = !server.Status ? @"Start" : @"Stop";
        StartStopItem.Click += StartStopClicked;
        Items.Add(StartStopItem);

        Items.Add(new ToolStripSeparator());

        server.PropertyChanged += (_, args) =>
        {
            if(args.PropertyName != nameof(server.Status)) return;
            
            StartStopItem.Text = server.Status ? @"Stop" : @"Start";
            DescriptionItem.Enabled = server.Status;
        };
    }

    private void StartStopClicked(object? _, EventArgs args)
    {
        if (StartStopItem.Text == @"Start")
        {
            Server.Start();
        }
        else
        {
            Server.Stop();
        }
    }

    protected void DescriptionClickInvoke(string value) => OnDescriptionClick?.Invoke(null, value);

    public void Dispose()
    {
        StartStopItem.Click -= StartStopClicked;
    }
}