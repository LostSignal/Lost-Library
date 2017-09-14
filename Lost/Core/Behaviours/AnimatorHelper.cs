//-----------------------------------------------------------------------
// <copyright file="AnimatorHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    [RequireComponent(typeof(Animator))]
    public class AnimatorHelper : MonoBehaviour
    {
        private Animator animator;

        public void SetBoolTrue(string paramName)
        {
            this.animator.SetBool(paramName, true);
        }
        
        public void SetBoolFalse(string paramName)
        {
            this.animator.SetBool(paramName, false);
        }

        public void ToggleBool(string paramName)
        {
            this.animator.SetBool(paramName, !this.animator.GetBool(paramName));
        }

        public void TimedDisable(float seconds)
        {
            this.ExecuteDelayed(seconds, () => this.animator.enabled = false);
        }

        private void Awake()
        {
            this.animator = this.GetComponent<Animator>();
        }
    }
}
