using System;
using Shared.DependencyInjector.Atributes;

namespace Shared.DependencyInjector
{
    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class InjectAttribute : InjectAttributeBase { }
}