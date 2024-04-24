using MainApp.Servers;

namespace MainApp;

public interface IServerFactory
{
    IServer GetServer(ServerConfig config);
}