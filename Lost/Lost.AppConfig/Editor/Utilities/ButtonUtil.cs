//-----------------------------------------------------------------------
// <copyright file="LostEditorUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using UnityEngine;

    public static class ButtonUtil
    {
        private static Texture2D upTexture;
        private static Texture2D downTexture;
        private static Texture2D deleteTexture;
        private static Texture2D addTexture;

        private static GUIStyle buttonStyle;
        private static GUIContent addTextureGuiContent;
        private static GUIContent deleteTextureGuiContent;

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

        public static Texture2D AddTexture
        {
            get
            {
                if (!addTexture)
                {
                    addTexture = Resources.Load<Texture2D>("LostIcons/Add");
                }

                return addTexture;
            }
        }

        public static bool DrawDeleteButton(Rect buttonRect)
        {
            return GUI.Button(buttonRect, GetDeleteTextureGuiContent(), GetButtonStyle());
        }

        public static bool DrawDeleteButton(float size)
        {
            return GUILayout.Button(GetDeleteTextureGuiContent(), GetButtonStyle(), GUILayout.Width(size), GUILayout.Height(size));
        }

        public static bool DrawAddButton(Rect buttonRect)
        {
            return GUI.Button(buttonRect, GetAddTextureGuiContent(), GetButtonStyle());
        }

        private static GUIStyle GetButtonStyle()
        {
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle();
                buttonStyle.contentOffset = new Vector2(0, 3);
                buttonStyle.padding.top = 0;
                buttonStyle.padding.left = 0;
                buttonStyle.padding.right = 0;
                buttonStyle.padding.bottom = 0;
            }

            return buttonStyle;
        }

        private static GUIContent GetAddTextureGuiContent()
        {
            if (addTextureGuiContent == null)
            {
                addTextureGuiContent = new GUIContent(ButtonUtil.AddTexture);
            }

            return addTextureGuiContent;
        }

        private static GUIContent GetDeleteTextureGuiContent()
        {
            if (deleteTextureGuiContent == null)
            {
                deleteTextureGuiContent = new GUIContent(ButtonUtil.DeleteTexture);
            }

            return deleteTextureGuiContent;
        }
    }
}
