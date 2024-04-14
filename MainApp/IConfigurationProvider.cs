using Servers;

namespace MainApp;

public interface IConfigurationProvider
{
    List<ServerConfig> GetConfig();
    void SetConfig(IEnumerable<ServerConfig> config);
}