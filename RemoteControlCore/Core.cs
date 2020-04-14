using DependencyFactory;
using RemoteControlCore.Abstract;
using RemoteControlCore.Config;
using RemoteControlCore.Interfaces;
using System;

namespace RemoteControlCore
{
    public class Core
    {
        IHttpListener _listener;
        AbstractController _apiController;
        AbstractController _httpController;

        UriBuilder _ub = new UriBuilder();

        public Core()
        {
            _apiController = Factory.GetInstance<AbstractController>(MyDependencyFactoryConfig.ApiNavigationOption);
            _httpController = Factory.GetInstance<AbstractController>(MyDependencyFactoryConfig.HttpNavigationOption);

            _listener = Factory.GetInstance<IHttpListener>();

            _listener.OnApiRequest += _apiController.ProcessRequest;
            _listener.OnHttpRequest += _httpController.ProcessRequest;
        }

        public void Start((string, string, string) values)
        {
            TrySetUrl(values);
            Start();
        }

        public void Start()
        {
            _listener.StartListen(_ub.Uri.ToString());
        }

        public void Stop()
        {
            _listener.StopListen();
        }

        public void Restart((string, string, string) values)
        {
            TrySetUrl(values);
            Restart();
        }

        public void Restart()
        {
            _listener.RestartListen(_ub.Uri.ToString());
        }

        public UriBuilder GetUriBuilder()
        {
            return _ub;
        }

        private void TrySetUrl((string, string, string) values)
        {
            try
            {
                _ub.Scheme = values.Item1;
                _ub.Host = values.Item2;
                _ub.Port = Convert.ToInt32(values.Item3);
            }
            catch
            {
                throw;
            }
        }       
    }
}