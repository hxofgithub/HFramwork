using UnityEngine;
using System.Collections;

using ABSFramework.Systems;

public class LevelTest : MonoBehaviour
{

    public string bundleName;
    public string sceneName;

    void Start()
    {
        AppFacade.Init();
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(30, 50, 200, 60), "Load Level"))
        {
            LevelManager.Instance.LoadLevel(bundleName, sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }


}
