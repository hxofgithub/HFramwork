using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.Profiling;
#endif
public static class Log
{
    public static void Info(string msg, params object[] args)
    {
        Debug.LogFormat(msg, args);
    }

    public static void Warning(string msg, params object[] args)
    {
        Debug.LogWarningFormat(msg, args);
    }

    public static void Error(string msg, params object[] args)
    {
        Debug.LogErrorFormat(msg, args);
    }

    public static void Info(object msg)
    {
        Debug.Log(msg);
    }

    public static void Warning(object msg)
    {
        Debug.LogWarning(msg);
    }

    public static void Error(object msg)
    {
        Debug.LogError(msg);
    }

    public static void BeginSample(string name)
    {
#if UNITY_EDITOR
        Profiler.BeginSample(name);
#endif
    }

    public static void BeginSample(string name, Object target)
    {
#if UNITY_EDITOR
        Profiler.BeginSample(name, target);
#endif
    }

    public static void EndSample()
    {
#if UNITY_EDITOR
        Profiler.EndSample();
#endif
    }

}