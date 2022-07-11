using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshComponents.Extensions
{
    [ExecuteAlways]
    [AddComponentMenu("Navigation/NavMeshCollectRootSources2d", 30)]
    public class NavMeshCollectRootSources2d : NavMeshExtension
    {
        public List<GameObject> RootSources { get => _rootSources; set => _rootSources = value; }
        [SerializeField]
        List<GameObject> _rootSources;

        protected override void Awake()
        {
            Order = -1000;
            base.Awake();
        }

        public override void CollectSources(NavMeshSurface surface, List<NavMeshBuildSource> sources, NavMeshBuilderState navMeshState) =>
            navMeshState.Roots = _rootSources;
    }
}