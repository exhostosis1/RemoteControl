using System.Net;

namespace Shared.Wrappers.HttpListener;

public class PrefixCollection(HttpListenerPrefixCollection collection) : IPrefixesCollection
{
    private readonly HttpListenerPrefixCollection _collection = collection;

    public void Add(string prefix) => _collection.Add(prefix);
}