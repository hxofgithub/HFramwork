using System.Collections;
using HFramework;
using UnityEngine;

public class ResourceSyetems : MonoBehaviour
{
    public static ResourceSyetems Instance { get; private set; }
    public static bool InitDown { get; private set; }

    public static void Init()
    {
        if (Instance == null)
        {
            InitDown = false;
            Instance = new GameObject("ResourceSyetems").AddComponent(typeof(ResourceSyetems)) as ResourceSyetems;
        }            
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        AssetBundleManager.SetSourceAssetBundleURL();
        yield return AssetBundleManager.Initialize();

        gameObject.AddComponent<TextureManager>();

        InitDown = true;
    }
}
