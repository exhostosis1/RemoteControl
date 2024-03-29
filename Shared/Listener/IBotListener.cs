using Shared.DataObjects.Bot;
using System.ComponentModel;

namespace Shared.Listener;

public interface IBotListener : IListener<BotContext, BotParameters>, INotifyPropertyChanged
{
}