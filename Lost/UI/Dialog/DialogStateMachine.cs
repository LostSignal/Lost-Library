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
        
        private float showStateLength = 0.0f;
        private float hideStateLength = 0.0f;
        private State state;

        public bool IsInShownState
        {
            get { return this.state == State.Shown; }
        }

        public bool IsInHideState
        {
            get { return this.state == State.Hidden; }
        }
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (stateInfo.shortNameHash == ShownHash)
            {
                this.showStateLength = 0.0f;
            }
            else if (stateInfo.shortNameHash == HiddenHash)
            {
                this.hideStateLength = 0.0f;
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        
            if (stateInfo.shortNameHash == ShownHash)
            {
                this.showStateLength += Time.deltaTime;

                if (this.showStateLength >= stateInfo.length)
                {
                    this.state = State.Shown;
                }
            }
            else if (stateInfo.shortNameHash == HiddenHash)
            {
                this.hideStateLength += Time.deltaTime;

                if (this.hideStateLength >= stateInfo.length)
                {
                    this.state = State.Hidden;
                }
            }
        }

        private enum State
        {
            Hidden,
            Shown
        }
    }
}
