//-----------------------------------------------------------------------
// <copyright file="GuidComponent.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    [ExecuteInEditMode]
    public class GuidComponent : MonoBehaviour
    {
        #pragma warning disable 0649
        [ReadOnly, SerializeField] private string guid;
        #pragma warning restore 0649

        public string Guid => this.guid;

        private void OnValidate()
        {
        }

        private void Awake()
        {
            #if UNITY_EDITOR
            if (this.guid == null)
            {
                this.guid = System.Guid.NewGuid().ToString();
            }
            #endif

            if (Application.isPlaying)
            {
                GuidManager.Instance.RegisterGuid(this);
            }
        }

        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                GuidManager.Instance.UnregisterGuid(this);
            }
        }
    }
}
