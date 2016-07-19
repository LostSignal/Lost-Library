//-----------------------------------------------------------------------
// <copyright file="DefineEditorWizard.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    public class DefineEditorWizard : EditorWindow
    {
        [MenuItem("Lost/Wizards/Defines Editor")]
        private static void Init()
        {
            EditorWindow.GetWindow(typeof(DefineEditorWizard), false, "Defines Editor", true).Show();
        }

        private void OnFocus()
        {
            // TODO [bgish] - Do initialization (Getting Defines from settigns, and getting defines from gmcs file)
            Logger.LogInfo("Got Focus!");
        }

        void OnGUI()
        {
            //// GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            //// myString = EditorGUILayout.TextField("Text Field", myString);
            //// 
            //// groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            //// myBool = EditorGUILayout.Toggle("Toggle", myBool);
            //// myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
            //// EditorGUILayout.EndToggleGroup();
        }

        //// private void writeFiles(string data)
        //// {
        ////     var smcsFile = Path.Combine(Application.dataPath, "smcs.rsp");
        ////     var gmcsFile = Path.Combine(Application.dataPath, "gmcs.rsp");
        //// 
        ////     // -define:debug;poop
        ////     File.WriteAllText(smcsFile, data);
        ////     File.WriteAllText(gmcsFile, data);
        //// }

        //// private void reimportSomethingToForceRecompile()
        //// {
        ////     var dataPathDir = new DirectoryInfo(Application.dataPath);
        ////     var dataPathUri = new System.Uri(Application.dataPath);
        ////     foreach (var file in dataPathDir.GetFiles("GlobalDefinesWizard.cs", SearchOption.AllDirectories))
        ////     {
        ////         var relativeUri = dataPathUri.MakeRelativeUri(new System.Uri(file.FullName));
        ////         var relativePath = System.Uri.UnescapeDataString(relativeUri.ToString());
        ////         AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);
        ////     }
        //// }

        //// void OnGUI()
        //// {
        ////     var toRemove = new List<GlobalDefine>();
        //// 
        ////     foreach (var define in _globalDefines)
        ////     {
        ////         // EditorGUILayout.BeginHorizontal();
        ////         // 
        ////         // define.define = EditorGUILayout.TextField(define.define);
        ////         // define.enabled = EditorGUILayout.Toggle(define.enabled);
        ////         // 
        ////         // var remove = false;
        ////         // if (GUILayout.Button("Remove"))
        ////         //     remove = true;
        ////         // 
        ////         // EditorGUILayout.EndHorizontal();
        ////         //
        ////         // return remove;
        //// 
        ////         if (defineEditor(define))
        ////             toRemove.Add(define);
        ////     }
        //// 
        ////     foreach (var define in toRemove)
        ////         _globalDefines.Remove(define);
        //// 
        ////     if (GUILayout.Button("Add Define"))
        ////     {
        ////         var d = new GlobalDefine();
        ////         d.define = "NEW_DEFINE";
        ////         d.enabled = false;
        ////         _globalDefines.Add(d);
        ////     }
        ////     GUILayout.Space(40);
        //// 
        ////     if (GUILayout.Button("Save"))
        ////     {
        ////         save();
        ////         Close();
        ////     }
        //// }
    }
}
