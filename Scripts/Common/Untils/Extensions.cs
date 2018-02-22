using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static readonly Vector3 HIDEPOS = Vector3.one * int.MaxValue;

    /// <summary>
    /// Hide the GameObject. Instead of SetActive(false).
    /// </summary>
    /// <param name="go">Go.</param>
    public static void Hide(this MonoBehaviour mono)
    {
        mono.transform.position = HIDEPOS;
    }

    /// <summary>
    /// Resume the GameObject. Instead of SetActive(true).
    /// </summary>
    public static void Resume(this MonoBehaviour mono,Vector3 originPos = default(Vector3))
    {
        mono.transform.localPosition = originPos;
    }
}
