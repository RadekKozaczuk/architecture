#if UNITY_EDITOR
using Common.Config;
using Common.Factories;
using Common.Signals;
using Shared.Systems;
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
        void FailMission() => SignalFactory.SendMissionFailedSignal();

        [InfoBox("Instantly completes the mission for this player.", InfoMessageType.None)]
        [Button]
        void CompleteMission() => SignalFactory.SendMissionCompleteSignal();
    }
}
#endif