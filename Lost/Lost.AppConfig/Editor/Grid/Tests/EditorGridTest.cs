//-----------------------------------------------------------------------
// <copyright file="EditorGridTest.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using UnityEditor;
    using UnityEngine;

    //// [CustomEditor(typeof(GridTestObject))]
    public class EditorGridTest : Editor
    {
        private const int ColumnCount = 10;
        private static EditorGrid grid;

        // Use this for initialization
        static EditorGridTest()
        {
            var gridDefinition = new EditorGridDefinition();

            for (int i = 0; i < ColumnCount; i++)
            {
                gridDefinition.AddColumn("Column " + (i + 1), 100);
            }

            grid = new EditorGrid(gridDefinition);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            using (new BeginGridHelper(grid))
            {
                using (new BeginGridRowHelper(grid))
                {
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        grid.DrawFloat(i + 1.0f);
                    }
                }

                using (new BeginGridRowHelper(grid))
                {
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        grid.DrawInt(i + 1);
                    }
                }

                using (new BeginGridRowHelper(grid))
                {
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        grid.DrawBool(i % 2 == 0);
                    }
                }

                using (new BeginGridRowHelper(grid))
                {
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        grid.DrawColor(Color.white);
                    }
                }

                using (new BeginGridRowHelper(grid))
                {
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        grid.DrawLabel("Label " + (i + 1));
                    }
                }

                using (new BeginGridRowHelper(grid))
                {
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        grid.DrawObject(null, true);
                    }
                }

                using (new BeginGridRowHelper(grid))
                {
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        grid.DrawSprite(null, true);
                    }
                }

                using (new BeginGridRowHelper(grid))
                {
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        grid.DrawString("String " + (i + 1));
                    }
                }

                using (new BeginGridRowHelper(grid))
                {
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        grid.DrawVector3(new Vector3(i + 1, i + 1, i + 1));
                    }
                }
            }
        }
    }
}
