using System;
using System.Collections;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#endif
using ControlFlow.SignalProcessing;
using UnityEngine;

namespace Shared.Systems
{
    /// <summary>
    /// General purpose signal processing system.
    /// Signals are grouped per type.
    /// Groups are executed in alphabetical order.
    /// Within each group, signals are executed in a FIFO manner.
    /// If any new signals are sent during signal execution, they are executed only once
    /// all original signals are processed. <br/>
    /// <i>Warning: Sending a signal without a corresponding React method will result in an error.</i>
    /// </summary>
    public static class SignalProcessor
    {
        static readonly Dictionary<Type, Queue> _signalQueues = new();
        static readonly Dictionary<Type, int> _stashedSignalQueueLengths = new();

        static int _signalCount = 0;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        static int _lastFrameCount = 0;
#endif

        static SignalProcessor()
        {
            Assembly commonAssembly = Assembly.Load("Common");
            Type[] signals = (from t in commonAssembly.GetTypes()
                              where t.IsClass && t.Namespace == "Common.Signals"
                              select t).OrderBy(type => type.Name).ToArray();

            foreach (Type signal in signals)
            {
                _signalQueues[signal] = new Queue();
                _stashedSignalQueueLengths[signal] = 0;
            }
        }

        /// <summary>
        /// Process and execute all signals sent.
        /// </summary>
        public static void ExecuteSentSignals()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Assert.False(_lastFrameCount == Time.frameCount, "Signals should be executed once per frame");
            _lastFrameCount = Time.frameCount;

            int signalIterationCounter = 0;
#endif

            while (_signalCount > 0)
            {
                // Stash starting queue lengths
                foreach (KeyValuePair<Type, Queue> signalQueuePair in _signalQueues)
                    _stashedSignalQueueLengths[signalQueuePair.Key] = signalQueuePair.Value.Count;

                // Execute original signals
                foreach (KeyValuePair<Type, Queue> signalQueuePair in _signalQueues)
                {
                    Type signalType = signalQueuePair.Key;

                    if (_stashedSignalQueueLengths[signalType] == 0)
                        continue;

                    Queue signalQueue = signalQueuePair.Value;
                    Delegate signalReaction = SignalProcessorInternal.SignalMethods[signalType];

                    while (_stashedSignalQueueLengths[signalType] > 0)
                    {
                        var signal = (AbstractSignal)signalQueue.Dequeue();
                        signalReaction.DynamicInvoke(signal);

                        _signalCount--;
                        _stashedSignalQueueLengths[signalType]--;
                    }
                }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                signalIterationCounter++;

                // With too many iterations there is probably a loop
                if(signalIterationCounter > 40)
                    throw new Exception($"Maximum signal execution depth has been crossed. "
                                        + "Suggestion: Check for loops between signal react methods.");
#endif
            }
        }

        /// <summary>
        /// Sends a Signal of type <typeparamref name="T" />.
        /// <para><typeparamref name="T" /> needs to be of the explicit type you want send.</para>
        /// </summary>
        public static void SendSignal<T>(T signal) where T : AbstractSignal
        {
            if (SignalProcessorInternal.SignalMethods.ContainsKey(typeof(T)))
            {
                if (_signalQueues.TryGetValue(typeof(T), out Queue signalQueue))
                {
                    signalQueue.Enqueue(signal);
                    _signalCount++;
                }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                else
                    throw new Exception($"No matching signal queue for the {typeof(T)} signal. "
                                        + "Check whether the signal queue is being instantiated correctly.");
#endif
            }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            else
                throw new Exception($"No matching [React] method for the {typeof(T)} signal. "
                                    + "You either don't have a controller/system decorated with [ReactOnSignals] attribute with a corresponding private [React] method."
                                    + "Additionally such controllers must be created as NonLazy() in the corresponding Installer.");
#endif
        }
    }
}