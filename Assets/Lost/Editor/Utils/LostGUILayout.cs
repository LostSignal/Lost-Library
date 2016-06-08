//-----------------------------------------------------------------------
// <copyright file="LostGUILayout.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public static class LostGUILayout
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly float DeleteButtonWidth = 15;

        /// <summary>
        /// Draws a Vector2 Prperty within the given width (very compact).
        /// </summary>
        /// <param name="label"></param>
        /// <param name="property"></param>
        /// <param name="width"></param>
        public static void Vector2Property(string label, SerializedProperty property, float width)
        {
            Rect rect;
            using (new BeginHorizontalHelper(out rect))
            {
                if (string.IsNullOrEmpty(label) == false)
                {
                    EditorGUILayout.PrefixLabel(label);
                    width -= EditorGUIUtility.labelWidth;
                }

                rect.x = rect.x + EditorGUIUtility.labelWidth;
                rect.width = width;
                rect.height = 10;

                Vector2 newVector2 = EditorGUI.Vector2Field(rect, GUIContent.none, property.vector2Value);
                
                if (property.vector2Value != newVector2)
                {
                    property.vector2Value = newVector2;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        /// <summary>
        /// Draws a Vector3 Prperty within the given width (very compact).
        /// </summary>
        /// <param name="label"></param>
        /// <param name="property"></param>
        /// <param name="width"></param>
        public static void Vector3Property(string label, SerializedProperty property, float width)
        {
            Rect rect;
            using (new BeginHorizontalHelper(out rect))
            {
                if (string.IsNullOrEmpty(label) == false)
                {
                    EditorGUILayout.PrefixLabel(label);
                    width -= EditorGUIUtility.labelWidth;
                }

                rect.x = rect.x + EditorGUIUtility.labelWidth;
                rect.width = width;
                rect.height = 10;

                Vector3 newVector3 = EditorGUI.Vector3Field(rect, GUIContent.none, property.vector3Value);

                if (property.vector3Value != newVector3)
                {
                    property.vector3Value = newVector3;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="label"></param>
        /// <param name="weakPrefab"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static T WeakPrefab<T>(string label, T weakPrefab, float width) where T : WeakPrefab, new()
        {
            using (new BeginHorizontalHelper())
            {
                if (string.IsNullOrEmpty(label) == false)
                {
                    EditorGUILayout.PrefixLabel(label);
                    width -= EditorGUIUtility.labelWidth;
                }

                return PrivateDrawWeakPrefab<T>(weakPrefab, width);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weakPrefab"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private static T PrivateDrawWeakPrefab<T>(T weakPrefab, float width) where T : WeakPrefab, new()
        {
            if (weakPrefab == null)
            {
                Object newPrefab = EditorGUILayout.ObjectField(null, (new T()).PrefabeType, false, GUILayout.Width(width));

                if (newPrefab != null)
                {
                    weakPrefab = new T();
                    weakPrefab.Prefab = newPrefab;
                }
            }
            else
            {
                Object newPrefab = EditorGUILayout.ObjectField(weakPrefab.Prefab, weakPrefab.PrefabeType, false, GUILayout.Width(width));

                if (newPrefab != weakPrefab.Prefab)
                {
                    weakPrefab.Prefab = newPrefab;
                }
            }

            return weakPrefab;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="label"></param>
        /// <param name="weakPrefabs"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static T[] WeakPrefabArray<T>(string label, T[] weakPrefabs, float width) where T : WeakPrefab, new()
        {
            using (new BeginHorizontalHelper())
            {
                if (string.IsNullOrEmpty(label) == false)
                {
                    EditorGUILayout.PrefixLabel(label);
                    width -= EditorGUIUtility.labelWidth;
                }

                using (new BeginVerticalHelper())
                {
                    if (weakPrefabs != null)
                    {
                        for (int i = 0; i < weakPrefabs.Length; i++)
                        {
                            Rect rect;
                            using (new BeginHorizontalHelper(out rect))
                            {
                                T newWeakPrefab = PrivateDrawWeakPrefab<T>(weakPrefabs[i], width - DeleteButtonWidth);

                                if (newWeakPrefab != weakPrefabs[i])
                                {
                                    weakPrefabs[i] = newWeakPrefab;
                                }

                                // setting up the delete button for existing objs
                                rect.x += width - 10;
                                rect.y += 2;
                                rect.width = DeleteButtonWidth;
                                rect.height = 11;

                                if (GUI.Button(rect, "-"))
                                {
                                    List<T> objs = new List<T>(weakPrefabs);
                                    objs.RemoveAt(i);
                                    return objs.ToArray();
                                }
                            }
                        }
                    }

                    // drawing one more object field that doesn't exist, just in case we want to add one to the end of the array
                    T weakPrefabToAdd = PrivateDrawWeakPrefab<T>(null, width - DeleteButtonWidth);

                    if (weakPrefabToAdd != null)
                    {
                        if (weakPrefabs == null)
                        {
                            T[] newArray = new T[1];
                            newArray[0] = weakPrefabToAdd;
                            return newArray;
                        }
                        else
                        {
                            List<T> objs = new List<T>(weakPrefabs);
                            objs.Add(weakPrefabToAdd);
                            return objs.ToArray();
                        }
                    }
                }
            }

            return weakPrefabs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="objects"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static T[] ObjectsArray<T>(string label, T[] objects, float width) where T : Object
        {
            using (new BeginHorizontalHelper())
            {
                if (string.IsNullOrEmpty(label) == false)
                {
                    EditorGUILayout.PrefixLabel(label);
                    width -= EditorGUIUtility.labelWidth;
                }

                using (new BeginVerticalHelper())
                {
                    if (objects != null)
                    {
                        for (int i = 0; i < objects.Length; i++)
                        {
                            Rect rect;
                            using (new BeginHorizontalHelper(out rect))
                            {
                                T newObject = EditorGUILayout.ObjectField(objects[i], typeof(T), true, GUILayout.Width(width - DeleteButtonWidth)) as T;

                                if (newObject != objects[i])
                                {
                                    objects[i] = newObject;
                                }

                                // setting up the delete button for existing objs
                                rect.x += width - 10;
                                rect.y += 2;
                                rect.width = DeleteButtonWidth;
                                rect.height = 11;

                                if (GUI.Button(rect, "-"))
                                {
                                    List<T> objs = new List<T>(objects);
                                    objs.RemoveAt(i);
                                    return objs.ToArray();
                                }
                            }
                        }
                    }

                    // drawing one more object field that doesn't exist, just in case we want to add one to the end of the array
                    T objectToAdd = EditorGUILayout.ObjectField(null, typeof(T), true, GUILayout.Width(width - DeleteButtonWidth)) as T;

                    if (objectToAdd != null)
                    {
                        if (objects == null)
                        {
                            T[] newArray = new T[1];
                            newArray[0] = objectToAdd;
                            return newArray;
                        }
                        else
                        {
                            List<T> objs = new List<T>(objects);
                            objs.Add(objectToAdd);
                            return objs.ToArray();
                        }
                    }
                }
            }
            
            return objects;
        }
    }
}