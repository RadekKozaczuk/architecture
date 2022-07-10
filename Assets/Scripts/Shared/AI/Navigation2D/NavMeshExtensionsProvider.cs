using System.Collections.Generic;

namespace NavMeshComponents.Extensions
{
    public interface INavMeshExtensionsProvider
    {
        int Count { get; }
        NavMeshExtension this[int index] { get; }
        void Add(NavMeshExtension extension, int order);
        void Remove(NavMeshExtension extension);
    }

    internal class NavMeshExtensionsProvider : INavMeshExtensionsProvider
    {
        readonly List<NavMeshExtensionMeta> _extensions = new();

        static readonly Comparer<NavMeshExtensionMeta> _comparer = Comparer<NavMeshExtensionMeta>.Create(
            (x, y) => x.order > y.order
                ? 1
                : x.order < y.order
                    ? -1
                    : 0);

        public NavMeshExtension this[int index] => _extensions[index].extension;

        public int Count => _extensions.Count;

        public void Add(NavMeshExtension extension, int order)
        {
            var meta = new NavMeshExtensionMeta(order, extension);
            int at = _extensions.BinarySearch(meta, _comparer);
            if (at < 0)
            {
                _extensions.Add(meta);
                _extensions.Sort(_comparer);
            }
            else
                _extensions.Insert(at, meta);
        }

        public void Remove(NavMeshExtension extension)
        {
            _extensions.RemoveAll(x => x.extension = extension);
        }
    }
}