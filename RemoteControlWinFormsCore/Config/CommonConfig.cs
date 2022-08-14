using System.ComponentModel;
using RemoteControl.App.Interfaces;

namespace RemoteControl.Config
{
    [DisplayName("Common")]
    public class CommonConfig : IConfigItem
    {
        [DisplayName("autostart")] 
        internal bool Autostart { get; set; } = false;
    }
}
