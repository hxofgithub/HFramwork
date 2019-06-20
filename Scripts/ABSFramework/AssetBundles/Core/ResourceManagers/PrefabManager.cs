
using UnityEngine;

namespace ABSFramework.Systems
{
    public sealed class PrefabManager : ResourcesManager<GameObject, PrefabManager>
    {
        protected override void OnInitailize()
        {
            _instace = this;
            base.OnInitailize();
        }

        protected override GameObject OnLoadAssetDone(GameObject data)
        {
            return GameObject.Instantiate(data);
        }

        protected override bool PreOnLoad(string assetBundleName, string resName, OnResourceLoadDone callback, object customData)
        {
            var key = MakeKey(assetBundleName, resName);
            if (_assetsDict.ContainsKey(key))
            {
                if (callback != null)
                {
                    var g = GameObject.Instantiate(_assetsDict[key]);
                    callback(g, assetBundleName, resName, customData);
                    return false;
                }
            }
            return true;
        }

        protected override bool PreUnload(string assetBundleName, string resName, GameObject data)
        {
            if (data != null)
                GameObject.Destroy(data);

            return base.PreUnload(assetBundleName, resName, data);
        }

    }
}
