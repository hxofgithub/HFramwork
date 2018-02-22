using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace HFramwork
{
    public class AssetBundleUtils
    {
        public static string GetPlatformName()
        {
            #if UNITY_EDITOR
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
            #else
            return GetPlatformForAssetBundles(Application.platform);
            #endif
        }

        #if UNITY_EDITOR
        private static string GetPlatformForAssetBundles(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return "OSX";
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }
        #endif

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }

        public static string GetDocumentPath()
        {
            string path;
            #if UNITY_IPHONE && !UNITY_EDITOR
            path = Application.persistentDataPath + "/";
            #elif UNITY_ANDROID && !UNITY_EDITOR
            path = Application.persistentDataPath + "/Documents/";
            #elif UNITY_WP8 && !UNITY_EDITOR
            path = Application.persistentDataPath + "/Documents/";
            #else
            path = Application.dataPath + "/Documents/";
            #endif
            return path;
        }

        public static string GetStreamingPath()
        {
            string path;
            #if UNITY_IPHONE && !UNITY_EDITOR
            path = "file://" + Application.streamingAssetsPath + "/";
            #elif UNITY_ANDROID && !UNITY_EDITOR
            path = Application.streamingAssetsPath + "/";
            #else
            path = Application.streamingAssetsPath + "/";
            #endif

            return path;

        }
    }
}
