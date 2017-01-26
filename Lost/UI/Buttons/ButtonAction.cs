//-----------------------------------------------------------------------
// <copyright file="ButtonAction.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    // NOTE [bgish]: this must match UnityEngine.UI.Selectable.SelectionState
    public enum ButtonActoinState
    {
        Normal,
        Highlighted,
        Pressed,
        Disabled
    }
    
    [ExecuteInEditMode]
    public abstract class ButtonAction : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private int order;
        [SerializeField] private ButtonActoinState state;
        #pragma warning restore 0649

        public abstract string Name { get; }
        
        public int Order
        {
            get { return this.order; }

            #if UNITY_EDITOR
            set { this.order = value; }
            #endif
        }

        public ButtonActoinState State
        {
            get { return this.state; }

            #if UNITY_EDITOR
            set { this.state = value; }
            #endif
        }

        public abstract UnityEngine.Object ActionObject { get; set; }

        public abstract System.Type ActionObjectType { get; }

        public abstract object ActionValue { get; set; }

        public abstract System.Type ActionValueType { get; }

        public abstract void Apply();

        public abstract void Revert();

        protected virtual void Awake()
        {
            this.hideFlags = HideFlags.HideInInspector;
        }
    }
}
