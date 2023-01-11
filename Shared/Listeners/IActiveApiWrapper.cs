using Shared.DataObjects.Bot;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Listeners;

public interface IActiveApiWrapper
{
    public Task<IEnumerable<BotContext>> GetContextAsync(string apiUrl, string apiKey, IEnumerable<string> usernames, CancellationToken token);

    public Task<string> SendResponseAsync(string apiUrl, string apiKey, int chatId, string message, CancellationToken token,
        IEnumerable<IEnumerable<string>>? buttons = null);
}