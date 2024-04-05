using Servers;
using Shared.Config;

namespace MainUI.CustomControls.MenuItems;

internal class BotMenuItemGroup : ServerMenuItemGroup
{
    public BotMenuItemGroup(Server server) : base(server)
    {
        DescriptionItem.Text = server.Config.UsernamesString;

        server.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName != nameof(server.Config)) return;

            ConfigChanged(server.Config);
        };
    }

    private void ConfigChanged(ServerConfig config)
    {
        NameItem.Text = config.Name;
        DescriptionItem.Text = config.UsernamesString;
    }
}