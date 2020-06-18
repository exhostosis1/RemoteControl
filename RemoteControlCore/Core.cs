using DependencyFactory;
using RemoteControlCore.Abstract;
using RemoteControlCore.Config;
using RemoteControlCore.Interfaces;
using System;

namespace RemoteControlCore
{
    public class Core
    {
        private readonly IHttpListener _listener;
        private readonly IApiListener _apiListener;

        private UriBuilder _ub = new UriBuilder();
        private bool _simple;

        public Core(bool socket)
        {
            var apiController = Factory.GetInstance<AbstractController>(MyDependencyFactoryConfig.ApiNavigationOption);
            var httpController = Factory.GetInstance<AbstractController>(MyDependencyFactoryConfig.WebNavigationOption);

            _listener = Factory.GetInstance<IHttpListener>();

            _listener.OnHttpRequest += httpController.ProcessRequest;

            _apiListener = Factory.GetInstance<IApiListener>(socket
                ? MyDependencyFactoryConfig.SocketNavigationOption
                : MyDependencyFactoryConfig.HttpNavigationOption);

            _apiListener.OnApiRequest += apiController.ProcessApiRequest;
        }

        public void Start(UriBuilder ub, bool simple)
        {
            _ub = ub;
            _simple = simple;
            Start();
        }

        public void Start()
        {
            _listener.StartListen(_ub.Uri.ToString(), _simple);
            if (!_apiListener.Listening) _apiListener.StartListen(_ub);
        }

        public void Stop()
        {
            _listener.StopListen();
            if (_apiListener.Listening) _apiListener.StopListen();
        }

        public void Restart(UriBuilder ub, bool simple)
        {
            _ub = ub;
            _simple = simple;
            Restart();
        }

        public void Restart()
        {
            _listener.RestartListen(_ub.Uri.ToString(), _simple);
            _apiListener.RestartListen(_ub);
        }

        public (UriBuilder, bool) GetCurrentConfig()
        {
            return (_ub, _simple);
        }   
    }
}