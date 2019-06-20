#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace ABSFramework
{
    public class SimulationAssetLoader : IAssetLoader, IBaseLoader
    {
        public string assetBundleName { get; private set; }

        public bool IsDone { get; private set; }

        public float progress { get; private set; }

        public string resName { get; private set; }

        public event OnExecuteCompleted onCompleted;
        public event OnProgressChanged onProgressChanged;

        public T GetAsset<T>() where T : UnityEngine.Object
        {
            if (_object != null)
                return _object as T;
            else
                return null;
        }

        public void SetParams(string bundleName, string resName, Type tp, OnExecuteCompleted complete)
        {
            this.assetBundleName = bundleName;
            this.resName = resName;
            this.onCompleted = complete;
            this._type = tp;
        }

        public void Execute()
        {
            if (!IsDone)
            {
                OnUpdate();
                if(IsDone)
                {
                    if (onCompleted != null)
                    {
                        onCompleted(this);
                    }
                }
            }
        }
        protected void OnUpdate()
        {
            progress = 0;
            var paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, resName);
            foreach (var p in paths)
            {
                var o = AssetDatabase.LoadAssetAtPath(p, _type);
                if(o != null)
                {
                    _object = o;                    
                    break;
                }
            }

            progress = 1;
            IsDone = true;
            if (onProgressChanged != null)
                onProgressChanged(1);
        }

        private System.Type _type;
        private UnityEngine.Object _object;
    }
}
#endif