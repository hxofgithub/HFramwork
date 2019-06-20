using System;

namespace ABSFramework.Systems
{
    internal class OperatorFactory
    {
        public static IOperator CreateBundleLoader()
        {
            switch (AssetBundleConfig.Mode)
            {
                case AssetBundleConfig.LoadMode.Sync:
                    return new BundleLoaderSync_FromFile();
                case AssetBundleConfig.LoadMode.Async:
                    return new BundleLoaderAsync_FromFile();
            }
            return null;
        }
        public static IOperator CreateAssetLoader()
        {
            switch (AssetBundleConfig.Mode)
            {
                case AssetBundleConfig.LoadMode.Sync:
                    return new LoadAssetFromAssetBundle();
                case AssetBundleConfig.LoadMode.Async:
                    return new LoadAssetFromAssetBundle_Async();
            }
            return null;
        }
        public static IOperator CreateSceneLoader()
        {
            switch (AssetBundleConfig.Mode)
            {
                case AssetBundleConfig.LoadMode.Sync:
                    return new LoadSceneFromAssetBundle(); 
                case AssetBundleConfig.LoadMode.Async:
                    return new LoadSceneFromAssetBundle_Async();
            }
            return null;
        }

        public static IOperator CreateOperator(string assemblyName, string typeName)
        {
            if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(typeName))
                return null;
            var o = Activator.CreateInstance(assemblyName, typeName);
            return o as IOperator;
        }
    }
}