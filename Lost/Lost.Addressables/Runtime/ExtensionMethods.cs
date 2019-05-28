//-----------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_ADDRESSABLES

namespace Lost
{
    using UnityEngine;

    public static class ExtensionMethods
    {
        public static void ReleaseAddressable(this GameObject gameObject)
        {
            if (gameObject != null)
            {
                UnityEngine.AddressableAssets.Addressables.ReleaseInstance(gameObject);
            }
        }

        public static void ReleaseAddressable(this Component component)
        {
            if (component != null)
            {
                UnityEngine.AddressableAssets.Addressables.ReleaseInstance(component.gameObject);
            }
        }
    }
}

#endif
