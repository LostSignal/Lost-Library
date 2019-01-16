//--------------------------------------------------------------------s---
// <copyright file="SpinnerBox.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using Text = TMPro.TMP_Text;

    public class SpinnerBox : SingletonDialogResource<SpinnerBox>
    {
        #pragma warning disable 0649
        [Header("SpinnerBox")]
        [SerializeField] private Text title;
        [SerializeField] private Text body;
        [SerializeField] private Button cancelButton;
        #pragma warning restore 0649

        private Action cancelButtonAction;

        public void Show(string title, string body)
        {
            this.PrivateShow(title, body, false, null);
        }

        public void ShowWithCancel(string title, string body, Action cancelButtonAction)
        {
            this.PrivateShow(title, body, true, cancelButtonAction);
        }

        public void UpdateBodyText(string body)
        {
            this.body.text = body;
        }

        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return;
            }
#endif

            Debug.Assert(this.body != null, "SpinnerBox didn't specify body text", this);
            Debug.Assert(this.cancelButton != null, "SpinnerBox didn't specify cancelButton", this);

            this.cancelButton.onClick.AddListener(this.CancelButtonClicked);
        }

        private void PrivateShow(string title, string body, bool showCancelButton, Action cancelButtonAction)
        {
            if (this.Dialog.IsShowing)
            {
                Debug.LogError("SpinnerBox.Show called while already showing.  SpinnerBox may not function correctly.", this);
            }

            if (this.title != null)
            {
                this.title.text = title;
            }

            this.UpdateBodyText(body);
            this.cancelButton.gameObject.SetActive(showCancelButton);
            this.cancelButtonAction = cancelButtonAction;

            this.Dialog.Show();
        }

        private void CancelButtonClicked()
        {
            if (this.cancelButtonAction != null)
            {
                this.cancelButtonAction.Invoke();
                this.cancelButtonAction = null;
            }

            this.Dialog.Hide();
        }
    }
}
