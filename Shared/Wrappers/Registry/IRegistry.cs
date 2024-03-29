using Shared.Wrappers.Registry;

namespace Shared.Wrappers.RegistryWrapper;

public interface IRegistry
{
    public IRegistryKey CurrentUser { get; }
}