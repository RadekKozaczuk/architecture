#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core.Enums;
using UnityEngine;

namespace Presentation.Views
{
    /// <summary>
    /// Spawn point is only visible in editor. Its meshFilter and MeshRenderer components are destroy on Awake.
    /// </summary>
    class SpawnPointView : MonoBehaviour
    {
        [SerializeField]
        PlayerId _playerId;

        [SerializeField]
        MeshFilter _meshFilter;

        [SerializeField]
        MeshRenderer _meshRenderer;

        void Awake()
        {
            Destroy(_meshFilter);
            Destroy(_meshRenderer);
        }
    }
}