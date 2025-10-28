﻿using MainApp.Interfaces;
using MainApp.Servers;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MainApp;

internal class JsonConfigurationProvider(ILogger logger, string filePath) : IConfigurationProvider
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public List<ServerConfig> GetConfig()
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Getting config from file {filePath}", filePath);

        List<ServerConfig> result = [];

        if (!File.Exists(filePath))
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning("No config file");
            return result;
        }

        try
        {
            result = JsonSerializer.Deserialize<List<ServerConfig>>(File.ReadAllText(filePath)) ?? [];
        }
        catch (JsonException e)
        {
            if (logger.IsEnabled(LogLevel.Error))
                logger.LogError("{message}", e.Message);
        }

        return result;
    }

    public void SetConfig(IEnumerable<ServerConfig> appConfig)
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Writing config to file {filePath}", filePath);

        File.WriteAllText(filePath,
            JsonSerializer.Serialize(appConfig, _jsonOptions));
    }
}