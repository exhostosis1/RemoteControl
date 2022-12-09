using System.Collections.Generic;
using System.Linq;

namespace Shared.Config;

public class AppConfig
{
    public ICollection<ProcessorConfigItem> ProcessorConfigs { get; set; } = new List<ProcessorConfigItem>();

    public ProcessorConfigItem? GetProcessorConfigByName(string name) =>
        ProcessorConfigs.FirstOrDefault(x => x.Name == name);
}