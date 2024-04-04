using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Shared.Config;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WindowsEntryPoint;

internal class JsonConfigurationProvider(string path) : IConfigurationProvider, IConfigurationSource
{
    public bool TryGet(string key, out string? value)
    {
        throw new NotImplementedException();
    }

    public void Set(string key, string? value)
    {
        throw new NotImplementedException();
    }

    public IChangeToken GetReloadToken()
    {
        throw new NotImplementedException();
    }

    public void Load()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath)
    {
        throw new NotImplementedException();
    }

    public AppConfig GetJson()
    {
        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<AppConfig>(json) ?? throw new NullReferenceException();
        }
        catch (Exception e)
        {
            
        }

        return new AppConfig();
    }

    public void WriteJson(AppConfig config)
    {
        try
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            
        }
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return this;
    }
}