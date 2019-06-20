using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
namespace ABSFramework.Systems
{
    public class LevelManager : IExecuteSystem, IInitailizeSystem
    {
        public static LevelManager Instance { get; private set; }

        public void LoadLevel(string assetbundleName, string levelName, LoadSceneMode m = LoadSceneMode.Single)
        {
            AssetBundleManager.LoadAssetBundle(assetbundleName);
            _queue.Enqueue(CreateSceneLoader(assetbundleName, levelName, m));
        }

        public void Execute()
        {
            if (_queue.Count > 0)
            {
                var opera = _queue.Peek();
                opera.Execute();
                if (opera.IsDone)
                {
                    _queue.Dequeue();
                }
            }
        }

        IOperator CreateSceneLoader(string assetbundleName, string levelName, LoadSceneMode m)
        {
            ISceneLoader oper;
#if UNITY_EDITOR
            if (AssetBundleUtils.SimulationModeInEditor)
                oper = new SimulationSceneLoader();
            else
#endif
                oper = OperatorFactory.CreateSceneLoader() as ISceneLoader;
            oper.SetParams(assetbundleName, levelName, m);
            return oper as IOperator;
        }

        void IInitailizeSystem.Initailize()
        {
            Instance = this;
        }

        Queue<IOperator> _queue = new Queue<IOperator>();

    }

}
