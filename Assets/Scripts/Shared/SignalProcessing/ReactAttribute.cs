using System;
using JetBrains.Annotations;

namespace Shared.SignalProcessing
{
    /// <summary>
    /// Methods marked with this attribute should not be public.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse]
    public class ReactAttribute : Attribute { }
}