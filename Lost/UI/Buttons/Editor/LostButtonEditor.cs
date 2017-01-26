//-----------------------------------------------------------------------
// <copyright file="LostButtonEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEditor.UI;
    using UnityEngine;

    [CustomEditor(typeof(LostButton))]
    public class LostButtonEditor : ButtonEditor
    {
        private static GridDefinition gridDefinition;
        private static Grid grid;

        // button state information
        private int stateIndex = 0;
        private string[] buttonStateNames;

        // button action information
        private int actionIndex = 0;
        private Type[] buttonActionTypes;
        private string[] buttonActionNames;

        protected override void OnEnable()
        {
            base.OnEnable();

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

            this.buttonStateNames = Enum.GetNames(typeof(ButtonActoinState));
            this.buttonActionTypes = TypeUtil.GetAllTypesOf<ButtonAction>().ToArray();
            this.buttonActionNames = this.buttonActionTypes.Select(x => x.Name).ToArray();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // early out if no types were found
            if (this.buttonActionTypes.Length == 0)
            {
                return;
            }

            var lostButton = this.target as LostButton;

            GUILayout.Label(string.Empty);
            
            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("Button State", GUILayout.Width(90));
                this.stateIndex = EditorGUILayout.Popup(this.stateIndex, this.buttonStateNames, GUILayout.Width(150));
            }

            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("Button Action", GUILayout.Width(90));
                this.actionIndex = EditorGUILayout.Popup(this.actionIndex, this.buttonActionNames, GUILayout.Width(150));
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
                    var newComponent = lostButton.gameObject.AddComponent(this.buttonActionTypes[this.actionIndex]) as ButtonAction;
                    newComponent.State = (ButtonActoinState)this.stateIndex;
                    newComponent.hideFlags = HideFlags.HideInInspector;
                }
            }

            GUILayout.Label(string.Empty);

            var buttonActionComponents = lostButton.GetComponents<ButtonAction>().OrderBy(x => x.State).ThenBy(x => x.Order).ToList();

            // early out so we don't draw the grid if the button doesn't have any actions yet
            if (buttonActionComponents.Count == 0)
            {
                return;
            }

            bool orderChanged = false;

            using (new BeginGridHelper(grid))
            {
                for (int i = 0; i < buttonActionComponents.Count; i++)
                {
                    var buttonAction = buttonActionComponents[i];

                    // making sure all button actions are hidden in editor
                    buttonAction.hideFlags = HideFlags.HideInInspector;

                    if (buttonAction.Order != i)
                    {
                        orderChanged = true;
                        buttonAction.Order = i;
                    }
                    
                    using (new BeginGridRowHelper(grid))
                    {
                        grid.DrawLabel(buttonAction.State.ToString());

                        buttonAction.ActionObject = grid.DrawObject(buttonAction.ActionObjectType, buttonAction.ActionObject, true);

                        grid.DrawLabel(buttonAction.Name);

                        buttonAction.ActionValue = grid.DrawUnknownObject(buttonAction.ActionValue);
                    }

                    if (grid.RowButtonPressed == GridButton.Delete)
                    {
                        GameObject.DestroyImmediate(buttonAction);
                        break;
                    }
                    else if (grid.RowButtonPressed == GridButton.MoveUp && i != 0)
                    {
                        buttonActionComponents[i].Order--;
                        buttonActionComponents[i - 1].Order++;
                        break;
                    }
                    else if (grid.RowButtonPressed == GridButton.MoveDown && i != buttonActionComponents.Count - 1)
                    {
                        buttonActionComponents[i].Order++;
                        buttonActionComponents[i + 1].Order--;
                        break;
                    }
                }

                GUILayout.Label(string.Empty);
                
                if (GUI.changed || orderChanged)
                {
                    EditorUtility.SetDirty(this.target);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }
        }
    }
}
