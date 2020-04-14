using DependencyFactory;
using DependencyFactory.Config;
using RemoteControlCore.Abstract;
using RemoteControlCore.Controllers;
using RemoteControlCore.Interfaces;
using RemoteControlCore.Listeners;

namespace RemoteControlCore.Config
{
    internal static class MyDependencyFactoryConfig
    {
        private enum Options
        {
            Api,
            Http
        }

        public static NavigationOption ApiNavigationOption = Factory.GetNavigationOption(Options.Api);
        public static NavigationOption HttpNavigationOption = Factory.GetNavigationOption(Options.Http);

        static MyDependencyFactoryConfig()
        {
            Factory.AddConfig<IHttpListener, MyHttpListener>();
            Factory.AddConfig<AbstractController, ApiController>(ApiNavigationOption, DependencyBehavior.Singleton);
            Factory.AddConfig<AbstractController, HttpController>(HttpNavigationOption, DependencyBehavior.Singleton);
        }        
    }
}
