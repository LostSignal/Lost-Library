//-----------------------------------------------------------------------
// <copyright file="AnimatorEditorWindow.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Linq;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Animations;

    public class AnimatorEditorWindow : EditorWindow
    {
        string animationClipName = string.Empty;

        [MenuItem("Assets/Animator/Add Sub-Animation Clip", true, 100)]
        public static bool AddSubAnimationClipToAnimatorValidator()
        {
            return Selection.objects.Length == 1 && Selection.objects[0] is AnimatorController;
        }

        [MenuItem("Assets/Animator/Add Sub-Animation Clip", false, 100)]
        public static void AddSubAnimationClipToAnimator()
        {
            var window = (AnimatorEditorWindow)EditorWindow.GetWindow(typeof(AnimatorEditorWindow));
            window.titleContent = new GUIContent("Add Anim");
            window.minSize = new Vector2(300, 75);
            window.maxSize = new Vector2(300, 75);
            window.ShowPopup();
        }

        private void OnGUI()
        {
            AnimatorController animator = Selection.objects.FirstOrDefault() as AnimatorController;

            if (animator == null)
            {
                return;
            }
            
            this.animationClipName = EditorGUILayout.TextField("Animation Clip Name", this.animationClipName);
            
            if (GUILayout.Button("Add"))
            {
                AssetDatabase.AddObjectToAsset(new AnimationClip() { name = animationClipName }, animator);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animator));
                this.animationClipName = string.Empty;
            }
        }
    }
}
