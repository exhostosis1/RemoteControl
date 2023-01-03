using Shared.Config;

namespace Shared.ControlProcessor;

public interface IBotProcessor : IControlProcessor
{
    new BotConfig CurrentConfig { get; set; }
    public void Start(BotConfig? config = null);
    public void Restart(BotConfig? config = null);
}