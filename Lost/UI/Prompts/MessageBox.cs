//--------------------------------------------------------------------s---
// <copyright file="MessageBox.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    #if USE_TEXTMESH_PRO
    using Text = TMPro.TextMeshProUGUI;
    #else
    using Text = UnityEngine.UI.Text;
    #endif

    public enum OkResult
    {
        Ok,
    }

    public enum YesNoResult
    {
        Yes,
        No,
    }

    public enum LeftRightResult
    {
        Left,
        Right,
    }

    public class MessageBox : SingletonDialogResource<MessageBox>
    {
        #pragma warning disable 0649
        [Header("MessageBox")]
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button okButton;
        [SerializeField] private Text title;
        [SerializeField] private Text body;
        [SerializeField] private Text leftButtonText;
        [SerializeField] private Text rightButtonText;
        [SerializeField] private Text okButtonText;
        #pragma warning restore 0649
        
        private LeftRightResult result;

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

            this.leftButton = this.DebugCreateButton(content, "LeftButton", "LeftText", "No", new Vector3(-300, -300));
            this.rightButton = this.DebugCreateButton(content, "RightButton", "RightText", "Yes", new Vector3(300, -300));
            this.okButton = this.DebugCreateButton(content, "OkButton", "OkText", "Ok", new Vector3(0, -300));

            this.leftButtonText = this.leftButton.GetComponentInChildren<Text>();
            this.rightButtonText = this.rightButton.GetComponentInChildren<Text>();
            this.okButtonText = this.okButton.GetComponentInChildren<Text>();
        }

        public override void OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            this.LeftButtonClicked();
        }

        public UnityTask<OkResult> ShowOk(string title, string body)
        {
            return UnityTask<OkResult>.Run(this.ShowOkInternal(title, body));
        }

        public UnityTask<YesNoResult> ShowYesNo(string title, string body)
        {
            return UnityTask<YesNoResult>.Run(this.ShowYesNoInternal(title, body));
        }
        
        public UnityTask<LeftRightResult> Show(string title, string body, string leftButtonText, string rightButtonText)
        {
            return UnityTask<LeftRightResult>.Run(this.ShowInternal(title, body, leftButtonText, rightButtonText));
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

            Debug.Assert(this.leftButton != null, "MessageBox didn't define left button", this);
            Debug.Assert(this.rightButton != null, "MessageBox didn't define right button", this);
            Debug.Assert(this.okButton != null, "MessageBox didn't define an OK button", this);
            Debug.Assert(this.title != null, "MessageBox didn't define title", this);
            Debug.Assert(this.body != null, "MessageBox didn't define body", this);

            this.leftButton.onClick.AddListener(this.LeftButtonClicked);
            this.rightButton.onClick.AddListener(this.RightButtonClicked);
            this.okButton.onClick.AddListener(this.OkButtonClicked);
        }

        private IEnumerator<OkResult> ShowOkInternal(string title, string body)
        {
            // TODO [bgish]: If "in use", then wait till it becomes available

            this.SetOkButtonText(title, body);
            this.Show();

            // waiting for it to start showing
            while (this.IsShowing == false)
            {
                yield return default(OkResult);
            }

            // waiting for it to return to the hidden state
            while (this.IsHidden == false)
            {
                yield return default(OkResult);
            }
        }

        private IEnumerator<YesNoResult> ShowYesNoInternal(string title, string body)
        {
            // TODO [bgish]: If "in use", then wait till it becomes available

            this.SetLeftRightText(title, body, "No", "Yes");
            this.Show();

            // waiting for it to start showing
            while (this.IsShowing == false)
            {
                yield return default(YesNoResult);
            }

            // waiting for it to return to the hidden state
            while (this.IsHidden == false)
            {
                yield return default(YesNoResult);
            }

            if (this.result == LeftRightResult.Left)
            {
                yield return YesNoResult.No;
            }
            else if (this.result == LeftRightResult.Right)
            {
                yield return YesNoResult.Yes;
            }
            else
            {
                throw new NotImplementedException("MessageBox.ShowYesNo does not handle result " + this.result.ToString());
            }
        }

        private IEnumerator<LeftRightResult> ShowInternal(string title, string body, string leftButtonText, string rightButtonText)
        {
            // TODO [bgish]: If "in use", then wait till it becomes available

            if (string.IsNullOrEmpty(leftButtonText) == false && this.leftButtonText == null)
            {
                Debug.LogErrorFormat(this, "Unable to set MessageBox left button to {0} besause LeftButtonText object is null.", leftButtonText);
            }

            if (string.IsNullOrEmpty(rightButtonText) == false && this.rightButtonText == null)
            {
                Debug.LogErrorFormat(this, "Unable to set MessageBox right button to {0} besause RightButtonText object is null.", rightButtonText);
            }

            this.SetLeftRightText(title, body, leftButtonText, rightButtonText);
            this.Show();

            // waiting for it to start showing
            while (this.IsShowing == false)
            {
                yield return default(LeftRightResult);
            }

            // waiting for it to return to the hidden state
            while (this.IsHidden == false)
            {
                yield return default(LeftRightResult);
            }

            yield return this.result;
        }
        
        private void LeftButtonClicked()
        {
            this.result = LeftRightResult.Left;
            this.Hide();
        }

        private void RightButtonClicked()
        {
            this.result = LeftRightResult.Right;
            this.Hide();
        }

        private void OkButtonClicked()
        {
            this.Hide();
        }

        private void SetLeftRightText(string title, string body, string leftButton, string rightButton)
        {
            this.leftButton.gameObject.SetActive(true);
            this.rightButton.gameObject.SetActive(true);
            this.okButton.gameObject.SetActive(false);

            this.title.text = title;
            this.body.text = body;

            if (this.leftButtonText != null)
            {
                this.leftButtonText.text = leftButton;
            }

            if (this.rightButtonText != null)
            {
                this.rightButtonText.text = rightButton;
            }
        }

        private void SetOkButtonText(string title, string body)
        {
            this.leftButton.gameObject.SetActive(false);
            this.rightButton.gameObject.SetActive(false);
            this.okButton.gameObject.SetActive(true);

            this.title.text = title;
            this.body.text = body;

            if (this.okButtonText != null)
            {
                this.okButtonText.text = "OK";
            }
        }
    }
}
