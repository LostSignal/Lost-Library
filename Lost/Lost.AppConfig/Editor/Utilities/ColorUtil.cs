//-----------------------------------------------------------------------
// <copyright file="LostEditorUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public static class ColorUtil
    {
        private static Dictionary<Color, Texture2D> colorCache = new Dictionary<Color, Texture2D>();

        public static bool IsProTheme()
        {
            return EditorGUIUtility.isProSkin;
        }

        public static Texture2D MakeColorTexture(Color col)
        {
            if (colorCache.ContainsKey(col))
            {
                return colorCache[col];
            }
            else
            {
                Color[] pix = new Color[1];
                pix[0] = col;

                Texture2D result = new Texture2D(1, 1);
                result.hideFlags = HideFlags.DontSave;
                result.SetPixels(pix);
                result.Apply();
                colorCache[col] = result;
                return result;
            }
        }
    }
}
