//-----------------------------------------------------------------------
// <copyright file="SingletonEditorCleanup.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;
    
    [InitializeOnLoad]
    public static class SingletonEditorCleanup
    {
        static SingletonEditorCleanup()
        {
            EditorApplication.playmodeStateChanged += DeletingSingletonGameObjectAfterLevaingEditorPlayMode;
        }
       
        /// <summary>
        /// On occasion, leaving play in editor mode can leave singleton object behind.  This was made to clean that up.
        /// </summary>
        private static void DeletingSingletonGameObjectAfterLevaingEditorPlayMode()
        {
            // testing if we have just left playmode
            if (EditorApplication.isPlayingOrWillChangePlaymode == false && EditorApplication.isPlaying == false)
            {
                var rootSingleton = GameObject.Find("/Singletons");
                
                if (rootSingleton != null)
                {
                    Logger.LogWarning("Destroy rouge singleton objects.");
                    rootSingleton.DestroyAllRecursively();
                }
            }
        }
    }
}
