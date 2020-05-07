using DependencyFactory;
using RemoteControlCore.Abstract;
using RemoteControlCore.Config;
using RemoteControlCore.Interfaces;
using System;

namespace RemoteControlCore
{
    public class Core
    {
        readonly IHttpListener _listener;
        readonly AbstractController _apiController;
        readonly AbstractController _httpController;

        UriBuilder _ub = new UriBuilder();
        bool _simple = false;

        public Core()
        {
            _apiController = Factory.GetInstance<AbstractController>(MyDependencyFactoryConfig.ApiNavigationOption);
            _httpController = Factory.GetInstance<AbstractController>(MyDependencyFactoryConfig.HttpNavigationOption);

            _listener = Factory.GetInstance<IHttpListener>();

            _listener.OnApiRequest += _apiController.ProcessRequest;
            _listener.OnHttpRequest += _httpController.ProcessRequest;
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
        }

        public void Stop()
        {
            _listener.StopListen();
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
        }

        public (UriBuilder, bool) GetCurrentConfig()
        {
            return (_ub, _simple);
        }   
    }
}