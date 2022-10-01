namespace Shared;

public interface IContainer
{
    public IAutostartService Autostart { get; }
    public IConfigProvider Config { get; }
    public IServer Server { get; }
    public IUserInterface UserInterface { get; }
}