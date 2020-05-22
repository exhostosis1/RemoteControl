using System;
using System.Collections.Generic;

namespace DependencyFactory.Config
{
    internal class DependencyFactoryConfigItem
    {
        internal Type InterfaceType { get; private set; }
        internal Type ObjectType { get; private set; }
        internal DependencyBehavior Behavior { get; private set; }

        internal NavigationOption NavigationOption;        

        internal DependencyFactoryConfigItem(Type interfaceType, Type objectType, DependencyBehavior behavior, NavigationOption navigationOption)
        {
            InterfaceType = interfaceType;
            ObjectType = objectType;
            Behavior = behavior;
            NavigationOption = navigationOption;
        }
    }

    internal static class DependencyFactoryConfig
    {
        private static readonly List<DependencyFactoryConfigItem> Items = new List<DependencyFactoryConfigItem>();

        internal static void AddConfig(DependencyFactoryConfigItem item)
        {
            Items.Add(item);
        }
            
        internal static IReadOnlyList<DependencyFactoryConfigItem> GetConfig()
        {
            return Items;
        }

        internal static void ResetConfig()
        {
            Items.Clear();
        }
    }
}
