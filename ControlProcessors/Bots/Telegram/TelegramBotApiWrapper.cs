using Bots.Telegram.ApiObjects.Request;
using Bots.Telegram.ApiObjects.Response;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Shared.Bot;
using Shared.Logging.Interfaces;

namespace Bots.Telegram;

public class TelegramBotApiWrapper: IApiWrapper
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

    private readonly ILogger _logger;

    public TelegramBotApiWrapper(ILogger logger)
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

    public async Task<IUpdateResponse> GetUpdates(string apiUrl, string apiKey, CancellationToken token)
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

    public async Task<string> SendResponse(string apiUrl, string apiKey, int chatId, string message, CancellationToken token, IEnumerable<IEnumerable<string>>? buttons = null)
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