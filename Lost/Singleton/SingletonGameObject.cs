//-----------------------------------------------------------------------
// <copyright file="SingletonGameObject.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public abstract class SingletonGameObject<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static object instanceLock = new object();
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (IsInitialized == false)
                {
                    Initialize();
                }

                return instance;
            }
        }
        
        public static bool IsInitialized
        {
            get { return instance != null; }
        }

        /// <summary>
        ///  Must be called before you use this Singleton GameObject.
        /// </summary>
        public static void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            // highly unlikely this would need a lock since all unity calls need to happen on the main thread, but hey, being safe
            lock (instanceLock)
            {
                if (IsInitialized)
                {
                    return;
                }

                Transform singletonContainer = SingletonUtil.GetSingletonContainer();

                T[] childSingleton = singletonContainer.GetComponentsInChildren(typeof(T)) as T[];

                if (childSingleton != null && childSingleton.Length != 0)
                {
                    instance = childSingleton[0];
                    Logger.LogError(instance, "Singleton {0} already had an instance!  Taking the first one we found.", typeof(T).Name);
                }
                else
                {
                    GameObject childObject = new GameObject(typeof(T).Name);
                    childObject.transform.position = Vector3.zero;
                    childObject.transform.rotation = Quaternion.identity;
                    childObject.transform.localScale = Vector3.one;
                    childObject.transform.parent = singletonContainer;

                    //// BUG For some reason a singleton can't receive a GameObject.SendMessage if you set the HideFlags.DontSave
                    //// childObject.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
                    //// childObject.hideFlags = HideFlags.DontSaveInEditor;

                    instance = childObject.AddComponent<T>();
                }
            }
        }

        /// <summary>
        /// Called when object starts up, makes sure only one instance is created.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Logger.LogError(instance, "Singleton GameObject {0} has been created multiple times!", typeof(T).Name);
            }
        }
    }
}
