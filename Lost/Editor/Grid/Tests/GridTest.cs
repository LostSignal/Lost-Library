//-----------------------------------------------------------------------
// <copyright file="GridTest.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    //// [CustomEditor(typeof(GridTestObject))]
    public class GridTest : Editor
    {
        private const int ColumnCount = 10;
        private static Grid grid;
        
        // Use this for initialization
        static GridTest()
        {
            var gridDefinition = new GridDefinition();

            for (int i = 0; i < ColumnCount; i++)
            {
                gridDefinition.AddColumn("Column " + (i + 1), 100);
            }
            
            grid = new Grid(gridDefinition);
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
