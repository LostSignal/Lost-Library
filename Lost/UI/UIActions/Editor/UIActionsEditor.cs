//-----------------------------------------------------------------------
// <copyright file="UIActionsEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    public class UIActionsEditor : MonoBehaviour
    {
        private static GridDefinition gridDefinition;
        private static Grid grid;

        // state information
        private static int stateIndex = 0;
        private static string[] stateNames;

        // action information
        private static int uiActionIndex = 0;
        private static Type[] uiActionTypes;
        private static string[] uiActionNames;

        private static bool initialized = false;

        public static void DrawUIActionGUI(UnityEngine.Object target)
        {
            Initialize();

            // early out if no types were found
            if (uiActionTypes.Length == 0)
            {
                return;
            }

            var targetMonobehaviour = target as MonoBehaviour;

            GUILayout.Label(string.Empty);

            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("State", GUILayout.Width(90));
                stateIndex = EditorGUILayout.Popup(stateIndex, stateNames, GUILayout.Width(150));
            }

            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("UI Action", GUILayout.Width(90));
                uiActionIndex = EditorGUILayout.Popup(uiActionIndex, uiActionNames, GUILayout.Width(150));
            }

            // early out if we just changed drop downs and didn't actually change anything (don't want to mark scene dirty for no reason)
            if (GUI.changed)
            {
                return;
            }

            using (new BeginHorizontalHelper())
            {
                if (GUILayout.Button("Add", GUILayout.Width(245), GUILayout.Height(14)))
                {
                    var newComponent = targetMonobehaviour.gameObject.AddComponent(uiActionTypes[uiActionIndex]) as UIAction;
                    newComponent.State = (UIActionState)stateIndex;
                    newComponent.hideFlags = HideFlags.HideInInspector;
                }
            }

            GUILayout.Label(string.Empty);

            var uiActionComponents = targetMonobehaviour.GetComponents<UIAction>().OrderBy(x => x.State).ThenBy(x => x.Order).ToList();

            // early out so we don't draw the grid if the ui object doesn't have any actions yet
            if (uiActionComponents.Count == 0)
            {
                return;
            }

            bool orderChanged = false;

            using (new BeginGridHelper(grid))
            {
                for (int i = 0; i < uiActionComponents.Count; i++)
                {
                    var uiAction = uiActionComponents[i];

                    // making sure all button actions are hidden in editor
                    uiAction.hideFlags = HideFlags.HideInInspector;

                    if (uiAction.Order != i)
                    {
                        orderChanged = true;
                        uiAction.Order = i;
                    }

                    using (new BeginGridRowHelper(grid))
                    {
                        grid.DrawLabel(uiAction.State.ToString());

                        uiAction.ActionObject = grid.DrawObject(uiAction.ActionObjectType, uiAction.ActionObject, true);

                        grid.DrawLabel(uiAction.Name);

                        uiAction.ActionValue = grid.DrawUnknownObject(uiAction.ActionValue);
                    }

                    if (grid.RowButtonPressed == GridButton.Delete)
                    {
                        GameObject.DestroyImmediate(uiAction);
                        break;
                    }
                    else if (grid.RowButtonPressed == GridButton.MoveUp && i != 0)
                    {
                        uiActionComponents[i].Order--;
                        uiActionComponents[i - 1].Order++;
                        break;
                    }
                    else if (grid.RowButtonPressed == GridButton.MoveDown && i != uiActionComponents.Count - 1)
                    {
                        uiActionComponents[i].Order++;
                        uiActionComponents[i + 1].Order--;
                        break;
                    }
                }

                GUILayout.Label(string.Empty);

                if (GUI.changed || orderChanged)
                {
                    EditorUtility.SetDirty(target);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }
        }

        private static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;

            if (grid == null)
            {
                gridDefinition = new GridDefinition();
                gridDefinition.AddColumn("State", 85);
                gridDefinition.AddColumn("Object", 175);
                gridDefinition.AddColumn("Action", 85);
                gridDefinition.AddColumn("Value", 175);
                gridDefinition.RowButtons = GridButton.All;

                grid = new Grid(gridDefinition);
            }

            stateNames = Enum.GetNames(typeof(UIActionState));
            uiActionTypes = TypeUtil.GetAllTypesOf<UIAction>().ToArray();
            uiActionNames = uiActionTypes.Select(x => x.Name).ToArray();
        }
    }
}
