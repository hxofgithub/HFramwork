
using UnityEngine;
using UnityEngine.SceneManagement;
using UObj = UnityEngine.Object;

namespace ABSFramework
{
    public abstract class BaseOperator : IBaseLoader
    {
        public bool IsDone { get; protected set; }

        public float progress { get; private set; }
        public event OnExecuteCompleted onCompleted;
        public event OnProgressChanged onProgressChanged;
        
        public void Execute()
        {
            if (!IsDone)
            {
                OnUpdate();
                if (IsDone)
                {
                    if (onCompleted != null)
                    {
                        onCompleted(this);
                    }
                }
            }

        }

        protected abstract void OnUpdate();

        protected void SetProgress(float val)
        {
            if (progress == val)
                return;
            progress = val;
            if (onProgressChanged != null)
                onProgressChanged(val);
        }
    }


    #region BundleLoaders

    internal abstract class BaseBundleLoader : BaseOperator, IBundleLoader
    {
        public void SetLoadParams(string bundlePath, OnExecuteCompleted completedFunc)
        {
            assetBundlePath = bundlePath;
            onCompleted += completedFunc;
        }

        public string assetBundlePath { get; private set; }

        public AssetBundle bundle { get; protected set; }

        public string errorMsg { get; protected set; }

    }
    
    internal sealed class BundleLoaderAsync_FromFile : BaseBundleLoader
    {
        protected override void OnUpdate()
        {
            if (_request == null)
                _request = AssetBundle.LoadFromFileAsync(assetBundlePath);

            if (_request.isDone)
            {
                bundle = _request.assetBundle;
                if (bundle == null)
                {
                    errorMsg = string.Format("Cannt load bundle [{0}]", assetBundlePath);
                    Debug.LogError(errorMsg);
                }
            }
                

            SetProgress(_request.progress);
            IsDone = _request.isDone;

            Debug.Log(progress);
        }

        AssetBundleCreateRequest _request;
    }

    internal sealed class BundleLoaderSync_FromFile : BaseBundleLoader
    {
        protected override void OnUpdate()
        {
            bundle = AssetBundle.LoadFromFile(assetBundlePath);
            if (bundle == null)
            {
                errorMsg = string.Format("Cannt load bundle [{0}]", assetBundlePath);
                Debug.LogError(errorMsg);
            }
                
            SetProgress(1);
            IsDone = true;
        }
    }
    
    internal sealed class UnLoadBundle : IBundleUnloader
    {
        public UnLoadBundle(string bundleName, OnExecuteCompleted completedFunc = null, bool unloadAllLoadedObjects = true)
        {
            assetBundleName = bundleName;
            onCompleted += completedFunc;
            this.unloadAllLoadedObjects = unloadAllLoadedObjects;
        }

        public bool unloadAllLoadedObjects { get; private set; }
        public string assetBundleName { get; private set; }
        public bool IsDone { get; private set; }
        public event OnExecuteCompleted onCompleted;

        void IOperator.Execute()
        {
            if (!IsDone)
            {
                IsDone = true;
                if (onCompleted != null)
                    onCompleted(this);
            }
        }
    }

    #endregion


    #region AssetLoaders        

    internal abstract class BaseAssetLoader : BaseOperator, IAssetLoader
    {
        void IAssetLoader.SetParams(string _assetBundleName, string _resName, System.Type _type, OnExecuteCompleted _complete)
        {
            assetBundleName = _assetBundleName;
            resName = _resName;
            m_type = _type;
            onCompleted += _complete;
        }

        public T GetAsset<T>() where T : UnityEngine.Object
        {
            if (_asset != null)
                return _asset as T;
            return null;
        }

        public string resName { get; private set; }
        public string assetBundleName { get; private set; }

        protected UObj _asset;
        protected System.Type m_type;
    }
    internal sealed class LoadAssetFromAssetBundle : BaseAssetLoader
    {
        protected override void OnUpdate()
        {
            var abref = Systems.AssetBundleManager.GetAssetBundleRef(assetBundleName);
            if (abref != null)
            {
                _asset = abref.assetBundle.LoadAsset(resName, m_type);
                SetProgress(1);
                IsDone = true;
            }
            else
            {
                var oper = Systems.AssetBundleManager.GetAssetBundleOperator(assetBundleName);
                if (oper != null)
                {
                    var loader = oper as IProgressor;
                    if (loader != null)
                    {
                        SetProgress(loader.progress * 0.5f);
                    }
                }
            }
        }
    }
    internal sealed class LoadAssetFromAssetBundle_Async : BaseAssetLoader
    {
        protected override void OnUpdate()
        {
            if (_request == null)
            {
                var oper = Systems.AssetBundleManager.GetAssetBundleOperator(assetBundleName);
                if (oper != null)
                {
                    var loader = oper as IProgressor;
                    if (loader != null)
                    {
                        SetProgress(loader.progress * 0.5f);
                    }
                }
                else
                {
                    var abref = Systems.AssetBundleManager.GetAssetBundleRef(assetBundleName);
                    if (abref != null)
                    {
                        _request = abref.assetBundle.LoadAssetAsync(resName, m_type);
                        SetProgress(.5f);
                    }
                }
            }
            else
            {
                SetProgress(0.5f + _request.progress * .5f);
                IsDone = _request.isDone;
                if (IsDone)
                    _asset = _request.asset;
            }
        }

        private AssetBundleRequest _request;
    }


    internal abstract class LoadSceneFromAssetBundleBase : BaseOperator, ISceneLoader
    {
        protected string _assetBundleName, _sceneName;
        protected LoadSceneMode _mode;

        void ISceneLoader.SetParams(string assetBundleName, string sceneName, LoadSceneMode mode)
        {
            _mode = mode;
            _assetBundleName = assetBundleName;
            _sceneName = sceneName;
        }
    }
    internal sealed class LoadSceneFromAssetBundle : LoadSceneFromAssetBundleBase
    {
        protected override void OnUpdate()
        {
            var oper = Systems.AssetBundleManager.GetAssetBundleOperator(_assetBundleName);
            if (oper != null)
            {
                var loader = oper as IProgressor;
                if (loader != null)
                {
                    SetProgress(loader.progress * 0.5f);
                }
            }
            else
            {
                var abref = Systems.AssetBundleManager.GetAssetBundleRef(_assetBundleName);
                if (abref != null)
                {
                    SceneManager.LoadScene(_sceneName, _mode);
                    SetProgress(1);
                    IsDone = true;
                }
            }
        }
    }
    internal sealed class LoadSceneFromAssetBundle_Async : LoadSceneFromAssetBundleBase
    {
        protected override void OnUpdate()
        {
            if (_request == null)
            {
                var oper = Systems.AssetBundleManager.GetAssetBundleOperator(_assetBundleName);
                if (oper != null)
                {
                    var loader = oper as IProgressor;
                    if (loader != null)
                    {
                        SetProgress(loader.progress * 0.5f);
                    }
                }
                else
                {
                    var abref = Systems.AssetBundleManager.GetAssetBundleRef(_assetBundleName);
                    if (abref != null)
                    {
                        _request = SceneManager.LoadSceneAsync(_sceneName, _mode);
                        SetProgress(.5f);
                    }
                }
            }
            else
            {
                SetProgress(.5f + _request.progress * .5f);
                IsDone = _request.isDone;
            }
        }
        private AsyncOperation _request;
    }

    #endregion
}
