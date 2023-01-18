#if UNITY_EDITOR || DEVELOPMENT_BUILD
using Common.Signals;
using Shared.CheatEngine;
using Shared.Systems;

namespace Common
{
    // todo: should be called via reflection
    public static class CommonDebugCommands
    {
        public static void InitializeDebugCommands()
        {
            DebugCommands.AddCommand(() =>
            {
                SignalProcessor.SendSignal(new MissionCompleteSignal());
            }, "Win Mission", "Instantly wins the mission.");

            DebugCommands.AddCommand(() =>
            {
                SignalProcessor.SendSignal(new MissionFailedSignal());
            }, "Fail Mission", "Instantly wins the current mission.");
        }
    }
}
#endif