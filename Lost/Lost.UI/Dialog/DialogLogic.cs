//-----------------------------------------------------------------------
// <copyright file="DialogLogic.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    //// NOTE [bgish]: Possible Future Events
    //// OnShown (when showing is done)
    //// OnHidden (when hidding is done)
    //// OnCoverred (when a dialog has been covered up by another)
    //// OnUncoverred (when a dialog has been uncovered from a dialog going away)

    [AddComponentMenu("")]
    [RequireComponent(typeof(Dialog))]
    public abstract class DialogLogic : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField, HideInInspector]
        private Dialog dialog;
        #pragma warning restore 0649

        public Dialog Dialog
        {
            get { return this.dialog; }
        }

        protected virtual void Awake()
        {
            if (this.dialog == null)
            {
                this.dialog = this.GetComponent<Dialog>();
            }

            this.dialog.OnShow.AddListener(this.OnShow);
            this.dialog.OnHide.AddListener(this.OnHide);
            this.dialog.OnBackButtonPressed.AddListener(this.OnBackButtonPressed);

            DialogManager.RegisterDialog(this);
        }

        protected virtual void OnDestroy()
        {
            DialogManager.UnregisterDialog(this);
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnBackButtonPressed()
        {
        }

        private void Reset()
        {
            this.dialog = this.GetComponent<Dialog>();
        }
    }
}
