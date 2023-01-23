using Shared.Bots.Telegram.ApiObjects.Request;
using Shared.Bots.Telegram.ApiObjects.Response;
using Shared.Bots.Telegram.ApiObjects.Response.Keyboard;
using Shared.DataObjects.Bot;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Bots.Telegram;

public class TelegramBotApiProvider : IBotApiProvider
{
    class LocalHttpRequest : IHttpClientRequest
    {
        public HttpMethod Method { get; set; }
        public string RequestUri { get; set; }
        public string? Content { get; set; }

        public LocalHttpRequest(HttpMethod method, string requestUri)
        {
            Method = method;
            RequestUri = requestUri;
        }
    }

    private int? _lastUpdateId = null;

    private readonly IHttpClient _client;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly ILogger<TelegramBotApiProvider> _logger;

    public TelegramBotApiProvider(IHttpClient client, ILogger<TelegramBotApiProvider> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<T> SendBotApiRequestAsync<T>(string apiUrl, string apiKey, string method, object parameters, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        return await _client.PostAsJsonAndGetResultAsync<T>($"{apiUrl}{apiKey}/{method}", parameters, _jsonOptions, token);
    }

    public async Task SendBotApiRequestAsync(string apiUrl, string apiKey, string method, object parameters, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        await _client.PostAsJsonAsync($"{apiUrl}{apiKey}/{method}", parameters, _jsonOptions, token);
    }

    public void SendBotApiRequest(string apiUrl, string apiKey, string method, object parameters)
    {
        var request = new LocalHttpRequest(HttpMethod.Post, $"{apiUrl}{apiKey}/{method}")
        {
            Content = JsonSerializer.Serialize(parameters, _jsonOptions)
        };

        _client.Send(request);
    }

    public T SendBotApiRequest<T>(string apiUrl, string apiKey, string method, object parameters) where T: class
    {
        var request = new LocalHttpRequest(HttpMethod.Post, $"{apiUrl}{apiKey}/{method}")
        {
            Content = JsonSerializer.Serialize(parameters, _jsonOptions)
        };

        var response = _client.Send(request);

        return response.IsSuccessStatusCode
            ? response.Content?.ReadFromJsonAsync<T>().GetAwaiter().GetResult() ??
              throw new JsonException("Cannot parse response")
            : throw new Exception($"Server returned error with code {response.StatusCode}");
    }

    private static KeyboardMarkup? GenerateKeyboardMarkup(ButtonsMarkup? buttons)
    {
        return buttons switch
        {
            ReplyButtonsMarkup reply => new ReplyKeyboardMarkup
            {
                Keyboard = reply.Items.Select(x => x.Select(y => new KeyboardButton { Text = y.Text }).ToArray())
                    .ToArray(),
                ResizeKeyboard = reply.Resize,
                OneTimeKeyboard = reply.OneTime,
                Persistent = reply.Persistent
            },
            RemoveButtonsMarkup remove => new ReplyKeyboardRemove(),
            _ => null
        };
    }

    public async Task<UpdateResponse> GetUpdatesAsync(string apiUrl, string apiKey, CancellationToken token = default)
    {
        var response = await SendBotApiRequestAsync<UpdateResponse>(apiUrl, apiKey, ApiMethods.GetUpdates, new GetUpdatesParameters(_lastUpdateId), token);

        _lastUpdateId = response.Result.LastOrDefault()?.UpdateId;

        return response;
    }

    public UpdateResponse GetUpdates(string apiUrl, string apiKey)
    {
        var response = SendBotApiRequest<UpdateResponse>(apiUrl, apiKey, ApiMethods.GetUpdates, new GetUpdatesParameters(_lastUpdateId));

        _lastUpdateId = response.Result.LastOrDefault()?.UpdateId;

        return response;
    }

    private SendMessageParameters GetParameters(int chatId, string message, ButtonsMarkup? buttons)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message can not be empty", nameof(message));

        var parameters = new SendMessageParameters
        {
            ChatId = chatId,
            Text = message,
            ReplyMarkup = GenerateKeyboardMarkup(buttons)
        };

        return parameters;
    }

    public async Task SendResponseAsync(string apiUrl, string apiKey, int chatId, string message, ButtonsMarkup? buttons = null, CancellationToken token = default) =>
        await SendBotApiRequestAsync(apiUrl, apiKey, ApiMethods.SendMessage, GetParameters(chatId, message, buttons), token);

    public void SendResponse(string apiUrl, string apiKey, int chatId, string message, ButtonsMarkup? buttons = null) =>
        SendBotApiRequest(apiUrl, apiKey, ApiMethods.SendMessage, GetParameters(chatId, message, buttons));
}