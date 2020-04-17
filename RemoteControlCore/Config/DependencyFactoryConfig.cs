using DependencyFactory;
using DependencyFactory.Config;
using RemoteControlCore.Abstract;
using RemoteControlCore.Controllers;
using RemoteControlCore.Interfaces;
using RemoteControlCore.Listeners;
using RemoteControlCore.Enums;
using RemoteControlCore.Services;
using RemoteControlCore.Utility;

namespace RemoteControlCore.Config
{
    internal static class MyDependencyFactoryConfig
    {
        public static NavigationOption ApiNavigationOption = Factory.GetNavigationOption(NavigationOptions.Api);
        public static NavigationOption HttpNavigationOption = Factory.GetNavigationOption(NavigationOptions.Http);

        static MyDependencyFactoryConfig()
        {
            Factory.AddConfig<IHttpListener, MyHttpListener>();
            Factory.AddConfig<AbstractController, ApiController>(ApiNavigationOption, DependencyBehavior.Singleton);
            Factory.AddConfig<AbstractController, HttpController>(HttpNavigationOption, DependencyBehavior.Singleton);
            Factory.AddConfig<IInputService, InputsimService>();
            Factory.AddConfig<IAudioService, AudioService>();
            Factory.AddConfig<ICoordinates, Point>(DependencyBehavior.NewInstance);
        }        
    }
}
