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
        [ReadOnly, SerializeField] private string guid;

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
