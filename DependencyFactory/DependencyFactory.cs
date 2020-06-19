using DependencyFactory.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyFactory
{
    public static class Factory
    {
        private static readonly Dictionary<Type, object> ObjectCache = new Dictionary<Type, object>();

        private static readonly NavigationOption GeneralNavOption = new NavigationOption();

        public static T GetInstance<T>(NavigationOption navigationOption, params object[] prms) where T: class
        {            
            var objectConfig = DependencyFactoryConfig.GetConfig().FirstOrDefault(x => x.InterfaceType == typeof(T) && x.Id == navigationOption.Value);

            if (objectConfig == null)
            {
                throw new ArgumentException("No such interface in config");
            }

            object result;

            switch(objectConfig.Behavior)
            {
                case DependencyBehavior.NewInstance:
                    result = CreateNew(objectConfig.ObjectType, prms);
                    break;
                case DependencyBehavior.Singleton:
                    result = GetFromCacheOrCreateNew(objectConfig.ObjectType, prms);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return (T)result;
        }

        public static T GetInstance<T>(params object[] prms) where T: class
        {
            return GetInstance<T>(GeneralNavOption, prms);
        }

        private static object CreateNew(Type type, object[] prms)
        {
            if (type == null) throw new ArgumentException("Type is null", nameof(type));

            return type.GetConstructor(prms.Select(x => x.GetType()).ToArray())?.Invoke(prms);
        }

        private static object GetFromCacheOrCreateNew(Type type, object[] prms)
        {
            if (!ObjectCache.ContainsKey(type))
                ObjectCache.Add(type, CreateNew(type, prms));

            return ObjectCache[type];
        }
        public static void AddConfig<TFrom, TTo>(NavigationOption navigationOption, DependencyBehavior behavior = DependencyBehavior.NewInstance)
        {
            if (DependencyFactoryConfig.GetConfig().Any(x => x.InterfaceType == typeof(TFrom) && x.Id == navigationOption.Value))
                throw new Exception("Duplicate entries in factory config");

            if (!typeof(TFrom).IsAssignableFrom(typeof(TTo)))
                throw new ArgumentException("Target class does not deride from parent class");

            DependencyFactoryConfig.AddConfig(new DependencyFactoryConfigItem(typeof(TFrom), typeof(TTo), behavior, navigationOption.Value));
        }
        public static void AddConfig<TFrom, TTo>(DependencyBehavior behavior = DependencyBehavior.NewInstance)
        {
            AddConfig<TFrom, TTo>(GeneralNavOption, behavior);
        }

        public static void ResetConfig()
        {
            DependencyFactoryConfig.ResetConfig();
        }
    }
}
