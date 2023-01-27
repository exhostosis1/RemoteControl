using System;
using Shared.DataObjects;
using Shared.DataObjects.Bot;
using Shared.DataObjects.Web;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shared;

public abstract record StartParameters(string Uri);

public record WebParameters(string Uri) : StartParameters(Uri);

public record BotParameters(string Uri, string ApiKey, List<string> Usernames) : StartParameters(Uri);

public interface IListener<TContext, in TParam> where TContext : IContext where TParam : StartParameters
{
    public bool IsListening { get; }
    public void StartListen(TParam param);
    public void StopListen();
    public Task<TContext> GetContextAsync(CancellationToken token = default);
    public TContext GetContext();
}

public interface IWebListener : IListener<WebContext, WebParameters>
{
}

public interface IBotListener : IListener<BotContext, BotParameters>, IObservable<bool>
{
}