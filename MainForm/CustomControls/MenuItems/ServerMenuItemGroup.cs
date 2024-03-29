using Shared.Config;
using Shared.Observable;
using Shared.Server;

namespace MainUI.CustomControls.MenuItems;

internal class HttpMenuItemGroup : ServerMenuItemGroup
{
    public HttpMenuItemGroup(IServer<WebConfig> server) : base(server)
    {
        DescriptionItem.Text = server.CurrentConfig.Uri.ToString();
        DescriptionItem.Click += (_, _) => DescriptionClickInvoke(DescriptionItem.Text);

        var configUnsubscriber = server.Subscribe(new MyObserver<WebConfig>(ConfigChanged));
        Disposed += (_, _) => configUnsubscriber.Dispose();
    }

    private void ConfigChanged(WebConfig config)
    {
        NameItem.Text = config.Name;
        DescriptionItem.Text = config.Uri.ToString();
    }
}