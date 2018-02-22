using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class UIHelperEditor : MonoBehaviour
{
    [MenuItem("Tools/UI/Make Pos Perfect of All UI's")]
    public static void MakePosPerfectOfAll()
    {
        EditorUtility.DisplayProgressBar("Make Pos Perfect", "Excuting...", 0);
        var objs = Resources.LoadAll("Prefabs/UI");
        for (int i = 0; i < objs.Length; i++)
        {
            EditorUtility.DisplayProgressBar("Make Pos Perfect", "Excuting...", (i + 1) * 1.0f / objs.Length);

            GameObject g = (objs[i] as GameObject);
            if (g != null)
                MakePosPerfect(g);
        }

        EditorUtility.ClearProgressBar();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/UI/Remove Null Script")]
    public static void RemoveNullScripts()
    {
        EditorUtility.DisplayProgressBar("Remove Null Script", "Excuting...", 0);
        var objs = Resources.LoadAll("Prefabs/UI");
        for (int i = 0; i < objs.Length; i++)
        {
            EditorUtility.DisplayProgressBar("Remove Null Script", "Excuting...", (i + 1) * 1.0f / objs.Length);
            GameObject g = (objs[i] as GameObject);
            if (g != null)
            {
                var list = g.GetComponentsInChildren<Component>(true);

                for (int m = 0; m < list.Length; m++)
                {
                    if (list[m] == null)
                    {
                        Debug.LogFormat("Find null script at:{0}", g.name);
                    }
                }
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void MakePosPerfect(GameObject g)
    {

        Transform[] trans = g.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < trans.Length; i++)
        {
            if (trans[i] is RectTransform)
            {
                RectTransform rectTransform = trans[i] as RectTransform;
                Vector2 pos = rectTransform.anchoredPosition;
                pos.x = Mathf.Round(pos.x);
                pos.y = Mathf.Round(pos.y);
                rectTransform.anchoredPosition = pos;
            }
            else
            {
                Vector3 pos = trans[i].localPosition;
                pos.z = Mathf.Round(pos.z);
                pos.x = Mathf.Round(pos.x);
                pos.y = Mathf.Round(pos.y);
                trans[i].localPosition = pos;
            }
        }
        EditorUtility.SetDirty(g);
    }

    [MenuItem("Tools/UI/Remove unuse raycast tag")]
    public static void RemoveUnUseRaycatTag()
    {
        string ignoreName = "Mask";

        EditorUtility.DisplayProgressBar("Remove unuse raycast tag", "Excuting...", 0);
        var objs = Resources.LoadAll("Prefabs/UI");
        for (int i = 0; i < objs.Length; i++)
        {
            EditorUtility.DisplayProgressBar("Remove unuse raycast tag", "Excuting...", (i + 1) * 1.0f / objs.Length);
            GameObject g = (objs[i] as GameObject);
            if (g != null)
            {
                var trans = g.GetComponentsInChildren<Transform>(true);
                for (int m = 0; m < trans.Length; m++)
                {
                    var maskGraphic = trans[m].GetComponent<MaskableGraphic>();
                    if (maskGraphic != null)
                    {
                        maskGraphic.raycastTarget = maskGraphic.GetComponent<IEventSystemHandler>() != null || trans[m].name.Contains(ignoreName);
                        EditorUtility.SetDirty(maskGraphic);
                    }
                }
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
}
