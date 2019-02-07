//-----------------------------------------------------------------------
// <copyright file="LazyGameObjectPropertyDrawer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_ADDRESSABLES

namespace Lost
{
    using Lost.AppConfig;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(WeakReference), true)]
    public class WeakReferencePropertyDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, GuidComponent> guidComponentCache = new Dictionary<string, GuidComponent>();
        private static readonly Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 16;
        }

        private Type GetType(SerializedProperty property)
        {
            Type type;

            if (typeCache.TryGetValue(property.type, out type) == false)
            {
                Type propertyType = TypeUtil.GetTypeByName<WeakReference>(property.type);
                object propertyObject = Activator.CreateInstance(propertyType);
                WeakReference weakReference = propertyObject as WeakReference;

                type = weakReference.Type;
                typeCache.Add(property.type, type);
            }

            return type;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty guid = property.FindPropertyRelative("guid");
            System.Type type = this.GetType(property);

            GuidComponent currentGuidComponent = this.GetGuidComponent(guid.stringValue);
            var currentComponent = currentGuidComponent?.GetComponent(type);

            var newValue = EditorGUI.ObjectField(position, label, currentComponent, type, true);

            if (currentGuidComponent != newValue)
            {
                if (newValue is GameObject)
                {
                    var gameObject = newValue as GameObject;
                    var guidComponent = gameObject.GetComponent<GuidComponent>();

                    // Making sure the object has a GuidComponent
                    if (guidComponent == null)
                    {
                        guidComponent = gameObject.AddComponent<GuidComponent>();
                    }

                    // Caching the guid component for easier lookup
                    if (guidComponentCache.ContainsKey(guidComponent.Guid) == false)
                    {
                        guidComponentCache.Add(guidComponent.Guid, guidComponent);
                    }

                    guid.stringValue = guidComponent.Guid;
                }
                else if (newValue is Component)
                {
                    var component = newValue as Component;
                    var guidComponent = component.gameObject.GetComponent<GuidComponent>();

                    // Making sure the object has a GuidComponent
                    if (guidComponent == null)
                    {
                        guidComponent = component.gameObject.AddComponent<GuidComponent>();
                    }

                    // Caching the guid component for easier lookup
                    if (guidComponentCache.ContainsKey(guidComponent.Guid) == false)
                    {
                        guidComponentCache.Add(guidComponent.Guid, guidComponent);
                    }

                    guid.stringValue = guidComponent.Guid;
                }
                else
                {
                    Debug.LogErrorFormat("WeakReferencePropertyDrawer found unknown type {0}", newValue.GetType());
                }
            }
        }

        private GuidComponent GetGuidComponent(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            if (guidComponentCache.TryGetValue(guid, out GuidComponent guidComponent))
            {
                return guidComponent;
            }

            return null;
        }
    }
}

#endif
