using Autostart;
using Microsoft.Win32;
using Moq;
using Shared.Logging.Interfaces;
using Shared.RegistryWrapper;
using System.Diagnostics;

namespace Tests;

public class WinregistryAutostartServiceTests: IDisposable
{
    private readonly WinRegistryAutostartService _service;
    private static MockCurrentUser _regKey;

    private class MockCurrentUser : IRegistryKey
    {
        public string Name { get; set; } = string.Empty;
        public string? Value { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void DeleteValue(string name, bool throwOnMissingValue)
        {
            if (Name != "Run") throw new InvalidOperationException();
        }

        public object? GetValue(string? name, object? defaultValue)
        {
            if (Name != "Run" || name != "Remote Control") throw new InvalidOperationException();

            return $"\"{Process.GetCurrentProcess().MainModule?.FileName}\"";
        }

        public IRegistryKey? OpenSubKey(string name)
        {
            var result = new MockCurrentUser()
            {
                Name = name
            };

            _regKey = result;
            return result;
        }

        public IRegistryKey? OpenSubKey(string name, bool writable)
        {
            return OpenSubKey(name);
        }

        public void SetValue(string? name, object value, RegistryValueKind valueKind)
        {
            if (Name != "Run" || name != "Remote Control") throw new InvalidOperationException();

            Value = value.ToString();
        }
    }

    private class MockRegistry : IRegistry
    {
        public IRegistryKey CurrentUser { get; } = new MockCurrentUser();
    }

    public WinregistryAutostartServiceTests()
    {
        var logger = Mock.Of<ILogger<WinRegistryAutostartService>>();
        var registry = new MockRegistry();
        _service = new WinRegistryAutostartService(registry, logger);
    }

    [Fact]
    public void CheckAutostartTest()
    {
        var result = _service.CheckAutostart();
        Assert.True(result);
    }

    [Fact]
    public void SetAutostartTest()
    {
        _service.SetAutostart(true);
        Assert.True(_regKey.Value == $"\"{Process.GetCurrentProcess().MainModule?.FileName}\"");
    }

    public void Dispose()
    {
    }
}