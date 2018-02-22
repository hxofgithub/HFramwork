using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFramwork
{
    public class SysObjectListPool<T>
    {
        public static List<T> Get()
        {
            return _componentPool.Get();
        }

        public static void Release(List<T> l)
        {
            _componentPool.Release(l);
        }

        private readonly static ObjectPool<List<T>> _componentPool = new ObjectPool<List<T>>(() =>
            {
                return new List<T>();
            }, l => l.Clear());
    }
}