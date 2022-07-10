using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace UnityEngine.AI
{
    public enum CollectObjects2d
    {
        All = 0,
        Volume = 1,
        Children = 2
    }

    [ExecuteAlways]
    [DefaultExecutionOrder(-102)]
    [AddComponentMenu("Navigation/NavMeshSurface2d", 30)]
    [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
    public class NavMeshSurface2d : MonoBehaviour
    {
        public int AgentTypeID { get => _agentTypeID; set => _agentTypeID = value; }
        [SerializeField]
        int _agentTypeID;
        
        public CollectObjects2d CollectObjects { get => _collectObjects; set => _collectObjects = value; }
        [SerializeField]
        CollectObjects2d _collectObjects = CollectObjects2d.All;
        
        public Vector3 Size { get => _size; set => _size = value; }
        [SerializeField]
        Vector3 _size = new(10.0f, 10.0f, 10.0f);

        public Vector3 Center { get => _center; set => _center = value; }
        [SerializeField]
        Vector3 _center = new(0, 2.0f, 0);
        
        public LayerMask LayerMask { get => _layerMask; set => _layerMask = value; }
        [SerializeField]
        LayerMask _layerMask = ~0;
        
        public NavMeshCollectGeometry UseGeometry { get => _useGeometry; set => _useGeometry = value; }
        [SerializeField]
        NavMeshCollectGeometry _useGeometry = NavMeshCollectGeometry.RenderMeshes;
        
        public bool OverrideByGrid { get => _overrideByGrid; set => _overrideByGrid = value; }
        [SerializeField]
        bool _overrideByGrid;
        
        public GameObject UseMeshPrefab { get => _useMeshPrefab; set => _useMeshPrefab = value; }
        [SerializeField]
        GameObject _useMeshPrefab;
        
        public bool CompressBounds { get => _compressBounds; set => _compressBounds = value; }
        [SerializeField]
        bool _compressBounds;
        
        public Vector3 OverrideVector { get => _overrideVector; set => _overrideVector = value; }
        [SerializeField]
        Vector3 _overrideVector = Vector3.one;
        
        public int DefaultArea { get => _defaultArea; set => _defaultArea = value; }
        [SerializeField]
        int _defaultArea;

        public bool IgnoreNavMeshAgent { get => _ignoreNavMeshAgent; set => _ignoreNavMeshAgent = value; }
        [SerializeField]
        bool _ignoreNavMeshAgent = true;
        
        public bool IgnoreNavMeshObstacle { get => _ignoreNavMeshObstacle; set => _ignoreNavMeshObstacle = value; }
        [SerializeField]
        bool _ignoreNavMeshObstacle = true;
        
        public bool OverrideTileSize { get => _overrideTileSize; set => _overrideTileSize = value; }
        [SerializeField]
        bool _overrideTileSize;
        
        public int TileSize { get => _tileSize; set => _tileSize = value; }
        [SerializeField]
        int _tileSize = 256;
        
        public bool OverrideVoxelSize { get => _overrideVoxelSize; set => _overrideVoxelSize = value; }
        [SerializeField]
        bool _overrideVoxelSize;
        
        public float VoxelSize { get => _voxelSize; set => _voxelSize = value; }
        [SerializeField]
        float _voxelSize;
        
        // Currently not supported advanced options
        public bool BuildHeightMesh { get => _buildHeightMesh; set => _buildHeightMesh = value; }
        [SerializeField]
        bool _buildHeightMesh;

        [SerializeField]
        bool _hideEditorLogs;

        public bool HideEditorLogs { get => _hideEditorLogs; set => _hideEditorLogs = value; }

        

        public NavMeshData NavMeshData { get => _navMeshData; set => _navMeshData = value; }
        [SerializeField]
        NavMeshData _navMeshData;
        
        // Do not serialize - runtime only state.
        NavMeshDataInstance _mNavMeshDataInstance;
        Vector3 _mLastPosition = Vector3.zero;
        Quaternion _mLastRotation = Quaternion.identity;

        public static List<NavMeshSurface2d> ActiveSurfaces { get; } = new();

        void OnEnable()
        {
            Register(this);
            AddData();
        }

        void OnDisable()
        {
            RemoveData();
            Unregister(this);
        }

        public void AddData()
        {
#if UNITY_EDITOR
            bool isInPreviewScene = EditorSceneManager.IsPreviewSceneObject(this);
            bool isPrefab = isInPreviewScene || EditorUtility.IsPersistent(this);
            if (isPrefab)
                //Debug.LogFormat("NavMeshData from {0}.{1} will not be added to the NavMesh world because the gameObject is a prefab.",
                //    gameObject.name, name);
                return;
#endif
            if (_mNavMeshDataInstance.valid)
                return;

            if (_navMeshData != null)
            {
                _mNavMeshDataInstance = NavMesh.AddNavMeshData(_navMeshData, transform.position, transform.rotation);
                _mNavMeshDataInstance.owner = this;
            }

            _mLastPosition = transform.position;
            _mLastRotation = transform.rotation;
        }

        public void RemoveData()
        {
            _mNavMeshDataInstance.Remove();
            _mNavMeshDataInstance = new NavMeshDataInstance();
        }

        public NavMeshBuildSettings GetBuildSettings()
        {
            NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByID(_agentTypeID);
            if (buildSettings.agentTypeID == -1)
            {
                if (!_hideEditorLogs) Debug.LogWarning("No build settings for agent type ID " + AgentTypeID, this);
                buildSettings.agentTypeID = _agentTypeID;
            }

            if (OverrideTileSize)
            {
                buildSettings.overrideTileSize = true;
                buildSettings.tileSize = TileSize;
            }

            if (OverrideVoxelSize)
            {
                buildSettings.overrideVoxelSize = true;
                buildSettings.voxelSize = VoxelSize;
            }

            return buildSettings;
        }

        public void BuildNavMesh()
        {
            List<NavMeshBuildSource> sources = CollectSources();

            // Use unscaled bounds - this differs in behaviour from e.g. collider components.
            // But is similar to reflection probe - and since navmesh data has no scaling support - it is the right choice here.
            var sourcesBounds = new Bounds(_center, Abs(_size));
            if (_collectObjects != CollectObjects2d.Volume)
                sourcesBounds = CalculateWorldBounds(sources);

            NavMeshData data = NavMeshBuilder.BuildNavMeshData(
                GetBuildSettings(),
                sources, sourcesBounds, transform.position, transform.rotation);

            if (data != null)
            {
                data.name = gameObject.name;
                RemoveData();
                _navMeshData = data;
                if (isActiveAndEnabled)
                    AddData();
            }
        }

        // Source: https://github.com/Unity-Technologies/NavMeshComponents/issues/97#issuecomment-528692289
        public AsyncOperation BuildNavMeshAsync()
        {
            RemoveData();
            _navMeshData = new NavMeshData(_agentTypeID) {name = gameObject.name, position = transform.position, rotation = transform.rotation};

            if (isActiveAndEnabled)
                AddData();

            return UpdateNavMesh(_navMeshData);
        }

        public AsyncOperation UpdateNavMesh(NavMeshData data)
        {
            List<NavMeshBuildSource> sources = CollectSources();

            // Use unscaled bounds - this differs in behaviour from e.g. collider components.
            // But is similar to reflection probe - and since navmesh data has no scaling support - it is the right choice here.
            var sourcesBounds = new Bounds(_center, Abs(_size));
            if (_collectObjects != CollectObjects2d.Volume)
                sourcesBounds = CalculateWorldBounds(sources);

            return NavMeshBuilder.UpdateNavMeshDataAsync(data, GetBuildSettings(), sources, sourcesBounds);
        }

        static void Register(NavMeshSurface2d surface)
        {
#if UNITY_EDITOR
            bool isInPreviewScene = EditorSceneManager.IsPreviewSceneObject(surface);
            bool isPrefab = isInPreviewScene || EditorUtility.IsPersistent(surface);
            if (isPrefab)
                return;
#endif
            if (ActiveSurfaces.Count == 0)
                NavMesh.onPreUpdate += UpdateActive;

            if (!ActiveSurfaces.Contains(surface))
                ActiveSurfaces.Add(surface);
        }

        static void Unregister(NavMeshSurface2d surface)
        {
            ActiveSurfaces.Remove(surface);

            if (ActiveSurfaces.Count == 0)
                NavMesh.onPreUpdate -= UpdateActive;
        }

        static void UpdateActive()
        {
            for (int i = 0; i < ActiveSurfaces.Count; ++i)
                ActiveSurfaces[i].UpdateDataIfTransformChanged();
        }

        void AppendModifierVolumes(ref List<NavMeshBuildSource> sources)
        {
#if UNITY_EDITOR
            StageHandle myStage = StageUtility.GetStageHandle(gameObject);
            if (!myStage.IsValid())
                return;
#endif
            // Modifiers
            List<NavMeshModifierVolume> modifiers;
            if (_collectObjects == CollectObjects2d.Children)
            {
                modifiers = new List<NavMeshModifierVolume>(GetComponentsInChildren<NavMeshModifierVolume>());
                modifiers.RemoveAll(x => !x.isActiveAndEnabled);
            }
            else
                modifiers = NavMeshModifierVolume.ActiveModifiers;

            foreach (NavMeshModifierVolume m in modifiers)
            {
                if ((_layerMask & 1 << m.gameObject.layer) == 0)
                    continue;
                if (!m.AffectsAgentType(_agentTypeID))
                    continue;
#if UNITY_EDITOR
                if (!myStage.Contains(m.gameObject))
                    continue;
#endif
                Vector3 mcenter = m.transform.TransformPoint(m.Center);
                Vector3 scale = m.transform.lossyScale;
                var msize = new Vector3(m.Size.x * Mathf.Abs(scale.x), m.Size.y * Mathf.Abs(scale.y), m.Size.z * Mathf.Abs(scale.z));

                var src = new NavMeshBuildSource
                    {
                        shape = NavMeshBuildSourceShape.ModifierBox, 
                        transform = Matrix4x4.TRS(mcenter, m.transform.rotation, Vector3.one), 
                        size = msize,
                        area = m.Area
                    };
                sources.Add(src);
            }
        }

        List<NavMeshBuildSource> CollectSources()
        {
            var sources = new List<NavMeshBuildSource>();
            var markups = new List<NavMeshBuildMarkup>();

            List<NavMeshModifier> modifiers;
            if (_collectObjects == CollectObjects2d.Children)
            {
                modifiers = new List<NavMeshModifier>(GetComponentsInChildren<NavMeshModifier>());
                modifiers.RemoveAll(x => !x.isActiveAndEnabled);
            }
            else
                modifiers = NavMeshModifier.ActiveModifiers;

            foreach (NavMeshModifier m in modifiers)
            {
                if ((_layerMask & 1 << m.gameObject.layer) == 0)
                    continue;
                if (!m.AffectsAgentType(_agentTypeID))
                    continue;
                var markup = new NavMeshBuildMarkup();
                markup.root = m.transform;
                markup.overrideArea = m.OverrideArea;
                markup.area = m.Area;
                markup.ignoreFromBuild = m.IgnoreFromBuild;
                markups.Add(markup);
            }

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                if (_collectObjects == CollectObjects2d.All)
                    UnityEditor.AI.NavMeshBuilder.CollectSourcesInStage(
                        null, _layerMask, _useGeometry, _defaultArea, markups, gameObject.scene, sources);
                else if (_collectObjects == CollectObjects2d.Children)
                    UnityEditor.AI.NavMeshBuilder.CollectSourcesInStage(
                        transform, _layerMask, _useGeometry, _defaultArea, markups, gameObject.scene, sources);
                else if (_collectObjects == CollectObjects2d.Volume)
                {
                    Matrix4x4 localToWorld = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                    Bounds worldBounds = GetWorldBounds(localToWorld, new Bounds(_center, _size));

                    UnityEditor.AI.NavMeshBuilder.CollectSourcesInStage(
                        worldBounds, _layerMask, _useGeometry, _defaultArea, markups, gameObject.scene, sources);
                }

                if (!HideEditorLogs && !Mathf.Approximately(transform.eulerAngles.x, 270f))
                    Debug.LogWarning("NavMeshSurface2d is not rotated respectively to (x-90;y0;z0). Apply rotation unless intended.");
                
                var builder = new NavMeshBuilder2dState
                {
                    defaultArea = DefaultArea, 
                    layerMask = LayerMask, 
                    agentID = AgentTypeID, 
                    useMeshPrefab = UseMeshPrefab,
                    overrideByGrid = OverrideByGrid,
                    compressBounds = CompressBounds,
                    overrideVector = OverrideVector,
                    CollectGeometry = UseGeometry,
                    CollectObjects = CollectObjects,
                    parent = gameObject,
                    hideEditorLogs = HideEditorLogs
                };
                
                NavMeshBuilder2d.CollectSources(sources, builder);
            }
            else
#endif
            {
                if (_collectObjects == CollectObjects2d.All)
                    NavMeshBuilder.CollectSources(null, _layerMask, _useGeometry, _defaultArea, markups, sources);
                else if (_collectObjects == CollectObjects2d.Children)
                    NavMeshBuilder.CollectSources(transform, _layerMask, _useGeometry, _defaultArea, markups, sources);
                else if (_collectObjects == CollectObjects2d.Volume)
                {
                    Matrix4x4 localToWorld = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                    Bounds worldBounds = GetWorldBounds(localToWorld, new Bounds(_center, _size));
                    NavMeshBuilder.CollectSources(worldBounds, _layerMask, _useGeometry, _defaultArea, markups, sources);
                }

                if (!HideEditorLogs && !Mathf.Approximately(transform.eulerAngles.x, 270f))
                    Debug.LogWarning("NavMeshSurface2d is not rotated respectively to (x-90;y0;z0). Apply rotation unless intended.");
                var builder = new NavMeshBuilder2dState();
                builder.defaultArea = DefaultArea;
                builder.layerMask = LayerMask;
                builder.agentID = AgentTypeID;
                builder.useMeshPrefab = UseMeshPrefab;
                builder.overrideByGrid = OverrideByGrid;
                builder.compressBounds = CompressBounds;
                builder.overrideVector = OverrideVector;
                builder.CollectGeometry = UseGeometry;
                builder.CollectObjects = CollectObjects;
                builder.parent = gameObject;
                builder.hideEditorLogs = HideEditorLogs;
                NavMeshBuilder2d.CollectSources(sources, builder);
            }

            if (_ignoreNavMeshAgent)
                sources.RemoveAll(x => x.component != null && x.component.gameObject.GetComponent<NavMeshAgent>() != null);

            if (_ignoreNavMeshObstacle)
                sources.RemoveAll(x => x.component != null && x.component.gameObject.GetComponent<NavMeshObstacle>() != null);

            AppendModifierVolumes(ref sources);

            return sources;
        }

        static Vector3 Abs(Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        public static Bounds GetWorldBounds(Matrix4x4 mat, Bounds bounds)
        {
            Vector3 absAxisX = Abs(mat.MultiplyVector(Vector3.right));
            Vector3 absAxisY = Abs(mat.MultiplyVector(Vector3.up));
            Vector3 absAxisZ = Abs(mat.MultiplyVector(Vector3.forward));
            Vector3 worldPosition = mat.MultiplyPoint(bounds.center);
            Vector3 worldSize = absAxisX * bounds.size.x + absAxisY * bounds.size.y + absAxisZ * bounds.size.z;
            return new Bounds(worldPosition, worldSize);
        }

        Bounds CalculateWorldBounds(List<NavMeshBuildSource> sources)
        {
            // Use the unscaled matrix for the NavMeshSurface
            Matrix4x4 worldToLocal = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            worldToLocal = worldToLocal.inverse;
            var result = new Bounds();
            if (CollectObjects != CollectObjects2d.Children)
                result.Encapsulate(CalculateGridWorldBounds(worldToLocal));

            foreach (NavMeshBuildSource src in sources)
                switch (src.shape)
                {
                    case NavMeshBuildSourceShape.Mesh:
                    {
                        var m = src.sourceObject as Mesh;
                        result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, m.bounds));
                        break;
                    }
                    case NavMeshBuildSourceShape.Terrain:
                    {
                        // Terrain pivot is lower/left corner - shift bounds accordingly
                        var t = src.sourceObject as TerrainData;
                        result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, new Bounds(0.5f * t.size, t.size)));
                        break;
                    }
                    case NavMeshBuildSourceShape.Box:
                    case NavMeshBuildSourceShape.Sphere:
                    case NavMeshBuildSourceShape.Capsule:
                    case NavMeshBuildSourceShape.ModifierBox:
                        result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, new Bounds(Vector3.zero, src.size)));
                        break;
                }

            // Inflate the bounds a bit to avoid clipping co-planar sources
            result.Expand(0.1f);
            return result;
        }

        static Bounds CalculateGridWorldBounds(Matrix4x4 worldToLocal)
        {
            var bounds = new Bounds();
            var grid = FindObjectOfType<Grid>();
            Tilemap[] tilemaps = grid.GetComponentsInChildren<Tilemap>();
            if (tilemaps == null || tilemaps.Length < 1)
                throw new NullReferenceException("Add at least one tilemap");
            foreach (Tilemap tilemap in tilemaps)
            {
                //Debug.Log($"From Local Bounds [{tilemap.name}]: {tilemap.localBounds}");
                Bounds lbounds = GetWorldBounds(worldToLocal * tilemap.transform.localToWorldMatrix, tilemap.localBounds);
                bounds.Encapsulate(lbounds);
                //Debug.Log($"To World Bounds: {bounds}");
            }

            bounds.Expand(0.1f);
            return bounds;
        }

        bool HasTransformChanged()
        {
            if (_mLastPosition != transform.position) 
                return true;
            return _mLastRotation != transform.rotation;
        }

        void UpdateDataIfTransformChanged()
        {
            if (HasTransformChanged())
            {
                RemoveData();
                AddData();
            }
        }

