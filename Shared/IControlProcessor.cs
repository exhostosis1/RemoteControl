using Shared.Config;

namespace Shared;

public interface IControlProcessor
{
    public void Start(ConfigItem config);
    public void Stop();
}