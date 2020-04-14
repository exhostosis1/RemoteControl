using System;
using System.Linq;
using System.Collections.Generic;
using DependencyFactory.Config;

namespace DependencyFactory
{
    public static class Factory
    {
        private static readonly Guid uniqueKey = new Guid();

        private static readonly Dictionary<Type, object> ObjectCache = new Dictionary<Type, object>();
        private static readonly Dictionary<object, NavigationOption> navigationOptionsCache = new Dictionary<object, NavigationOption>()
        {
            { uniqueKey, new NavigationOption(uniqueKey) }
        };

        public static T GetInstance<T>(NavigationOption navigationOption, params object[] prms) where T: class
        {            
            var objectConfig = DependencyFactoryConfig.GetConfig().FirstOrDefault(x => x.InterfaceType == typeof(T) && x.NavigationOption == navigationOption);

            if (objectConfig == null)
            {
                throw new ArgumentException("No such interface in config");
            }

            object result = null;

            switch(objectConfig.Behavior)
            {
                case DependencyBehavior.NewInstance:
                    result = CreateNew(objectConfig.ObjectType, prms);
                    break;
                case DependencyBehavior.Singleton:
                    result = GetFromCacheOrCreateNew(objectConfig.ObjectType, prms);
                    break;
                default:
                    break;
            }

            return (T)result;
        }

        public static T GetInstance<T>(params object[] prms) where T: class
        {
            return GetInstance<T>(navigationOptionsCache[uniqueKey], prms);
        }

        private static object CreateNew(Type type, object[] prms)
        {
            return type.GetConstructor(prms.Select(x => x.GetType()).ToArray()).Invoke(prms);
        }

        private static object GetFromCacheOrCreateNew(Type type, object[] prms)
        {
            if (!ObjectCache.ContainsKey(type))
                ObjectCache.Add(type, CreateNew(type, prms));

            return ObjectCache[type];
        }
        public static void AddConfig<From, To>(NavigationOption navigationOption, DependencyBehavior behavior = DependencyBehavior.NewInstance)
        {
            if (DependencyFactoryConfig.GetConfig().Any(x => x.InterfaceType == typeof(From) && x.NavigationOption == navigationOption))
                throw new Exception("Duplicate entries in factory config");

            if (!typeof(From).IsAssignableFrom(typeof(To)))
                throw new ArgumentException("Target class does not deride from parent class");

            DependencyFactoryConfig.AddConfig(new DependencyFactoryConfigItem(typeof(From), typeof(To), behavior, navigationOption));
        }
        public static void AddConfig<From, To>(DependencyBehavior behavior = DependencyBehavior.NewInstance)
        {
            AddConfig<From, To>(navigationOptionsCache[uniqueKey], behavior);
        }

        public static NavigationOption GetNavigationOption(object id)
        {
            if (id == null)
                throw new ArgumentNullException();

            if (!navigationOptionsCache.ContainsKey(id))
                navigationOptionsCache.Add(id, new NavigationOption(id));

            return navigationOptionsCache[id];
        }

        public static void ResetConfig()
        {
            DependencyFactoryConfig.ResetConfig();
        }
    }
}
