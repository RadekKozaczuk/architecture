using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshComponents.Extensions
{
    public abstract class NavMeshExtension : MonoBehaviour
    {
        public int Order { get; protected set; }

        public NavMeshSurface NavMeshSurfaceOwner
        {
            get
            {
                if (_mNavMeshOwner == null)
                    _mNavMeshOwner = GetComponent<NavMeshSurface>();
                return _mNavMeshOwner;
            }
        }
        NavMeshSurface _mNavMeshOwner;

        protected virtual void Awake() => ConnectToVcam(true);

        protected virtual void OnEnable() { }

        public virtual void CollectSources(NavMeshSurface surface, List<NavMeshBuildSource> sources, NavMeshBuilderState navMeshState) { }

        public virtual void CalculateWorldBounds(NavMeshSurface surface, List<NavMeshBuildSource> sources, NavMeshBuilderState navMeshState) { }

        public virtual void PostCollectSources(NavMeshSurface surface, List<NavMeshBuildSource> sources, NavMeshBuilderState navMeshState) { }

#if UNITY_EDITOR
        [DidReloadScripts]
        static void OnScriptReload()
        {
            var extensions = Resources.FindObjectsOfTypeAll(typeof(NavMeshExtension)) as NavMeshExtension[];
            foreach (NavMeshExtension e in extensions)
                e.ConnectToVcam(true);
        }
#endif

        protected virtual void OnDestroy() => ConnectToVcam(false);

        protected virtual void ConnectToVcam(bool connect)
        {
            if (connect && NavMeshSurfaceOwner == null)
                Debug.LogError("NevMeshExtension requires a NavMeshSurface component");
            if (NavMeshSurfaceOwner == null)
                return;

            if (connect)
                NavMeshSurfaceOwner.NevMeshExtensions.Add(this, Order);
            else
                NavMeshSurfaceOwner.NevMeshExtensions.Remove(this);
        }
    }
}