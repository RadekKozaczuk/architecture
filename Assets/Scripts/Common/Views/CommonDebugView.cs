#if UNITY_EDITOR
using Common.Config;
using Common.Signals;
using Shared.SignalProcessing;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Common.Views
{
    [DisallowMultipleComponent]
    public class CommonDebugView : MonoBehaviour
    {
        static readonly DebugConfig _debugConfig;

        void Awake() => DontDestroyOnLoad(gameObject);

        [InfoBox("Instantly fails the mission for this player.", InfoMessageType.None)]
        [Button]
        void FailMission() => SignalProcessor.SendSignal(new MissionFailedSignal());

        [InfoBox("Instantly completes the mission for this player.", InfoMessageType.None)]
        [Button]
        void CompleteMission() => SignalProcessor.SendSignal(new MissionCompleteSignal());
    }
}
#endif