
using UnityEngine;
using ABSFramework.Systems;

public class AppFacade : MonoBehaviour
{
    public static bool Initialize { get; private set; }
    public static void Init()
    {
        var g = GameObject.Find("AppFacde");
        if (g == null)
            g = new GameObject("AppFacde", typeof(AppFacade));
    }

    void Awake()
    {
        //Makes the object target not be destroyed automatically when loading a new scene.
        DontDestroyOnLoad(gameObject);


        _systems = new SystemManager()
                .AddSystem(new AssetBundleManager())
                .AddSystem(new LevelManager())
                .AddSystem(new PrefabManager());

        ABSFramework.Debug.Log("AppFacade Init over.");
    }

    void Start()
    {
        _systems.Initailize();
        Initialize = true;
    }

    void Update()
    {
        _systems.Execute();
    }

    void OnDestroy()
    {
        _systems.Cleanup();
    }

    private SystemManager _systems;
}
