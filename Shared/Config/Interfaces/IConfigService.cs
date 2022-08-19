using Shared.Config;

namespace Shared.Config.Interfaces
{
    public interface IConfigService
    {
        public AppConfig GetConfig();
        public bool SetConfig(AppConfig config);
    }
}
