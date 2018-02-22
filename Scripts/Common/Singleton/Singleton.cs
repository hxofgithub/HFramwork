namespace HFramwork
{
    public class Singleton<T> where T : class, ISingleton, new()
    {
        public static T Instance
        { 
            get
            { 
                if (_instance == null)
                {
                    _instance = new T();
                    _instance.Init();
                }                
                return _instance;
            }
        }

        public static void Dispose()
        {
            if (_instance != null)
                _instance = null;
        }

        private static T _instance;
    }
}