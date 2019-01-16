//-----------------------------------------------------------------------
// <copyright file="LocalizationTableEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(LocalizationTable))]
    public class LocalizationTableEditor : Editor
    {
        [MenuItem("Lost/Localization Table Editor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<LocalizationTableEditorWindow>(false, "Localization");
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Localization Table Editor Window"))
            {
                ShowWindow();
            }
        }
    }

    public class LocalizationTableEditorWindow : EditorWindow
    {
        //// private static NewGrid entriesGrid;

        static LocalizationTableEditorWindow()
        {
            //// var gridDefinition = new GridDefinition();
            //// gridDefinition.AddColumn("Id", 60);
            //// gridDefinition.AddColumn("Text", 600);
            ////
            //// entriesGrid = new NewGrid(gridDefinition);
        }

        private void OnGUI()
        {
            // TODO [bgish]: Re-implement with new grid rendering system
            //// LocTable locTable = UnityEditor.Selection.activeObject as LocTable;
            ////
            //// if (locTable != null)
            //// {
            ////     if (GUILayout.Button("Add Localized Text"))
            ////     {
            ////         locTable.AddNewText("");
            ////     }
            ////
            ////     SerializedObject serializedObject = new SerializedObject(locTable);
            ////
            ////     using (new BeginVerticalHelper())
            ////     {
            ////         using (new BoxHelper(entriesGrid.GetGridWitdh(), "Entries"))
            ////         {
            ////             entriesGrid.Draw(false, serializedObject.FindProperty("entries"));
            ////         }
            ////     }
            //// }
            //// else
            //// {
            ////     EditorGUILayout.LabelField("No Localization Table Selected!");
            //// }
        }
    }
}
