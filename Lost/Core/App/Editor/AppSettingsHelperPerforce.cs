//-----------------------------------------------------------------------
// <copyright file="AppSettingsHelperPerforce.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    public static partial class AppSettingsHelper
    {
        private static readonly string p4IngnoreEditorPref = "P4IgnoreFileName";

        public static void CreateOrOverwriteP4IgnoreFile()
        {
            AppSettings appSettings = GetAppSettings();

            string templateFileName = "p4ignore.txt";
            string p4IgnoreFileTemplate = FindLostFile(templateFileName);
            
            if (File.Exists(p4IgnoreFileTemplate) == false)
            {
                Debug.LogErrorFormat("Unable to create file {0}.  Couldn't find template file \"{1}\".", AppSettings.Instance.P4IgnoreFileName, templateFileName);
                return;
            }

            // checking if the P4Ignore file already exists
            if (File.Exists(AppSettings.Instance.P4IgnoreFileName) == false)
            {
                CreateFile(File.ReadAllText(p4IgnoreFileTemplate), AppSettings.Instance.P4IgnoreFileName, true, appSettings.ProjectLineEndings);
            }
            else
            {
                try
                {
                    CopyFile(p4IgnoreFileTemplate, AppSettings.Instance.P4IgnoreFileName, true, appSettings.ProjectLineEndings);
                }
                catch
                {
                    Debug.LogErrorFormat("Unable to update file {0}.  Is it read only?", p4IgnoreFileTemplate);
                }
            }
        }

        public static string GetCurrentP4IgnoreVariable()
        {
            var currentP4IgnoreFileName = EditorPrefs.GetString(p4IngnoreEditorPref, null);

            // checking to see if we already cached the value
            if (string.IsNullOrEmpty(currentP4IgnoreFileName))
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    currentP4IgnoreFileName = GetCurrentP4IgnoreVariableWindows();
                }
                else if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    currentP4IgnoreFileName = GetCurrentP4IgnoreVariableMac();
                }
                else
                {
                    Debug.LogError("Can't Get P4IGNORE Variable: Unsupported Platform.");
                }
            }

            EditorPrefs.SetString(p4IngnoreEditorPref, currentP4IgnoreFileName);
            return currentP4IgnoreFileName;
        }

        public static void SetP4IgnoreFileVariable(string p4IgnoreFileName)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                SetP4IgnoreVariableForWindows(p4IgnoreFileName);
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                SetP4IgnoreVariableForMac(p4IgnoreFileName);
            }
            else
            {
                Debug.LogError("Can't Set P4IGNORE Variable: Unsupported Platform.");
            }

            // invalidating the cached P4IGNORE variable
            EditorPrefs.SetString(p4IngnoreEditorPref, string.Empty);
        }
        
        private static string GetCurrentP4IgnoreVariableWindows()
        {
            try
            {
                var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "p4",
                    Arguments = "set P4IGNORE",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                });

                return process.StandardOutput.ReadToEnd().Replace("P4IGNORE=", string.Empty).Replace("(set)", string.Empty).Trim();
            }
            catch
            {
                return null;
            }
        }

        private static string GetCurrentP4IgnoreVariableMac()
        {
            var p4environmentFile = Path.Combine(System.Environment.GetEnvironmentVariable("HOME"), ".p4enviro");

            if (File.Exists(p4environmentFile))
            {
                foreach (var line in File.ReadAllLines(p4environmentFile))
                {
                    if (line.StartsWith("P4IGNORE=") && line.Length > 9)
                    {
                        return line.Substring(9).Trim();
                    }
                }
            }

            return null;
        }

        private static void SetP4IgnoreVariableForWindows(string p4ignoreFileName)
        {
            try
            {
                System.Diagnostics.Process.Start("p4", "set P4IGNORE=" + p4ignoreFileName);
            }
            catch
            {
                Debug.LogError("Unable To Set P4IGNORE Variable.  Is P4 installed?");
            }
        }

        private static void SetP4IgnoreVariableForMac(string p4ignoreFileName)
        {
            var p4environmentFile = Path.Combine(System.Environment.GetEnvironmentVariable("HOME"), ".p4enviro");
            var p4IgnoreLine = "P4IGNORE=" + p4ignoreFileName;
            var fileContents = new StringBuilder();
            var foundP4IgnoreLine = false;

            if (File.Exists(p4environmentFile))
            {
                foreach (var line in File.ReadAllLines(p4environmentFile))
                {
                    if (line.Trim().StartsWith("P4IGNORE="))
                    {
                        foundP4IgnoreLine = true;
                        fileContents.AppendLine(p4IgnoreLine);
                    }
                    else
                    {
                        fileContents.AppendLine(line);
                    }
                }
            }

            if (foundP4IgnoreLine == false)
            {
                fileContents.AppendLine(p4IgnoreLine);
            }

            Debug.Log("Updating P4IGNORE Settings.  You may need to restart P4V before settings take effect.");
            File.WriteAllText(p4environmentFile, fileContents.ToString());
        }
    }
}
