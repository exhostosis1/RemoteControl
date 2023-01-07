namespace Shared.Config;

public interface IConfigProvider
{
    public SerializableAppConfig GetSerializableConfig();
    public AppConfig GetConfig();
    public void SetSerializableConfig(SerializableAppConfig config);
    public void SetConfig(AppConfig config);
}