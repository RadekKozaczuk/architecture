namespace NavMeshComponents.Extensions
{
    internal class NavMeshExtensionMeta
    {
        public readonly int order;

        public NavMeshExtensionMeta(int order, NavMeshExtension extension)
        {
            this.order = order;
            this.extension = extension;
        }

        public NavMeshExtension extension;
    }
}