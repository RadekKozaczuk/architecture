#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable

using UnityEngine;

namespace Common
{
    public static class Layers
    {
        // Physics Layer
        public static readonly int Default = LayerMask.NameToLayer("Default");
        public static readonly int TransparentFX = LayerMask.NameToLayer("TransparentFX");
        public static readonly int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        public static readonly int Water = LayerMask.NameToLayer("Water");
        public static readonly int UI = LayerMask.NameToLayer("UI");
        public static readonly int Props = LayerMask.NameToLayer("Props");
        public static readonly int Collectibles = LayerMask.NameToLayer("Collectibles");
        public static readonly int Npcs = LayerMask.NameToLayer("Npcs");

        // Masks
        public static readonly int PropsMask = 1 << Props;
        public static readonly int CollectiblesMask = 1 << Collectibles;
        public static readonly int PropsAndCollectiblesAndNpcsMask = LayerMask.GetMask("Props", "Collectibles", "Npcs");

        public static void ChangeLayer(GameObject gameObject, int layer, bool withChildren = true)
        {
            gameObject.layer = layer;
            if (withChildren)
            {
                Transform transform = gameObject.transform;
                for (int i = 0; i < transform.childCount; i++)
                    ChangeLayer(transform.GetChild(i).gameObject, layer);
            }
        }
    }
}