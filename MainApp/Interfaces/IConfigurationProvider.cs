using MainApp.Servers;

namespace MainApp.Interfaces;

public interface IConfigurationProvider
{
    List<ServerConfig> GetConfig();
    void SetConfig(IEnumerable<ServerConfig> config);
}