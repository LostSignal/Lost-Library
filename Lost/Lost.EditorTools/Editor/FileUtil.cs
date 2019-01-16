//-----------------------------------------------------------------------
// <copyright file="FileUtil.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.IO;
    using UnityEditor;
    using UnityEditor.VersionControl;
    using UnityEngine;

    public static class FileUtil
    {
        public static void CreateFile(string contents, string destinationFile, bool sourceControlAdd, LineEndingsMode lineEndings)
        {
            string fileContents = ConvertLineEndings(contents, lineEndings);

            // actually writing out the contents
            File.WriteAllText(destinationFile, fileContents);

            // telling source control to add the file
            if (sourceControlAdd && Provider.enabled && Provider.isActive)
            {
                AssetDatabase.Refresh();
                Provider.Add(new Asset(destinationFile), false).Wait();
            }
        }

        public static void CopyFile(string sourceFile, string destinationFile, bool sourceControlCheckout, LineEndingsMode lineEndings)
        {
            if (File.Exists(sourceFile) == false)
            {
                Debug.LogErrorFormat("Unable to copy file {0} to {1}.  Source file does not exist!", sourceFile, destinationFile);
            }

            string fileContents = ConvertLineEndings(File.ReadAllText(sourceFile), lineEndings);

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

        public static void CopyFile(TextAsset textAsset, string destinationFile, bool sourceControlCheckout, LineEndingsMode lineEndings)
        {
            string fileContents = ConvertLineEndings(textAsset.text, lineEndings);

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

        public static string ConvertLineEndings(string inputText, LineEndingsMode lineEndings)
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

            if (lineEndings == LineEndingsMode.Unix)
            {
                // do nothing, already in Unix
            }
            else if (lineEndings == LineEndingsMode.Windows)
            {
                // convert all unix to windows
                inputText = inputText.Replace("\n", "\r\n");
            }
            else if (lineEndings == LineEndingsMode.OSNative)
            {
                // convert all os native to windows
                inputText = inputText.Replace("\n", System.Environment.NewLine);
            }
            else
            {
                Debug.LogErrorFormat("Unable to convert line endings, unknown line ending type found: {0}", lineEndings);
            }

            return inputText;
        }

        public static void RemoveEmptyDirectories(string directory)
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
