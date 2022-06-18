using System;
using JetBrains.Annotations;

namespace Common.SignalProcessing
{
    /// <summary>
    ///     Indicates that this system is reacting on signals.
    ///     All wiring is done automatically in SignalProcessor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse]
    public class ReactOnSignalsAttribute : Attribute
    {
    }
}