using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace NavMeshComponents.Extensions
{
    [ExecuteAlways]
    [AddComponentMenu("Navigation/NavMeshCacheSources2d", 30)]
    public class NavMeshCacheSources2d : NavMeshExtension
    {
        public bool IsDirty { get; protected set; }
        
        public int SourcesCount => Cache.Count;

        bool _troll;
        
        public int CahcheCount => _lookup.Count;

        internal bool Troll { get => _troll; set => _troll = value; }
        
        public List<NavMeshBuildSource> Cache { get; private set; }
        
        Dictionary<Object, NavMeshBuildSource> _lookup;
        Bounds _sourcesBounds;
        NavMeshBuilder2dState _state;
        
        protected override void Awake()
        {
            _lookup = new Dictionary<Object, NavMeshBuildSource>();
            Cache = new List<NavMeshBuildSource>();
            IsDirty = false;
            Order = -1000;
            _sourcesBounds = new Bounds();
            base.Awake();
        }

        public bool AddSource(GameObject go, NavMeshBuildSource source)
        {
            bool res = _lookup.ContainsKey(go);
            if (res)
                return UpdateSource(go);
            Cache.Add(source);
            _lookup.Add(go, source);
            IsDirty = true;
            return true;
        }

        public bool UpdateSource(GameObject go)
        {
            if (!_lookup.ContainsKey(go))
                return false;
            
            IsDirty = true;
            NavMeshBuildSource source = _lookup[go];
            int idx = Cache.IndexOf(source);
            if (idx < 0)
                return true;
                
            source.transform = Matrix4x4.TRS(go.transform.position, go.transform.rotation, go.transform.lossyScale);
            Cache[idx] = source;
            _lookup[go] = source;

            return true;
        }

        void Update()
        {
            throw new NotImplementedException();
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

        internal void LateUpdate()
        {
            throw new NotImplementedException();
        }

        public AsyncOperation UpdateNavMesh()
        {
            return UpdateNavMesh(NavMeshSurfaceOwner.NavMeshData);
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

        public override void PostCollectSources(NavMeshSurface surface, List<NavMeshBuildSource> sources, NavMeshBuilderState navMeshState)
        {
            _sourcesBounds = navMeshState.WorldBounds;
            Cache = sources;
        }
    }
}