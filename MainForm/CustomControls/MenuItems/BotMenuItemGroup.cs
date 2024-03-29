using Shared.Config;
using Shared.Server;

namespace MainUI.CustomControls.MenuItems;

internal class BotMenuItemGroup : ServerMenuItemGroup
{
    public BotMenuItemGroup(IServer<BotConfig> server) : base(server)
    {
        DescriptionItem.Text = server.CurrentConfig.UsernamesString;

        server.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName != nameof(server.Config)) return;

            ConfigChanged(server.CurrentConfig);
        };
    }

    private void ConfigChanged(BotConfig config)
    {
        NameItem.Text = config.Name;
        DescriptionItem.Text = config.UsernamesString;
    }
}