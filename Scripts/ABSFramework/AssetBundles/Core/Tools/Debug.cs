
namespace ABSFramework
{
    public static class Debug
    {
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        public static void LogFormat(string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(message, args);
        }

        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }


        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }

}
