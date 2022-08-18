namespace Shared.Interfaces;

public interface IContainer
{
    public object Get(Type interfaceType);
    public TInterface Get<TInterface>() where TInterface : class;
    public object GetUnregistered(Type concreteType);
    public TConcrete GetUnregistered<TConcrete>() where TConcrete : class;
    public IContainer Register<TInterface, TConcrete>() where TInterface : class where TConcrete : TInterface;
}