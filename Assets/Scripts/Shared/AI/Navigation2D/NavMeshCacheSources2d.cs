using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshComponents.Extensions
{
    [ExecuteAlways]
    [AddComponentMenu("Navigation/NavMeshCacheSources2d", 30)]
    public class NavMeshCacheSources2d : NavMeshExtension
    {
        Dictionary<Object, NavMeshBuildSource> _lookup;
        Bounds _sourcesBounds;
        public bool IsDirty { get; protected set; }

        NavMeshBuilder2dState _state;

        public int SourcesCount => Cache.Count;
        public int CahcheCount => _lookup.Count;

        public List<NavMeshBuildSource> Cache { get; private set; }

        protected override void Awake()
        {
            _lookup = new Dictionary<Object, NavMeshBuildSource>();
            Cache = new List<NavMeshBuildSource>();
            IsDirty = false;
            Order = -1000;
            _sourcesBounds = new Bounds();
            base.Awake();
        }

        public bool AddSource(GameObject gameObject, NavMeshBuildSource source)
        {
            bool res = _lookup.ContainsKey(gameObject);
            if (res)
                return UpdateSource(gameObject);
            Cache.Add(source);
            _lookup.Add(gameObject, source);
            IsDirty = true;
            return true;
        }

        public bool UpdateSource(GameObject gameObject)
        {
            if (!_lookup.ContainsKey(gameObject))
                return false;
            
            IsDirty = true;
            NavMeshBuildSource source = _lookup[gameObject];
            int idx = Cache.IndexOf(source);
            if (idx < 0)
                return true;
                
            source.transform = Matrix4x4.TRS(gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.lossyScale);
            Cache[idx] = source;
            _lookup[gameObject] = source;

            return true;
        }

        public bool RemoveSource(GameObject gameObject)
        {
            if (!_lookup.ContainsKey(gameObject))
                return false;
            
            IsDirty = true;
            NavMeshBuildSource source = _lookup[gameObject];
            _lookup.Remove(gameObject);
            Cache.Remove(source);

            return true;
        }

        public AsyncOperation UpdateNavMesh(NavMeshData data)
        {
            IsDirty = false;
            return NavMeshBuilder.UpdateNavMeshDataAsync(data, NavMeshSurfaceOwner.GetBuildSettings(), Cache, _sourcesBounds);
        }

        public AsyncOperation UpdateNavMesh()
        {
            return UpdateNavMesh(NavMeshSurfaceOwner.navMeshData);
        }

        public override void CollectSources(NavMeshSurface surface, List<NavMeshBuildSource> sources, NavMeshBuilderState navMeshState)
        {
            _lookup.Clear();
            IsDirty = false;
            _state = navMeshState.GetExtraState<NavMeshBuilder2dState>();
            _state.lookupCallback = LookupCallback;
        }

        void LookupCallback(Object component, NavMeshBuildSource source)
        {
            if (component == null)
                return;
            _lookup.Add(component, source);
        }

        public override void PostCollectSources(NavMeshSurface surface, List<NavMeshBuildSource> sources, NavMeshBuilderState navNeshState)
        {
            _sourcesBounds = navNeshState.WorldBounds;
            Cache = sources;
        }
    }
}