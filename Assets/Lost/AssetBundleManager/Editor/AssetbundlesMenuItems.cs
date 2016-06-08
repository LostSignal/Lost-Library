//-----------------------------------------------------------------------
// <copyright file="AssetBundlesMenuItems.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;

    public class AssetBundlesMenuItems
    {
        private const string SimulationMode = "Assets/AssetBundles/Simulation Mode";

        [MenuItem(SimulationMode)]
        public static void ToggleSimulationMode()
        {
            AssetBundleManager.SimulateAssetBundleInEditor = !AssetBundleManager.SimulateAssetBundleInEditor;
        }

        [MenuItem(SimulationMode, true)]
        public static bool ToggleSimulationModeValidate()
        {
            Menu.SetChecked(SimulationMode, AssetBundleManager.SimulateAssetBundleInEditor);
            return true;
        }

        [MenuItem("Assets/AssetBundles/Build AssetBundles")]
        public static void BuildAssetBundles()
        {
            CloudBuildHelper.BuildAssetBundles(true);
        }
    }
}
