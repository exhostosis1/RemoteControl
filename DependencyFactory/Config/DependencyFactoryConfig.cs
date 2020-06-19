using System;
using System.Collections.Generic;

namespace DependencyFactory.Config
{
    internal class DependencyFactoryConfigItem
    {
        internal Type InterfaceType { get; }
        internal Type ObjectType { get; }
        internal DependencyBehavior Behavior { get; }

        internal readonly Guid Id;        

        internal DependencyFactoryConfigItem(Type interfaceType, Type objectType, DependencyBehavior behavior, Guid navigationOption)
        {
            InterfaceType = interfaceType;
            ObjectType = objectType;
            Behavior = behavior;
            Id = navigationOption;
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
