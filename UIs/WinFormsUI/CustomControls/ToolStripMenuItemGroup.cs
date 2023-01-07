using Shared;
using Shared.Config;
using Shared.ControlProcessor;

namespace WinFormsUI.CustomControls;

internal class ToolStripMenuItemGroup: IDisposable
{
    public int Id { get; set; }
    private readonly IControlProcessor _processor;

    private readonly ToolStripMenuItem _nameItem = new()
    {
        Enabled = false
    };
    private readonly ToolStripMenuItem _descriptionItem = new();

    private readonly ToolStripMenuItem _startStopItem = new();

    public event EventHandler? OnDescriptionClick;
    public event IntEventHandler? OnStartClick;
    public event IntEventHandler? OnStopClick;

    public readonly ToolStripItem[] ItemsArray = new ToolStripItem[4];

    private readonly IDisposable _unsubscriber;

    public ToolStripMenuItemGroup(IControlProcessor processor)
    {
        Id = processor.Id;
        _processor = processor;

        _nameItem.Text = processor.CurrentConfig.Name;

        ItemsArray[0] = _nameItem;

        var description = string.Empty;

        if (processor.Working)
        {
            switch (processor)
            {
                case IServerProcessor s:
                    _descriptionItem.Text = description = s.CurrentConfig.Uri?.ToString();
                    _descriptionItem.Click += (sender, args) => OnDescriptionClick?.Invoke(sender, args);
                    break;
                case IBotProcessor b:
                    _descriptionItem.Text = description = b.CurrentConfig.UsernamesString;
                    break;
            }
        }
        else
        {
            _descriptionItem.Text = @"Stopped";
            _descriptionItem.Enabled = false;
        }
        ItemsArray[1] = _descriptionItem;

        _startStopItem.Text = !processor.Working ? @"Start" : @"Stop";
        _startStopItem.Click += StartStopClicked;

        ItemsArray[2] = _startStopItem;
        ItemsArray[3] = new ToolStripSeparator();

        _unsubscriber = processor.Subscribe(new Observer<bool>(working =>
        {
            _descriptionItem.Text = working ? description : @"Stopped";
            _descriptionItem.Enabled = working;
            _startStopItem.Text = working ? @"Stop" : @"Start";
        }));

        processor.ConfigChanged += ConfigChanged;
    }

    private void ConfigChanged(CommonConfig config)
    {
            _nameItem.Text = config.Name;
            _descriptionItem.Text = config switch
            {
                ServerConfig s => s.Uri?.ToString(),
                BotConfig b => b.UsernamesString,
                _ => _descriptionItem.Text
            };
    }

    private void StartStopClicked(object? _, EventArgs args)
    {
        if(_startStopItem.Text == @"Start")
        {
            OnStartClick?.Invoke(Id);
        }
        else
        {
            OnStopClick?.Invoke(Id);
        }
    }

    public void Dispose()
    {
        _unsubscriber.Dispose();
        _processor.ConfigChanged -= ConfigChanged;

        _nameItem.Dispose();
        _descriptionItem.Dispose();
        _startStopItem.Dispose();
    }
}