using System;

namespace Shared.Wrappers.Registry;

public enum RegValueType
{
    None = -1,
    Unknown = 0,
    String = 1,
    ExpandString = 2,
    Binary = 3,
    DWord = 4,
    MultiString = 7,
    QWord = 11,
}

public interface IRegistryKey : IDisposable
{
    public void DeleteValue(string name, bool throwOnMissingValue);
    public object? GetValue(string? name, object? defaultValue);
    public IRegistryKey? OpenSubKey(string name);
    public IRegistryKey? OpenSubKey(string name, bool writable);
    public IRegistryKey CreateSubKey(string name, bool writable);
    public void SetValue(string? name, object value, RegValueType valueKind);
}