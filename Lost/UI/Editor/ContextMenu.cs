//-----------------------------------------------------------------------
// <copyright file="ContextMenu.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEngine;

    public class ContextMenu : MonoBehaviour
    {
        [MenuItem("Assets/Animator/Add Show And Hide Sub Animations", true, 100)]
        public static bool AddChildAnimationClipValidator()
        {
            return Selection.activeObject is AnimatorController;
        }

        [MenuItem("Assets/Animator/Add Show And Hide Sub Animations", false, 100)]
        public static void AddChildAnimationClip()
        {
            AssetDatabase.AddObjectToAsset(new AnimationClip() { name = "Show" }, Selection.activeObject);
            AssetDatabase.AddObjectToAsset(new AnimationClip() { name = "Hide" }, Selection.activeObject);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Selection.activeObject));
        }
    }
}
