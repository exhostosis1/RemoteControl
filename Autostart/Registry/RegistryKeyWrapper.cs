using System.Runtime.InteropServices;
using Microsoft.Win32;
using Shared.RegistryWrapper;

#pragma warning disable CA1416

namespace Autostart.Registry;

public class RegistryKeyWrapper : IRegistryKey
{
    private readonly RegistryKey _key;

    public RegistryKeyWrapper(RegistryKey key)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) throw new Exception("OS not supported");

        _key = key;
    }

    public void DeleteValue(string name, bool throwOnMissingValue) => _key.DeleteValue(name, throwOnMissingValue);

    public object? GetValue(string? name, object? defaultValue) => _key.GetValue(name, defaultValue);

    public IRegistryKey? OpenSubKey(string name)
    {
        var result = _key.OpenSubKey(name);
        return result == null ? null : new RegistryKeyWrapper(result);
    }

    public IRegistryKey? OpenSubKey(string name, bool writable)
    {
        var result = _key.OpenSubKey(name, writable);
        return result == null ? null : new RegistryKeyWrapper(result);
    }

    public void SetValue(string? name, object value, RegistryValueKind valueKind) =>
        _key.SetValue(name, value, valueKind);

    public void Dispose()
    {
        _key.Dispose();
    }
}