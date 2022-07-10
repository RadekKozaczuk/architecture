using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace UnityEngine.AI
{
    class NavMeshBuilder2dState
    {
        public Dictionary<Sprite, Mesh> map;
        public Dictionary<uint, Mesh> coliderMap;
        public Action<Object, NavMeshBuildSource> lookupCallback;
        public int defaultArea;
        public int layerMask;
        public int agentID;
        public bool overrideByGrid;
        public GameObject useMeshPrefab;
        public bool compressBounds;
        public Vector3 overrideVector;
        public NavMeshCollectGeometry CollectGeometry;
        public CollectObjects2d CollectObjects;
        public GameObject parent;
        public bool hideEditorLogs;

        public IEnumerable<GameObject> Root => _root ?? GetRoot();
        protected IEnumerable<GameObject> _root;

        public NavMeshBuilder2dState()
        {
            map = new Dictionary<Sprite, Mesh>();
            coliderMap = new Dictionary<uint, Mesh>();
            _root = null;
        }

        public Mesh GetMesh(Sprite sprite)
        {
            Mesh mesh;
            if (map.ContainsKey(sprite))
                mesh = map[sprite];
            else
            {
                mesh = new Mesh();
                NavMeshBuilder2d.Sprite2Mesh(sprite, mesh);
                map.Add(sprite, mesh);
            }

            return mesh;
        }

        public Mesh GetMesh(Collider2D collider)
        {
            Mesh mesh;
            uint hash = collider.GetShapeHash();
            if (coliderMap.ContainsKey(hash))
                mesh = coliderMap[hash];
            else
            {
                mesh = collider.CreateMesh(false, false);
                coliderMap.Add(hash, mesh);
            }

            return mesh;
        }

        public void SetRoot(IEnumerable<GameObject> root)
        {
            _root = root;
        }

        public IEnumerable<GameObject> GetRoot()
        {
            switch (CollectObjects)
            {
                case CollectObjects2d.Children: return new[] {parent};
                case CollectObjects2d.Volume:
                case CollectObjects2d.All:
                default:
                {
                    var list = new List<GameObject>();
                    var testlist = new List<GameObject>();
                    for (int i = 0; i < SceneManager.sceneCount; ++i)
                    {
                        Scene s = SceneManager.GetSceneAt(i);
                        s.GetRootGameObjects(list);
                        testlist.AddRange(list);
                    }

                    return testlist;
                }
            }
        }
    }

    class NavMeshBuilder2d
    {
        public static void CollectSources(List<NavMeshBuildSource> sources, NavMeshBuilder2dState builder)
        {
            foreach (GameObject it in builder.Root)
            {
                if (!it.activeSelf)
                    continue;
                CollectSources(it, sources, builder);
            }

            if (!builder.hideEditorLogs) Debug.Log("Sources " + sources.Count);
        }

        public static void CollectSources(GameObject root, List<NavMeshBuildSource> sources, NavMeshBuilder2dState builder)
        {
            foreach (NavMeshModifier modifier in root.GetComponentsInChildren<NavMeshModifier>())
            {
                if ((0x1 << modifier.gameObject.layer & builder.layerMask) == 0)
                    continue;
                if (!modifier.AffectsAgentType(builder.agentID))
                    continue;

                int area = builder.defaultArea;
                //if it is walkable
                if (builder.defaultArea != 1 && !modifier.IgnoreFromBuild)
                    AddDefaultWalkableTilemap(sources, builder, modifier);

                if (modifier.OverrideArea)
                    area = modifier.Area;
                if (!modifier.IgnoreFromBuild)
                    CollectSources(sources, builder, modifier, area);
            }
        }

        public static void CollectSources(List<NavMeshBuildSource> sources, NavMeshBuilder2dState builder, NavMeshModifier modifier, int area)
        {
            if (builder.CollectGeometry == NavMeshCollectGeometry.PhysicsColliders)
            {
                var collider = modifier.GetComponent<Collider2D>();
                if (collider != null)
                    CollectSources(sources, collider, area, builder);
            }
            else
            {
                var tilemap = modifier.GetComponent<Tilemap>();
                if (tilemap != null)
                    CollectTileSources(sources, tilemap, area, builder);
                var sprite = modifier.GetComponent<SpriteRenderer>();
                if (sprite != null)
                    CollectSources(sources, sprite, area, builder);
            }
        }

        static void AddDefaultWalkableTilemap(List<NavMeshBuildSource> sources, NavMeshBuilder2dState builder, NavMeshModifier modifier)
        {
            var tilemap = modifier.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            if (builder.compressBounds)
                tilemap.CompressBounds();

            if (!builder.hideEditorLogs)
                Debug.Log($"Walkable Bounds [{tilemap.name}]: {tilemap.localBounds}");
            NavMeshBuildSource box = BoxBoundSource(NavMeshSurface2d.GetWorldBounds(tilemap.transform.localToWorldMatrix, tilemap.localBounds));
            box.area = builder.defaultArea;
            sources.Add(box);
        }

        public static void CollectSources(List<NavMeshBuildSource> sources, SpriteRenderer spriteRenderer, int area, NavMeshBuilder2dState builder)
        {
            if (spriteRenderer == null)
                return;
            Mesh mesh = builder.GetMesh(spriteRenderer.sprite);
            if (mesh == null)
            {
                if (!builder.hideEditorLogs)
                    Debug.Log($"{spriteRenderer.name} mesh is null");
                return;
            }

            var src = new NavMeshBuildSource
            {
                shape = NavMeshBuildSourceShape.Mesh,
                component = spriteRenderer,
                area = area,
                transform = Matrix4x4.TRS(
                    Vector3.Scale(spriteRenderer.transform.position, builder.overrideVector), spriteRenderer.transform.rotation,
                    spriteRenderer.transform.lossyScale),
                sourceObject = mesh
            };
            sources.Add(src);

            builder.lookupCallback?.Invoke(spriteRenderer.gameObject, src);
        }

        public static void CollectSources(List<NavMeshBuildSource> sources, Collider2D collider, int area, NavMeshBuilder2dState builder)
        {
            if (collider.usedByComposite)
                collider = collider.GetComponent<CompositeCollider2D>();

            Mesh mesh = builder.GetMesh(collider);
            if (mesh == null)
            {
                if (!builder.hideEditorLogs)
                    Debug.Log($"{collider.name} mesh is null");
                return;
            }

            var src = new NavMeshBuildSource
            {
                shape = NavMeshBuildSourceShape.Mesh,
                area = area,
                component = collider,
                sourceObject = mesh,
                transform = collider.attachedRigidbody
                    ? Matrix4x4.TRS(
                        Vector3.Scale(collider.attachedRigidbody.transform.position, builder.overrideVector),
                        collider.attachedRigidbody.transform.rotation, Vector3.one)
                    : Matrix4x4.identity
            };

            sources.Add(src);

            builder.lookupCallback?.Invoke(collider.gameObject, src);
        }

        public static void CollectTileSources(List<NavMeshBuildSource> sources, Tilemap tilemap, int area, NavMeshBuilder2dState builder)
        {
            BoundsInt bound = tilemap.cellBounds;
            var vec3int = new Vector3Int(0, 0, 0);

            var size = new Vector3(tilemap.layoutGrid.cellSize.x, tilemap.layoutGrid.cellSize.y, 0);
            Mesh sharedMesh = null;
            Quaternion rot = default;

            var src = new NavMeshBuildSource {area = area};

            if (builder.useMeshPrefab != null)
            {
                sharedMesh = builder.useMeshPrefab.GetComponent<MeshFilter>().sharedMesh;
                size = builder.useMeshPrefab.transform.localScale;
                rot = builder.useMeshPrefab.transform.rotation;
            }

            for (int i = bound.xMin; i < bound.xMax; i++)
            {
                for (int j = bound.yMin; j < bound.yMax; j++)
                {
                    vec3int.x = i;
                    vec3int.y = j;
                    if (!tilemap.HasTile(vec3int))
                        continue;

                    CollectTile(tilemap, builder, vec3int, size, sharedMesh, rot, ref src);
                    sources.Add(src);

                    builder.lookupCallback?.Invoke(tilemap.GetInstantiatedObject(vec3int), src);
                }
            }
        }

        static void CollectTile(
            Tilemap tilemap, NavMeshBuilder2dState builder, Vector3Int vec3int, Vector3 size, Mesh sharedMesh, Quaternion rot,
            ref NavMeshBuildSource src
        )
        {
            if (!builder.overrideByGrid && tilemap.GetColliderType(vec3int) == Tile.ColliderType.Sprite)
            {
                Sprite sprite = tilemap.GetSprite(vec3int);
                if (sprite == null)
                    return;

                Mesh mesh = builder.GetMesh(sprite);
                src.component = tilemap;
                src.transform = GetCellTransformMatrix(tilemap, builder.overrideVector, vec3int);
                src.shape = NavMeshBuildSourceShape.Mesh;
                src.sourceObject = mesh;
            }
            else if (builder.useMeshPrefab != null || builder.overrideByGrid && builder.useMeshPrefab != null)
            {
                src.transform = Matrix4x4.TRS(Vector3.Scale(tilemap.GetCellCenterWorld(vec3int), builder.overrideVector), rot, size);
                src.shape = NavMeshBuildSourceShape.Mesh;
                src.sourceObject = sharedMesh;
            }
            else //default to box
            {
                src.transform = GetCellTransformMatrix(tilemap, builder.overrideVector, vec3int);
                src.shape = NavMeshBuildSourceShape.Box;
                src.size = size;
            }
        }

        public static Matrix4x4 GetCellTransformMatrix(Tilemap tilemap, Vector3 scale, Vector3Int vec3int)
        {
            return Matrix4x4.TRS(
                       Vector3.Scale(tilemap.GetCellCenterWorld(vec3int), scale) - tilemap.layoutGrid.cellGap, tilemap.transform.rotation,
                       tilemap.transform.lossyScale)
                   * tilemap.orientationMatrix
                   * tilemap.GetTransformMatrix(vec3int);
        }

        internal static void Sprite2Mesh(Sprite sprite, Mesh mesh)
        {
            var vert = new Vector3[sprite.vertices.Length];
            for (int i = 0; i < sprite.vertices.Length; i++)
                vert[i] = new Vector3(sprite.vertices[i].x, sprite.vertices[i].y, 0);
            mesh.vertices = vert;
            mesh.uv = sprite.uv;
            int[] tri = new int[sprite.triangles.Length];
            for (int i = 0; i < sprite.triangles.Length; i++)
                tri[i] = sprite.triangles[i];
            mesh.triangles = tri;
        }

        static NavMeshBuildSource BoxBoundSource(Bounds localBounds)
        {
            return new NavMeshBuildSource
            {
                transform = Matrix4x4.Translate(localBounds.center), shape = NavMeshBuildSourceShape.Box, size = localBounds.size, area = 0
            };
        }
    }
}