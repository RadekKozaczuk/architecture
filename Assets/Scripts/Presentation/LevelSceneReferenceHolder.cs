using System.Collections.Generic;
using Common.Enums;
using Presentation.Views;
using UnityEngine;

namespace Presentation
{
    class LevelSceneReferenceHolder : MonoBehaviour
    {
        // todo: add assertion that checks if spawn points are not duplicated

        internal Transform GetSpawnPoint(PlayerId playerId) => _spawnPoints[(int)playerId].transform;

        [SerializeField]
        SpawnPointView[] _spawnPoints;

        [SerializeField]
        internal List<Collider2D> Colliders;

        [SerializeField]
        internal Transform LightContainer;

        [SerializeField]
        internal Transform EnemySpawnPointContainer;

        [SerializeField]
        internal Transform EnemyContainer;

        internal void CopyLightReferences(List<Light> list)
        {
            list.Clear();

            for (int i = 0; i < LightContainer.childCount; i++)
                list.Add(LightContainer.GetChild(i).GetComponent<Light>());
        }
    }
}