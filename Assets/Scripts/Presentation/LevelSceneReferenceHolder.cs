#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Presentation.Views;
using UnityEngine;

namespace Presentation
{
    class LevelSceneReferenceHolder : MonoBehaviour
    {
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
        // todo: add assertion that checks if spawn points are not duplicated

        internal Transform GetSpawnPoint(int playerId) => _spawnPoints[playerId].transform;

        internal void CopyLightReferences(List<Light> list)
        {
            list.Clear();

            for (int i = 0; i < LightContainer.childCount; i++)
                list.Add(LightContainer.GetChild(i).GetComponent<Light>());
        }

        void Awake()
        {
            Debug.Log("LevelSceneReferenceHolder AWAKE");
        }

        void Start()
        {
            Debug.Log("LevelSceneReferenceHolder START");
        }
    }
}