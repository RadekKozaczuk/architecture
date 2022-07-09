using System;
using System.Diagnostics;

namespace Shared.DependencyInjector.DataModels
{
    [DebuggerStepThrough]
    public struct BindingIdDto : IEquatable<BindingIdDto>
    {
        public Type Type;

        public BindingIdDto(Type type)
        {
            Type = type;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 29 + Type.GetHashCode();
                hash = hash * 29 + 0;
                return hash;
            }
        }

        public override bool Equals(object other)
        {
            if (other is BindingIdDto otherId)
                return otherId == this;

            return false;
        }

        public bool Equals(BindingIdDto that) => this == that;

        public static bool operator ==(BindingIdDto left, BindingIdDto right) => left.Type == right.Type;

        public static bool operator !=(BindingIdDto left, BindingIdDto right) => !left.Equals(right);
    }
}