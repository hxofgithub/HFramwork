
using System.Collections.Generic;

namespace ABSFramework.Tools
{
    public static class ListPool<T>
    {
        public static List<T> Get()
        {
            return _pool.Get();
        }

        public static void Release(List<T> element)
        {
            _pool.Release(element);
        }


        private static ObjectPool<List<T>> _pool = new ObjectPool<List<T>>(onReleaseAction: l => { l.Clear(); });
    }
}


