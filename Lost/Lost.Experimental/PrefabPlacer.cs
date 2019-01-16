//-----------------------------------------------------------------------
// <copyright file="PrefabPlacer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    [ExecuteInEditMode]
    public class PrefabPlacer : MonoBehaviour, ISerializationCallbackReceiver
    {
        #pragma warning disable 0649
        private GameObject prefab;
        #pragma warning restore 0649

        private GameObject prefabInstance;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.Initialize();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (this.prefabInstance != null)
            {
                Pooler.DestroyImmediate(this.prefabInstance);
                this.prefabInstance = null;
            }
        }

        private void Update()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            if (this.prefabInstance == null && this.prefab != null)
            {
                this.prefabInstance = Pooler.Instantiate(this.prefab);
                this.prefabInstance.hideFlags = HideFlags.HideAndDontSave;
                this.prefabInstance.transform.SetParent(this.transform);
                this.prefabInstance.transform.localPosition = Vector3.zero;
            }
        }
    }

    //// public GameObject Prefab;
    //// private GameObject prefabInstance;
    //// private GameObject currentSelection;
    ////
    //// #if UNITY_EDITOR
    //// void Update()
    //// {
    ////     if (Application.isPlaying == false && this.prefabInstance == null && this.Prefab != null)
    ////     {
    ////         this.prefabInstance = Pooler.Instantiate<GameObject>(this.Prefab);
    ////         this.prefabInstance.hideFlags = HideFlags.HideAndDontSave;
    ////         this.prefabInstance.transform.SetParent(this.transform);
    ////         this.prefabInstance.transform.localPosition = Vector3.zero;
    ////     }
    //// }
    //// #endif
    ////
    //// void Awake()
    //// {
    ////     if (Application.isPlaying)
    ////     {
    ////         var instance = Pooler.Instantiate<GameObject>(this.Prefab);
    ////         instance.name = this.name;
    ////         instance.transform.SetParent(this.transform.parent);
    ////         instance.transform.localPosition = this.transform.localPosition;
    ////         instance.transform.localRotation = this.transform.localRotation;
    ////         instance.transform.localScale = this.transform.localScale;
    ////
    ////         int index = this.transform.GetSiblingIndex();
    ////         Pooler.DestroyImmediate(this.gameObject);
    ////         instance.transform.SetSiblingIndex(index);
    ////     }
    //// }
}
