//-----------------------------------------------------------------------
// <copyright file="AssetBundleLoadLevelSimulationOperation.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    #if UNITY_EDITOR

    using UnityEngine;

    public class AssetBundleLoadLevelSimulationOperation : AssetBundleLoadOperation
    {
        private AsyncOperation operation = null;

        public AssetBundleLoadLevelSimulationOperation(string assetBundleName, string levelName, bool isAdditive)
        {
            string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, levelName);

            if (levelPaths.Length == 0)
            {
                // TODO: The error needs to differentiate that an asset bundle name doesn't exist
                //       from that there right scene does not exist in the asset bundle...
                Debug.LogError("There is no scene with name \"" + levelName + "\" in " + assetBundleName);
                return;
            }

            if (isAdditive)
            {
                this.operation = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode(levelPaths[0]);
            }
            else
            {
                this.operation = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode(levelPaths[0]);
            }
        }

        public override bool Update()
        {
            return false;
        }

        public override bool IsDone()
        {
            return this.operation == null || this.operation.isDone;
        }
    }

    #endif
}
