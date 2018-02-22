#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace HFramwork
{
    public class BuildScript
    {
        public static string overloadedDevelopmentServerURL = "";
        public static string AssetBundlesOutputPath = Application.streamingAssetsPath + "/AssetBundles";

        public static void BuildAssetBundles()
        {

            // Choose the output path according to the build target.
            string outputPath = Path.Combine(AssetBundlesOutputPath, AssetBundleUtils.GetPlatformName());
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            //@TODO: use append hash... (Make sure pipeline works correctly with it.)
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.AppendHashToAssetBundleName, EditorUserBuildSettings.activeBuildTarget);
        }

        public static void BuildPlayer()
        {
            var outputPath = EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
            if (outputPath.Length == 0)
                return;

            string[] levels = GetLevelsFromBuildSettings();
            if (levels.Length == 0)
            {
                Log.Info("Nothing to build.");
                return;
            }

            string targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
            if (targetName == null)
                return;

            BuildAssetBundles();

            BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
            BuildPipeline.BuildPlayer(levels, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option | BuildOptions.CompressWithLz4);
        }

        public static string GetBuildTargetName(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "/test.apk";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "/test.exe";
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return "/test.app";
                case BuildTarget.WebGL:
                    return "";
                // Add more build targets for your own.
                default:
                    Log.Info("Target not implemented.");
                    return null;
            }
        }

        static string[] GetLevelsFromBuildSettings()
        {
            List<string> levels = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                    levels.Add(EditorBuildSettings.scenes[i].path);
            }

            return levels.ToArray();
        }
    }
}
#endif