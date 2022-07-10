﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

namespace NavMeshComponents.Extensions
{
    [ExecuteAlways]
    [AddComponentMenu("Navigation/NavMeshCollectSources2d", 30)]
    public class NavMeshCollectSources2d : NavMeshExtension
    {
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
        
        public override void CalculateWorldBounds(NavMeshSurface surface, List<NavMeshBuildSource> sources, NavMeshBuilderState navMeshState)
        {
            if (surface.CollectObjects != CollectObjects.Volume)
                navMeshState.WorldBounds.Encapsulate(CalculateGridWorldBounds(surface, navMeshState.WorldToLocal, navMeshState.WorldBounds));
        }

        static Bounds CalculateGridWorldBounds(NavMeshSurface surface, Matrix4x4 worldToLocal, Bounds bounds)
        {
            var grid = FindObjectOfType<Grid>();
            Tilemap[] tilemaps = grid?.GetComponentsInChildren<Tilemap>();
            if (tilemaps == null || tilemaps.Length < 1)
                return bounds;
            foreach (Tilemap tilemap in tilemaps)
            {
                Bounds lbounds = NavMeshSurface.GetWorldBounds(worldToLocal * tilemap.transform.localToWorldMatrix, tilemap.localBounds);
                bounds.Encapsulate(lbounds);
            }

            return bounds;
        }

        public override void CollectSources(NavMeshSurface surface, List<NavMeshBuildSource> sources, NavMeshBuilderState navMeshState)
        {
            if (!surface.HideEditorLogs)
            {
                if (!Mathf.Approximately(transform.eulerAngles.x, 270f))
                    Debug.LogWarning("NavMeshSurface is not rotated respectively to (x-90;y0;z0). Apply rotation unless intended.");
                if (Application.isPlaying)
                    if (surface.UseGeometry == NavMeshCollectGeometry.PhysicsColliders && Time.frameCount <= 1)
                        Debug.LogWarning(
                            "Use Geometry - Physics Colliders option in NavMeshSurface may cause inaccurate mesh bake if executed before Physics update.");
            }

            var builder = navMeshState.GetExtraState<NavMeshBuilder2dState>();
            builder.defaultArea = surface.DefaultArea;
            builder.layerMask = surface.LayerMask;
            builder.agentID = surface.AgentTypeID;
            builder.useMeshPrefab = UseMeshPrefab;
            builder.overrideByGrid = OverrideByGrid;
            builder.compressBounds = CompressBounds;
            builder.overrideVector = OverrideVector;
            builder.CollectGeometry = surface.UseGeometry;
            builder.CollectObjects = (CollectObjects2d)(int)surface.CollectObjects;
            builder.parent = surface.gameObject;
            builder.hideEditorLogs = surface.HideEditorLogs;
            builder.SetRoot(navMeshState.Roots);
            NavMeshBuilder2d.CollectSources(sources, builder);
        }
    }
}