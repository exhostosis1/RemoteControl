using System.ComponentModel;
using Shared.Config.Interfaces;

namespace Shared.Config
{
    [DisplayName("Common")]
    public class CommonConfig : IConfigItem
    {
        [DisplayName("autostart")]
        public bool Autostart { get; set; } = false;
    }
}
