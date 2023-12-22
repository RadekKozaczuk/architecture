#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable
using Sirenix.OdinInspector;
using UnityEngine;

namespace Common.Config
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/Common/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        /// <summary>
        /// Player's max health.
        /// </summary>
        [Min(0)]
        public int MaxHealth = 100;

        /// <summary>
        /// Player's speed.
        /// </summary>
        [Min(0)]
        public int Speed = 5;

        [Range(1, 10)]
        [SuffixLabel("m")]
        [InfoBox("Push distance in meters.", InfoMessageType.None)]
        public float PushDistance = 5;
    }
}