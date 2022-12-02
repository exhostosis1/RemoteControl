namespace Shared.Config;

public interface IConfigProvider
{
    public AppConfig GetConfig();
    public bool SetConfig(AppConfig config);
}