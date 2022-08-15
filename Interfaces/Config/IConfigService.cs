namespace Shared.Config
{
    public interface IConfigService
    {
        public AppConfig GetConfig();
        public bool SetConfig(AppConfig config);
    }
}
