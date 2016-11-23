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
        private static readonly int ShowHash = Animator.StringToHash("Show");
        private static readonly int HideHash = Animator.StringToHash("Hide");

        private float transitionFinishTime;
        private State state;

        public bool IsShowing
        {
            get { return this.state == State.Showing || this.state == State.Shown; }
        }

        public bool IsShown
        {
            get { return this.state == State.Shown; }
        }

        public bool IsHidding
        {
            get { return this.state == State.Hiding || this.state == State.Hidden; }
        }

        public bool IsHidden
        {
            get { return this.state == State.Hidden; }
        }
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash == ShowHash)
            {
                this.state = State.Showing;
                this.transitionFinishTime = Time.time + stateInfo.length;
            }
            else if (stateInfo.shortNameHash == HideHash)
            {
                this.state = State.Hiding;
                this.transitionFinishTime = Time.time + stateInfo.length;
            }
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Time.time > this.transitionFinishTime)
            {
                this.transitionFinishTime = float.MaxValue;

                if (this.state == State.Hiding)
                {
                    this.state = State.Hidden;
                }
                else if (this.state == State.Showing)
                {
                    this.state = State.Shown;
                }
            }
        }

        private enum State
        {
            Hidden,
            Shown,
            Showing,
            Hiding,
        }
    }
}
