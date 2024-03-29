using Shared.Config;
using Shared.Server;

namespace MainUI.CustomControls.MenuItems;

internal class HttpMenuItemGroup : ServerMenuItemGroup
{
    public HttpMenuItemGroup(IServer<WebConfig> server) : base(server)
    {
        DescriptionItem.Text = server.CurrentConfig.Uri.ToString();
        DescriptionItem.Click += (_, _) => DescriptionClickInvoke(DescriptionItem.Text);

        server.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName != nameof(server.Config)) return;
            ConfigChanged(server.CurrentConfig);
        };
    }

    private void ConfigChanged(WebConfig config)
    {
        NameItem.Text = config.Name;
        DescriptionItem.Text = config.Uri.ToString();
    }
}