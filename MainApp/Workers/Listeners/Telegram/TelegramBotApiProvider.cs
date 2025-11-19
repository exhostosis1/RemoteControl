using MainApp.Workers.Listeners.Telegram.ApiObjects.Request;
using MainApp.Workers.Listeners.Telegram.ApiObjects.Response;
using MainApp.Workers.Listeners.Telegram.ApiObjects.Response.Keyboard;
using MainApp.Workers.Listeners.Telegram.BotButtons;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MainApp.Workers.Listeners.Telegram;

internal static class HttpClientExtensions
{
    public static async Task<T> PostAsJsonAndGetResultAsync<T>(this HttpClient client, string? uri, object content,
        JsonSerializerOptions? options = null, CancellationToken token = default)
    {
        HttpResponseMessage response;

        try
        {
            response = await client.PostAsJsonAsync(uri, content, options, token);
        }
        catch (TaskCanceledException e)
        {
            if (e.InnerException is TimeoutException)
                throw e.InnerException;

            throw;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Error sending request", null, response.StatusCode);
        }

        return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(token)) ?? throw new JsonException("Cannot parse response");
    }
}

internal class TelegramBotApiProvider(string apiUrl, string apiKey)
{
    private int? _lastUpdateId;

    private readonly HttpClient _client = new()
    {
        Timeout = TimeSpan.FromSeconds(15)
    };

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private async Task<T> SendBotApiRequestAsync<T>(string method, object parameters, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        return await _client.PostAsJsonAndGetResultAsync<T>($"{apiUrl}{apiKey}/{method}", parameters, _jsonOptions, token);
    }

    private async Task SendBotApiRequestAsync(string method, object parameters, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        await _client.PostAsJsonAsync($"{apiUrl}{apiKey}/{method}", parameters, _jsonOptions, token);
    }

    private void SendBotApiRequest(string method, object parameters)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{apiUrl}{apiKey}/{method}")
        {
            Content = new StringContent(JsonSerializer.Serialize(parameters, _jsonOptions), Encoding.UTF8, "application/json")
        };

        _client.Send(request);
    }

    private T SendBotApiRequest<T>(string method, object parameters) where T : class
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{apiUrl}{apiKey}/{method}")
        {
            Content = new StringContent(JsonSerializer.Serialize(parameters, _jsonOptions), Encoding.UTF8, "application/json")
        };

        var response = _client.Send(request);

        return response.IsSuccessStatusCode
            ? response.Content.ReadFromJsonAsync<T>().GetAwaiter().GetResult() ??
              throw new JsonException("Cannot parse response")
            : throw new Exception($"Server returned error with code {response.StatusCode}");
    }

    private static KeyboardMarkup? GenerateKeyboardMarkup(IButtonsMarkup? buttons)
    {
        return buttons switch
        {
            ReplyButtonsMarkup reply => new ReplyKeyboardMarkup
            {
                Keyboard = [.. reply.Items.Select(x => x.Select(y => new KeyboardButton { Text = y.Text }).ToArray())],
                ResizeKeyboard = reply.Resize,
                OneTimeKeyboard = reply.OneTime,
                Persistent = reply.Persistent
            },
            RemoveButtonsMarkup => new ReplyKeyboardRemove(),
            _ => null
        };
    }

    public async Task<UpdateResponse> GetUpdatesAsync(CancellationToken token = default)
    {
        var response = await SendBotApiRequestAsync<UpdateResponse>(ApiMethods.GetUpdates, new GetUpdatesParameters(_lastUpdateId), token);

        _lastUpdateId = response.Result.LastOrDefault()?.UpdateId;

        return response;
    }

    public UpdateResponse GetUpdates()
    {
        var response = SendBotApiRequest<UpdateResponse>(ApiMethods.GetUpdates, new GetUpdatesParameters(_lastUpdateId));

        _lastUpdateId = response.Result.LastOrDefault()?.UpdateId;

        return response;
    }

    private static SendMessageParameters GetParameters(int chatId, string message, IButtonsMarkup? buttons)
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

    public async Task SendResponseAsync(int chatId, string message, IButtonsMarkup? buttons = null, CancellationToken token = default) =>
        await SendBotApiRequestAsync(ApiMethods.SendMessage, GetParameters(chatId, message, buttons), token);

    public void SendResponse(int chatId, string message, IButtonsMarkup? buttons = null) =>
        SendBotApiRequest(ApiMethods.SendMessage, GetParameters(chatId, message, buttons));
}