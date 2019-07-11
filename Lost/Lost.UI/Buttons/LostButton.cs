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
        private SelectionState selectionState = SelectionState.Normal;
        private List<UIAction> actions = new List<UIAction>();
        private bool isFirstStateChange = true;
        private RectTransform rectTransform;
        private bool isInitialized = false;

        public RectTransform RectTransform
        {
            get
            {
                if (!this.rectTransform)
                {
                    this.rectTransform = this.GetComponent<RectTransform>();
                }

                return this.rectTransform;
            }
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

            this.Initialize();

            if (this.selectionState != state)
            {
                this.UpdateButtonActions(this.selectionState, state);
                this.selectionState = state;
            }
        }

        private void Initialize()
        {
            if (this.isInitialized)
            {
                return;
            }

            this.isInitialized = true;
            this.actions.AddRange(this.GetComponentsInChildren<UIAction>());
            this.actions.Sort((x, y) => { return x.Order.CompareTo(y.Order); });
        }

        private void UpdateButtonActions(SelectionState oldState, SelectionState newState)
        {
            // There's nothing to revert on the first state change
            if (this.isFirstStateChange == false)
            {
                // Revert the old button actions
                foreach (var action in this.actions.Where(x => (int)x.State == (int)oldState))
                {
                    action.Revert();
                }
            }

            // Apply the new button actions
            foreach (var action in this.actions.Where(x => (int)x.State == (int)newState))
            {
                action.Apply();
            }

            this.isFirstStateChange = false;
        }
    }
}