#if UNITY_EDITOR
        bool UnshareNavMeshAsset()
        {
            // Nothing to unshare
            if (_navMeshData == null)
                return false;

            // Prefab parent owns the asset reference
            bool isInPreviewScene = EditorSceneManager.IsPreviewSceneObject(this);
            bool isPersistentObject = EditorUtility.IsPersistent(this);
            if (isInPreviewScene || isPersistentObject)
                return false;

            // An instance can share asset reference only with its prefab parent
            NavMeshSurface2d prefab = PrefabUtility.GetCorrespondingObjectFromSource(this);
            if (prefab != null && prefab.NavMeshData == NavMeshData)
                return false;

            // Don't allow referencing an asset that's assigned to another surface
            for (int i = 0; i < ActiveSurfaces.Count; ++i)
            {
                NavMeshSurface2d surface = ActiveSurfaces[i];
                if (surface != this && surface._navMeshData == _navMeshData)
                    return true;
            }

            // Asset is not referenced by known surfaces
            return false;
        }

        void OnValidate()
        {
            if (UnshareNavMeshAsset())
            {
                if (!_hideEditorLogs) Debug.LogWarning("Duplicating NavMeshSurface does not duplicate the referenced navmesh data", this);
                _navMeshData = null;
            }

            NavMeshBuildSettings settings = NavMesh.GetSettingsByID(_agentTypeID);
            if (settings.agentTypeID == -1)
                return;
            
            // When unchecking the override control, revert to automatic value.
            const float KMinVoxelSize = 0.01f;
            if (!_overrideVoxelSize)
                _voxelSize = settings.agentRadius / 3.0f;
            if (_voxelSize < KMinVoxelSize)
                _voxelSize = KMinVoxelSize;

            // When unchecking the override control, revert to default value.
            const int KMinTileSize = 16;
            const int KMaxTileSize = 1024;
            const int KDefaultTileSize = 256;

            if (!_overrideTileSize)
                _tileSize = KDefaultTileSize;
            // Make sure tilesize is in sane range.
            if (_tileSize < KMinTileSize)
                _tileSize = KMinTileSize;
            if (_tileSize > KMaxTileSize)
                _tileSize = KMaxTileSize;
        }
#endif
    }
}