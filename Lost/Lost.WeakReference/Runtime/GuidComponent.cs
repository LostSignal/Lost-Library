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
            this.GenerateGuid(false);
        }

        private void Awake()
        {
            #if UNITY_EDITOR
            while (GuidManager.Instance.DoesGuidExist(this))
            {
                this.GenerateGuid(true);
            }
            #endif

            GuidManager.Instance.RegisterGuid(this);
        }

        private void OnDestroy()
        {
            GuidManager.Instance.UnregisterGuid(this);
        }

        private void GenerateGuid(bool forceRegenerateGuid)
        {
            if (this.guid == null || forceRegenerateGuid)
            {
                this.guid = System.Guid.NewGuid().ToString();
            }
        }
    }
}
