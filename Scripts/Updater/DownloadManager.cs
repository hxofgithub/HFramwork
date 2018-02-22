
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadManager : MonoBehaviour
{
    private class DownloadDetail
    {
        public string url;
        public string savePath;
        public string fileName;
        public Action callback;
        public Action<float> downloadingCallback;
    }

    public static void DownloadFromServer(string _url, string _savaPath, string _fileName, Action _callback, Action<float> _downloadingCallback)
    {
        lock (lockObject)
        {
            _downList.Add(new DownloadDetail() { url = _url, savePath = _savaPath, fileName = _fileName, callback = _callback, downloadingCallback = _downloadingCallback });
        }

    }

    IEnumerator Start()
    {
        DownloadDetail currentLoad = null;
        HttpDownLoad httpDownLoad = new HttpDownLoad();
        while (true)
        {
            if (currentLoad != null)
            {
                if (currentLoad.url.StartsWith("https"))
                {
                    ///请求https不会返回下载的总大小(在gitee上测试的)
                    ///因此不返回下载进度
                    if (currentLoad.downloadingCallback != null)
                        currentLoad.downloadingCallback(1);
                    UnityWebRequest unityWebRequest = new UnityWebRequest(currentLoad.url) { downloadHandler = new DownloadHandlerBuffer() };
                    yield return unityWebRequest.Send();

                    if (currentLoad.downloadingCallback != null)
                        currentLoad.downloadingCallback(1);

                    System.IO.File.WriteAllBytes(string.Format("{0}/{1}", currentLoad.savePath, currentLoad.fileName), unityWebRequest.downloadHandler.data);
                    unityWebRequest.downloadHandler.Dispose();

                    if (currentLoad.callback != null)
                        currentLoad.callback();

                    currentLoad = null;
                }
                else
                {
                    httpDownLoad.DownLoad(currentLoad.url, currentLoad.savePath, currentLoad.fileName, currentLoad.callback);
                    while (!httpDownLoad.isDone)
                    {
                        if (currentLoad.downloadingCallback != null)
                            currentLoad.downloadingCallback(httpDownLoad.progress);
                        yield return GameData.EndOfFrame;
                    }

                    if (currentLoad.callback != null)
                        currentLoad.callback();

                    currentLoad = null;
                }
            }
            else
            {
                lock (lockObject)
                {
                    if (_downList.Count > 0)
                    {
                        currentLoad = _downList[0];
                        _downList.RemoveAt(0);
                    }
                }
            }
            yield return GameData.EndOfFrame;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private static object lockObject = new object();
    private static List<DownloadDetail> _downList = new List<DownloadDetail>();
}
