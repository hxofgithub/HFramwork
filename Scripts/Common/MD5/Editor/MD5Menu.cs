#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using HFramework;

public class MD5Menu
{
    [MenuItem("Tools/Make Md5 for AssetBundles")]
    public static void GenMD5Hash()
    {
        try
        {
            DirectoryInfo d = new DirectoryInfo(Application.dataPath + "/AssetBundles/" + HFramework.AssetBundleUtils.GetPlatformName());
            Dictionary<string,string> _md5Dict = new Dictionary<string, string>();

            EditorUtility.DisplayProgressBar("Make MD5, Hold on!", "", 0);
            GetMD5(d, _md5Dict);

            EditorUtility.DisplayProgressBar("Write MD5 File, Hold on!", "", 1);
            StringBuilder sb = new StringBuilder();

            //sb.AppendLine(string.Format("size = {0}", _md5Dict.Count));
            foreach (var item in _md5Dict)
                sb.AppendLine(string.Format("{0} = {1}", item.Key, item.Value));

            CreateMD5File(sb.ToString());
            AssetDatabase.Refresh();
        }
        catch (System.Exception ex)
        {
            Log.Error("{0}\r\n{1}", ex.StackTrace, ex.Message);
        }

        EditorUtility.ClearProgressBar();


    }

    static void GetMD5(DirectoryInfo info, Dictionary<string,string> dict)
    {
        if (!info.Exists)
            return;
        var files = info.GetFileSystemInfos();

        foreach (var item in files)
        {
            EditorUtility.DisplayProgressBar("Make MD5, Hold on!", item.FullName, 1);
            if (item is DirectoryInfo)
                GetMD5(item as DirectoryInfo, dict);
            else
            {
                string rootPath = Application.dataPath + "/";
                string key = item.FullName.Replace("\\", "/").Replace(rootPath, string.Empty);
                string error = string.Empty;
                dict[key] = MD5Utils.GetFileHash(item.FullName, out error);
                if (!string.IsNullOrEmpty(error))
                    Log.Error(error);
            }
        }
    }

    static void CreateMD5File(string content)
    {
        string dire = Path.GetDirectoryName(FileName);
        if (!Directory.Exists(dire))
            Directory.CreateDirectory(dire);

        FileStream fs = new FileStream(FileName,FileMode.OpenOrCreate);

//        if (!File.Exists(FileName))
//            fs = new FileStream(FileName,FileMode.OpenOrCreate);
//        else
//            fs = new FileStream(FileName, FileMode.Open);
        var data = Encoding.UTF8.GetBytes(content);
        fs.Write(data, 0, data.Length);
        fs.Close();
        fs.Dispose();
    }

    public static string FileName
    {
        get{ return HFramework.AssetBundleUtils.GetDocumentPath() + MD5Utils.MD5FileName; }
    }

}
#endif