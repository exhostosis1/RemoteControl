namespace MainApp.Servers;

public record StartParameters(string Uri, string? ApiKey = null, List<string>? Usernames = null);