using Shared.Wrappers.RegistryWrapper;

namespace Shared.RegistryWrapper;

public interface IRegistry
{
    public IRegistryKey CurrentUser { get; }
}