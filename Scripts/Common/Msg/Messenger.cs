namespace HFramwork
{
    using System;
    using System.Collections.Generic;

    public class Messenger
    {
        #region Add and Remove Listener

        public static void AddListener(string key, Action callback)
        {            
            AddListener(key, (Delegate)callback);
        }

        public static void AddListener<T>(string key, Action<T> callback)
        {
            AddListener(key, (Delegate)callback);
        }

        public static void AddListener<T,U>(string key, Action<T,U> callback)
        {
            AddListener(key, (Delegate)callback);
        }

        public static void AddListener<T,U,V>(string key, Action<T,U,V> callback)
        {
            AddListener(key, (Delegate)callback);
        }

        public static void AddListener<T,U,V,W>(string key, Action<T,U,V,W> callback)
        {
            AddListener(key, (Delegate)callback);
        }

        private static void AddListener(string key, Delegate callback)
        {
            try
            {
                Delegate tempListener = null;
                if (_mListeners.TryGetValue(key, out tempListener))
                    _mListeners[key] = Delegate.Combine(tempListener, callback);
                else
                    _mListeners[key] = callback;
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

        }


        public static void RemoveListener(string key, Action callback)
        {
            RemoveListener(key, (Delegate)callback);
        }

        public static void RemoveListener<T>(string key, Action<T> callback)
        {
            RemoveListener(key, (Delegate)callback);
        }

        public static void RemoveListener<T,U>(string key, Action<T,U> callback)
        {
            RemoveListener(key, (Delegate)callback);
        }

        public static void RemoveListener<T,U,V>(string key, Action<T,U,V> callback)
        {
            RemoveListener(key, (Delegate)callback);
        }

        public static void RemoveListener<T,U,V,W>(string key, Action<T,U,V,W> callback)
        {
            RemoveListener(key, (Delegate)callback);
        }

        private static void RemoveListener(string key, Delegate callback)
        {
            try
            {
                Delegate tempListener = null;
                if (_mListeners.TryGetValue(key, out tempListener))
                    _mListeners[key] = Delegate.Remove(tempListener, callback);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

        }

        #endregion

        #region Dispatch Messenger

        public static void Dispatch(string key)
        {
            try
            {
                var listener = _mListeners.GetValue(key);
                if (listener != null)
                    ((Action)listener)();
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

        }

        public static void Dispatch<T>(string key, T arg1)
        {
            try
            {
                var listener = _mListeners.GetValue(key);
                if (listener != null)
                    ((Action<T>)listener)(arg1);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

        }

        public static void Dispatch<T,U>(string key, T arg1, U arg2)
        {
            try
            {
                var listener = _mListeners.GetValue(key);
                if (listener != null)
                    ((Action<T,U>)listener)(arg1, arg2);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

        }

        public static void Dispatch<T,U,V>(string key, T arg1, U arg2, V arg3)
        {
            try
            {
                var listener = _mListeners.GetValue(key);
                if (listener != null)
                    ((Action<T,U,V>)listener)(arg1, arg2, arg3);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

        }

        public static void Dispatch<T,U,V,W>(string key, T arg1, U arg2, V arg3, W arg4)
        {
            try
            {
                var listener = _mListeners.GetValue(key);
                if (listener != null)
                    ((Action<T,U,V,W>)listener)(arg1, arg2, arg3, arg4);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

        }

        #endregion

        private static void LogError(Exception ex)
        {
            Log.Error("{0}\r\n{1}", ex.Message, ex.StackTrace);
        }

        private static Dictionary<string, Delegate> _mListeners = new Dictionary<string, Delegate>();
    }
}
