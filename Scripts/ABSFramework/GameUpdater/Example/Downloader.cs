using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class DownloadMD5 : IMD5Downloader, IDisposable
{
    public string error
    {
        get
        {
            return w.error;
        }
    }

    public string text
    {
        get
        {
            return w.text;
        }
    }

    public void GetFile(string url)
    {
        w = new WWW(url);
    }

    public object Current
    {
        get
        {
            return null;
        }
    }
    public bool MoveNext()
    {
        return !w.isDone;
    }

    public void Reset()
    {

    }

    public void Dispose()
    {
        if (w != null)
        {
            w.Dispose();
            w = null;
        }
    }

    private WWW w;
}

public class MD5Parser : IMD5Parser
{
    public event System.Func<string, bool> overrideConditionChecker;
    public string[] Parse(string content)
    {
        var lines = content.Replace("\r", string.Empty).Split('\n');
        return lines;
    }

    public bool ConditionChecker(string val)
    {
        if (overrideConditionChecker != null)
            return overrideConditionChecker(val);
        return true;
    }
}

public class FileDownloader : IFileDownloader
{
    public FileDownloader()
    {
        _client = new WebClient();
        _client.DownloadFileCompleted += onDownloadFileCompleted;
        _client.DownloadProgressChanged += onDownloadProgressChanged;
    }

    private void onDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        if (onProgressChanged != null)
        {
            var progress = (_totalCnt - _downloadList.Count + e.ProgressPercentage / 100f) * _prePercent;
            onProgressChanged(progress);
        }
    }

    private void onDownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        if (e.Error != null)
        {
            error = e.Error.ToString();
            _currentKey = null;
        }
        else
        {
            var name = _downloadList[_currentKey];
            Debug.LogFormat("Download {0} done!", name);
            if (onDownloadCompleted != null)
                onDownloadCompleted(name);
            _downloadList.Remove(_currentKey);

            StartWWW();
        }

    }

    private void StartWWW()
    {
        if (_downloadList.Count > 0)
        {
            foreach (var element in _downloadList)
            {
                _currentKey = element.Key;
                _client.DownloadFileAsync(new Uri(_currentKey), element.Value);
                break;
            }
        }
        else
        {
            _client.Dispose();
            _client = null;
            _currentKey = null;
        }
    }

    public void DownloadFiles(Dictionary<string, string> downloadList)
    {
        _totalCnt = downloadList.Count;
        if (_totalCnt > 0)
            _prePercent = 1f / _totalCnt;
        _downloadList = downloadList;
        StartWWW();
    }

    private void WriteFile(byte[] bytes, string fileName)
    {
        fileName = Path.GetFileName(fileName);
        string path = Application.streamingAssetsPath + "/Windows/";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        File.WriteAllBytes(path + fileName, bytes);
    }

    public bool MoveNext()
    {
        return !string.IsNullOrEmpty(_currentKey);
    }

    public void Reset() { }

    public void Dispose()
    {
        if (_client != null)
        {
            _client.Dispose();
            _client = null;
        }
    }

    public string error { get; private set; }

    private Dictionary<string, string> _downloadList;
    private WebClient _client = new WebClient();

    public object Current { get { return null; } }
    public event Action<string> onDownloadCompleted;
    public event Action<float> onProgressChanged;

    private string _currentKey;
    private int _totalCnt = 0;
    private float _prePercent = 0;
}

