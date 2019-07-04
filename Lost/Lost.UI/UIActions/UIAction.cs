//-----------------------------------------------------------------------
// <copyright file="UIAction.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    // NOTE [bgish]: this must match UnityEngine.UI.Selectable.SelectionState
    public enum UIActionState
    {
        Normal = 0,
        Highlighted = 1,
        Pressed = 2,
        Selected = 3,
        Disabled = 4
    }

    [ExecuteInEditMode]
    public abstract class UIAction : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private int order;
        [SerializeField] private UIActionState state;
        #pragma warning restore 0649

        public abstract string Name { get; }

        public int Order
        {
            get { return this.order; }

            #if UNITY_EDITOR
            set { this.order = value; }
            #endif
        }

        public UIActionState State
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
