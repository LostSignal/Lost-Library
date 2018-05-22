//-----------------------------------------------------------------------
// <copyright file="Grid.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class Grid
    {
        public static readonly int RowHeight = 20;
        public static readonly int Padding = 4;

        private static GUIStyle columnHeaderGuiStyle = null;
        private static GUIStyle rowGuiStyle = null;
        private static List<Color> backgroundColors = new List<Color>();

        private GridDefinition gridDefinition;
        private int currentColumnIndex = 0;
        private int currentRowIndex = 0;
        private float totalWidth;
        private int rowButtonCount;

        static Grid()
        {
            if (LostEditorUtil.IsProTheme())
            {
                backgroundColors.Add(new Color(0.204f, 0.094f, 0.094f));  // dark red
                backgroundColors.Add(new Color(0.208f, 0.176f, 0.106f));  // dark yellow
                backgroundColors.Add(new Color(0.180f, 0.204f, 0.208f));  // dark blue
            }
            else
            {
                backgroundColors.Add(new Color(0.612f, 0.282f, 0.282f));  // red
                backgroundColors.Add(new Color(0.624f, 0.528f, 0.318f));  // yellow
                backgroundColors.Add(new Color(0.540f, 0.612f, 0.624f));  // blue
            }
        }

        public static bool UpdateList<T>(GridButton gridButtonPressed, List<T> items, int currnentIndex)
        {
            var currentItem = items[currnentIndex];

            if (gridButtonPressed == GridButton.Delete)
            {
                items.Remove(currentItem);
                return true;
            }
            else if (gridButtonPressed == GridButton.MoveUp && currnentIndex != 0)
            {
                items.Remove(currentItem);
                items.Insert(currnentIndex - 1, currentItem);
                return true;
            }
            else if (gridButtonPressed == GridButton.MoveDown && currnentIndex != items.Count - 1)
            {
                items.Remove(currentItem);
                items.Insert(currnentIndex + 1, currentItem);

                return true;
            }

            return false;
        }

        public Grid(GridDefinition gridDefinition)
        {
            this.gridDefinition = gridDefinition;
            this.rowButtonCount = this.gridDefinition.RowButtonCount;
        }

        public GridButton RowButtonPressed { get; private set; }

        private static GUIStyle ColumnHeaderGuiStyle
        {
            get
            {
                if (columnHeaderGuiStyle == null)
                {
                    columnHeaderGuiStyle = new GUIStyle(GUI.skin.box);
                    columnHeaderGuiStyle.padding = new RectOffset(0, 0, 3, 3);
                    columnHeaderGuiStyle.margin = new RectOffset(0, 0, 0, 0);
                    columnHeaderGuiStyle.alignment = TextAnchor.LowerCenter;
                    columnHeaderGuiStyle.stretchWidth = true;

                    if (LostEditorUtil.IsProTheme())
                    {
                        columnHeaderGuiStyle.normal.textColor = Color.white;
                    }
                    else
                    {
                        columnHeaderGuiStyle.normal.background = LostEditorUtil.MakeColorTexture(new Color(0.7f, 0.7f, 0.7f));
                        columnHeaderGuiStyle.normal.textColor = Color.black;
                    }
                }

                return columnHeaderGuiStyle;
            }
        }

        private static GUIStyle RowGuiStyle
        {
            get
            {
                if (rowGuiStyle == null)
                {
                    rowGuiStyle = new GUIStyle();
                    rowGuiStyle.padding = new RectOffset(0, 0, 0, 0);
                    rowGuiStyle.margin = new RectOffset(0, 0, 1, 1);
                }

                return rowGuiStyle;
            }
        }

        private int RowButtonsWidth
        {
            get { return this.rowButtonCount == 0 ? 0 : (5 + (this.rowButtonCount * 27)); }
        }

        public void BeginGrid()
        {
            this.totalWidth = this.GetTotalCellWidth();

            if (this.gridDefinition.DrawHeader)
            {
                this.DrawHeader();
            }

            this.currentRowIndex = 0;
        }

        public void EndGrid()
        {
            //// private bool DrawAddButton(SerializedProperty property, float totalWidth)
            //// {
            ////     // drawing the add button at the bottom of the grid
            ////     Rect addButtonPosition;
            ////     using (new BeginVerticalHelper(out addButtonPosition, GUILayout.Width(totalWidth)))
            ////     {
            ////         EditorGUILayout.Space();
            ////         EditorGUILayout.Space();
            ////
            ////         addButtonPosition.x += Padding;
            ////         addButtonPosition.y += 3;
            ////         addButtonPosition.width = totalWidth;
            ////         addButtonPosition.height = 13;
            ////
            ////         if (GUI.Button(addButtonPosition, "+"))
            ////         {
            ////             property.InsertArrayElementAtIndex(property.arraySize);
            ////             property.serializedObject.ApplyModifiedProperties();
            ////             return true;
            ////         }
            ////     }
            ////
            ////     return false;
            //// }

            EditorGUILayout.Space();
        }

        public void BeginRow()
        {
            this.currentColumnIndex = 0;

            RowGuiStyle.normal.background = LostEditorUtil.MakeColorTexture(this.GetCurrentRowColor());

            EditorGUILayout.BeginHorizontal(RowGuiStyle, GUILayout.Height(RowHeight), GUILayout.Width(this.totalWidth));

            //// public BeginGridRowHelper(float width, int height, Color rowColor, out Rect position)
            //// {
            ////     GUIStyle newStyle = new GUIStyle();
            ////     newStyle.padding = new RectOffset();
            ////     newStyle.margin = new RectOffset();
            ////     newStyle.normal.background = LostEditorUtil.MakeTexture(rowColor);
            ////
            ////     position = EditorGUILayout.BeginHorizontal(newStyle, GUILayout.Height(height), GUILayout.Width(width));
            //// }
        }

        public void EndRow()
        {
            this.RowButtonPressed = this.DrawRowButtons();

            EditorGUILayout.EndHorizontal();
            this.currentRowIndex++;
        }

        #region Button Drawing

        public bool DrawAddButton()
        {
            return GUILayout.Button("+", GUILayout.Width(this.totalWidth - 5));
        }

        #endregion

        #region Cell Drawing

        public void DrawLabel(string value)
        {
            var column = this.GetNextColumn();
            GUILayout.Space(5);
            EditorGUILayout.LabelField(value, GUILayout.Width(column.Width - 10));
            GUILayout.Space(1);
        }

        public void DrawReadOnlyString(string stringValue)
        {
            GUI.enabled = false;
            this.DrawString(stringValue);
            GUI.enabled = true;
        }

        public string DrawString(string stringValue)
        {
            var column = this.GetNextColumn();

            GUILayout.Space(5);
            var newValue = EditorGUILayout.TextField(GUIContent.none, stringValue, GUILayout.Width(column.Width - 10));
            GUILayout.Space(1);

            return newValue;
        }

        public bool DrawBool(bool boolValue)
        {
            var column = this.GetNextColumn();

            float actualToggleControlWidth = 10.0f;
            float spaceWidth = (column.Width / 2) - (actualToggleControlWidth / 2);
            float toggleWidth = column.Width - spaceWidth;
            GUILayout.Space(spaceWidth - 2);

            return EditorGUILayout.Toggle(boolValue, GUILayout.Width(toggleWidth - 2));
        }

        public Color DrawColor(Color colorValue)
        {
            var column = this.GetNextColumn();

            GUILayout.Space(5);
            var newValue = EditorGUILayout.ColorField(colorValue, GUILayout.Width(column.Width - 10));
            GUILayout.Space(1);

            return newValue;
        }

        public void DrawReadOnlyFloat(float floatValue)
        {
            GUI.enabled = false;
            this.DrawFloat(floatValue);
            GUI.enabled = true;
        }

        public float DrawFloat(float floatValue)
        {
            return this.DrawFloat(floatValue, float.MinValue, float.MaxValue);
        }

        public float DrawFloat(float floatValue, float minValue, float maxValue)
        {
            var column = this.GetNextColumn();

            GUILayout.Space(5);
            float newValue = EditorGUILayout.FloatField(GUIContent.none, floatValue, GUILayout.Width(column.Width - 10));
            GUILayout.Space(1);

            return Mathf.Clamp(newValue, minValue, maxValue);
        }

        public void DrawReadOnlyInt(int intValue)
        {
            GUI.enabled = false;
            this.DrawInt(intValue);
            GUI.enabled = true;
        }

        public int DrawInt(int intValue)
        {
            return this.DrawInt(intValue, int.MinValue, int.MaxValue);
        }

        public int DrawInt(int intValue, int minValue, int maxValue)
        {
            var column = this.GetNextColumn();

            GUILayout.Space(5);
            int newValue = EditorGUILayout.IntField(GUIContent.none, intValue, GUILayout.Width(column.Width - 10));
            GUILayout.Space(1);

            return Mathf.Clamp(newValue, minValue, maxValue);
        }

        public string DrawPopup(string[] values, string currentValue)
        {
            var column = this.GetNextColumn();

            int currentIndex = Array.IndexOf(values, currentValue);

            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            GUILayout.Space(3);
            int newIndex = EditorGUILayout.Popup(currentIndex, values, GUILayout.Width(column.Width - 6));

            return newIndex != currentIndex ? values[newIndex] : currentValue;
        }

        public string DrawPopup(List<string> values, string currentValue)
        {
            return this.DrawPopup(values.ToArray(), currentValue);
        }

        public T DrawEnum<T>(T value) where T : struct, IConvertible
        {
            var column = this.GetNextColumn();

            var enumValues = Enum.GetValues(typeof(T));
            var enumNames = enumValues.OfType<T>().Select(x => x.ToString()).ToList();
            int currentIndex = enumNames.IndexOf(value.ToString());

            GUILayout.Space(3);
            int newIndex = EditorGUILayout.Popup(currentIndex, enumNames.ToArray(), GUILayout.Width(column.Width - 3));

            return newIndex != currentIndex ? (T)enumValues.GetValue(newIndex) : value;
        }

        public void DrawReadOnlyEnum<T>(T value) where T : struct, IConvertible
        {
            GUI.enabled = false;
            this.DrawEnum<T>(value);
            GUI.enabled = true;
        }

        public Sprite DrawSprite(Sprite value, bool allowSceneObjects)
        {
            var column = this.GetNextColumn();

            GUILayout.Space(5);
            var newValue = (Sprite)EditorGUILayout.ObjectField(GUIContent.none, value, typeof(Sprite), allowSceneObjects, GUILayout.Width(column.Width - 10));
            GUILayout.Space(1);

            return newValue;
        }

        public UnityEngine.Object DrawObject(Type type, UnityEngine.Object value, bool allowSceneObjects)
        {
            var column = this.GetNextColumn();

            GUILayout.Space(5);
            var newValue = EditorGUILayout.ObjectField(GUIContent.none, value, type, allowSceneObjects, GUILayout.Width(column.Width - 10), GUILayout.ExpandHeight(false));
            GUILayout.Space(1);

            return newValue;
        }

        public UnityEngine.Object DrawObject(UnityEngine.Object value, bool allowSceneObjects)
        {
            return this.DrawObject(typeof(UnityEngine.Object), value, allowSceneObjects);
        }

        public T DrawObject<T>(T value, bool allowSceneObjects) where T : UnityEngine.Object
        {
            return this.DrawObject(typeof(T), value, allowSceneObjects) as T;
        }

        public Vector3 DrawVector3(Vector3 value)
        {
            var column = this.GetNextColumn();

            using (new BeginHorizontalHelper())
            {
                float width = (column.Width / 3.0f) - 6;

                GUILayout.Space(5);
                float x = EditorGUILayout.FloatField(GUIContent.none, value.x, GUILayout.Width(width));
                float y = EditorGUILayout.FloatField(GUIContent.none, value.y, GUILayout.Width(width));
                float z = EditorGUILayout.FloatField(GUIContent.none, value.z, GUILayout.Width(width));

                return new Vector3(x, y, z);
            }
        }

        public object DrawUnknownObject(object obj)
        {
            if (obj is string)
            {
                return this.DrawString((string)obj);
            }
            else if (obj is int)
            {
                return this.DrawInt((int)obj);
            }
            else if (obj is float)
            {
                return this.DrawFloat((float)obj);
            }
            else if (obj is Vector3)
            {
                return this.DrawVector3((Vector3)obj);
            }
            else if (obj is bool)
            {
                return this.DrawBool((bool)obj);
            }
            else if (obj is Color)
            {
                return this.DrawColor((Color)obj);
            }
            else if (obj is UnityEngine.Object)
            {
                return this.DrawObject((UnityEngine.Object)obj, true);
            }
            else
            {
                Debug.LogErrorFormat("Grid.DrawUnknownObject was given an unknown object type {0}", obj.GetType().Name);
                return null;
            }
        }

        #endregion

        private void DrawHeader()
        {
            using (new BeginHorizontalHelper())
            {
                for (int i = 0; i < this.gridDefinition.ColumnCount; i++)
                {
                    var column = this.gridDefinition[i];

                    using (new BeginHorizontalHelper(GUILayout.MaxWidth(column.Width), GUILayout.MinWidth(column.Width)))
                    {
                        string tooltip = column.Tooltip;

                        if (string.IsNullOrEmpty(tooltip))
                        {
                            tooltip = column.Name;
                        }

                        EditorGUILayout.LabelField(new GUIContent(column.Name, tooltip), ColumnHeaderGuiStyle);
                    }
                }

                if (this.rowButtonCount != 0)
                {
                    using (new BeginHorizontalHelper(GUILayout.MaxWidth(this.RowButtonsWidth), GUILayout.MinWidth(this.RowButtonsWidth)))
                    {
                        EditorGUILayout.LabelField(string.Empty, ColumnHeaderGuiStyle);
                    }
                }
            }
        }

        private Color GetCurrentRowColor()
        {
            if (this.gridDefinition.AlternateColors)
            {
                return backgroundColors[this.currentRowIndex % backgroundColors.Count];
            }
            else
            {
                return LostEditorUtil.IsProTheme() ? new Color(0.298f, 0.298f, 0.298f) : new Color(0.867f, 0.867f, 0.867f);
            }
        }

        private GridDefinition.Column GetNextColumn()
        {
            return this.gridDefinition[this.currentColumnIndex++];
        }

        private GridButton DrawRowButtons()
        {
            // early out if nothing to draw
            if (this.rowButtonCount == 0)
            {
                return GridButton.None;
            }

            GUILayout.Space(5);

            if ((this.gridDefinition.RowButtons & GridButton.Delete) != 0 && GUILayout.Button(LostEditorUtil.DeleteTexture))
            {
                return GridButton.Delete;
            }

            if ((this.gridDefinition.RowButtons & GridButton.MoveUp) != 0 && GUILayout.Button(LostEditorUtil.UpTexture))
            {
                return GridButton.MoveUp;
            }

            if ((this.gridDefinition.RowButtons & GridButton.MoveDown) != 0 && GUILayout.Button(LostEditorUtil.DownTexture))
            {
                return GridButton.MoveDown;
            }

            GUILayout.Space(this.rowButtonCount == 1 ? 3 : 4);  // HACK [bgish]: to make 1 button look perfect

            return GridButton.None;
        }

        private float GetTotalCellWidth()
        {
            float total = 0;

            for (int i = 0; i < this.gridDefinition.ColumnCount; i++)
            {
                total += this.gridDefinition[i].Width;
            }

            total += this.RowButtonsWidth;

            return total;
        }
    }
}
