//--------------------------------------------------------------------s---
// <copyright file="MessageBox.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public enum YesNoResult
    {
        Yes,
        No
    }

    public enum LeftRightResult
    {
        Left,
        Right
    }

    public class MessageBox : SingletonDialogResource<MessageBox>
    {
        #pragma warning disable 0649
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button okButton;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI body;
        [SerializeField] private TextMeshProUGUI leftButtonText;
        [SerializeField] private TextMeshProUGUI rightButtonText;
        [SerializeField] private TextMeshProUGUI okButtonText;
        #pragma warning restore 0649
        
        private LeftRightResult result;

        public IEnumerator ShowOk(string title, string body)
        {
            // TODO [bgish]: If "in use", then wait till it becomes available

            this.SetOkButtonText(title, body);
            this.Show();

            // waiting for it to start showing
            while (this.IsShowing == false)
            {
                yield return null;
            }

            // waiting for it to return to the hidden state
            while (this.IsHidden == false)
            {
                yield return null;
            }
        }

        public IEnumerator<YesNoResult> ShowYesNo(string title, string body)
        {
            // TODO [bgish]: If "in use", then wait till it becomes available

            this.SetLeftRightText(title, body, "Yes", "No");
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
                yield return YesNoResult.Yes;
            }
            else if (this.result == LeftRightResult.Right)
            {
                yield return YesNoResult.No;
            }
            else
            {
                throw new NotImplementedException("MessageBox.ShowYesNo does not handle result " + this.result.ToString());
            }
        }

        public IEnumerator<LeftRightResult> Show(string title, string body, string leftButtonText, string rightButtonText)
        {
            // TODO [bgish]: If "in use", then wait till it becomes available

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
