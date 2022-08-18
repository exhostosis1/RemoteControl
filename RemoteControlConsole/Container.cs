using Shared.Interfaces;
using Shared.Interfaces.Web;
using Shared.Interfaces.Web.Attributes;
using System.Reflection;

namespace RemoteControlConsole
{
    internal class Container: IContainer
    {
        private Dictionary<Type, Type> RegisteredTypes { get; } = new();
        private Dictionary<Type, object> ObjectCache { get; } = new();
        private readonly ControllerMethods _controllerMethods = new ();

        public Container()
        {
            RegisteredTypes.Add(typeof(ControllerMethods), typeof(ControllerMethods));
            ObjectCache.Add(typeof(ControllerMethods), _controllerMethods);
        }

        public IContainer RegisterController<TController>() where TController: IController
        {
            var type = typeof(TController);

            var controllerKey = type.GetCustomAttribute<ControllerAttribute>()?.Name;

            if (string.IsNullOrEmpty(controllerKey)) return this;

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.ReturnType == typeof(string) && x.GetParameters().Length == 1 &&
                            x.GetParameters().First().ParameterType == typeof(string)).ToArray();

            if (methods.Length == 0) return this;

            var controllerValue = new Dictionary<string, Func<string, string?>>();
            var controller = GetUnregistered(type);

            foreach (var methodInfo in methods)
            {
                var action = methodInfo.GetCustomAttribute<ActionAttribute>()?.Name;
                if (string.IsNullOrEmpty(action)) continue;

                var value = methodInfo.CreateDelegate<Func<string, string?>>(controller);

                controllerValue.Add(action, value);
            }

            if (controllerValue.Count > 0)
            {
                _controllerMethods.Add(controllerKey, controllerValue);
            }

            return this;
        }

        public void RegisterSelf()
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
