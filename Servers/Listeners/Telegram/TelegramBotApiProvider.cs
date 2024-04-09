using Servers.Listeners.Telegram.ApiObjects.Request;
using Servers.Listeners.Telegram.ApiObjects.Response;
using Servers.Listeners.Telegram.ApiObjects.Response.Keyboard;
using Shared;
using Shared.DataObjects.Bot;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Servers.Listeners.Telegram;

public class TelegramBotApiProvider
{
    private int? _lastUpdateId = null;

    private readonly HttpClient _client = new()
    {
        Timeout = TimeSpan.FromSeconds(15)
    };

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

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
        var request = new HttpRequestMessage(HttpMethod.Post, $"{apiUrl}{apiKey}/{method}")
        {
            Content = new StringContent(JsonSerializer.Serialize(parameters, _jsonOptions), Encoding.UTF8, "application/json")
        };

        _client.Send(request);
    }

    public T SendBotApiRequest<T>(string apiUrl, string apiKey, string method, object parameters) where T: class
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{apiUrl}{apiKey}/{method}")
        {
            Content = new StringContent(JsonSerializer.Serialize(parameters, _jsonOptions), Encoding.UTF8, "application/json")
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