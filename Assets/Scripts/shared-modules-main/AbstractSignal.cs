using ControlFlow.SignalProcessing;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Linq;
using System.Collections.Generic;
#endif
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !UNITY_IOS
using System.Diagnostics;
#endif

namespace Shared
{
    public abstract class AbstractSignal : AbstractSignalBase
    {
        protected AbstractSignal()
        {
            // assertions
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !UNITY_IOS
            // get call stack
            var stackTrace = new StackTrace();

            // TODO: limited to only be present on Android because the assembly name is different on iPhone
            // TODO: probably because the hardcoded '2' index points at something else, maybe iPhone injects something into the stack?

            // get name of the calling assembly two levels above
            // ReSharper disable once PossibleNullReferenceException
            CallingAssemblyAssertion(stackTrace.GetFrame(2).GetMethod().DeclaringType.Assembly.GetName().Name);
#endif
        }

        // assertions
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        const string DtoCallingAssemblyViolationMessage = "Calling assembly violates chosen architectural pattern. "
                                                          + "Signals should be called only from 'Common', 'GameLogic' or 'Presentation' assemblies.";

        readonly List<string> _allowedCallingAssemblies = new() {"Common", "GameLogic", "Presentation", "Boot"};

        void CallingAssemblyAssertion(string assemblyName)
        {
            if (_allowedCallingAssemblies.All(x => x != assemblyName))
                throw new Exception(DtoCallingAssemblyViolationMessage + $"Was called from {assemblyName}.");
        }
#endif
    }
}