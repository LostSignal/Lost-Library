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

        public bool IsUnknown
        {
            get { return this.state == State.Unknown; }
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
            else
            {
                this.state = State.Unknown;
            }
        }

        private enum State
        {
            Unknown,
            Hidden,
            Shown,
        }
    }
}
