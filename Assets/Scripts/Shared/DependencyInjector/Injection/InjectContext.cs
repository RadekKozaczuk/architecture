using System;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.DataModels;
using Shared.DependencyInjector.Main;
using Shared.DependencyInjector.Util;

namespace Shared.DependencyInjector.Injection
{
    [NoReflectionBaking]
    public class InjectContext : IDisposable
    {
        public BindingIdDto BindingIdDto => _bindingIdDto;

        // The type of the constructor parameter, field or property
        public Type MemberType
        {
            get => _bindingIdDto.Type;
            set => _bindingIdDto.Type = value;
        }
        
        // The type of the object which is having its members injected
        // NOTE: This is null for root calls to Resolve<> or Instantiate<>
        public Type ObjectType;

        // The instance which is having its members injected
        // Note that this is null when injecting into the constructor
        public object ObjectInstance;

        // The constructor parameter name, or field name, or property name
        public string MemberName;
        
        // When optional, null is a valid value to be returned
        public bool Optional;

        // When set to true, this will only look up dependencies in the local container and will not
        // search in parent containers
        public InjectSources SourceType;

        // When optional, this is used to provide the value
        public object FallBackValue;

        // The container used for this injection
        public DiContainer Container;
        
        BindingIdDto _bindingIdDto;

        public InjectContext()
        {
            Reset();
        }

        public void Dispose() => ZenPools.DespawnInjectContext(this);

        internal void Reset()
        {
            ObjectType = null;
            ObjectInstance = null;
            MemberName = "";
            Optional = false;
            SourceType = InjectSources.Any;
            FallBackValue = null;
            Container = null;
            _bindingIdDto.Type = null;
        }

        internal InjectContext Clone()
        {
            var clone = new InjectContext();

            clone.ObjectType = ObjectType;
            clone.ObjectInstance = ObjectInstance;
            clone.MemberType = MemberType;
            clone.MemberName = MemberName;
            clone.Optional = Optional;
            clone.SourceType = SourceType;
            clone.FallBackValue = FallBackValue;
            clone.Container = Container;

            return clone;
        }
    }
}