using Shared.Config;

namespace Shared;

public interface IConfigProvider
{
    public AppConfig GetConfig();
    public bool SetConfig(AppConfig config);
}