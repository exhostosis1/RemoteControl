using System;
using System.Runtime.InteropServices;
using Shared.RegistryWrapper;
using Shared.RegistryWrapper.Registry;

#pragma warning disable CA1416

namespace Shared.Wrappers.RegistryWrapper;

public class RegistryWrapper : IRegistry
{
    public IRegistryKey CurrentUser => new RegistryKeyWrapper(Microsoft.Win32.Registry.CurrentUser);

    public RegistryWrapper()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");
    }
}