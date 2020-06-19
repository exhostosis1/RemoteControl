using DependencyFactory;
using RemoteControlCore.Abstract;
using RemoteControlCore.Config;
using RemoteControlCore.Interfaces;
using System;

namespace RemoteControlCore
{
    public class Core
    {
        private readonly IHttpListener _httplistener;
        private readonly IApiListener _apiListener;

        public UriBuilder HttpHost { get; set; }
        public UriBuilder ApiHost { get; set; }

        public Core(UriBuilder http, UriBuilder api, bool simple, bool socket)
        {
            HttpHost = http;
            ApiHost = api;

            var apiController = Factory.GetInstance<AbstractController>(MyDependencyFactoryConfig.ApiNavigationOption);
            var httpController = Factory.GetInstance<AbstractController>(MyDependencyFactoryConfig.WebNavigationOption);

            _httplistener = Factory.GetInstance<IHttpListener>();

            if (simple)
            {
                _httplistener.OnHttpRequest += httpController.ProcessSimpleRequest;
            }
            else
            {
                _httplistener.OnHttpRequest += httpController.ProcessRequest;
            }

            _apiListener = Factory.GetInstance<IApiListener>(socket
                ? MyDependencyFactoryConfig.SocketNavigationOption
                : MyDependencyFactoryConfig.HttpNavigationOption);

            _apiListener.OnApiRequest += apiController.ProcessApiRequest;
        }

        public void Start()
        {
            _httplistener.StartListen(HttpHost);
            if (!_apiListener.Listening) _apiListener.StartListen(ApiHost);
        }

        public void Stop()
        {
            _httplistener.StopListen();
            if (_apiListener.Listening) _apiListener.StopListen();
        }

        public void Restart()
        {
            _httplistener.RestartListen(HttpHost);
            _apiListener.RestartListen(ApiHost);
        } 
    }
}