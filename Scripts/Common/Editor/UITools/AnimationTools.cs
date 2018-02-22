using UnityEngine;
using UnityEditor;
using System.IO;
namespace HFramework
{
    public class AnimationTools
    {

        #region Copy
        [MenuItem("Assets/AnimationClip/Copy Animation From Selection")]
        static void CopyAnimation()
        {
            var animationArray = Selection.GetFiltered<AnimationClip>(SelectionMode.Unfiltered);
            for (int m = 0; m < animationArray.Length; m++)
            {
                var newClip = new AnimationClip();
                EditorUtility.CopySerialized(animationArray[m], newClip);
                AssetDatabase.CreateAsset(newClip, string.Format("{0}/{1}_new.anim",
                    Path.GetDirectoryName(AssetDatabase.GetAssetPath(animationArray[m])), animationArray[m].name));
            }
        }

        [MenuItem("Assets/AnimationClip/Copy Animation From Selection", true)]
        static bool CopyAnimationValidFunc()
        {
            return Selection.GetFiltered<AnimationClip>(SelectionMode.Unfiltered).Length != 0;
        }
        #endregion

        #region Serialize
        
        [MenuItem("Assets/AnimationClip/Serialize")]
        static void SerializeAnimation()
        {
            EditorUtility.DisplayProgressBar("Compressing", "Checking...", 0);
            try
            {
                var objects = new Object[Selection.objects.Length];
                System.Array.Copy(Selection.objects, objects, objects.Length);
                for (int i = 0; i < objects.Length; i++)
                {
                    var clip = objects[i] as AnimationClip;
                    var newClip = SerializeAnim(clip);
                    var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(clip)) + "/" + clip.name + ".anim";
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.CreateAsset(newClip, path);

                }
                AssetDatabase.Refresh();
            }
            catch (System.Exception)
            {
                EditorUtility.ClearProgressBar();
                throw;
            }

            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Assets/AnimationClip/Serialize", true)]
        static bool SerializeAnimationValidFunc()
        {
            var objects = Selection.objects;
            for (int i = 0; i < objects.Length; i++)
            {
                if (!(objects[i] is AnimationClip))
                    return false;
            }
            return true;
        }



        static AnimationClip SerializeAnim(AnimationClip clip)
        {
            try
            {
                var newAnimationClip = new AnimationClip();
                EditorUtility.CopySerialized(clip, newAnimationClip);
                newAnimationClip.ClearCurves();
                var bindings = AnimationUtility.GetCurveBindings(clip);
                for (int m = 0; m < bindings.Length; m++)
                {
                    var binding = bindings[m];

                    EditorUtility.DisplayProgressBar("Compressing", "Compress " + binding.path, (m + 1) / bindings.Length);
                    var newCurve = new AnimationCurve(AnimationUtility.GetEditorCurve(clip, binding).keys);
                    var keys = newCurve.keys;
                    if (keys == null)
                        continue;
                    for (int k = keys.Length - 1; k >= 0; k--)
                    {
                        newCurve.RemoveKey(k);
                        keys[k].time = float.Parse(keys[k].time.ToString("f3"));
                        keys[k].value = float.Parse(keys[k].value.ToString("f3"));
                        keys[k].inTangent = float.Parse(keys[k].inTangent.ToString("f3"));
                        keys[k].outTangent = float.Parse(keys[k].outTangent.ToString("f3"));
                    }
                    newCurve.keys = keys;
                    newAnimationClip.SetCurve(binding.path, binding.type, binding.propertyName, newCurve);
                }
                return newAnimationClip;
            }
            catch (System.Exception)
            {
                EditorUtility.ClearProgressBar();
                throw;
            }
        }

        #endregion
    }
}
