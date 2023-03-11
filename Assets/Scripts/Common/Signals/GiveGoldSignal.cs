using Shared;
using System.Diagnostics;

namespace Common.Signals {
	public sealed class GiveGoldSignal : AbstractSignal {

		public GiveGoldSignal(int value) {
			MyDebug.Log("Add " + value.ToString() + " gold");
		}
	}
}