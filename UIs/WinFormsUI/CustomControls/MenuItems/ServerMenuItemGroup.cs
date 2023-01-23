using Shared;
using Shared.Config;
using Shared.Server;

namespace WinFormsUI.CustomControls.MenuItems;

internal class ServerMenuItemGroup : ProcessorMenuItemGroup
{
    public ServerMenuItemGroup(IServer<ServerConfig> processor) : base(processor)
    {
        DescriptionItem.Text = processor.CurrentConfig.Uri.ToString();
        DescriptionItem.Click += (_, _) => DescriptionClickInvoke(DescriptionItem.Text);

        var configUnsubscriber = processor.Subscribe(new Observer<ServerConfig>(ConfigChanged));
        Disposed += (_, _) => configUnsubscriber.Dispose();
    }

    private void ConfigChanged(ServerConfig config)
    {
        NameItem.Text = config.Name;
        DescriptionItem.Text = config.Uri.ToString();
    }
}