//-----------------------------------------------------------------------
// <copyright file="AssetBundleManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    #if UNITY_EDITOR
    using UnityEditor;
    #endif
    using UnityEngine;

    /// <summary>
    /// The AssetBundle Manager provides a High-Level API for working with AssetBundles.
    /// It will take care of loading dependent asset bundles, including their variants.
    /// </summary>
    public class AssetBundleManager : MonoBehaviour
    {
        #if UNITY_EDITOR
        private const string SimulateAssetBundles = "SimulateAssetBundles";
        private static int simulateAssetBundleInEditor = -1;
        #endif

        private static LogMode logMode = LogMode.All;
        private static string baseDownloadingURL = string.Empty;
        private static string[] activeVariants = new string[0];
        private static AssetBundleManifest assetBundleManifest = null;

        private static Dictionary<string, LoadedAssetBundle> loadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
        private static Dictionary<string, string> downloadingErrors = new Dictionary<string, string>();
        private static List<string> downloadingBundles = new List<string>();
        private static List<AssetBundleLoadOperation> inProgressOperations = new List<AssetBundleLoadOperation>();
        private static Dictionary<string, string[]> dependencies = new Dictionary<string, string[]>();

        public delegate string OverrideBaseDownloadingURLDelegate(string bundleName);

        // Implements per-bundle base downloading URL override. The subscribers must return null values for unknown bundle names;
        public static event OverrideBaseDownloadingURLDelegate OverrideBaseDownloadingURL;

        public enum LogMode
        {
            All,
            JustErrors
        }

        public enum LogType
        {
            Info,
            Warning,
            Error
        }

        public static LogMode LoggingMode
        {
            get { return logMode; }
            set { logMode = value; }
        }

        // The base downloading url which is used to generate the full downloading url with the assetBundle names.
        public static string BaseDownloadingURL
        {
            get { return baseDownloadingURL; }
            set { baseDownloadingURL = value; }
        }

        // Variants which is used to define the active variants.
        public static string[] ActiveVariants
        {
            get { return activeVariants; }
            set { activeVariants = value; }
        }

        // AssetBundleManifest object which can be used to load the
        // dependencies and check suitable assetBundle variants.
        public static AssetBundleManifest AssetBundleManifestObject
        {
            set { assetBundleManifest = value; }
        }

        #if UNITY_EDITOR
        // Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
        public static bool SimulateAssetBundleInEditor
        {
            get
            {
                if (simulateAssetBundleInEditor == -1)
                {
                    simulateAssetBundleInEditor = EditorPrefs.GetBool(SimulateAssetBundles, true) ? 1 : 0;
                }

                return simulateAssetBundleInEditor != 0;
            }

            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != simulateAssetBundleInEditor)
                {
                    simulateAssetBundleInEditor = newValue;
                    EditorPrefs.SetBool(SimulateAssetBundles, value);
                }
            }
        }
        #endif

        // sets the source asset bundles to point to a web url
        public static void SetSourceToWeb(string webUrl)
        {
            SetSourceAssetBundleURL(webUrl);
        }

        // sets the source asset bundles to point to the streaming assets folder
        public static void SetSourceToStreamingAssets()
        {
            if (Application.isEditor)
            {
                SetSourceAssetBundleURL("file://" + Path.Combine(Application.streamingAssetsPath, AssetBundleUtility.AssetBundlesFolderName));
            }
            else
            {
                SetSourceAssetBundleURL(Platform.GetStreamingAssetsURL(AssetBundleUtility.AssetBundlesFolderName));
            }
        }

        // Retrieves an asset bundle that has previously been requested via LoadAssetBundle.
        // Returns null if the asset bundle or one of its dependencies have not been downloaded yet.
        public static LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName, out string error)
        {
            if (downloadingErrors.TryGetValue(assetBundleName, out error))
            {
                return null;
            }

            LoadedAssetBundle bundle = null;
            loadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle == null)
            {
                return null;
            }

            // No dependencies are recorded, only the bundle itself is required.
            string[] dependencies = null;
            if (AssetBundleManager.dependencies.TryGetValue(assetBundleName, out dependencies) == false)
            {
                return bundle;
            }

            // Make sure all dependencies are loaded
            foreach (var dependency in dependencies)
            {
                if (downloadingErrors.TryGetValue(assetBundleName, out error))
                {
                    return bundle;
                }

                // Wait all the dependent assetBundles being loaded.
                LoadedAssetBundle dependentBundle;
                loadedAssetBundles.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null)
                {
                    return null;
                }
            }

            return bundle;
        }

        // Returns true if certain asset bundle has been downloaded without checking
        // whether the dependencies have been loaded.
        public static bool IsAssetBundleDownloaded(string assetBundleName)
        {
            return loadedAssetBundles.ContainsKey(assetBundleName);
        }

        // Initializes asset bundle namager and starts download of manifest asset bundle.
        // Returns the manifest asset bundle downolad operation object.
        public static AssetBundleLoadManifestOperation Initialize()
        {
            return Initialize(AssetBundleUtility.GetPlatformName());
        }

        // Initializes asset bundle namager and starts download of manifest asset bundle.
        // Returns the manifest asset bundle downolad operation object.
        public static AssetBundleLoadManifestOperation Initialize(string manifestAssetBundleName)
        {
            #if UNITY_EDITOR
            Log(LogType.Info, "Simulation Mode: " + (SimulateAssetBundleInEditor ? "Enabled" : "Disabled"));
            #endif

            var go = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
            GameObject.DontDestroyOnLoad(go);

            // puting this gameobject under the singleton container
            go.transform.SetParent(SingletonUtil.GetSingletonContainer());

            #if UNITY_EDITOR
            // If we're in Editor simulation mode, we don't need the manifest assetBundle.
            if (SimulateAssetBundleInEditor)
            {
                return null;
            }
            #endif

            LoadAssetBundle(manifestAssetBundleName, true);
            var operation = new AssetBundleLoadManifestOperation(manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
            inProgressOperations.Add(operation);
            return operation;
        }

        // Unloads assetbundle and its dependencies.
        public static void UnloadAssetBundle(string assetBundleName)
        {
            #if UNITY_EDITOR
            // If we're in Editor simulation mode, we don't have to load the manifest assetBundle.
            if (SimulateAssetBundleInEditor)
            {
                return;
            }
            #endif

            UnloadAssetBundleInternal(assetBundleName);
            UnloadDependencies(assetBundleName);
        }

        // Starts a load operation for an asset from the given asset bundle.
        public static AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, System.Type type)
        {
            Log(LogType.Info, "Loading " + assetName + " from " + assetBundleName + " bundle");

            AssetBundleLoadAssetOperation operation = null;

            #if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                // NOTE [bgish]: testing if it's a full asset path, if so use LoadAssetAtPath instead.  Might be worth using this function in the else as well at a later date.
                if (assetName.Contains("/"))
                {
                    UnityEngine.Object target = AssetDatabase.LoadAssetAtPath(assetName, type);
                    operation = new AssetBundleLoadAssetOperationSimulation(target);
                }
                else
                {
                    string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
                    if (assetPaths.Length == 0)
                    {
                        Debug.LogError("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
                        return null;
                    }

                    // @TODO: Now we only get the main object from the first asset. Should consider type also.
                    UnityEngine.Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
                    operation = new AssetBundleLoadAssetOperationSimulation(target);
                }
            }
            else
            #endif
            {
                assetBundleName = RemapVariantName(assetBundleName);
                LoadAssetBundle(assetBundleName);
                operation = new AssetBundleLoadAssetOperationFull(assetBundleName, assetName, type);

                inProgressOperations.Add(operation);
            }

            return operation;
        }

        // Starts a load operation for a level from the given asset bundle.
        public static AssetBundleLoadOperation LoadLevelAsync(string assetBundleName, string levelName, bool isAdditive)
        {
            Log(LogType.Info, "Loading " + levelName + " from " + assetBundleName + " bundle");

            AssetBundleLoadOperation operation = null;

            #if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                operation = new AssetBundleLoadLevelSimulationOperation(assetBundleName, levelName, isAdditive);
            }
            else
            #endif
            {
                assetBundleName = RemapVariantName(assetBundleName);
                LoadAssetBundle(assetBundleName);
                operation = new AssetBundleLoadLevelOperation(assetBundleName, levelName, isAdditive);

                inProgressOperations.Add(operation);
            }

            return operation;
        }

        // Temporarily work around a il2cpp bug
        private static void LoadAssetBundle(string assetBundleName)
        {
            LoadAssetBundle(assetBundleName, false);
        }

        // Starts the download of the asset bundle identified by the given name, and asset bundles
        // that this asset bundle depends on.
        private static void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest)
        {
            Log(LogType.Info, "Loading Asset Bundle " + (isLoadingAssetBundleManifest ? "Manifest: " : ": ") + assetBundleName);

            #if UNITY_EDITOR
            // If we're in Editor simulation mode, we don't have to really load the assetBundle and its dependencies.
            if (SimulateAssetBundleInEditor)
            {
                return;
            }
            #endif

            if (isLoadingAssetBundleManifest == false)
            {
                if (assetBundleManifest == null)
                {
                    Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                    return;
                }
            }

            // Check if the assetBundle has already been processed.
            bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest);

            // Load dependencies.
            if (!isAlreadyProcessed && !isLoadingAssetBundleManifest)
            {
                LoadDependencies(assetBundleName);
            }
        }

        // Returns base downloading URL for the given asset bundle.
        // This URL may be overridden on per-bundle basis via overrideBaseDownloadingURL event.
        private static string GetAssetBundleBaseDownloadingURL(string bundleName)
        {
            if (OverrideBaseDownloadingURL != null)
            {
                foreach (OverrideBaseDownloadingURLDelegate method in OverrideBaseDownloadingURL.GetInvocationList())
                {
                    string res = method(bundleName);
                    if (res != null)
                    {
                        return res;
                    }
                }
            }

            return baseDownloadingURL;
        }

        // Checks who is responsible for determination of the correct asset bundle variant
        // that should be loaded on this platform.
        //
        // On most platforms, this is done by the AssetBundleManager itself. However, on
        // certain platforms (iOS at the moment) it's possible that an external asset bundle
        //  variant resolution mechanism is used. In these cases, we use base asset bundle
        // name (without the variant tag) as the bundle identifier. The platform-specific
        // code is responsible for correctly loading the bundle.
        private static bool UsesExternalBundleVariantResolutionMechanism(string baseAssetBundleName)
        {
            #if ENABLE_IOS_APP_SLICING
            var url = GetAssetBundleBaseDownloadingURL(baseAssetBundleName);
            if (url.ToLower().StartsWith("res://"))
            {
                return true;
            }
            #endif

            return false;
        }

        // Remaps the asset bundle name to the best fitting asset bundle variant.
        private static string RemapVariantName(string assetBundleName)
        {
            string[] bundlesWithVariant = assetBundleManifest.GetAllAssetBundlesWithVariant();

            // Get base bundle name
            string baseName = assetBundleName.Split('.')[0];

            if (UsesExternalBundleVariantResolutionMechanism(baseName))
            {
                return baseName;
            }

            int bestFit = int.MaxValue;
            int bestFitIndex = -1;

            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < bundlesWithVariant.Length; i++)
            {
                string[] curSplit = bundlesWithVariant[i].Split('.');
                string curBaseName = curSplit[0];
                string curVariant = curSplit[1];

                if (curBaseName != baseName)
                {
                    continue;
                }

                int found = System.Array.IndexOf(activeVariants, curVariant);

                // If there is no active variant found. We still want to use the first
                if (found == -1)
                {
                    found = int.MaxValue - 1;
                }

                if (found < bestFit)
                {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }

            if (bestFit == int.MaxValue - 1)
            {
                Debug.LogWarning("Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[bestFitIndex]);
            }

            if (bestFitIndex != -1)
            {
                return bundlesWithVariant[bestFitIndex];
            }
            else
            {
                return assetBundleName;
            }
        }

        // Sets up download operation for the given asset bundle if it's not downloaded already.
        private static bool LoadAssetBundleInternal(string assetBundleName, bool isLoadingAssetBundleManifest)
        {
            // Already loaded.
            LoadedAssetBundle bundle = null;
            loadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle != null)
            {
                bundle.ReferencedCount++;
                return true;
            }

            // @TODO: Do we need to consider the referenced count of WWWs?
            // In the demo, we never have duplicate WWWs as we wait LoadAssetAsync()/LoadLevelAsync() to be finished before calling another LoadAssetAsync()/LoadLevelAsync().
            // But in the real case, users can call LoadAssetAsync()/LoadLevelAsync() several times then wait them to be finished which might have duplicate WWWs.
            if (downloadingBundles.Contains(assetBundleName))
            {
                return true;
            }

            string bundleBaseDownloadingURL = GetAssetBundleBaseDownloadingURL(assetBundleName);

            if (bundleBaseDownloadingURL.ToLower().StartsWith("odr://"))
            {
                #if ENABLE_IOS_ON_DEMAND_RESOURCES
                Log(LogType.Info, "Requesting bundle " + assetBundleName + " through ODR");
                inProgressOperations.Add(new AssetBundleDownloadFromODROperation(assetBundleName));
                #else
                new ApplicationException("Can't load bundle " + assetBundleName + " through ODR: this Unity version or build target doesn't support it.");
                #endif
            }
            else if (bundleBaseDownloadingURL.ToLower().StartsWith("res://"))
            {
                #if ENABLE_IOS_APP_SLICING
                Log(LogType.Info, "Requesting bundle " + assetBundleName + " through asset catalog");
                inProgressOperations.Add(new AssetBundleOpenFromAssetCatalogOperation(assetBundleName));
                #else
                new ApplicationException("Can't load bundle " + assetBundleName + " through asset catalog: this Unity version or build target doesn't support it.");
                #endif
            }
            else
            {
                WWW download = null;
                string url = bundleBaseDownloadingURL + assetBundleName;

                // For manifest assetbundle, always download it as we don't have hash for it.
                if (isLoadingAssetBundleManifest)
                {
                    download = new WWW(url);
                }
                else
                {
                    download = WWW.LoadFromCacheOrDownload(url, assetBundleManifest.GetAssetBundleHash(assetBundleName), 0);
                }

                inProgressOperations.Add(new AssetBundleDownloadFromWebOperation(assetBundleName, download));
            }

            downloadingBundles.Add(assetBundleName);

            return false;
        }

        // Where we get all the dependencies and load them all.
        private static void LoadDependencies(string assetBundleName)
        {
            if (assetBundleManifest == null)
            {
                Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return;
            }

            // Get dependecies from the AssetBundleManifest object..
            string[] dependencies = assetBundleManifest.GetAllDependencies(assetBundleName);
            if (dependencies.Length == 0)
            {
                return;
            }

            for (int i = 0; i < dependencies.Length; i++)
            {
                dependencies[i] = RemapVariantName(dependencies[i]);
            }

            // Record and load all dependencies.
            AssetBundleManager.dependencies.Add(assetBundleName, dependencies);
            for (int i = 0; i < dependencies.Length; i++)
            {
                LoadAssetBundleInternal(dependencies[i], false);
            }
        }

        private static void UnloadDependencies(string assetBundleName)
        {
            string[] dependencies = null;
            if (AssetBundleManager.dependencies.TryGetValue(assetBundleName, out dependencies) == false)
            {
                return;
            }

            // Loop dependencies.
            foreach (var dependency in dependencies)
            {
                UnloadAssetBundleInternal(dependency);
            }

            AssetBundleManager.dependencies.Remove(assetBundleName);
        }

        private static void UnloadAssetBundleInternal(string assetBundleName)
        {
            string error;
            LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error);
            if (bundle == null)
            {
                return;
            }

            if (--bundle.ReferencedCount == 0)
            {
                bundle.OnUnload();
                loadedAssetBundles.Remove(assetBundleName);

                Log(LogType.Info, assetBundleName + " has been unloaded successfully");
            }
        }

        private static void SetSourceAssetBundleURL(string absolutePath)
        {
            // making sure everything is a "/" and the base url ends with a "/"
            absolutePath = absolutePath.Replace("\\", "/").AppendIfDoesntExist("/");
            BaseDownloadingURL = absolutePath + AssetBundleUtility.GetPlatformName() + "/";
        }

        private static void Log(LogType logType, string text)
        {
            if (logType == LogType.Error)
            {
                Debug.LogError("[AssetBundleManager] " + text);
            }
            else if (logMode == LogMode.All)
            {
                Debug.Log("[AssetBundleManager] " + text);
            }
        }

        private void Update()
        {
            // Update all in progress operations
            for (int i = 0; i < inProgressOperations.Count;)
            {
                var operation = inProgressOperations[i];
                if (operation.Update())
                {
                    i++;
                }
                else
                {
                    inProgressOperations.RemoveAt(i);
                    this.ProcessFinishedOperation(operation);
                }
            }
        }

        private void ProcessFinishedOperation(AssetBundleLoadOperation operation)
        {
            AssetBundleDownloadOperation download = operation as AssetBundleDownloadOperation;
            if (download == null)
            {
                return;
            }

            if (download.Error == null)
            {
                loadedAssetBundles.Add(download.AssetBundleName, download.AssetBundle);
            }
            else
            {
                string msg = string.Format("Failed downloading bundle {0} from {1}: {2}", download.AssetBundleName, download.GetSourceURL(), download.Error);
                downloadingErrors.Add(download.AssetBundleName, msg);
            }

            downloadingBundles.Remove(download.AssetBundleName);
        }
    }
}
