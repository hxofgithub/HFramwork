
using ABSFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameUpdater : MonoBehaviour
{
    /// <summary>
    /// Set md5 file downloader, md5Parser and file downloader
    /// </summary>
    /// <param name="md5Downloader"></param>
    /// <param name="parser"></param>
    /// <param name="fileDownloader"></param>
    public void SetGetUpdateFile(IMD5Downloader md5Downloader, IMD5Parser parser, IFileDownloader fileDownloader)
    {
        m_md5Downloader = md5Downloader;
        m_md5Parser = parser;
        m_fileDownloader = fileDownloader;
    }

    /// <summary>
    /// Begin
    /// </summary>
    /// <param name="md5URL"></param>
    /// <param name="onDownloadCompleted"></param>
    public void BeginCheck(string md5URL, System.Action onDownloadCompleted)
    {
        if (!m_IsDownloading)
            StartCoroutine(CheckUpdate(md5URL, onDownloadCompleted));
        else
            ABSFramework.Debug.LogError("Dumplicate updating.");
    }

    private IEnumerator CheckUpdate(string url, System.Action onDownloadCompleted)
    {
        m_IsDownloading = true;
        Dictionary<string, string> updateList = new Dictionary<string, string>();
        m_md5Downloader.GetFile(url);
        yield return m_md5Downloader;
        if (string.IsNullOrEmpty(m_md5Downloader.error))
        {
            string serverURL = string.Empty;
            var originFileList = m_md5Parser.Parse(m_md5Downloader.text);            
            if (originFileList.Length > 1)
                serverURL = originFileList[0];
            DisposeElement(m_md5Downloader);
            

            for (int i = 1; i < originFileList.Length; i++)
            {
                var item = originFileList[i];
                if (m_md5Parser.ConditionChecker(item))
                {
                    var localURL = Path.Combine(AssetBundleUtils.GetLocalAssetPath(), AssetBundleUtils.GetFloderName());
                    updateList.Add(serverURL + item, Path.Combine(localURL, item));
                }                    
            }
            DisposeElement(m_md5Parser);


            m_fileDownloader.DownloadFiles(updateList);
            yield return m_fileDownloader;
            if (string.IsNullOrEmpty(m_fileDownloader.error))
            {
                m_IsDownloading = true;
                if (onDownloadCompleted != null)
                    onDownloadCompleted();
            }
            else
                ABSFramework.Debug.LogError(m_fileDownloader.error);
            DisposeElement(m_fileDownloader);
        }
        else
        {
            ABSFramework.Debug.LogError(m_md5Downloader.error);
        }
    }

    private void DisposeElement(IUpdaterElement element)
    {
        var dis = element as IDisposable;
        if (dis != null)
            dis.Dispose();
    }

    
    private IMD5Downloader m_md5Downloader;
    private IMD5Parser m_md5Parser;
    private IFileDownloader m_fileDownloader;
    private bool m_IsDownloading;

}