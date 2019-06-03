//-----------------------------------------------------------------------
// <copyright file="NewBehaviourScript.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEditor.VersionControl;
    using UnityEngine;

    public static class MenuItemTools
    {
        [MenuItem("Lost/Actions/Convert All C# Files to utf-8-bom, lf, trim_trailing_whitespace, insert_final_newline")]
        public static void ConvertAllCSharpFiles()
        {
            string path = ".";

            // If a directory is selected, then only convert the things under the directory
            if (Selection.activeObject != null)
            {
                string rootDirectoryAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

                if (Directory.Exists(rootDirectoryAssetPath))
                {
                    path = rootDirectoryAssetPath;
                }
            }

            foreach (string file in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories))
            {
                StringBuilder fileBuilder = new StringBuilder();

                string[] lines = File.ReadAllLines(file);
                int lastLineIndex = lines.Length - 1;

                // calculating the line of the file that actually has content
                while (lastLineIndex > 0 && string.IsNullOrEmpty(lines[lastLineIndex].Trim()))
                {
                    lastLineIndex--;
                }

                // converting each line to our project standards
                for (int i = 0; i <= lastLineIndex; i++)
                {
                    string convertedLine = lines[i];

                    convertedLine = convertedLine.TrimEnd();              // trim_trailing_whitespace = true
                    convertedLine = convertedLine.Replace("\t", "    ");  // indent_style = space, indent_size = 4
                    convertedLine = convertedLine + '\n';                 // insert_final_newline = true, end_of_line = lf

                    fileBuilder.Append(convertedLine);
                }

                // Generating the final file bytes
                MemoryStream finalFileContents = new MemoryStream();
                var utf8BomEncoder = new UTF8Encoding(true);

                byte[] preamble = utf8BomEncoder.GetPreamble();
                finalFileContents.Write(preamble, 0, preamble.Length);

                byte[] contents = utf8BomEncoder.GetBytes(fileBuilder.ToString());
                finalFileContents.Write(contents, 0, contents.Length);

                byte[] finalFileBytes = finalFileContents.ToArray();

                // Checking if the file needs to be saved out
                if (AreByteArraysEqual(File.ReadAllBytes(file), finalFileBytes) == false)
                {
                    Debug.Log("Updating File " + file);
                    Provider.Checkout(file.Replace("\\", "/").Replace("./", string.Empty), CheckoutMode.Asset).Wait();
                    File.WriteAllBytes(file, finalFileBytes);
                }
            }
        }

        [MenuItem("Lost/Actions/Fix Bad Line Endings")]
        public static void FixBadLineEndings()
        {
            string path = ".";

            // If a directory is selected, then only convert the things under the directory
            if (Selection.activeObject != null)
            {
                string rootDirectoryAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

                if (Directory.Exists(rootDirectoryAssetPath))
                {
                    path = rootDirectoryAssetPath;
                }
            }

            foreach (string file in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories))
            {
                string text = File.ReadAllText(file);

                if (text.Contains("\r\r\n"))
                {
                    text = text.Replace("\r\r\n", "\n");
                    text = text.Replace("\r\n", "\n");

                    Provider.Checkout(file.Replace("\\", "/").Replace("./", string.Empty), CheckoutMode.Asset).Wait();
                    File.WriteAllText(file, text);
                }
            }
        }

        [MenuItem("Lost/Actions/Convert all C# files to project line endings")]
        public static void ConvertAllCSharpFileLineEndings()
        {
            foreach (string file in Directory.GetFiles(".", "*.cs", SearchOption.AllDirectories))
            {
                // removing the first 2 characters "./" so it's a relative path from the project folder
                string fileName = file.Substring(2);

                FileUtil.CopyFile(fileName, fileName, true, EditorSettings.lineEndingsForNewScripts);
            }
        }

        [MenuItem("Lost/Actions/Create Warnings As Errors File")]
        public static void GenerateWarngingsAsErrorsFile()
        {
            string mcsFilePath = "Assets/mcs.rsp";
            string warningsAsErrors = "-warnaserror+";

            if (File.Exists(mcsFilePath))
            {
                StringBuilder fileContents = new StringBuilder();
                fileContents.AppendLine(warningsAsErrors);

                foreach (var line in File.ReadAllLines(mcsFilePath))
                {
                    if (line.Trim() == warningsAsErrors)
                    {
                        return; // no need to generate the file since the warnings as error command exists
                    }
                    else
                    {
                        fileContents.AppendLine(line);
                    }
                }

                // checking out the file
                if (Provider.enabled && Provider.isActive)
                {
                    Provider.Checkout(mcsFilePath, CheckoutMode.Asset).Wait();
                }

                try
                {
                    File.WriteAllText(mcsFilePath, fileContents.ToString());
                }
                catch
                {
                    Debug.LogErrorFormat("Unable to update file {0}.  Is it read only?", mcsFilePath);
                }
            }
            else
            {
                FileUtil.CreateFile(warningsAsErrors, mcsFilePath, true, EditorSettings.lineEndingsForNewScripts);
            }
        }

        [MenuItem("Lost/Actions/Remove Warnings As Errors")]
        public static void RemoveWarngingsAsErrorsFile()
        {
            string mcsFilePath = "Assets/mcs.rsp";
            string warningsAsErrors = "-warnaserror+";

            if (File.Exists(mcsFilePath))
            {
                StringBuilder fileContents = new StringBuilder();

                foreach (var line in File.ReadAllLines(mcsFilePath))
                {
                    // skip the warnings as errors line
                    if (line.Trim() == warningsAsErrors)
                    {
                        continue;
                    }

                    fileContents.AppendLine(line);
                }

                // if the file is empty, then we should just delete it
                bool shouldDeleteFile = string.IsNullOrEmpty(fileContents.ToString().Trim());

                if (Provider.enabled && Provider.isActive)
                {
                    if (shouldDeleteFile)
                    {
                        Provider.Delete(mcsFilePath).Wait();
                    }
                    else
                    {
                        Provider.Checkout(mcsFilePath, CheckoutMode.Asset).Wait();
                    }
                }

                try
                {
                    if (shouldDeleteFile)
                    {
                        // making sure it still exists (Provider.Delete might have already deleted it)
                        if (File.Exists(mcsFilePath))
                        {
                            File.Delete(mcsFilePath);
                        }
                    }
                    else
                    {
                        File.WriteAllText(mcsFilePath, fileContents.ToString());
                    }
                }
                catch
                {
                    Debug.LogErrorFormat("Unable to update file {0}.  Is it read only?", mcsFilePath);
                }

                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Lost/Actions/Remove Empty Directories")]
        public static void RemoveEmptyDirectories()
        {
            FileUtil.RemoveEmptyDirectories("Assets");
        }

        [MenuItem("Lost/Generate/Git Ignore File")]
        public static void GenerateGitIgnoreFile()
        {
            GenerateFile("gitignore", ".gitignore", "fae63426d3cf11c4cb39244488e2ec17");
        }

        [MenuItem("Lost/Generate/P4 Ignore File")]
        public static void GenerateP4IgnoreFile()
        {
            GenerateFile("p4ignore", ".p4ignore", "6d6c8d3e6aeaff34d89c7f2be0a80a0d");
        }

        [MenuItem("Lost/Generate/EditorConfig File")]
        public static void GenerateEditorConfigFile()
        {
            GenerateFile("editor config", ".editorconfig", "f6c774b1ff43524428c88bc6afaca2d7");
        }

        public static void GenerateFile(string displayName, string filePath, string guid)
        {
            var textAsset = EditorUtil.GetAssetByGuid<TextAsset>(guid);

            if (textAsset != null)
            {
                if (File.Exists(filePath) == false)
                {
                    FileUtil.CreateFile(textAsset.text, filePath, false, EditorSettings.lineEndingsForNewScripts);
                }
                else
                {
                    try
                    {
                        FileUtil.CopyFile(textAsset, filePath, false, EditorSettings.lineEndingsForNewScripts);
                    }
                    catch
                    {
                        Debug.LogErrorFormat("Unable to update {0} file. Is it read only?", displayName);
                    }
                }
            }
            else
            {
                Debug.LogErrorFormat("Unable to find {0} asset file!", displayName);
            }
        }

        private static bool AreByteArraysEqual(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
