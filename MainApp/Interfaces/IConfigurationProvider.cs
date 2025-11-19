using MainApp.Workers;

namespace MainApp.Interfaces;

public interface IConfigurationProvider
{
    List<WorkerConfig> GetConfig();
    void SetConfig(IEnumerable<WorkerConfig> config);
}