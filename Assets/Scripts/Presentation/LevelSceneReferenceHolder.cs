using System.Collections.Generic;
using Presentation.Views;
using UnityEngine;

namespace Presentation
{
    class LevelSceneReferenceHolder : MonoBehaviour
    {
        [SerializeField]
        internal PlayerView Player;

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