//-----------------------------------------------------------------------
// <copyright file="Attachment.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    [ExecuteInEditMode]
    public class Attachment : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private GameObject objectToAttachTo;
        [SerializeField] private Vector3 offset;
        [SerializeField] private bool positionOnly = true;
        #pragma warning restore 0649
        
        public GameObject ObjectToAttachTo
        {
            get { return this.objectToAttachTo; }
            set { this.objectToAttachTo = value; }
        }

        public Vector3 Offset
        {
            get { return this.offset; }
            set { this.offset = value; }
        }
        
        public bool PositionOnly
        {
            get { return this.positionOnly; }
            set { this.positionOnly = value; }
        }
        
        public void LateUpdate()
        {
            if (this.objectToAttachTo != null)
            {
                if (this.positionOnly)
                {
                    Vector3 newPostion = this.objectToAttachTo.transform.position + this.offset;

                    if (this.transform.position != newPostion)
                    {
                        this.transform.position = newPostion;
                    }
                }
                else
                {
                    Matrix4x4 mat = this.objectToAttachTo.transform.localToWorldMatrix * Matrix4x4.TRS(this.offset, this.transform.rotation, this.transform.localScale);
                    this.transform.position = mat.GetPosition();
                    this.transform.rotation = mat.GetRotation();
                    this.transform.localScale = mat.GetScale();
                }
            }
        }
    }
}
