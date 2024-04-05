using System.Collections.Generic;

namespace Shared.Listener;

public record StartParameters(string Uri, string? ApiKey = null, List<string>? Usernames = null);