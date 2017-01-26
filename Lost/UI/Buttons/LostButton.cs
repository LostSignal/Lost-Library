//-----------------------------------------------------------------------
// <copyright file="LostButton.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    public class LostButton : Button
    {
        private SelectionState lostButtonSelectionState = SelectionState.Normal;
        private List<ButtonAction> buttonActions = new List<ButtonAction>();
        private bool isFirstStateChange = true;
        
        protected override void Awake()
        {
            base.Awake();
            this.buttonActions.AddRange(this.GetComponentsInChildren<ButtonAction>());
            this.buttonActions.Sort((x, y) => { return x.Order.CompareTo(y.Order); });
        }
        
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            #if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return;
            }
            #endif
            
            if (this.lostButtonSelectionState != state)
            {
                this.UpdateButtonActions(this.lostButtonSelectionState, state);
                this.lostButtonSelectionState = state;
            }
        }
        
        private void UpdateButtonActions(SelectionState oldState, SelectionState newState)
        {            
            // there's nothing to revert on the first state change
            if (this.isFirstStateChange == false)
            {
                // revert the old button actions
                foreach (var action in this.buttonActions.Where(x => (int)x.State == (int)oldState))
                {
                    action.Revert();
                }
            }
            
            // apply the new button actions
            foreach (var action in this.buttonActions.Where(x => (int)x.State == (int)newState))
            {
                action.Apply();
            }
            
            this.isFirstStateChange = false;
        }
    }
}
