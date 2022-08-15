using System.ComponentModel;
using Shared.Interfaces;

namespace Shared.Config
{
    [DisplayName("Common")]
    public class CommonConfig : IConfigItem
    {
        [DisplayName("autostart")]
        public bool Autostart { get; set; } = false;
    }
}
