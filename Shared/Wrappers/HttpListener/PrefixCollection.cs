using System.Net;

namespace Shared.Wrappers.HttpListener;

public class PrefixCollection(HttpListenerPrefixCollection collection) : IPrefixesCollection
{
    public void Add(string prefix) => collection.Add(prefix);
}