#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ABSFramework
{
    public class SimulationSceneLoader : ISceneLoader, IOperator
    {
        public bool IsDone { get; private set; }
        
        public float progress { get; private set; }

        public event OnExecuteCompleted onCompleted;
        public event OnProgressChanged onProgressChanged;

        public void SetParams(string assetBundleName, string sceneName, LoadSceneMode mode)
        {
            _request = SceneManager.LoadSceneAsync(sceneName, mode);
            _request.allowSceneActivation = true;
        }

        public void Execute()
        {
            if (!IsDone)
            {
                if (_request != null)
                {
                    progress = _request.progress;
                    if (onProgressChanged != null)
                        onProgressChanged(progress);

                    if(_request.isDone)
                    {
                        if (onCompleted != null)
                        {
                            onCompleted(this);
                        }
                    }
                    IsDone = _request.isDone;
                }
            }
        }

        private AsyncOperation _request;
    }
}
#endif
