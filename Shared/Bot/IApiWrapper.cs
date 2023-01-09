using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Bot;

public interface IApiWrapper
{
    public Task<IUpdateResponse> GetUpdates(string apiUrl, string apiKey, CancellationToken token);

    public Task<string> SendResponse(string apiUrl, string apiKey, int chatId, string message, CancellationToken token,
        IEnumerable<IEnumerable<string>>? buttons = null);
}