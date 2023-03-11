using Shared;

namespace Common.Signals
{
    public sealed class MissionCompleteSignal : AbstractSignal {
		public MissionCompleteSignal() {
			MyDebug.Log("Instantly wins the mission.");
		}
	}
}