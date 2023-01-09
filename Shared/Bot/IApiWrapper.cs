using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.Bot.ApiObjects.Response;

namespace Shared.Bot;

public interface IApiWrapper
{
    public Task<UpdateResponse> GetUpdates(string apiUrl, string apiKey, CancellationToken token);

    public Task<string> SendResponse(string apiUrl, string apiKey, int chatId, string message, CancellationToken token,
        IEnumerable<IEnumerable<string>>? buttons = null);
}