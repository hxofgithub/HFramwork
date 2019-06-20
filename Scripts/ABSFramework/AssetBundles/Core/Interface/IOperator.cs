namespace ABSFramework
{
    public interface IOperator
    {        
        event OnExecuteCompleted onCompleted;
        void Execute();
        bool IsDone { get; }
    }

    interface IProgressor
    {
        float progress { get; }
    }

    interface IProgressNotify : IProgressor
    {
        event OnProgressChanged onProgressChanged;
    }

    interface IBaseLoader : IOperator, IProgressNotify { }

    interface IBundleLoader
    {
        string errorMsg { get; }
        string assetBundlePath { get; }
        UnityEngine.AssetBundle bundle { get; }
        void SetLoadParams(string bundlePath, OnExecuteCompleted completedFunc);
    }


    interface IAssetLoader
    {
        string assetBundleName { get; }
        string resName { get; }
        T GetAsset<T>() where T : UnityEngine.Object;
        void SetParams(string bundleName, string resName, System.Type tp, OnExecuteCompleted complete);
    }


    interface ISceneLoader
    {
        void SetParams(string assetBundleName, string sceneName, UnityEngine.SceneManagement.LoadSceneMode mode);
    }

    interface IBundleUnloader : IOperator
    {
        string assetBundleName { get; }
        bool unloadAllLoadedObjects { get; }
    }
}
