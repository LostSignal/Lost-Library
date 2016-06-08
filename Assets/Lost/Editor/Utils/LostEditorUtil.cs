//-----------------------------------------------------------------------
// <copyright file="LostEditorUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// 
    /// </summary>
    public static class LostEditorUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsProTheme()
        {
            return EditorGUIUtility.isProSkin;
        }

        //TODO it seems this cache isn't working, double check at a later time

        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<Color, Texture2D> colorCache = new Dictionary<Color, Texture2D>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static Texture2D MakeTexture(Color col)
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

        /// <summary>
        /// http://wiki.unity3d.com/index.php?title=CreateScriptableObjectAsset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void CreateScriptableObject<T>(string defaultName) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (path == string.Empty)
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != string.Empty)
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + defaultName + ".asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}
