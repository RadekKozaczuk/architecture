using Sirenix.OdinInspector;
using UnityEngine;

namespace Common.Config
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/Common/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        /// <summary>
        ///     Player's max health.
        /// </summary>
        public ushort MaxHealth = 100;

        [Range(1, 10)]
        [SuffixLabel("m")]
        [InfoBox("Push distance in meters.", InfoMessageType.None)]
        public float PushDistance = 5;
    }
}