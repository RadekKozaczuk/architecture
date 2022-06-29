using System;

namespace Shared.DependencyInjector.Atributes
{
    [AttributeUsage(AttributeTargets.Class)]
    class NoReflectionBakingAttribute : Attribute { }
}