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
        public static readonly int Collectables = LayerMask.NameToLayer("Collectables");
        public static readonly int Npcs = LayerMask.NameToLayer("Npcs");
        public static readonly int Players = LayerMask.NameToLayer("Players");

        // Masks
        public static readonly int PropsMask = 1 << Props;
        public static readonly int CollectablesMask = 1 << Collectables;
        public static readonly int PropsAndPlayersMask = LayerMask.GetMask("Props", "Players");
        public static readonly int PropsAndCollectablesAndNpcsMask = LayerMask.GetMask("Props", "Collectables", "Npcs");

        public static void ChangeLayer(GameObject gameObject, int layer, bool withChildren = true)
        {
            gameObject.layer = layer;
            if (withChildren)
            {
                Transform transform = gameObject.transform;
                for (int i = 0 ; i < transform.childCount ; i++)
                    ChangeLayer(transform.GetChild(i).gameObject, layer);
            }
        }
    }
}