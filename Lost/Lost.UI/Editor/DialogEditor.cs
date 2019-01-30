//-----------------------------------------------------------------------
// <copyright file="DialogEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.Linq;
    using Lost.EditorGrid;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    [CustomEditor(typeof(Dialog))]
    public class DialogEditor : Editor
    {
        private static HashSet<int> ShowDialogComponents = new HashSet<int>();

        private SerializedObject dialogObject;

        private SerializedProperty showOnAwake;
        private SerializedProperty blockInput;
        private SerializedProperty tapOutsideToDismiss;
        private SerializedProperty dontChangeStateWhileTransitioning;

        private SerializedProperty registerForBackButton;
        private SerializedProperty hideOnBackButtonPressed;

        private SerializedProperty sendAnalyticEvent;
        private SerializedProperty storeType;

        private SerializedProperty onShow;
        private SerializedProperty onHide;
        private SerializedProperty onBackButtonPressed;

        public override void OnInspectorGUI()
        {
            var dialog = this.target as Dialog;
            dialog.InitializeFields();

            GUILayout.Space(10);

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Runtime Toggle Dilaog (Show/Hide)"))
                {
                    dialog.Toggle();
                }

                GUILayout.Space(15);
            }

            if (GUILayout.Button("Toggle Content/Blocker On/Off"))
            {
                var content = dialog.transform.Find("Content");
                var blocker = dialog.transform.Find("Blocker");

                bool visible = !content.gameObject.activeSelf;
                content.SafeSetActive(visible);
                blocker.SafeSetActive(visible);
            }

            // Drawing the Show/Hide components button
            bool componentsVisible = ShowDialogComponents.Contains(this.target.GetInstanceID());

            if (GUILayout.Button(componentsVisible ? "Hide Editor Components" : "Show Editor Components"))
            {
                if (componentsVisible)
                {
                    ShowDialogComponents.Remove(this.target.GetInstanceID());
                }
                else
                {
                    ShowDialogComponents.Add(this.target.GetInstanceID());
                }

                this.SetComponentsVisibility(!componentsVisible);
            }

            GUILayout.Space(10);

            this.DrawAnimator(dialog);
            this.DrawCanvas(dialog);
            this.DrawDialog(dialog);

            this.dialogObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            this.dialogObject = new SerializedObject(this.target);

            this.showOnAwake = this.dialogObject.FindProperty("showOnAwake");
            this.dontChangeStateWhileTransitioning = this.dialogObject.FindProperty("dontChangeStateWhileTransitioning");
            this.blockInput = this.dialogObject.FindProperty("blockInput");
            this.tapOutsideToDismiss = this.dialogObject.FindProperty("tapOutsideToDismiss");

            this.registerForBackButton = this.dialogObject.FindProperty("registerForBackButton");
            this.hideOnBackButtonPressed = this.dialogObject.FindProperty("hideOnBackButtonPressed");

            this.sendAnalyticEvent = this.dialogObject.FindProperty("sendAnalyticEvent");
            this.storeType = this.dialogObject.FindProperty("storeType");

            this.onShow = this.dialogObject.FindProperty("onShow");
            this.onHide = this.dialogObject.FindProperty("onHide");
            this.onBackButtonPressed = this.dialogObject.FindProperty("onBackButtonPressed");

            this.SetComponentsVisibility(ShowDialogComponents.Contains(this.target.GetInstanceID()));
        }

        private void DrawAnimator(Dialog dialog)
        {
            bool isVisible;
            using (new FoldoutScope(793215825, "Animator", out isVisible, false))
            {
                if (isVisible == false)
                {
                    return;
                }

                var animator = dialog.Animator.runtimeAnimatorController;
                var newAnimator = EditorGUILayout.ObjectField("Animator", animator, typeof(RuntimeAnimatorController), false) as RuntimeAnimatorController;
                if (animator != newAnimator)
                {
                    dialog.Animator.runtimeAnimatorController = newAnimator;
                }
            }
        }

        private void DrawCanvas(Dialog dialog)
        {
            bool isVisible;
            using (new FoldoutScope(793215826, "Canvas", out isVisible, false))
            {
                if (isVisible == false)
                {
                    return;
                }

                // Render Camera
                var renderCamera = dialog.Canvas.worldCamera;
                var newRenderCamera = EditorGUILayout.ObjectField("Render Camera", renderCamera, typeof(Camera), false) as Camera;
                if (renderCamera != newRenderCamera)
                {
                    dialog.Canvas.worldCamera = newRenderCamera;
                }

                // Canvas.planeDistance
                var planeDistance = dialog.Canvas.planeDistance;
                var newPlaneDistance = EditorGUILayout.FloatField("Plane Distance", planeDistance);
                if (planeDistance != newPlaneDistance)
                {
                    dialog.Canvas.planeDistance = newPlaneDistance;
                }

                // Canvas.sortingLayer
                string sortingLayer = dialog.Canvas.sortingLayerName;
                List<string> layersList = SortingLayer.layers.Select(x => x.name).ToList();
                string[] layersArray = layersList.ToArray();
                int layerIndex = layersList.IndexOf(sortingLayer);
                int newLayerIndex = EditorGUILayout.Popup("Sorting Layer", layerIndex, layersArray);

                if (layerIndex != newLayerIndex)
                {
                    dialog.Canvas.sortingLayerName = layersArray[newLayerIndex];
                }

                // Canvas.orderInLayer
                var sortingOrder = dialog.Canvas.sortingOrder;
                var newSortingOrder = EditorGUILayout.IntField("Order In Layer", sortingOrder);
                if (sortingOrder != newSortingOrder)
                {
                    dialog.Canvas.sortingOrder = newSortingOrder;
                }
            }
        }

        private void DrawDialog(Dialog dialog)
        {
            float originalLabelWidth = EditorGUIUtility.labelWidth;

            // Events
            bool areEventsVisible;
            using (new FoldoutScope(793215810, "Dialog Events", out areEventsVisible, false))
            {
                if (areEventsVisible)
                {
                    EditorGUILayout.PropertyField(this.onShow);
                    EditorGUILayout.PropertyField(this.onHide);
                    EditorGUILayout.PropertyField(this.onBackButtonPressed);
                }
            }

            // Properties
            bool dialogPropertiesVisible;
            using (new FoldoutScope(793215711, "Dialog Properties", out dialogPropertiesVisible, false))
            {
                if (dialogPropertiesVisible)
                {
                    // General
                    EditorGUIUtility.labelWidth = 250;

                    EditorGUILayout.PropertyField(this.showOnAwake);
                    EditorGUILayout.PropertyField(this.dontChangeStateWhileTransitioning);

                    // Back Button
                    EditorGUIUtility.labelWidth = 200;

                    EditorGUILayout.PropertyField(this.registerForBackButton);

                    if (this.registerForBackButton.boolValue)
                    {
                        EditorGUILayout.PropertyField(this.hideOnBackButtonPressed);
                    }

                    // Input Blocker
                    EditorGUIUtility.labelWidth = 180;

                    EditorGUILayout.PropertyField(this.blockInput);

                    if (this.blockInput.boolValue)
                    {
                        EditorGUILayout.PropertyField(this.tapOutsideToDismiss);
                    }

                    // Analytics
                    EditorGUIUtility.labelWidth = 150;

                    EditorGUILayout.PropertyField(this.sendAnalyticEvent);

                    if (this.sendAnalyticEvent.boolValue)
                    {
                        EditorGUILayout.PropertyField(this.storeType);
                    }
                }
            }

            EditorGUIUtility.labelWidth = originalLabelWidth;
        }

        public void SetComponentsVisibility(bool visible)
        {
            this.ToggleComponentVisibility<Canvas>(visible);
            this.ToggleComponentVisibility<CanvasScaler>(visible);
            this.ToggleComponentVisibility<HDCanvas>(visible);
            this.ToggleComponentVisibility<GraphicRaycaster>(visible);
            this.ToggleComponentVisibility<Animator>(visible);
            this.ToggleComponentVisibility<DialogSetupHelper>(visible);
        }

        private void ToggleComponentVisibility<T>(bool visible) where T : Component
        {
            Dialog dialog = this.target as Dialog;

            if (dialog == null)
            {
                return;
            }

            Component behaviour = dialog.GetComponent<T>();
            HideFlags hideFlags = behaviour.hideFlags;

            if (visible)
            {
                behaviour.hideFlags &= ~HideFlags.HideInInspector;
            }
            else
            {
                behaviour.hideFlags |= HideFlags.HideInInspector;
            }
        }
    }
}
