using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.DataModels;
using Shared.DependencyInjector.Main;
using Shared.DependencyInjector.Providers;

namespace Shared.DependencyInjector.Binding
{
    [NoReflectionBaking]
    public class ConcreteBinderGeneric<TContract> : FromBinder
    {
        internal ConcreteBinderGeneric(DiContainer bindContainer, BindInfo bindInfo, BindStatement bindStatement)
            : base(bindContainer, bindInfo, bindStatement)
        {
            SubFinalizer = new ScopableBindingFinalizer(
                BindInfo, (container, type) => new TransientProvider(type, container, BindInfo.InstantiatedCallback));
        }
        
        internal ConcreteBinderGeneric<TConcrete> To<TConcrete>() where TConcrete : TContract
        {
            BindInfo.ToChoice = ToChoices.Concrete;
            BindInfo.ToTypes.Clear();
            BindInfo.ToTypes.Add(typeof(TConcrete));

            return new ConcreteBinderGeneric<TConcrete>(BindContainer, BindInfo, BindStatement);
        }
    }
}