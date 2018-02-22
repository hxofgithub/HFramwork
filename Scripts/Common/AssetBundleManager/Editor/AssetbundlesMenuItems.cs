#if UNITY_EDITOR
using UnityEditor;
namespace HFramework
{
	public class AssetBundlesMenuItems
	{
		const string kSimulationMode = "Tools/AssetBundles/Simulation Mode";
	
		[MenuItem(kSimulationMode)]
		public static void ToggleSimulationMode ()
		{
			AssetBundleManager.SimulateAssetBundleInEditor = !AssetBundleManager.SimulateAssetBundleInEditor;
		}
	
		[MenuItem(kSimulationMode, true)]
		public static bool ToggleSimulationModeValidate ()
		{
			Menu.SetChecked(kSimulationMode, AssetBundleManager.SimulateAssetBundleInEditor);
			return true;
		}
		
        [MenuItem ("Tools/AssetBundles/Build AssetBundles")]
		static public void BuildAssetBundles ()
		{
			BuildScript.BuildAssetBundles();
            AssetDatabase.Refresh();
		}

        [MenuItem("Tools/AssetBundles/Build Player")]
        static public void PublishPlayer()
        {
            EditorUtility.DisplayProgressBar("Build AssetBundles", "Excuting", 0);
            BuildScript.BuildAssetBundles();

            EditorUtility.DisplayProgressBar("Gen MD5 Hash", "Excuting", 0);
            MD5Menu.GenMD5Hash();

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            BuildScript.BuildPlayer();
        }
	}
}
#endif