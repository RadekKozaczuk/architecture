#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable
using Sirenix.OdinInspector;
using UnityEngine;

namespace Common.Config
{
    [CreateAssetMenu(fileName = "BackendConfig", menuName = "Config/Common/BackendConfig")]
    class BackendConfig : ScriptableObject
    {
        [SerializeField]
        internal string ApiBaseAddress = "https://some-address.com";

        [Min(0)]
        [SuffixLabel("ms")]
        [InfoBox("Time, in milliseconds, after the game throws away an exception.", InfoMessageType.None)]
        [SerializeField]
        internal double TimeOutDuration = 700;
    }
}