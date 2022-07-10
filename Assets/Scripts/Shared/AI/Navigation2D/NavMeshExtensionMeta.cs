namespace NavMeshComponents.Extensions
{
    class NavMeshExtensionMeta
    {
        public readonly int Order;
        public NavMeshExtension Extension;

        public NavMeshExtensionMeta(int order, NavMeshExtension extension)
        {
            Order = order;
            Extension = extension;
        }
    }
}