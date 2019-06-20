using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ABSFramework.Systems
{
    public sealed class AssetBundleManager : IExecuteSystem, IInitailizeSystem
    {
        internal class AssetBundleRef
        {
            public AssetBundle assetBundle;
            public int refCnt;
        }
                
        #region public static methods

        public static string BaseDownloadingURL
        {
            get { return m_BaseDownloadingURL; }
            private set { m_BaseDownloadingURL = value; }
        }

        public static string[] ActiveVariants
        {
            get { return m_ActiveVariants; }
            set { m_ActiveVariants = value; }
        }

        public static void LoadAssetBundle(string bundleName)
        {
#if UNITY_EDITOR
            if (AssetBundleUtils.SimulationModeInEditor)
                return;
#endif

            if (m_AssetBundleManifest == null)
            {
                Debug.LogError("AssetBundleManagerError: AssetBundleManifest is null.");
            }
            else
            {
                LoadBundleInternal(bundleName);
            }
        }

        public static void UnLoadAssetBundle(string bundleName, bool unloadAllLoadedObjects = true)
        {
#if UNITY_EDITOR
            if (AssetBundleUtils.SimulationModeInEditor)
            {
                return;
            }
#endif
            if (m_AssetBundleManifest == null)
            {
                Debug.LogError("AssetBundleManagerError: AssetBundleManifest is null.");
            }
            else
            {
                UnLoadBundleInternal(bundleName, unloadAllLoadedObjects);
            }
        }

        public static IOperator GetAssetBundleOperator(string bundleName)
        {
            var key = MakePath(bundleName);
            IOperator element;
            m_BundleLoaderDict.TryGetValue(key, out element);
            return element;
        }

        public static void SetSourceAssetBundleDirectory(string relativePath)
        {
            BaseDownloadingURL = Path.Combine(AssetBundleUtils.GetLocalAssetPath(), relativePath);
        }

        #endregion

        #region internal methods
        internal static AssetBundleRef GetAssetBundleRef(string bundleName)
        {
            var key = MakePath(bundleName);
            AssetBundleRef element;
            m_BundleDict.TryGetValue(key, out element);
            return element;
        }

        internal static AssetBundleRef GetAssetBundleRefByBundlePath(string bundlePath)
        {
            AssetBundleRef element;
            m_BundleDict.TryGetValue(bundlePath, out element);
            return element;
        }
        #endregion

        #region private methods

        private static string RemapVariantName(string assetBundleName)
        {
            if (m_BundlesWithVariant == null)
                m_BundlesWithVariant = m_AssetBundleManifest.GetAllAssetBundlesWithVariant();
            else if (m_BundlesWithVariant.Length == 0)
                return assetBundleName;

            // Get base bundle name
            string baseName = assetBundleName.Split('.')[0];

            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < m_BundlesWithVariant.Length; i++)
            {
                string[] curSplit = m_BundlesWithVariant[i].Split('.');
                string curBaseName = curSplit[0];
                string curVariant = curSplit[1];

                if (curBaseName != baseName)
                    continue;

                int found = System.Array.IndexOf(m_ActiveVariants, curVariant);

                // If there is no active variant found. We still want to use the first
                if (found == -1)
                    found = int.MaxValue - 1;

                if (found < bestFit)
                {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }
            
            if (bestFitIndex != -1)
            {
                return m_BundlesWithVariant[bestFitIndex];
            }
            else
            {
                return assetBundleName;
            }
        }

        private static void LoadBundleInternal(string bundleName)
        {
            var bundleNames = GetDependencies(bundleName);
            for (int i = 0; i < bundleNames.Length; i++)
            {
                LoadBundleInternal(bundleNames[i]);
            }

            bundleName = RemapVariantName(bundleName);

            var bundlePath = MakePath(bundleName);
            //it is cached.
            if (m_BundleDict.ContainsKey(bundlePath))
            {
                //Increase refrence number.
                m_BundleDict[bundlePath].refCnt++;
                return;
            }
            string errorMsg;
            if (m_errorMsgDict.TryGetValue(bundlePath, out errorMsg))
            {
                Debug.LogError(errorMsg);
                return;
            }


            //it is loading.
            if (m_BundleInLoading.Contains(bundlePath))
                return;

            m_BundleInLoading.Add(bundlePath);
            IOperator exe = CreateBundleLoader(bundlePath, OnLoadBundleComplete);
            m_BundleLoaderList.Enqueue(exe);
            m_BundleLoaderDict.Add(bundlePath, exe);
        }

        private static void UnLoadBundleInternal(string bundleName, bool unloadAllLoadedObjects)
        {
            var bundleNames = GetDependencies(bundleName);
            for (int i = 0; i < bundleNames.Length; i++)
                UnLoadBundleInternal(bundleNames[i], unloadAllLoadedObjects);

            bundleName = RemapVariantName(bundleName);
            m_BundleLoaderList.Enqueue(new UnLoadBundle(bundleName, OnUnloadBundleComplete));
        }

        private static void LoadManifestInternal()
        {
#if UNITY_EDITOR
            if (AssetBundleUtils.SimulationModeInEditor)
                return;
#endif
            string manifestBundleName = AssetBundleUtils.GetFloderName();
            var path = MakePath(manifestBundleName);
            IOperator exe = CreateBundleLoader(path, OnLoadManifestCompleted);
            m_BundleLoaderList.Enqueue(exe);
        }

        private static void LoadAssetBundleConfig()
        {
            AssetBundleConfig.SetInistance(Resources.Load<AssetBundleConfig>("AssetBundleConfig"));
        }

        private static void OnLoadManifestCompleted(IOperator exe)
        {
            var loader = (IBundleLoader)exe;
            if (loader != null)
            {
                m_AssetBundleManifest = loader.bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                loader.bundle.Unload(false);

                Debug.Log("AssetBundleManifest Load Done.");
            }
        }

        private static void OnLoadBundleComplete(IOperator exe)
        {
            var loader = exe as IBundleLoader;
            if (loader != null)
            {
                if (string.IsNullOrEmpty(loader.errorMsg))
                    m_BundleDict.Add(loader.assetBundlePath, new AssetBundleRef() { assetBundle = loader.bundle, refCnt = 1 });
                else
                    m_errorMsgDict.Add(loader.assetBundlePath, loader.errorMsg);

                m_BundleInLoading.Remove(loader.assetBundlePath);
                m_BundleLoaderDict.Remove(loader.assetBundlePath);
            }
        }
        
        private static void OnUnloadBundleComplete(IOperator exe)
        {
            var loader = exe as IBundleUnloader;
            if (loader != null)
            {
                var key = MakePath(loader.assetBundleName);
                if (m_BundleDict.ContainsKey(key))
                {
                    var bundleRef = m_BundleDict[key];
                    if (--bundleRef.refCnt <= 0)
                    {
                        Debug.LogFormat("Unload AssetBundle {0} {1}", loader.assetBundleName, loader.unloadAllLoadedObjects);
                        bundleRef.assetBundle.Unload(loader.unloadAllLoadedObjects);
                        m_BundleDict.Remove(key);
                    }
                }
            }
        }
        
        private static string[] GetDependencies(string bundleName)
        {
            string[] element;
            if (!m_Dependencies.TryGetValue(bundleName, out element))
            {
                element = m_AssetBundleManifest.GetAllDependencies(bundleName);
                m_Dependencies.Add(bundleName, element);
            }
            return element;
        }

        private static IOperator CreateBundleLoader(string bundleName, OnExecuteCompleted onCompleted)
        {
            IOperator exe = OperatorFactory.CreateBundleLoader();
            (exe as IBundleLoader).SetLoadParams(bundleName, onCompleted);
            return exe;
        }

        private static string MakePath(string assetBundleName)
        {
            return Path.Combine(BaseDownloadingURL, assetBundleName);
        }

        void IExecuteSystem.Execute()
        {
            if (m_BundleLoaderList.Count > 0)
            {
                var exe = m_BundleLoaderList.Peek();
                exe.Execute();
                if (exe.IsDone)
                {
                    m_BundleLoaderList.Dequeue();                    
                }
            }
        }

        void IInitailizeSystem.Initailize()
        {
#if UNITY_EDITOR
            Debug.LogFormat("Simulation mode : {0}", AssetBundleUtils.SimulationModeInEditor ? "Enable" : "Disable");
#endif

            m_errorMsgDict = new Dictionary<string, string>();
            m_BundleDict = new Dictionary<string, AssetBundleRef>();
            m_BundleLoaderList = new Queue<IOperator>();
            m_BundleInLoading = new HashSet<string>();
            m_Dependencies = new Dictionary<string, string[]>();
            m_BundleLoaderDict = new Dictionary<string, IOperator>();
            m_ActiveVariants = new string[] { };
            SetSourceAssetBundleDirectory(AssetBundleUtils.GetFloderName());
            LoadAssetBundleConfig();
            LoadManifestInternal();
        }

        #endregion
        
        #region Properties
        private static AssetBundleManifest m_AssetBundleManifest;
        private static Dictionary<string, AssetBundleRef> m_BundleDict;
        private static Dictionary<string, string[]> m_Dependencies;
        private static Dictionary<string, IOperator> m_BundleLoaderDict;
        private static Dictionary<string, string> m_errorMsgDict;
        private static Queue<IOperator> m_BundleLoaderList;

        private static HashSet<string> m_BundleInLoading;

        private static string m_BaseDownloadingURL;
        private static string[] m_ActiveVariants;
        private static string[] m_BundlesWithVariant;
        #endregion
    }
}
