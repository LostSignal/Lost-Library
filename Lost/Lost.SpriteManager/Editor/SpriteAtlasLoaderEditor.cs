//-----------------------------------------------------------------------
// <copyright file="SpriteAtlasLoaderEditor.cs" company="Big Blind Interactive">
//     Copyright (c) Big Blind Interactive. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.U2D;

    [CustomEditor(typeof(SpriteAtlasLoader))]    public class SpriteAtlasLoaderEditor : Editor    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Find Atlases"))
            {
                var spriteAtlasManager = this.target as Lost.SpriteAtlasLoader;
                spriteAtlasManager.Atlases.Clear();

                var atlasGuids = AssetDatabase.FindAssets("t:SpriteAtlas");

                foreach (var atlasGuid in atlasGuids)
                {
                    string atlasPath = AssetDatabase.GUIDToAssetPath(atlasGuid);
                    var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);

                    // Figuring out if this is included in the build
                    var atlasSerializedObject = new SerializedObject(atlas);
                    var bindAsDefault = atlasSerializedObject.FindProperty("m_EditorData.bindAsDefault");
                    bool includeInBuild = bindAsDefault.boolValue;

                    if (includeInBuild == false)
                    {
                        spriteAtlasManager.Atlases.Add(new Lost.SpriteAtlasLoader.Atlas
                        {
                            Tag = atlas.tag,
                            Guid = atlasGuid,
                        });
                    }
                }

                EditorUtility.SetDirty(this.target);
            }
        }    }}