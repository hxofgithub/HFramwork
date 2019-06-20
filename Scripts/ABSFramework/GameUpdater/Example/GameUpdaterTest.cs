using UnityEngine;

public class GameUpdaterTest : MonoBehaviour
{
    public GameUpdater gameUpdater;
    void Start()
    {
        IFileDownloader fileDownloader = new FileDownloader();
        fileDownloader.onProgressChanged += (p) => 
        {
            ABSFramework.Debug.LogFormat("File downloaing , progress : {0:N2}", p);
        };

        //Set md5-downloader, md5-parser, and file downloader.
        gameUpdater.SetGetUpdateFile(new DownloadMD5(), new MD5Parser(), fileDownloader);

        //Load template server url.
        var textAsset = Resources.Load<TextAsset>("AssetBundleServerURL");        
        var resServerURL = textAsset.text;

        //Start
        gameUpdater.BeginCheck(resServerURL + "files.txt", ()=> 
        {
            AppFacade.Init();
        });

    }
}
