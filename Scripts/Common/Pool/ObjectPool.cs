using System.Collections.Generic;

namespace HFramework.Pools
{
    using Core.Delegates;
    public class ObjectPool<T>
    {
        public ObjectPool(DelegateFunc<T> action, DelegateAction<T> release)
        {
            getAction = action;
            releaseAction = release;
        }

        public T Get()
        {
            T t = default(T);
            if (stack.Count > 0)
            {
                t = stack.Pop();
                ObjectCount++;
            }
            else
            {
                if (getAction != null)
                {
                    t = getAction();
                    ObjectCount++;
                }
            }
            return t;
        }

        public void Release(T t)
        {
            stack.Push(t);
            if (releaseAction != null)
                releaseAction(t);
        }

        public int ObjectCount{ get; private set; }

        public int ActiveCount{ get { return ObjectCount - stack.Count; } }

        private DelegateFunc<T> getAction = null;
        private DelegateAction<T> releaseAction = null;
        private Stack<T> stack = new Stack<T>();
    }
}