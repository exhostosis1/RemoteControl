﻿using System;
using System.Linq;
using Shared.DataObjects.Http;
using Shared.Listeners;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using Shared.Logging.Interfaces;

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

public class HttpListenerWrapper : IHttpListener
{
    class LocalResponse : HttpContextResponse
    {
        private readonly HttpListenerResponse _response;

        public LocalResponse(HttpListenerResponse response)
        {
            _response = response;
        }

        public override void Close()
        {
            _response.StatusCode = (int)StatusCode;
            _response.ContentType = ContentType;

            if (Payload.Length > 0)
                _response.OutputStream.Write(Payload);

            _response.Close();
        }
    }

    private HttpListener _listener = new();
    public bool IsListening => _listener.IsListening;
    public IPrefixesCollection Prefixes { get; private set; }
    private readonly ILogger<HttpListenerWrapper> _logger;

    public HttpListenerWrapper(ILogger<HttpListenerWrapper> logger)
    {
        Prefixes = new PrefixCollection(_listener.Prefixes);
        _logger = logger;
    }

    public void Start()
    {
        if (_listener.IsListening) return;

        try
        {
            _listener.Start();
        }
        catch (HttpListenerException e)
        {
            if (e.Message.Contains("Failed to listen"))
            {
                _logger.LogError(e.Message);
                return;
            }

            var currentIps = Utils.GetCurrentIPs();
            var unavailableIps = _listener.Prefixes.Where(x => !currentIps.Contains(new Uri(x).Host)).ToList();

            if (unavailableIps.Count > 0)
            {
                _logger.LogError($"{string.Join(';', unavailableIps)} is currently unavailable");
                return;
            }

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw;

            _logger.LogWarn("Trying to add listening permissions to user");

            var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            var translatedValue = sid.Translate(typeof(NTAccount)).Value;

            foreach (var prefix in _listener.Prefixes)
            {
                var command = $"netsh http add urlacl url={prefix} user={translatedValue}";

                Utils.RunWindowsCommandAsAdmin(command);
            }

            _listener.Start();
        }
    }

    public void Stop()
    {
        if (_listener.IsListening)
            _listener.Stop();
    }

    public void GetNew()
    {
        if (_listener.IsListening)
            _listener.Stop();

        _listener = new HttpListener();
        Prefixes = new PrefixCollection(_listener.Prefixes);
    }

    private HttpContext ConvertContext(HttpListenerContext context)
    {
        var request = new HttpContextRequest(context.Request.RawUrl ?? string.Empty);
        var response = new LocalResponse(context.Response);
        return new HttpContext(request, response);
    }

    public HttpContext GetContext() => ConvertContext(_listener.GetContext());
    public async Task<HttpContext> GetContextAsync() => ConvertContext(await _listener.GetContextAsync());
}