using UnityEngine;
using ABSFramework.Systems;

public class AssetLoadTest : MonoBehaviour
{
    public string bundleName, assetName;

    void Start()
    {
        AppFacade.Init();
    }
       
    void OnGUI()
    {
        if (GUI.Button(new Rect(30, 50, 200, 60), "Load"))
        {
            ///have fun
            LoadPrefabs(bundleName, assetName);
        }

        if (GUI.Button(new Rect(30, 110, 200, 60), "Unload"))
        {
            PrefabManager.Instance.Unload(bundleName, assetName, _sprite);
        }
    }

    void OnResourceLoadDone(Object data, string assetBundleName, string resName, object customData)
    {
        Debug.LogFormat("Load prefab done. {0} - {1}", assetBundleName, resName);
        _sprite = data as GameObject;
        _sprite.transform.position = Vector3.zero;
    }

    void LoadPrefabs(string bundleName, string resName)
    {
        if (_sprite != null)
            PrefabManager.Instance.Unload(bundleName, resName, _sprite);
        Debug.LogFormat("Load {1} from {0}", resName, bundleName);
        PrefabManager.Instance.Load(bundleName, resName, OnResourceLoadDone);
    }

    GameObject _sprite;

}
