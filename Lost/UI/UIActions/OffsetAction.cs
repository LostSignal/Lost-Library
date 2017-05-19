//-----------------------------------------------------------------------
// <copyright file="OffsetAction.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    [AddComponentMenu("")]
    public class OffsetAction : UIAction
    {
        #pragma warning disable 0649
        [SerializeField] private Transform actionObject;
        [SerializeField] private Vector3 offsetValue;
        #pragma warning restore 0649

        private Vector3 originalPosition;

        public override string Name
        {
            get { return "Offset"; }
        }

        public override Type ActionObjectType
        {
            get { return typeof(Transform); }
        }

        public override UnityEngine.Object ActionObject
        {
            get { return this.actionObject; }
            set { this.actionObject = (Transform)value; }
        }
        
        public override Type ActionValueType
        {
            get { return typeof(Vector3); }
        }

        public override object ActionValue
        {
            get { return this.offsetValue; }
            set { this.offsetValue = (Vector3)value; }
        }

        public override void Apply()
        {
            this.originalPosition = this.actionObject.localPosition;
            this.actionObject.localPosition = this.originalPosition + this.offsetValue;
        }

        public override void Revert()
        {
            this.actionObject.localPosition = this.originalPosition;
        }
    }
}
