
using UnityEditor;
using System.IO;

namespace ABSFramework.Editor
{
    public class AssetBundleMenu
    {
        [MenuItem("AssetBundles/Build Asset Bundle")]
        static void BuildAssetBundle()
        {
            var output = Path.Combine(AssetBundleUtils.GetLocalAssetPath(), AssetBundleUtils.GetFloderName());
            Directory.CreateDirectory(output);
            BuildPipeline.BuildAssetBundles(output, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }


        const string SimulationMenuPath = "AssetBundles/Simulation";
        [MenuItem(SimulationMenuPath)]
        static void Simulation()
        {
            AssetBundleUtils.SimulationModeInEditor = !AssetBundleUtils.SimulationModeInEditor;
        }

        
        [MenuItem(SimulationMenuPath, true)]
        static bool SimulationValidate()
        {
            Menu.SetChecked(SimulationMenuPath, AssetBundleUtils.SimulationModeInEditor);
            return true;
        }

    }

}
