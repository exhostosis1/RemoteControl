using Shared.Logging.Interfaces;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Shared.Bot.ApiObjects.Request;
using Shared.Bot.ApiObjects.Response;
using Shared.DataObjects.Bot;
using Shared.Listeners;

namespace Listeners.Wrappers.Telegram;

public class TelegramBotApiWrapper : IActiveApiWrapper
{
    private readonly HttpClient _client = new()
    {
        Timeout = TimeSpan.FromSeconds(15),
    };

    private int? _lastUpdateId = null;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly ILogger<TelegramBotApiWrapper> _logger;

    public TelegramBotApiWrapper(ILogger<TelegramBotApiWrapper> logger)
    {
        _logger = logger;
    }

    private async Task<string> SendBotApiRequest(string apiUrl, string apiKey, string method, object parameters, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var content = new StringContent(JsonSerializer.Serialize(parameters, _jsonOptions), Encoding.UTF8, "application/json");

        var req = new HttpRequestMessage(HttpMethod.Post, $"{apiUrl}{apiKey}/{method}")
        {
            Content = content
        };

        HttpResponseMessage? response;

        try
        {
            response = await _client.SendAsync(req, token);
        }
        catch (TaskCanceledException e)
        {
            if (e.InnerException is TimeoutException)
                throw e.InnerException;

            throw;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Error sending request to bot api", null, response.StatusCode);
        }

        return await response.Content.ReadAsStringAsync(token);
    }

    private static ReplyKeyboardMarkup GenerateKeyboardMarkup(IEnumerable<IEnumerable<string>> buttons)
    {
        return new ReplyKeyboardMarkup
        {
            ResizeKeyboard = true,
            Keyboard = buttons.Select(x => x.Select(y => new KeyboardButton { Text = y }).ToArray()).ToArray()
        };
    }

    public async Task<UpdateResponse> GetContextAsync(string apiUrl, string apiKey, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var parameters = new GetUpdatesParameters();

        if (_lastUpdateId.HasValue)
        {
            parameters.Offset = _lastUpdateId + 1;
        }

        var responseString = await SendBotApiRequest(apiUrl, apiKey, ApiMethods.GetUpdates, parameters, token);

        var parsedResponse = JsonSerializer.Deserialize<UpdateResponse>(responseString) ?? throw new JsonException("Cannot parse api response");

        _lastUpdateId = parsedResponse.Result.LastOrDefault()?.UpdateId;

        return parsedResponse;
    }

    public async Task<IEnumerable<BotContext>> GetContextAsync(string apiUrl, string apiKey, IEnumerable<string> usernames, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var response = await GetContextAsync(apiUrl, apiKey, token);

        if (response is { Ok: true, Result.Length: > 0 })
        {
            return response.Result
                .Where(x =>
                    usernames.Any(y => y == x.Message?.From?.Username) &&
                    (DateTime.Now - x.Message?.ParsedDate)?.Seconds < 10 &&
                    x.Message?.Chat?.Id != null &&
                    !string.IsNullOrWhiteSpace(x.Message?.Text))
                .Select(x => new BotContext(x.Message!.Chat!.Id, x.Message.Text!));
        }

        return Enumerable.Empty<BotContext>();
    }

    public async Task<string> SendResponseAsync(string apiUrl, string apiKey, int chatId, string message, CancellationToken token, IEnumerable<IEnumerable<string>>? buttons = null)
    {
        token.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message can not be empty", nameof(message));

        var parameters = new SendMessageParameters
        {
            ChatId = chatId,
            Text = message,
            ReplyMarkup = buttons == null ? null : GenerateKeyboardMarkup(buttons)
        };

        return await SendBotApiRequest(apiUrl, apiKey, ApiMethods.SendMessage, parameters, token);
    }
}