//-----------------------------------------------------------------------
// <copyright file="TintGraphicAction.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [AddComponentMenu("")]
    public class TintGraphicAction : UIAction
    {
        #pragma warning disable 0649
        [SerializeField] private Graphic actionObject;
        [SerializeField] private Color tintValue = Color.white;
        #pragma warning restore 0649

        private Color originalColor;

        public override string Name
        {
            get { return "Tint Graphic"; }
        }

        public override Type ActionObjectType
        {
            get { return typeof(Graphic); }
        }

        public override UnityEngine.Object ActionObject
        {
            get { return this.actionObject; }
            set { this.actionObject = (Graphic)value; }
        }

        public override Type ActionValueType
        {
            get { return typeof(Color); }
        }

        public override object ActionValue
        {
            get { return this.tintValue; }
            set { this.tintValue = (Color)value; }
        }

        public override void Apply()
        {
            this.originalColor = this.actionObject.color;
            this.actionObject.color = this.tintValue;
        }

        public override void Revert()
        {
            this.actionObject.color = this.originalColor;
        }
    }
}
