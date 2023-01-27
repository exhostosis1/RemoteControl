using Shared;
using Shared.Config;
using Shared.Server;

namespace WinFormsUI.CustomControls.MenuItems;

internal class BotMenuItemGroup : ServerMenuItemGroup
{
    public BotMenuItemGroup(IServer<BotConfig> server) : base(server)
    {
        DescriptionItem.Text = server.CurrentConfig.UsernamesString;

        var configUnsubscriber = server.Subscribe(new Observer<BotConfig>(ConfigChanged));
        Disposed += (_, _) => configUnsubscriber.Dispose();
    }

    private void ConfigChanged(BotConfig config)
    {
        NameItem.Text = config.Name;
        DescriptionItem.Text = config.UsernamesString;
    }
}