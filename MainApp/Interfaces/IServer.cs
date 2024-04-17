namespace MainApp.Interfaces;

public interface IServer
{
    string Name { get; set; }
    Guid Id { get; set; }
    bool Status { get; set; }
    bool IsAutostart { get; set; }
}

public interface IWebServer : IServer
{
    Uri ListeningUri { get; set; }
}

public interface IBotServer : IServer
{
    Uri ApiUri { get; set; }
    string ApiKey { get; set; }
    IReadOnlyList<string> Usernames { get; set; }
}