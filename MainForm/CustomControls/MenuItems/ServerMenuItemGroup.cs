using MainApp.Config;
using Servers;

namespace MainUI.CustomControls.MenuItems;

internal class HttpMenuItemGroup : ServerMenuItemGroup
{
    public HttpMenuItemGroup(Server server) : base(server)
    {
        DescriptionItem.Text = server.Config.Uri.ToString();
        DescriptionItem.Click += (_, _) => DescriptionClickInvoke(DescriptionItem.Text);

        server.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName != nameof(server.Config)) return;
            ConfigChanged(server.Config);
        };
    }

    private void ConfigChanged(ServerConfig config)
    {
        NameItem.Text = config.Name;
        DescriptionItem.Text = config.Uri.ToString();
    }
}