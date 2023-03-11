using Shared;
using System.Diagnostics;

namespace Common.Signals
{
    public sealed class MissionFailedSignal : AbstractSignal {
        public MissionFailedSignal() {
            MyDebug.Log("Instantly fail current mission.");
        }
    }
}