using Microsoft.Win32;
using System;

namespace Shared.RegistryWrapper;

public interface IRegistryKey : IDisposable
{
    public void DeleteValue(string name, bool throwOnMissingValue);
    public object? GetValue(string? name, object? defaultValue);
    public IRegistryKey? OpenSubKey(string name);
    public IRegistryKey? OpenSubKey(string name, bool writable);
    public void SetValue(string? name, object value, RegistryValueKind valueKind);
}