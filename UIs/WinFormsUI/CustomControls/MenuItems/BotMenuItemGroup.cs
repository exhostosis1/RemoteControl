using Shared.Config;
using Shared;
using Shared.ControlProcessor;

namespace WinFormsUI.CustomControls.MenuItems;

internal class BotMenuItemGroup : ProcessorMenuItemGroup
{
    public BotMenuItemGroup(BotProcessor processor) : base(processor)
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