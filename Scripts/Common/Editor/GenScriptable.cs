#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.IO;

namespace HFramwork
{
    public class GenScriptable
    {
        [MenuItem("Assets/Try Generate Scriptable")]
        static void CreateScriptable()
        {
            var obj = Selection.activeObject;
            var s = ScriptableObject.CreateInstance(obj.name);
            if (s != null)
            {
                AssetDatabase.CreateAsset(s, Path.GetDirectoryName(AssetDatabase.GetAssetPath(obj)) + "/temp.asset");
            }
        }

        [MenuItem("Assets/Try Generate Scriptable", true)]
        static bool CreateScriptableValidFunc()
        {
            return Selection.activeObject is TextAsset;
        }

    }
}
#endif