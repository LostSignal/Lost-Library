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
        private static readonly int ShownHash = Animator.StringToHash("Shown");
        private static readonly int HiddenHash = Animator.StringToHash("Hidden");
        
        private State state;

        public bool IsInShownState
        {
            get { return this.state == State.Shown; }
        }

        public bool IsInHiddenState
        {
            get { return this.state == State.Hidden; }
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        
            if (stateInfo.shortNameHash == ShownHash)
            {
                this.state = State.Shown;
            }
            else if (stateInfo.shortNameHash == HiddenHash)
            {
                this.state = State.Hidden;
            }
        }

        private enum State
        {
            Hidden,
            Shown
        }
    }
}
