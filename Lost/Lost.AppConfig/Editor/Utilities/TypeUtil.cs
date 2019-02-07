//-----------------------------------------------------------------------
// <copyright file="TypeUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using System;
    using System.Collections.Generic;

    public static class TypeUtil
    {
        private static Dictionary<Type, HashSet<Type>> typesCache = new Dictionary<Type, HashSet<Type>>();

        public static IEnumerable<Type> GetAllTypesOf<T>()
        {
            Type type = typeof(T);
            HashSet<Type> types;

            if (typesCache.TryGetValue(type, out types) == false)
            {
                lock (string.Intern(type.FullName))
                {
                    if (typesCache.TryGetValue(type, out types) == false)
                    {
                        types = new HashSet<Type>();

                        foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                        {
                            foreach (Type assemblyType in assembly.GetTypes())
                            {
                                if (type.IsAssignableFrom(assemblyType) &&
                                    assemblyType.IsInterface == false &&
                                    assemblyType.IsAbstract == false &&
                                    types.Contains(assemblyType) == false)
                                {
                                    types.Add(assemblyType);
                                }
                            }
                        }

                        typesCache.Add(type, types);
                    }
                }
            }

            foreach (Type t in types)
            {
                yield return t;
            }
        }

        public static Type GetTypeByName<T>(string typeName)
        {
            foreach (Type t in TypeUtil.GetAllTypesOf<LazyAsset>())
            {
                if (t.Name == typeName)
                {
                    return t;
                }
            }

            return null;
        }
    }
}
