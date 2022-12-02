using Shared.Config;

namespace Shared;

public interface IControlProcessor
{
    public void Start(AppConfig config);
    public void Stop();
}