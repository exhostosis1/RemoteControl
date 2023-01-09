using Shared.ControlProcessor;

namespace WinFormsUI.CustomControls.MenuItems;

internal class ServerMenuItemGroup : ProcessorMenuItemGroup
{
    public ServerMenuItemGroup(ServerProcessor processor): base(processor)
    {
        DescriptionItem.Click += (_, _) => DescriptionClickInvoke(DescriptionItem.Text);
    }
}