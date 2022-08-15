using Shared.Config;

namespace Shared.Interfaces
{
    public interface IConfigService
    {
        public AppConfig GetConfig();
        public bool SetConfig(AppConfig config);
    }
}
