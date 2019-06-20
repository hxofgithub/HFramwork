
using UnityEngine;
using ABSFramework.Systems;

public class VariantTest : MonoBehaviour
{
    void Start()
    {
        AppFacade.Init();
    }

    bool hd, sd;
    void OnGUI()
    {

        GUI.Label(new Rect(5,5,800,50), "Load Sprite from assetbundle : example_prefabs");
        hd = GUI.Toggle(new Rect(5,25,80,20), hd,"HD");
        if (hd)
            sd = false;
        else if (!sd)
            hd = true;

        sd = GUI.Toggle(new Rect(90, 25, 80, 20), sd, "SD");
        if (sd)
            hd = false;
        else if (!hd)
            sd = true;

        if (GUI.Button(new Rect(30, 50, 200, 60), "Load Sprite-HD"))
        {
            LoadVarintPrefabs("example_prefabs", "Sprite", hd ? "hd" : "sd");
        }

        if (GUI.Button(new Rect(30, 110, 200, 60), "Unload Sprite"))
        {
            PrefabManager.Instance.Unload("example_prefabs", "Sprite", _sprite);
        }
    }

    void OnResourceLoadDone(Object data, string assetBundleName, string resName, object customData)
    {
        Debug.LogFormat("Load prefab done. {0} - {1}", assetBundleName, resName);
        _sprite = data as GameObject;
        _sprite.transform.position = Vector3.zero;
    }


    ///You can load varint asset like this.
    void LoadVarintPrefabs(string bundleName, string resName, string variantName)
    {
        if (_sprite != null)
            PrefabManager.Instance.Unload(bundleName, resName, _sprite);
        Debug.LogFormat("Load {1} from {0}", resName, bundleName);
        AssetBundleManager.ActiveVariants = new string[] { variantName };
        PrefabManager.Instance.Load(bundleName, "Sprite", OnResourceLoadDone);
    }


    GameObject _sprite;

}
