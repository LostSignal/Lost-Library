//-----------------------------------------------------------------------
// <copyright file="SerializedPropertyExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Reflection;
    using UnityEditor;

    public static class SerializedPropertyExtensions
    {
        public static Type GetSerializedPropertyType(this SerializedProperty serializedProperty)
        {
            return GetTypeRecursive(serializedProperty.propertyPath.Split('.'), 0, serializedProperty.serializedObject.targetObject.GetType());
        }

        private static Type GetTypeRecursive(string[] propertyPath, int currentIndex, Type currentType)
        {
            if (currentIndex < propertyPath.Length)
            {
                string currentFieldName = propertyPath[currentIndex];

                FieldInfo currentFieldInfo = currentType.GetField(currentFieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                do
                {
                    currentFieldInfo = currentType.GetField(currentFieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    currentType = currentType.BaseType;
                }
                while (currentFieldInfo == null && currentType != null);

                bool isArray = propertyPath.Length > currentIndex + 1 && propertyPath[currentIndex + 1] == "Array";

                if (isArray)
                {
                    Type fieldType = currentFieldInfo.FieldType;
                    Type elementType = null;

                    if (fieldType.IsArray)
                    {
                        elementType = fieldType.GetElementType();
                    }
                    else
                    {
                        elementType = fieldType.GenericTypeArguments[0];
                    }

                    return GetTypeRecursive(propertyPath, currentIndex + 3, elementType);
                }
                else
                {
                    return GetTypeRecursive(propertyPath, currentIndex + 1, currentFieldInfo.FieldType);
                }
            }
            else
            {
                return currentType;
            }
        }
    }
}
