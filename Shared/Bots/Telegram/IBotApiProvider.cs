using System.Threading;
using System.Threading.Tasks;
using Shared.Bots.Telegram.ApiObjects.Response;
using Shared.DataObjects.Bot;

namespace Shared.Bots.Telegram;

public interface IBotApiProvider
{
    public UpdateResponse GetUpdates(string apiUrl, string apiKey);
    public Task<UpdateResponse> GetUpdatesAsync(string apiUrl, string apiKey, CancellationToken token = default);
    public Task SendResponseAsync(string apiUrl, string apiKey, int chatId, string message,
        ButtonsMarkup? buttons = null, CancellationToken token = default);
    public void SendResponse(string apiUrl, string apiKey, int chatId, string message, ButtonsMarkup? buttons = null);
}