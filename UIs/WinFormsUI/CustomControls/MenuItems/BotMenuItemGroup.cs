using Shared;
using Shared.Config;
using Shared.Server;

namespace WinFormsUI.CustomControls.MenuItems;

internal class BotMenuItemGroup : ProcessorMenuItemGroup
{
    public BotMenuItemGroup(IServer<BotConfig> processor) : base(processor)
    {
        DescriptionItem.Text = processor.CurrentConfig.UsernamesString;

        var configUnsubscriber = processor.Subscribe(new Observer<BotConfig>(ConfigChanged));
        Disposed += (_, _) => configUnsubscriber.Dispose();
    }

    private void ConfigChanged(BotConfig config)
    {
        NameItem.Text = config.Name;
        DescriptionItem.Text = config.UsernamesString;
    }
}