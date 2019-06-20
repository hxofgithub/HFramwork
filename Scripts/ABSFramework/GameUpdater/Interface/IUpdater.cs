using System;
using System.Collections;
using System.Collections.Generic;

public interface IUpdaterElement { }

public interface IMD5Downloader : IEnumerator, IUpdaterElement
{
    void GetFile(string url);
    string text { get; }
    string error { get; }
}

public interface IMD5Parser : IUpdaterElement
{
    string[] Parse(string content);
    bool ConditionChecker(string val);
    event System.Func<string, bool> overrideConditionChecker;
}

public interface IFileDownloader : IEnumerator, IUpdaterElement
{
    void DownloadFiles(Dictionary<string, string> downloadList);
    event Action<string> onDownloadCompleted;
    event Action<float> onProgressChanged;
    string error { get; }
}