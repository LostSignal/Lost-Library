//-----------------------------------------------------------------------
// <copyright file="AppSettingsHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using UnityEditor;
    using UnityEditor.VersionControl;
    using UnityEngine;

    public static partial class AppSettingsHelper
    {
        #if !UNITY_CLOUD_BUILD
        [InitializeOnLoadMethod]
        public static void InitializeProjectSettings()
        {
            // making sure ForceText is on if this project uses source control
            if (AppSettings.Instance.SourceControl != SourceControlType.None && EditorSettings.serializationMode != SerializationMode.ForceText)
            {
                EditorSettings.serializationMode = SerializationMode.ForceText;
            }
            
            // testing if we should set the p4ignore variable
            if (AppSettings.Instance.UseP4IgnoreFile && AppSettings.Instance.SetP4IgnoreVariableAtStartup && AppSettings.Instance.P4IgnoreFileName != GetCurrentP4IgnoreVariable())
            {
                SetP4IgnoreFileVariable(AppSettings.Instance.P4IgnoreFileName);
            }

            if (AppSettings.Instance.WarningsAsErrors)
            {
                GenerateWarngingsAsErrorsFile();
            }
            else
            {
                RemoveWarngingsAsErrorsFile();
            }
            
            if (AppSettings.Instance.OverrideTemplateCShardFiles)
            {
                OverrideTemplateCSharpFiles();
            }
        }
        #endif

        [MenuItem("Lost/Actions/Override C# Template Files")]
        public static void OverrideTemplateCSharpFiles()
        {
            string unityDirectory = null;
            string templateFilesDirectory = null;

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                unityDirectory = Path.GetDirectoryName(System.Environment.CommandLine);
                templateFilesDirectory = Path.Combine(unityDirectory, "Data\\Resources\\ScriptTemplates");
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                unityDirectory = Path.GetDirectoryName(System.Environment.CommandLine);
                unityDirectory = Path.GetDirectoryName(unityDirectory);  // going back one more directory
                templateFilesDirectory = Path.Combine(unityDirectory, "Resources/ScriptTemplates");
            }
            else
            {
                Debug.LogError("Unable to override template files.  Unsupported Editor.");
                return;
            }
            
            if (Directory.Exists(templateFilesDirectory) == false)
            {
                Debug.LogError("Unable to override template files, couldn't find unity template directory.");
                return;
            }

            var templateFiles = GetAllUnityTemplateFiles();

            if (templateFiles.Count == 0)
            {
                Debug.LogError("Unable to override template files, couldn't find any template files.");
                return;
            }

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                var firstFile = new FileInfo(templateFiles.First());
                var sourceFolder = Path.GetDirectoryName(firstFile.FullName);
                var destinationFolder = templateFilesDirectory;
                string[] fileNames = templateFiles.Select(x => "\"" + Path.GetFileName(x) + "\"").ToArray();

                bool allFilesEqual = templateFiles.All(x =>
                {
                    return File.ReadAllText(x) == File.ReadAllText(Path.Combine(destinationFolder, Path.GetFileName(x)));
                });

                // no need to update since all files are equal
                if (allFilesEqual)
                {
                    return;
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "robocopy", Verb = "runas", Arguments = string.Format("\"{0}\" \"{1}\" {2}", sourceFolder, destinationFolder, string.Join(" ", fileNames))
                });
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                foreach (var templateFile in templateFiles)
                {
                    CopyFile(templateFile, Path.Combine(templateFilesDirectory, Path.GetFileName(templateFile)), false);
                }
            }
            else
            {
                Debug.LogError("Unable to override template files, couldn't find unity template directory.");
            }
        }

        [MenuItem("Lost/Actions/Convert all C# files to project line endings")]
        public static void ConvertAllCSharpFileLineEndings()
        {
            foreach (string file in Directory.GetFiles(".", "*.cs", SearchOption.AllDirectories))
            {
                // removing the first 2 characters "./" so it's a relative path from the project folder
                string fileName = file.Substring(2);

                CopyFile(fileName, fileName, true);
            }
        }

        [MenuItem("Lost/Actions/Generate Warnings As Errors File")]
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
                CreateFile(warningsAsErrors, mcsFilePath, true);
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
            RemoveEmptyDirectories("Assets");
        }

        public static void CreateFile(string contents, string destinationFile, bool sourceControlAdd)
        {
            string fileContents = ConvertLineEndings(contents);

            // actually writing out the contents
            File.WriteAllText(destinationFile, fileContents);

            // telling source control to add the file
            if (sourceControlAdd && Provider.enabled && Provider.isActive)
            {
                AssetDatabase.Refresh();
                Provider.Add(new Asset(destinationFile), false).Wait();
            }
        }

        public static void CopyFile(string sourceFile, string destinationFile, bool sourceControlCheckout)
        {
            if (File.Exists(sourceFile) == false)
            {
                Debug.LogErrorFormat("Unable to copy file {0} to {1}.  Source file does not exist!", sourceFile, destinationFile);
            }

            string fileContents = ConvertLineEndings(File.ReadAllText(sourceFile));

            if (fileContents != File.ReadAllText(destinationFile))
            {
                // checking out the file
                if (sourceControlCheckout && Provider.enabled && Provider.isActive)
                {
                    Provider.Checkout(destinationFile, CheckoutMode.Asset).Wait();
                }
                
                // actually writing out the contents
                File.WriteAllText(destinationFile, fileContents);
            }
        }

        public static string ConvertLineEndings(string inputText)
        {
            // checking for a really messed up situation that happens when mixing max/pc sometimes
            if (inputText.Contains("\r\r\n"))
            {
                inputText = inputText.Replace("\r\r\n", "\n");
            }
            
            // if it has windows line escaping, then convert everything to Unix
            if (inputText.Contains("\r\n"))
            {
                inputText = inputText.Replace("\r\n", "\n");
            }

            if (AppSettings.Instance.ProjectLineEndings == LineEndings.Windows)
            {
                // convert all unix to windows
                inputText = inputText.Replace("\n", "\r\n");
            }
            else if (AppSettings.Instance.ProjectLineEndings == LineEndings.Unix)
            {
                // do nothing, already in Unix
            }
            else
            {
                Debug.LogErrorFormat("Unable to convert line endings, unknown line ending type found: {0}", LineEndings.Unix);
            }

            return inputText;
        }

        private static string FindLostFile(string fileName)
        {
            foreach (var filePath in Directory.GetFiles(".", "*", SearchOption.AllDirectories))
            {
                if (Path.GetFileName(filePath) == fileName)
                {
                    // making sure this file lives in the Lost folder
                    var newFilePath = filePath.Replace("\\", "/");
                    if (newFilePath.Contains("/Lost/"))
                    {
                        return newFilePath;
                    }
                }
            }

            return null;
        }
        
        private static List<string> GetAllUnityTemplateFiles()
        {
            List<string> templateFiles = new List<string>();

            foreach (var templateFile in Directory.GetFiles(".", "*.txt", SearchOption.AllDirectories))
            {
                var sanitizedTemplateFile = templateFile.Replace("\\", "/");

                if (sanitizedTemplateFile.Contains("/Lost/") && Path.GetDirectoryName(sanitizedTemplateFile).EndsWith("/TemplateFiles"))
                {
                    templateFiles.Add(sanitizedTemplateFile);
                }
            }

            return templateFiles;
        }

        private static void RemoveEmptyDirectories(string directory)
        {
            foreach (var childDirectory in Directory.GetDirectories(directory))
            {
                RemoveEmptyDirectories(childDirectory);
            }

            if (Directory.GetDirectories(directory).Length == 0 && Directory.GetFiles(directory).Length == 0)
            {
                Debug.LogFormat("Removing Directory: {0}", directory);
                AssetDatabase.MoveAssetToTrash(directory);
            }
        }
    }
}
