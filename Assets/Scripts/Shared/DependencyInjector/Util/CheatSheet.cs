using Shared.DependencyInjector.Install;
using UnityEngine;

#pragma warning disable 219

namespace Shared.DependencyInjector.Util
{
    public class CheatSheet : Installer
    {
        public class Qux { }

        public class Foo : MonoBehaviour, IFoo2, IBar { }

        public class Foo1 : IFoo { }

        public class Foo2 : IFoo { }

        public class Baz { }

        public class Gui { }

        public class Bar : IBar { }

        public override void InstallBindings()
        {
            Container.Bind<Foo>().AsTransient().NonLazy();
            Container.BindInterfacesTo<Foo>().AsSingle().Lazy();

            Container.Bind<Foo>().NonLazy().AsTransient();
            Container.BindInterfacesTo<Foo>().Lazy().AsSingle();

            // Create a new instance of Foo for every class that asks for it
            Container.Bind<Foo>().AsTransient();

            // Create a new instance of Foo for every class that asks for an IFoo
            Container.Bind<IFoo>().To<Foo>().AsTransient();

            // Non generic version of the above
            Container.Bind(typeof(IFoo)).To(typeof(Foo)).AsTransient();

            ///////////// AsSingle

            // Create one definitive instance of Foo and re-use that for every class that asks for it
            Container.Bind<Foo>().AsSingle();

            // Create one definitive instance of Foo and re-use that for every class that asks for IFoo
            Container.Bind<IFoo>().To<Foo>().AsSingle();

            // Bind the same instance to multiple types
            // In this example, the same instance of Foo will be used for all three types
            // (we have to use the non-generic version of Bind when mapping to multiple types)
            Container.Bind(typeof(Foo), typeof(IFoo), typeof(IFoo2)).To<Foo>().AsSingle();

            ///////////// BindInterfaces

            // This will have the exact same effect as the above line
            // Bind all interfaces that Foo implements and Foo itself to a new singleton of type Foo
            Container.BindInterfacesAndSelfTo<Foo>().AsSingle();

            // Bind only the interfaces that Foo implements to an instance of Foo
            // This can be useful if you don't want any classes to directly reference the concrete
            // derived type
            Container.BindInterfacesTo<Foo>().AsSingle();

            ///////////// FromInstance

            // Use the given instance everywhere that Foo is used
            // This is simply a shortcut for the above binding
            // This can be a bit nicer since the type argument can be deduced from the parameter
            Container.BindInstance(new Foo());

            // Bind multiple instances at once
            Container.BindInstances(new Foo(), new Bar());

            ///////////// Binding primitive types

            // BindInstance is more commonly used with primitive types
            // Use the number 10 every time an int is requested
            Container.BindInstance(10);
            Container.BindInstance(false);

            // You'd never really want to do the above though - you should almost always use a When condition for primitive values
            Container.BindInstance(10).WhenInjectedInto<Foo>();

            ///////////// FromMethod

            InstallMore();
        }

        void InstallMore() => InstallMore2();

        void InstallMore2() => InstallMore3();

        void InstallMore3()
        {
            ///////////// Conditions

            // This will make Foo only visible to Bar
            // If we add Foo to the constructor of any other class it won't find it
            Container.Bind<Foo>().AsSingle().WhenInjectedInto<Bar>();

            // Use different implementations of IFoo dependending on which
            // class is being injected
            Container.Bind<IFoo>().To<Foo1>().AsSingle().WhenInjectedInto<Bar>();
            Container.Bind<IFoo>().To<Foo2>().AsSingle().WhenInjectedInto<Qux>();

            // Use "Foo1" as the default implementation except when injecting into
            // class Qux, in which case use Foo2
            // This works because if there is a condition match, that takes precedence
            Container.Bind<IFoo>().To<Foo1>().AsSingle();
            Container.Bind<IFoo>().To<Foo2>().AsSingle().WhenInjectedInto<Qux>();

            // Allow depending on Foo in only a few select classes
            Container.Bind<Foo>().AsSingle().WhenInjectedInto(typeof(Bar), typeof(Qux), typeof(Baz));

            // Supply 5 for all ints that are injected into the Gui class
            Container.BindInstance(5).WhenInjectedInto<Gui>();
        }

        public interface IFoo2 { }

        public interface IFoo { }

        public interface IBar : IFoo { }
    }
}