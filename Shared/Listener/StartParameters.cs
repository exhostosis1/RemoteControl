using System.Collections.Generic;

namespace Shared.Listener;

public abstract record StartParameters(string Uri);
public record WebParameters(string Uri) : StartParameters(Uri);
public record BotParameters(string Uri, string ApiKey, List<string> Usernames) : StartParameters(Uri);