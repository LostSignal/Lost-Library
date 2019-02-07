//-----------------------------------------------------------------------
// <copyright file="DialogStateMachine.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class DialogStateMachine : StateMachineBehaviour
    {
        private static readonly int ShownHash = Animator.StringToHash("Show");
        private static readonly int HiddenHash = Animator.StringToHash("Hide");

        private State state;

        public bool IsDoneShowing
        {
            get { return this.state == State.Shown; }
        }

        public bool IsDoneHiding
        {
            get { return this.state == State.Hidden; }
        }

        public bool IsInitialized
        {
            get { return this.state == State.Initialized; }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (stateInfo.shortNameHash == ShownHash && stateInfo.normalizedTime >= 1.0f)
            {
                this.state = State.Shown;
            }
            else if (stateInfo.shortNameHash == HiddenHash && stateInfo.normalizedTime >= 1.0f)
            {
                this.state = State.Hidden;
            }
        }

        private enum State
        {
            Initialized,
            Hidden,
            Shown,
        }
    }
}
