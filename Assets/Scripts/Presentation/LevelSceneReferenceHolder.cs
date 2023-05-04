using System.Collections.Generic;
using Common.Enums;
using Presentation.Views;
using UnityEngine;

namespace Presentation
{
    class LevelSceneReferenceHolder : MonoBehaviour
    {
        internal Transform GetSpawnPoint(PlayerId playerId) => SpawnPoints[(int)playerId].transform;

        [SerializeField]
        internal SpawnPointView[] SpawnPoints;

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