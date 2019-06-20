namespace ABSFramework.Tools
{
    public class ObjectPool<T> where T : new()
    {
        public ObjectPool(System.Action<T> onGetAction = null, System.Action<T> onReleaseAction = null)
        {
            _onGetAction = onGetAction;
            _onReleaseAction = onReleaseAction;
            _pool = new System.Collections.Generic.Stack<T>();
        }

        public T Get()
        {
            T element;
            if (_pool.Count > 0)
                element = _pool.Pop();
            else
                element = new T();
            if (_onGetAction != null)
                _onGetAction(element);
            return element;
        }

        public void Release(T element)
        {
            if (_pool.Count > 0 && ReferenceEquals(element, _pool.Peek()))
                Debug.LogError("Dumplicate release element.");
            if (_onReleaseAction != null)
                _onReleaseAction(element);
            _pool.Push(element);
        }

        private System.Action<T> _onGetAction;
        private System.Action<T> _onReleaseAction;
        private System.Collections.Generic.Stack<T> _pool;
    }

}
