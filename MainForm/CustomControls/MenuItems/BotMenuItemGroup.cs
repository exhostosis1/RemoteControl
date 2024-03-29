using Shared.Config;
using Shared.Observable;
using Shared.Server;

namespace MainUI.CustomControls.MenuItems;

internal class BotMenuItemGroup : ServerMenuItemGroup
{
    public BotMenuItemGroup(IServer<BotConfig> server) : base(server)
    {
        DescriptionItem.Text = server.CurrentConfig.UsernamesString;

        var configUnsubscriber = server.Subscribe(new MyObserver<BotConfig>(ConfigChanged));
        Disposed += (_, _) => configUnsubscriber.Dispose();
    }

    private void ConfigChanged(BotConfig config)
    {
        NameItem.Text = config.Name;
        DescriptionItem.Text = config.UsernamesString;
    }
}