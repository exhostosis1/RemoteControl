using System.Reflection;
using Shared.Interfaces;

namespace RemoteControlConsole
{
    internal class Container: IContainer
    {
        private Dictionary<Type, Type> RegisteredTypes { get; } = new();
        private Dictionary<Type, object> ObjectCache { get; } = new();

        public Container()
        {
            RegisteredTypes.Add(typeof(IContainer), typeof(Container));
            ObjectCache.Add(typeof(Container), this);
        }

        public IContainer Register<TInterfaceType, TConcreteType>() where TInterfaceType: class where TConcreteType: TInterfaceType
        {
            RegisteredTypes.Add(typeof(TInterfaceType), typeof(TConcreteType));

            return this;
        }

        public object Get(Type interfaceType)
        {
            if (!RegisteredTypes.ContainsKey(interfaceType))
                throw new ArgumentException($"Type {interfaceType} is not registered");

            return GetUnregistered(RegisteredTypes[interfaceType]);
        }

        public TInterfaceType Get<TInterfaceType>() where TInterfaceType : class
        {
            return (TInterfaceType) Get(typeof(TInterfaceType));
        }

        public object GetUnregistered(Type concreteType)
        {
            if (ObjectCache.ContainsKey(concreteType))
                return ObjectCache[concreteType];

            var constructor = concreteType
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public).MinBy(x => x.GetParameters().Length);

            if (constructor == null)
                throw new Exception($"Cannot find public non-static constructor for {concreteType}");

            var parameters = constructor.GetParameters().Select(x => Get(x.ParameterType)).ToArray();

            var ob = constructor.Invoke(parameters);

            ObjectCache.Add(concreteType, ob);

            return ob;
        }

        public TConcrete GetUnregistered<TConcrete>() where TConcrete : class
        {
            return (TConcrete) GetUnregistered(typeof(TConcrete));
        }
    }
}
