using Listeners.Wrappers.Telegram.ApiObjects.Request;
using Listeners.Wrappers.Telegram.ApiObjects.Response;
using Listeners.Wrappers.Telegram.ApiObjects.Response.Keyboard;
using Shared;
using Shared.DataObjects.Bot;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly ILogger<TelegramBotApiWrapper> _logger;

    public TelegramBotApiWrapper(ILogger<TelegramBotApiWrapper> logger)
    {
        _logger = logger;
    }

    private async Task<T> SendBotApiRequest<T>(string apiUrl, string apiKey, string method, object parameters, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return await _client.PostAsJsonAndGetResultAsync<T>($"{apiUrl}{apiKey}/{method}", parameters, token, _jsonOptions);
    }

    private static KeyboardMarkup? GenerateKeyboardMarkup(ButtonsMarkup? buttons)
    {
        return buttons switch
        {
            ReplyButtonsMarkup reply => new ReplyKeyboardMarkup()
            {
                Keyboard = reply.Items.Select(x => x.Select(y => new KeyboardButton() { Text = y.Text }).ToArray())
                    .ToArray(),
                ResizeKeyboard = reply.Resize,
                OneTimeKeyboard = reply.OneTime,
                Persistent = reply.Persistent
            },
            RemoveButtonsMarkup remove => new ReplyKeyboardRemove(),
            _ => null
        };
    }

    public async Task<UpdateResponse> GetUpdatesAsync(string apiUrl, string apiKey, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var parameters = new GetUpdatesParameters();

        if (_lastUpdateId.HasValue)
        {
            parameters.Offset = _lastUpdateId + 1;
        }

        var response = await SendBotApiRequest<UpdateResponse>(apiUrl, apiKey, ApiMethods.GetUpdates, parameters, token);

        _lastUpdateId = response.Result.LastOrDefault()?.UpdateId;

        return response;
    }

    public async Task<IEnumerable<BotContext>> GetContextAsync(string apiUrl, string apiKey, IEnumerable<string> usernames, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var response = await GetUpdatesAsync(apiUrl, apiKey, token);

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

    public async Task SendResponseAsync(string apiUrl, string apiKey, int chatId, string message, CancellationToken token, ButtonsMarkup? buttons = null)
    {
        token.ThrowIfCancellationRequested();

        await SendResponseAndGetResultAsync(apiUrl, apiKey,chatId, message, token, buttons);
    }

    public async Task<Message> SendResponseAndGetResultAsync(string apiUrl, string apiKey, int chatId, string message, CancellationToken token, ButtonsMarkup? buttons = null)
    {
        token.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message can not be empty", nameof(message));

        var parameters = new SendMessageParameters
        {
            ChatId = chatId,
            Text = message,
            ReplyMarkup = GenerateKeyboardMarkup(buttons)
        };

        return await SendBotApiRequest<Message>(apiUrl, apiKey, ApiMethods.SendMessage, parameters, token);
    }
}