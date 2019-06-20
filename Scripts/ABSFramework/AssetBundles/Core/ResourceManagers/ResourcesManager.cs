
using System;
using System.Collections.Generic;
using UnityEngine;
using ABSFramework.Tools;

namespace ABSFramework.Systems
{
    public abstract class ResourcesManager<T, ManagerT> : IInitailizeSystem, IExecuteSystem, ICleanupSystem where T : UnityEngine.Object
        where ManagerT : ResourcesManager<T, ManagerT>
    {

        class ResourcesLoadDoneCallback
        {
            public OnResourceLoadDone callback;
            public object customData;
        }

        public static ManagerT Instance
        {
            get { return _instace; }
        }

        public void Load(string assetBundleName, string resName, OnResourceLoadDone callback, object customData = null)
        {
            if (string.IsNullOrEmpty(assetBundleName) || string.IsNullOrEmpty(resName))
                return;
            if (PreOnLoad(assetBundleName, resName, callback, customData))
            {
                AssetBundleManager.LoadAssetBundle(assetBundleName);
                var op = CreateAssetLoader(assetBundleName, resName);
                _assetLoadDoneCallbacks.Add(op, new ResourcesLoadDoneCallback() { callback = callback, customData = customData });
                AddLoader(op);
            }
        }

        public void Unload(string assetBundleName, string resName, T data)
        {
            if (PreUnload(assetBundleName, resName, data))
            {
                AssetBundleManager.UnLoadAssetBundle(assetBundleName);
                _assetsDict.Remove(MakeKey(assetBundleName, resName));
            }
        }

        #region interface methods

        void ICleanupSystem.Cleanup()
        {
            OnCleanup();
        }

        void IExecuteSystem.Execute()
        {
            int min = _min(_assetLoader.Count, _maxDownloadCnt);
            if (min > 0)
            {
                var doneList = ListPool<int>.Get();
                for (int i = 0; i < min; i++)
                {
                    var ele = _assetLoader[i];
                    ele.Execute();
                    if (ele.IsDone)
                    {
                        doneList.Add(i);
                    }
                }
                for (int i = doneList.Count - 1; i >= 0; i--)
                {
                    _assetLoader.RemoveAt(doneList[i]);
                }

                ListPool<int>.Release(doneList);
            }
            OnUpdate();
        }

        void IInitailizeSystem.Initailize()
        {
            _assetLoader = new List<IOperator>();
            _assetsDict = new Dictionary<string, T>();
            _min = Mathf.Min;
            _assetLoadDoneCallbacks = new Dictionary<IOperator, ResourcesLoadDoneCallback>();
            OnInitailize();
        }

        #endregion
        
        #region internal methods
        private void AddLoader(IOperator opera)
        {
            _assetLoader.Add(opera);
        }

        private IOperator CreateAssetLoader(string assetBundleName, string resName)
        {
            IAssetLoader opera;
#if UNITY_EDITOR
            if (AssetBundleUtils.SimulationModeInEditor)
            {
                opera = new SimulationAssetLoader();
            }
            else
            {
#endif
                opera = OperatorFactory.CreateAssetLoader() as IAssetLoader;
#if UNITY_EDITOR
            }
#endif
            opera.SetParams(assetBundleName, resName, typeof(T), LoadAssetDone);
            return (opera as IOperator);
        }

        private void LoadAssetDone(IOperator opera)
        {
            var assetLoader = opera as IAssetLoader;
            _assetsDict.Add(MakeKey(assetLoader.assetBundleName, assetLoader.resName), assetLoader.GetAsset<T>());
            var g = OnLoadAssetDone(assetLoader.GetAsset<T>());
            ResourcesLoadDoneCallback r;
            if (_assetLoadDoneCallbacks.TryGetValue(opera, out r))
            {
                r.callback(g, assetLoader.assetBundleName, assetLoader.resName, r.customData);
                _assetLoadDoneCallbacks.Remove(opera);
            }
        }

        protected string MakeKey(string assetBundleName, string resName)
        {
            return assetBundleName + "_" + resName + "_" + typeof(T).Name;
        }

        #endregion
        
        #region methods to ovveride

        protected virtual bool PreOnLoad(string assetBundleName, string resName, OnResourceLoadDone callback, object customData)
        {
            return true;
        }

        protected virtual bool PreUnload(string assetBundleName, string resName, T data)
        {
            return true;
        }

        protected virtual T OnLoadAssetDone(T data) { return data; }
        protected virtual void OnCleanup() { }
        protected virtual void OnInitailize() { }
        protected virtual void OnUpdate() { }
        #endregion
        
        #region properties
        protected Dictionary<string, T> _assetsDict;
        private List<IOperator> _assetLoader;
        private Func<int, int, int> _min;
        protected int _maxDownloadCnt = 3;
        protected static ManagerT _instace;

        private Dictionary<IOperator, ResourcesLoadDoneCallback> _assetLoadDoneCallbacks;
        #endregion
    }
}

