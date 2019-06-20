using UnityEngine;

[CreateAssetMenu(fileName = "NewAssetBundleConfig", menuName = "AssetBundles/Config/AssetBundleConfig", order = 100)]
public class AssetBundleConfig : ScriptableObject
{
    public enum LoadMode : int
    {
        Sync = 10000,
        Async = -10000,
    }

    public LoadMode mode;
    
    public static LoadMode Mode
    {
        get { return _instance.mode; }
        set { _instance.mode = value; }
    }
        
    public static void SetInistance(AssetBundleConfig inst)
    {
        _instance = inst;
    }
    private static AssetBundleConfig _instance;
}