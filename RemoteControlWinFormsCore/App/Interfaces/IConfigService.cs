using RemoteControl.Config;

namespace RemoteControl.App.Interfaces
{
    public interface IConfigService
    {
        public AppConfig GetConfig();
        public bool SetConfig(AppConfig config);
    }
}
