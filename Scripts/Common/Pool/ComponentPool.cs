using System.Collections.Generic;
using UnityEngine;

namespace HFramwork
{

    public static class ComponentPool
    {
        public static List<Component> Get()
        {
            return _pool.Get();
        }

        public static void Release(List<Component> list)
        {
            _pool.Release(list);
        }

        private static readonly ObjectPool<List<Component>> _pool = new ObjectPool<List<Component>>(() =>
            {
                return new List<Component>();
            }, l => l.Clear());
    }
}