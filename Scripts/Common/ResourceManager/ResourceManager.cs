using System.Collections.Generic;
using UnityEngine;
using HFramework;
using HFramework.CollectionExtensions.Dictionary;

public abstract class ResourceManager<T, ManagerT> : MonoBehaviour where ManagerT : ResourceManager<T, ManagerT> where T : Object
{
    internal class ResLoadOpreation
    {
        public string abName, resName;
        public AssetBundleLoadAssetOperation opera = null;

        public OnResourceLoadDone callback;
        public object custonData;
    }

    internal class ResUnloadOpreation : ResLoadOpreation
    {
        public T res;
    }

    public delegate void OnResourceLoadDone(T res, string abName, string resName, object customData);

    public static ManagerT Instance { get; private set; }

    #region Mono Func

    void Awake()
    {
        Instance = this as ManagerT;
        OnInit(0);
    }

    private void Update()
    {
        UpdateRequest();
        if (_requestOpreList.Count > 0)
        {
            for (int i = 0; i < _requestOpreList.Count;)
            {
                var opre = _requestOpreList[i];
                if (opre is ResUnloadOpreation)
                {
                    var unOpre = opre as ResUnloadOpreation;
                    string error = string.Empty;
                    var ab = AssetBundleManager.GetLoadedAssetBundle(unOpre.abName, out error);
                    if (ab != null && string.IsNullOrEmpty(error))
                    {
                        //load done
                        if (ab.m_AssetBundle != null)
                        {
                            if (OnProcessUnLoadRes(unOpre.res, unOpre.abName, unOpre.resName))
                            {
                                AssetBundleManager.UnloadAssetBundle(unOpre.abName);
                                _zeroRef.Remove(MakeKey(unOpre.abName, unOpre.resName));
                            }
                            _requestOpreList.RemoveAt(i);
                        }
                        else
                            i++;
                    }
                    else
                    {
                        _requestOpreList.RemoveAt(i);
                    }
                }
                else
                {
                    if (opre.opera.IsDone())
                    {
                        OnLoadDone(opre);
                        _requestOpreList.RemoveAt(i);
                    }
                    else
                        i++;
                }
            }
        }
    }

    #endregion

    public void LoadAsset(string abName, string resName, OnResourceLoadDone callback, object custom = null)
    {
        if (!CheckAssetBundleAndAssetName(abName, resName))
            return;

        if (OnLoadAssetInternal(abName, resName, callback, custom))
        {
            LoadAssetInternal(abName, resName, callback, custom);
        }
    }

    public void UnLoadAsset(T res, string abName, string resName)
    {

        if (!CheckAssetBundleAndAssetName(abName, resName))
            return;

        ResUnloadOpreation r = new ResUnloadOpreation();
        r.abName = abName;
        r.resName = resName;
        r.res = res;
        _tempRequestList.Add(r);
    }

    protected string MakeKey(string abName, string resName)
    {
        return string.Format("{0}_{1}", abName, resName);
    }

    private void LoadAssetInternal(string abName, string resName, OnResourceLoadDone callback, object custom)
    {
        var _request = AssetBundleManager.LoadAssetAsync(abName, resName, typeof(T));
        if (_request != null)
        {
            ResLoadOpreation resOpre = new ResLoadOpreation();
            resOpre.abName = abName;
            resOpre.opera = _request;
            resOpre.resName = resName;
            resOpre.callback = callback;
            resOpre.custonData = custom;
            _tempRequestList.Add(resOpre);
        }
    }

    private void OnLoadDone(ResLoadOpreation opre)
    {
        T res = opre.opera.GetAsset<T>();
        OnProcessResLoadedDone(res, opre.abName, opre.resName, opre.custonData, opre.callback);
    }

    private bool CheckAssetBundleAndAssetName(string abName, string resName)
    {
        if (string.IsNullOrEmpty(abName))
        {
            Log.Error("assetbundle name is null.");
            return false;
        }

        if (string.IsNullOrEmpty(resName))
        {
            Log.Error("asset name is null.");
            return false;
        }

        return true;
    }

    private void UpdateRequest()
    {
        if (_tempRequestList.size > 0)
        {
            while (_tempRequestList.size > 0)
            {
                _requestOpreList.Add(_tempRequestList[0]);
                _tempRequestList.RemoveAt(0);
            }
        }
    }


    #region Virtual methods to ovverride

    protected virtual void OnInit(int maxZeroRef)
    {
        _maxZeroRef = maxZeroRef;
    }

    protected virtual bool OnLoadAssetInternal(string abName, string resName, OnResourceLoadDone callback, object custom)
    {
        return true;
    }

    protected virtual void OnProcessResLoadedDone(T res, string abName, string resName, object customData, OnResourceLoadDone callback)
    {
        if (callback != null)
            callback(res, abName, resName, customData);
    }

    protected virtual bool OnProcessUnLoadRes(T res, string abName, string resName)
    {
        var key = MakeKey(abName, resName);
        var r = _zeroRef.GetValue(key);
        if (++r > _maxZeroRef)
            return true;
        else
        {
            _zeroRef[key] = ++r;
            return false;
        }

    }
    #endregion

    private int _maxZeroRef = 0;
    Dictionary<string, int> _zeroRef = new Dictionary<string, int>();

    private BetterList<ResLoadOpreation> _tempRequestList = new BetterList<ResLoadOpreation>();

    private List<ResLoadOpreation> _requestOpreList = new List<ResLoadOpreation>();
}
