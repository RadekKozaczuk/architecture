﻿using System.Collections.Generic;

namespace NavMeshComponents.Extensions
{
    public interface INavMeshExtensionsProvider
    {
        int Count { get; }
        NavMeshExtension this[int index] { get; }
        void Add(NavMeshExtension extension, int order);
        void Remove(NavMeshExtension extension);
    }

    class NavMeshExtensionsProvider : INavMeshExtensionsProvider
    {
        public NavMeshExtension this[int index] => _extensions[index].Extension;
        readonly List<NavMeshExtensionMeta> _extensions = new();

        static readonly Comparer<NavMeshExtensionMeta> _comparer = Comparer<NavMeshExtensionMeta>.Create(
            (x, y) => x.Order > y.Order ? 1 : x.Order < y.Order ? -1 : 0);

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

        public void Remove(NavMeshExtension extension) => _extensions.RemoveAll(x => x.Extension = extension);
    }
}