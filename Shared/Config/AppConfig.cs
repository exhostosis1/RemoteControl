using Shared.Config.Interfaces;

namespace Shared.Config
{
    public class AppConfig
    {
        public UriConfig UriConfig { get; set; } = new();
        public CommonConfig Common { get; set; } = new();
    }
}
