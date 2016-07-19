//-----------------------------------------------------------------------
// <copyright file="Dialog.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(Animator))]
    public class Dialog : MonoBehaviour
    {
        public Canvas Canvas
        {
            get; private set;
        }

        public Animator Animator
        {
            get; private set;
        }

        private void Awake()
        {
            this.Canvas = this.GetComponent<Canvas>();
            this.Animator = this.GetComponent<Animator>();
        }
    }
}