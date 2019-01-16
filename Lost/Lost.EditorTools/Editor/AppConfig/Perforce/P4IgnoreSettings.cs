//-----------------------------------------------------------------------
// <copyright file="P4IgnoreSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.IO;
    using System.Text;
    using Lost.AppConfig;
    using UnityEditor;
    using UnityEngine;

    [AppConfigSettingsOrder(220)]
    public class P4IgnoreSettings : AppConfigSettings
    {
        private static readonly string p4IngnoreEditorPref = "P4IgnoreFileName";

        #pragma warning disable 0649
        [SerializeField] private string p4IgnoreFileName = ".p4ignore";
        [SerializeField] private bool autoGenerateP4IgnoreFile = true;
        [SerializeField] private bool autosetP4IgnoreVariable = true;
        [SerializeField] private TextAsset p4IgnoreTemplate;

        [SerializeField, HideInInspector]
        private bool hasBeenInitialized;
        #pragma warning restore 0649

        public override string DisplayName => "P4 Ignore";
        public override bool IsInline => false;

        private void OnEnable()
        {
            if (this.hasBeenInitialized == false)
            {
                this.p4IgnoreTemplate = EditorUtil.GetAssetByGuid<TextAsset>("6d6c8d3e6aeaff34d89c7f2be0a80a0d");
                this.hasBeenInitialized = true;
            }
        }

        public override void InitializeOnLoad(AppConfig.AppConfig appConfig)
        {
            var settings = appConfig.GetSettings<P4IgnoreSettings>();

            if (settings == null)
            {
                return;
            }

            if (settings.autoGenerateP4IgnoreFile)
            {
                MenuItemTools.GenerateFile("p4ignore", settings.p4IgnoreFileName, settings.p4IgnoreTemplate.GetGuid());
            }

            if (settings.autosetP4IgnoreVariable)
            {
                this.SetP4IgnoreFileVariable(settings.p4IgnoreFileName);
            }
        }

        private void SetP4IgnoreFileVariable(string p4IgnoreFileName)
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

        private string GetCurrentP4IgnoreVariable()
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

        private string GetCurrentP4IgnoreVariableWindows()
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

        private string GetCurrentP4IgnoreVariableMac()
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

        private void SetP4IgnoreVariableForWindows(string p4ignoreFileName)
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

        private void SetP4IgnoreVariableForMac(string p4ignoreFileName)
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
