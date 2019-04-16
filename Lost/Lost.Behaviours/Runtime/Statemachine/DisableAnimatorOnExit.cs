//-----------------------------------------------------------------------
// <copyright file="DisableAnimatorOnExit.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Hourglass
{
    using UnityEngine;

    public class DisableAnimatorOnExit : StateMachineBehaviour
    {
        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash) 
        {
            animator.enabled = false;
        }
    }
}
