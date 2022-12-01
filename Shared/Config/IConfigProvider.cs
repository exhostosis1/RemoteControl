namespace Shared.Config;

public interface IConfigProvider
{
    public ConfigItem GetConfig();
    public void SetConfig(ConfigItem config);
}