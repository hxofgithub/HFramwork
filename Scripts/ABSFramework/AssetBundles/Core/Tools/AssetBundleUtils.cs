
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif



namespace ABSFramework
{
    public class AssetBundleUtils
    {

        public static string GetFloderName()
        {
#if UNITY_EDITOR
            return GetFloderForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
            return GetFloderForAssetBundle(Application.platform);
#endif
        }


#if UNITY_EDITOR

        const string _kSimulationMode = "_kSimulationMode";
        public static bool SimulationModeInEditor
        {
            get
            {
                return EditorPrefs.GetBool(_kSimulationMode, false);
            }
            set
            {
                EditorPrefs.SetBool(_kSimulationMode, value);
            }
        }


        private static string GetFloderForAssetBundles(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }
#endif

        private static string GetFloderForAssetBundle(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }


        public static string GetLocalAssetPath()
        {
            if (Application.isEditor)
                return Application.streamingAssetsPath;
            else if (Application.isMobilePlatform)
                return Application.persistentDataPath;
            else
                return Application.streamingAssetsPath;

        }
    }
}
