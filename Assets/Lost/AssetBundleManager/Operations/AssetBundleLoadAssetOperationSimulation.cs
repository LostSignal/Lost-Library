//-----------------------------------------------------------------------
// <copyright file="AssetBundleLoadAssetOperationSimulation.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public class AssetBundleLoadAssetOperationSimulation : AssetBundleLoadAssetOperation
    {
        private UnityEngine.Object simulatedObject;

        public AssetBundleLoadAssetOperationSimulation(UnityEngine.Object simulatedObject)
        {
            this.simulatedObject = simulatedObject;
        }

        public override T GetAsset<T>()
        {
            return this.simulatedObject as T;
        }

        public override bool Update()
        {
            return false;
        }

        public override bool IsDone()
        {
            return true;
        }
    }
}
