using Shared.Config;

namespace Shared;

public interface IConfigProvider
{
    public AppConfig Config { get; set; }
}