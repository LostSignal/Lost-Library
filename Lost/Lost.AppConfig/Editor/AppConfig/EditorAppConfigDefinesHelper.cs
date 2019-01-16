//-----------------------------------------------------------------------
// <copyright file="EditorAppConfigDefinesHelper.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;

    public static class EditorAppConfigDefinesHelper
    {
        public static void UpdateProjectDefines()
        {
            if (EditorAppConfig.ActiveAppConfig == null || EditorAppConfig.AppConfigs == null)
            {
                return;
            }

            HashSet<string> activeDefines = new HashSet<string>();
            HashSet<string> definesToRemove = new HashSet<string>();

            GetActiveDefines(EditorAppConfig.ActiveAppConfig, activeDefines);
            GetAllDefines(EditorAppConfig.AppConfigs, definesToRemove);

            foreach (var define in activeDefines)
            {
                definesToRemove.Remove(define);
            }

            UpdateProjectDefines(activeDefines, definesToRemove);
        }

        public static void UpdateProjectDefines(HashSet<string> definesToAdd, HashSet<string> definesToRemove)
        {
            foreach (var buildTargetGroup in BuildTargetGroupUtil.GetValid())
            {
                string currentDefinesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                string definesString = GetDefinesString(buildTargetGroup, definesToAdd, definesToRemove);

                if (currentDefinesString != definesString)
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, definesString);
                }
            }
        }

        private static void GetActiveDefines(AppConfig appConfig, HashSet<string> defines)
        {
            if (appConfig != null)
            {
                if (appConfig.Defines != null)
                {
                    foreach (var define in appConfig.Defines)
                    {
                        defines.Add(define);
                    }
                }

                GetActiveDefines(appConfig.Parent, defines);
            }
        }

        private static void GetAllDefines(List<AppConfig> appConfigs, HashSet<string> defines)
        {
            foreach (var appConfig in appConfigs)
            {
                if (appConfig != null && appConfig.Defines != null)
                {
                    foreach (var define in appConfig.Defines)
                    {
                        defines.Add(define);
                    }
                }
            }
        }

        private static string GetDefinesString(BuildTargetGroup buildTargetGroup, HashSet<string> definesToAdd, HashSet<string> definesToRemove)
        {
            var currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';').ToList();

            foreach (var define in definesToAdd)
            {
                if (currentDefines.Contains(define) == false)
                {
                    currentDefines.Add(define);
                }
            }

            foreach (var define in definesToRemove)
            {
                if (currentDefines.Contains(define))
                {
                    currentDefines.Remove(define);
                }
            }

            currentDefines.Sort();

            return string.Join(";", currentDefines);
        }

    }
}
