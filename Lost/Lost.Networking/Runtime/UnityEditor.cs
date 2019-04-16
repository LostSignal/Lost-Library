//-----------------------------------------------------------------------
// <copyright file="UnityEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY_2018_3_OR_NEWER

namespace UnityEditor
{
    public class MenuItemAttribute : System.Attribute
    {
        public MenuItemAttribute(string path)
        {
        }
    }
}

#endif
