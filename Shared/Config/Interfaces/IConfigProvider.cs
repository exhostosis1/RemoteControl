namespace Shared.Config.Interfaces
{
    public interface IConfigProvider
    {
        public AppConfig GetConfig();
        public bool SetConfig(AppConfig config);
    }
}
