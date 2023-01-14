namespace Shared.RegistryWrapper;

public interface IRegistry
{
    public IRegistryKey CurrentUser { get; }
}