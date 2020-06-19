using DependencyFactory;
using DependencyFactory.Config;
using RemoteControlCore.Abstract;
using RemoteControlCore.Controllers;
using RemoteControlCore.Interfaces;
using RemoteControlCore.Listeners;
using RemoteControlCore.Providers;
using RemoteControlCore.Services;
using RemoteControlCore.Utility;

namespace RemoteControlCore.Config
{
    internal static class MyDependencyFactoryConfig
    {
        public static readonly NavigationOption ApiNavigationOption = new NavigationOption();
        public static readonly NavigationOption HttpNavigationOption = new NavigationOption();
        public static readonly NavigationOption SocketNavigationOption = new NavigationOption();
        public static readonly NavigationOption WebNavigationOption = new NavigationOption();

        static MyDependencyFactoryConfig()
        {
            Factory.AddConfig<IListener, MyHttpListener>();
            Factory.AddConfig<IHttpListener, MyHttpListener>();
            Factory.AddConfig<IApiListener, WebSocketListener>(SocketNavigationOption);
            Factory.AddConfig<IApiListener, MyHttpApiListener>(HttpNavigationOption);
            Factory.AddConfig<AbstractController, ApiController>(ApiNavigationOption);
            Factory.AddConfig<AbstractController, HttpController>(WebNavigationOption);
            Factory.AddConfig<IInputService, InputsimService>();
            Factory.AddConfig<IAudioService, AudioService>();
            Factory.AddConfig<ICoordinates, Point>();
            Factory.AddConfig<IInputProvider, MyInputProvider>(DependencyBehavior.Singleton);
        }        
    }
}
