//-----------------------------------------------------------------------
// <copyright file="BuildTargetGroupUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using System.Collections.Generic;
    using UnityEditor;

    public static class BuildTargetGroupUtil
    {
        private static List<BuildTargetGroup> validGroups = new List<BuildTargetGroup>()
        {
            BuildTargetGroup.Standalone,
            BuildTargetGroup.iOS,
            BuildTargetGroup.Android,
            BuildTargetGroup.WebGL,
            BuildTargetGroup.WSA,
            BuildTargetGroup.PS4,
            BuildTargetGroup.XboxOne,
            BuildTargetGroup.tvOS,
            BuildTargetGroup.Facebook,
            BuildTargetGroup.Switch,

            #if UNITY_2018_3_OR_NEWER
            BuildTargetGroup.Lumin,
            #endif
        };

        public static List<BuildTargetGroup> GetValid()
        {
            return validGroups;
        }
    }
}
