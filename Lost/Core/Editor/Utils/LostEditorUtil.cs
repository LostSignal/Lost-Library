//-----------------------------------------------------------------------
// <copyright file="LostEditorUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public static class LostEditorUtil
    {
        private static Dictionary<Color, Texture2D> colorCache = new Dictionary<Color, Texture2D>();
        private static Texture2D upTexture;
        private static Texture2D downTexture;
        private static Texture2D deleteTexture;

        public static Texture2D UpTexture
        {
            get
            {
                if (!upTexture)
                {
                    upTexture = Resources.Load<Texture2D>("LostIcons/Up");
                }

                return upTexture;
            }
        }

        public static Texture2D DownTexture
        {
            get
            {
                if (!downTexture)
                {
                    downTexture = Resources.Load<Texture2D>("LostIcons/Down");
                }

                return downTexture;
            }
        }

        public static Texture2D DeleteTexture
        {
            get
            {
                if (!deleteTexture)
                {
                    deleteTexture = Resources.Load<Texture2D>("LostIcons/Delete");
                }

                return deleteTexture;
            }
        }

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
