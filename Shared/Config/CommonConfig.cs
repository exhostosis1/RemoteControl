namespace Shared.Config;

public abstract class CommonConfig
{
    public string Name { get; set; } = string.Empty;
    public bool Autostart { get; set; }
}