//-----------------------------------------------------------------------
// <copyright file="EditorconfigSolutionItem.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

//
// https://forum.unity.com/threads/issues-with-the-auto-generation-of-c-project-in-unity.380710/
//

namespace Lost
{
    using System;
    using System.IO;
    using System.Text;
    using UnityEditor;

    public class EditorconfigSolutionItem : AssetPostprocessor
    {
        private const string editorconfigFileName = ".editorconfig";

        private static void OnGeneratedCSProjectFiles()
        {
            foreach (var solutionFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.sln"))
            {
                FixSolution(solutionFile);
            }
        }

        private static void FixSolution(string solutionFilePath)
        {
            string content = File.ReadAllText(solutionFilePath);

            if (File.Exists(editorconfigFileName) && content.Contains(editorconfigFileName) == false)
            {
                File.WriteAllText(solutionFilePath, content.Insert(content.IndexOf("Global"), GetEditorconfigString()));
            }
        }

        private static string GetEditorconfigString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = \"Solution Items\", \"Solution Items\", \"{NEW_GUID}\"");
            builder.AppendLine("    ProjectSection(SolutionItems) = preProject");
            builder.AppendLine("        .editorconfig = .editorconfig");
            builder.AppendLine("    EndProjectSection");
            builder.AppendLine("EndProject");

            return builder.ToString().Replace("NEW_GUID", Guid.NewGuid().ToString().ToUpper());
        }
    }
}
