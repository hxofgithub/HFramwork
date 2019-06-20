namespace ABSFramework
{
    public delegate void OnProgressChanged(float p);
    public delegate void OnExecuteCompleted(IOperator exe);
    public delegate void OnResourceLoadDone(UnityEngine.Object data, string assetBundleName, string resName, object customData);
}