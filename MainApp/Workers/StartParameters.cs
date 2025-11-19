namespace MainApp.Workers;

internal record StartParameters(string Uri, string? ApiKey = null, List<string>? Usernames = null);