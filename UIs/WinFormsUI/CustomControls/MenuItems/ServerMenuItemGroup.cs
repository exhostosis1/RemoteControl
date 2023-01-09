using Shared;
using Shared.Config;
using Shared.ControlProcessor;

namespace WinFormsUI.CustomControls.MenuItems;

internal class ServerMenuItemGroup : ProcessorMenuItemGroup
{
    private readonly ToolStripMenuItem _descriptionItem = new();
    private readonly ToolStripMenuItem _startStopItem = new();

    private readonly IDisposable _statusUnsubscriber;
    private readonly IDisposable _configUnsubscriber;

    public ServerMenuItemGroup(ServerProcessor processor): base(processor)
    {
        var description = string.Empty;

        if (processor.Working)
        {
            _descriptionItem.Text = description = processor.CurrentConfig.Uri?.ToString();
            _descriptionItem.Click += (_, _) => DescriptionClickInvoke(_descriptionItem.Text ?? string.Empty);
        }
        else
        {
            _descriptionItem.Text = @"Stopped";
            _descriptionItem.Enabled = false;
        }
        Items.Add(_descriptionItem);

        _startStopItem.Text = !processor.Working ? @"Start" : @"Stop";
        _startStopItem.Click += StartStopClicked;

        Items.Add(_startStopItem);
        Items.Add(new ToolStripSeparator());

        _statusUnsubscriber = processor.Subscribe(new Observer<bool>(working =>
        {
            _descriptionItem.Text = working ? description : @"Stopped";
            _descriptionItem.Enabled = working;
            _startStopItem.Text = working ? @"Stop" : @"Start";
        }));

        _configUnsubscriber = processor.Subscribe(new Observer<ServerConfig>(config => _descriptionItem.Text = config.Uri?.ToString()));
        
        Disposed += LocalDispose;
    }

    private void StartStopClicked(object? _, EventArgs args)
    {
        if (_startStopItem.Text == @"Start")
        {
            StartClickInvoke(Id);
        }
        else
        {
            StopClickInvoke(Id);
        }
    }

    private void LocalDispose(object? sender, EventArgs args)
    {
        _statusUnsubscriber.Dispose();
        _configUnsubscriber.Dispose();
        
        _descriptionItem.Dispose();
        _startStopItem.Dispose();
    }
}