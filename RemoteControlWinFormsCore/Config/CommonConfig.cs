using System.ComponentModel;

namespace RemoteControl.Config
{
    [DisplayName("Common")]
    internal class CommonConfig : IConfigItem
    {
        [DisplayName("autostart")] 
        internal bool Autostart { get; set; } = false;
    }
}
