namespace Shared.Config;

public interface IConfigProvider
{
    public AppConfig GetConfig();
    public void SetConfig(AppConfig config);
}