//-----------------------------------------------------------------------
// <copyright file="UnityEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY

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
