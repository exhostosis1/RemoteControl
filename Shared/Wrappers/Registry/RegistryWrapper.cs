using System;
using System.Runtime.InteropServices;
using Shared.Wrappers.RegistryWrapper;

#pragma warning disable CA1416

namespace Shared.Wrappers.Registry;

public class RegistryWrapper : IRegistry
{
    public IRegistryKey CurrentUser => new RegistryKeyWrapper(Microsoft.Win32.Registry.CurrentUser);

    public RegistryWrapper()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");
    }
}