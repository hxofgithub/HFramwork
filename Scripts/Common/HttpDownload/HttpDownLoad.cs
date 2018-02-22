using UnityEngine;
using System.Threading;
using System.IO;
using System.Net;
using System;

/// <summary>
/// 通过http下载资源
/// </summary>
public class HttpDownLoad
{
    //下载进度
    public float progress { get; private set; }

    //下载速度
    public uint speed { get; private set; }
    private DateTime lastRecordTime;
    private long lastLength = 0;

    //涉及子线程要注意,Unity关闭的时候子线程不会关闭，所以要有一个标识
    private bool isStop;
    //子线程负责下载，否则会阻塞主线程，Unity界面会卡主
    private Thread thread;
    //表示下载是否完成
    public bool isDone { get; private set; }
    const int ReadWriteTimeOut = 2 * 1000;//超时等待时间
    const int TimeOutWait = 5 * 1000;//超时等待时间

    /// <summary>
    /// 下载方法(断点续传)
    /// </summary>
    /// <param name="url">URL下载地址</param>
    /// <param name="savePath">Save path保存路径</param>
    /// <param name="callBack">Call back回调函数</param>
    public void DownLoad(string url, string savePath, string fileName, Action callBack,
        System.Threading.ThreadPriority threadPriority = System.Threading.ThreadPriority.Normal)
    {
        isStop = false;
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

        speed = 0;
        //开启子线程下载,使用匿名方法
        thread = new Thread(delegate ()
        {
            lastRecordTime = DateTime.Now;
            stopWatch.Start();
            //判断保存路径是否存在
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            //这是要下载的文件名，比如从服务器下载a.zip到D盘，保存的文件名是test
            string filePath = savePath + "/" + fileName;

            //使用流操作文件
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            //获取文件现在的长度
            long fileLength = fs.Length;
            //获取下载文件的总长度
            Log.Info(url + " " + fileName);
            long totalLength = GetLength(url);
            Log.Info("<color=red>文件:{0} 已下载{1}M，剩余{2}M</color>", fileName, fileLength / 1024 / 1024, (totalLength - fileLength) / 1024 / 1024);

            //如果没下载完
            if (fileLength < totalLength)
            {
                //断点续传核心，设置本地文件流的起始位置
                fs.Seek(fileLength, SeekOrigin.Begin);
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.ReadWriteTimeout = ReadWriteTimeOut;
                request.Timeout = TimeOutWait;
                //断点续传核心，设置远程访问文件流的起始位置
                request.AddRange((int)fileLength);
                Stream stream = request.GetResponse().GetResponseStream();
                byte[] buffer = new byte[1024];
                //使用流读取内容到buffer中
                //注意方法返回值代表读取的实际长度,并不是buffer有多大，stream就会读进去多少
                int length = stream.Read(buffer, 0, buffer.Length);
                //Debug.LogFormat("<color=red>length:{0}</color>" + length);
                while (length > 0)
                {
                    //如果Unity客户端关闭，停止下载
                    if (isStop) break;
                    //将内容再写入本地文件中
                    fs.Write(buffer, 0, length);
                    //计算进度
                    fileLength += length;
                    progress = fileLength / (float)totalLength;
                    //UnityEngine.Debug.Log(progress);
                    //类似尾递归
                    length = stream.Read(buffer, 0, buffer.Length);

                    if ((DateTime.Now - lastRecordTime).TotalMilliseconds >= 500)
                    {
                        speed = (uint)((fileLength - lastLength));
                        lastRecordTime = DateTime.Now;
                        lastLength = fileLength;
                        lastRecordTime = DateTime.Now;

                    }
                    Log.Warning("Speed" + speed);
                    Log.Info("Progress : " + progress);
                }
                stream.Close();
                stream.Dispose();
            }
            else
            {
                progress = 1;
            }
            stopWatch.Stop();
            Debug.Log("耗时: " + stopWatch.ElapsedMilliseconds / 1000.0f);
            fs.Close();
            fs.Dispose();
            //如果下载完毕，执行回调
            if (progress == 1)
            {
                isDone = true;
                if (callBack != null) callBack();
                thread.Abort();
            }
            Log.Info("download finished");
        });
        //开启子线程
        thread.IsBackground = true;
        thread.Priority = threadPriority;
        thread.Start();
    }


    /// <summary>
    /// 获取下载文件的大小
    /// </summary>
    /// <returns>The length.</returns>
    /// <param name="url">URL.</param>
    long GetLength(string url)
    {
        Log.Info(url);

        HttpWebRequest requet = WebRequest.Create(url) as HttpWebRequest;
        requet.Method = "HEAD";
        HttpWebResponse response = requet.GetResponse() as HttpWebResponse;
        return response.ContentLength;
    }

    public void Close()
    {
        isStop = true;
    }
}
