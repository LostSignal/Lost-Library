//-----------------------------------------------------------------------
// <copyright file="CatalogsEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(Catalogs))]
    public class CatalogsEditor : Editor
    {
        private string[] catalogVersions;
        private int activeCatalogIndex;

        public void OnEnable()
        {
            var catalogs = this.target as Catalogs;
            this.catalogVersions = catalogs.AllCatalogs.Select(x => x.Version).ToArray();
            this.activeCatalogIndex = 0;
        }

        public override void OnInspectorGUI()
        {
            this.DrawDefaultInspector();

            GUILayout.Label("");

            var catalogs = target as Catalogs;

            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("Selected Catalog", GUILayout.Width(120));
                this.activeCatalogIndex = EditorGUILayout.Popup(this.activeCatalogIndex, this.catalogVersions, GUILayout.Width(80));
            }

            GUILayout.Label("");

            if (GUILayout.Button("Edit Catalog", GUILayout.Width(200)))
            {
                var window = EditorWindow.GetWindow<CatalogEditor>();
                window.SetActiveCatalog(catalogs, catalogs.GetCatalog(this.catalogVersions[this.activeCatalogIndex]));
                window.Show();
            }
            
            GUILayout.Label("");
        }
    }
}
