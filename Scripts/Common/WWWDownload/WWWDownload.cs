
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class WWWDownload
{
    private static int WWWID = 0;

    public static WWWDownload DownLoad(string url)
    {
        var d = new WWWDownload() { ID = WWWID++ };
        return d.download(url);
    }

    WWWDownload download(string _url)
    {
        _www = new UnityWebRequest(_url) { downloadHandler = new DownloadHandlerBuffer() };
        AsyncOpera = _www.Send();
        return this;
    }

    static public void WriteFile(byte[] datas, string savePath, string fileName)
    {
        //判断保存路径是否存在
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        File.WriteAllBytes(Path.Combine(savePath, fileName), datas);
    }

    public int ID { get; private set; }

    public AsyncOperation AsyncOpera { get; private set; }
    public float downloadProgress
    {
        get { return _www.downloadProgress; }
    }
    private UnityWebRequest _www;
}
