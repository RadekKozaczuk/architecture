using UnityEngine;

namespace Presentation.Data
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Data/ZombieWolfData")]
    class ZombieWolfData : ScriptableObject
    {
        [Tooltip("Movement speed of the enemy")]
        [SerializeField]
        [Range(0.1f, 5f)]
        internal float BasicSpeed;
    }
}