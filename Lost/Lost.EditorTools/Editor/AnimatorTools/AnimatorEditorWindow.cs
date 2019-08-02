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
    using UnityEditor.VersionControl;

    public class AnimatorEditorWindow : EditorWindow
    {
        string animationClipName = string.Empty;

        [MenuItem("Assets/Animator/Add Sub-Animation Clip", true, 100)]
        public static bool AddSubAnimationClipToAnimatorValidator()
        {
            return Selection.objects.Length == 1 && Selection.objects.FirstOrDefault() is AnimatorController;
        }

        [MenuItem("Assets/Animator/Add Sub-Animation Clip", false, 100)]
        public static void AddSubAnimationClipToAnimator()
        {
            var window = (AnimatorEditorWindow)EditorWindow.GetWindow(typeof(AnimatorEditorWindow));
            window.animationClipName = string.Empty;
            window.titleContent = new GUIContent("Add Anim");
            window.minSize = new Vector2(300, 75);
            window.maxSize = new Vector2(300, 75);
            window.ShowPopup();
        }

        [MenuItem("Assets/Animator/Rename Sub-Animation Clip", true, 101)]
        public static bool RenameSubAnimationClipToAnimatorValidator()
        {
            var animationClip = Selection.objects.FirstOrDefault() as AnimationClip;
            var animationController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(animationClip));

            return Selection.objects.Length == 1 && animationClip != null && animationController != null;
        }

        [MenuItem("Assets/Animator/Rename Sub-Animation Clip", false, 101)]
        public static void RenameSubAnimationClipToAnimator()
        {
            var animationClip = Selection.objects.FirstOrDefault() as AnimationClip;

            var window = (AnimatorEditorWindow)EditorWindow.GetWindow(typeof(AnimatorEditorWindow));
            window.titleContent = new GUIContent("Rename Anim");
            window.animationClipName = animationClip.name;
            window.minSize = new Vector2(300, 75);
            window.maxSize = new Vector2(300, 75);
            window.ShowPopup();
        }

        [MenuItem("Assets/Animator/Delete Sub-Animation Clip", true, 200)]
        public static bool DeleteSubAnimationClipToAnimatorValidator()
        {
            var animationClip = Selection.objects.FirstOrDefault() as AnimationClip;
            var animationController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(animationClip));

            return Selection.objects.Length == 1 && animationClip != null && animationController != null;
        }

        [MenuItem("Assets/Animator/Delete Sub-Animation Clip", false, 200)]
        public static void DeleteSubAnimationClipToAnimator()
        {
            var animationClip = Selection.objects.FirstOrDefault() as AnimationClip;
            var animationController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(animationClip));

            if (Provider.isActive)
            {
                Provider.Checkout(animationController, CheckoutMode.Asset);
            }

            AssetDatabase.RemoveObjectFromAsset(animationClip);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animationController));
        }
        
        private void OnGUI()
        {
            if (Selection.objects.FirstOrDefault() is AnimatorController)
            {
                // This is an "Add" operation
                this.animationClipName = EditorGUILayout.TextField("Animation Clip Name", this.animationClipName);

                if (GUILayout.Button("Add"))
                {
                    var animationController = Selection.objects.FirstOrDefault() as AnimatorController;

                    if (Provider.isActive)
                    {
                        Provider.Checkout(animationController, CheckoutMode.Asset);
                    }

                    AssetDatabase.AddObjectToAsset(new AnimationClip() { name = animationClipName }, animationController);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animationController));

                    this.animationClipName = string.Empty;
                }
            }
            else if (Selection.objects.FirstOrDefault() is AnimationClip)
            {
                // This is a "Rename" operation
                this.animationClipName = EditorGUILayout.TextField("Animation Clip Name", this.animationClipName);

                if (GUILayout.Button("Rename"))
                {
                    var animationClip = Selection.objects.FirstOrDefault() as AnimationClip;
                    var animationController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(animationClip));

                    if (Provider.isActive)
                    {
                        Provider.Checkout(animationController, CheckoutMode.Asset);
                    }

                    animationClip.name = this.animationClipName;
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animationController));
                    this.Close();
                }
            }
        }
    }
}
