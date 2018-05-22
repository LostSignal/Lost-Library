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

    #if USE_TEXTMESH_PRO
    using Text = TMPro.TextMeshProUGUI;
    #else
    using Text = UnityEngine.UI.Text;
    #endif

    public class SpinnerBox : SingletonDialogResource<SpinnerBox>
    {
        #pragma warning disable 0649
        [Header("SpinnerBox")]
        [SerializeField] private Text title;
        [SerializeField] private Text body;
        [SerializeField] private Button cancelButton;
        #pragma warning restore 0649

        private Action cancelButtonAction;

        public void SetupDefault()
        {
            var content = this.transform.Find("Content").gameObject;
            content.GetOrAddComponent<Image>();
            content.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            content.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 800);
            content.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

            this.title = this.DebugCreateText(content, "TitleText", "Title", new Vector3(0, 300));
            this.body = this.DebugCreateText(content, "TitleBody", "Body", Vector3.zero);
            this.cancelButton = this.DebugCreateButton(content, "CanelButton", "Text", "Cancel", new Vector3(0, -300));
        }

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
            if (this.IsShowing)
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

            this.Show();
        }

        private void CancelButtonClicked()
        {
            if (this.cancelButtonAction != null)
            {
                this.cancelButtonAction.Invoke();
                this.cancelButtonAction = null;
            }

            this.Hide();
        }
    }
}
