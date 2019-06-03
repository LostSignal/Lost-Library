//-----------------------------------------------------------------------
// <copyright file="FolderFinder.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEditor;

    public static class FolderFinder
    {
        //// [MenuItem("Lost/Logs/Show Editor Logs")]
        //// public static void OpenEditorLogs()
        //// {
        ////     // macOS    ~/Library/Logs/Unity/Editor.log
        ////     // Windows  C:\Users\username\AppData\Local\Unity\Editor\Editor.log
        ////     EditorUtility.RevealInFinder(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        //// }
        ////
        //// [MenuItem("Lost/Logs/Show Player Log")]
        //// public static void OpenPlayerLog()
        //// {
        ////     // macOS    ~/Library/Logs/Unity/Player.log
        ////     // Windows  C:\Users\username\AppData\LocalLow\CompanyName\ProductName\Player.log
        ////     // Linux    ~/.config/unity3d/CompanyName/ProductName/Player.log
        ////     EditorUtility.RevealInFinder(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        //// }
    }
}
