using System.Net;

namespace Shared.Wrappers.HttpListener;

public class PrefixCollection : IPrefixesCollection
{
    private readonly HttpListenerPrefixCollection _collection;

    public PrefixCollection(HttpListenerPrefixCollection collection)
    {
        _collection = collection;
    }

    public void Add(string prefix) => _collection.Add(prefix);
}