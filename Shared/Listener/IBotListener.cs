using System;
using Shared.DataObjects.Bot;

namespace Shared.Listener;

public interface IBotListener : IListener<BotContext, BotParameters>, IObservable<bool>
{
}