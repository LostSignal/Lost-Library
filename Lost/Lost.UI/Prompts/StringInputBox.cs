//--------------------------------------------------------------------s---
// <copyright file="StringInputBox.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using InputField = TMPro.TMP_InputField;
    using Text = TMPro.TextMeshProUGUI;

    public enum StringInputResult
    {
        Cancel,
        Ok,
    }

    // TODO [bgish]: Make sure to move the Content Object up if TouchScreenKeyboard.visible is true

    public class StringInputBox : SingletonDialogResource<StringInputBox>
    {
        #pragma warning disable 0649
        [Header("StringInputBox")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button okButton;
        [SerializeField] private InputField inputField;
        [SerializeField] private Text title;
        [SerializeField] private Text body;
        [SerializeField] private RectTransform content;
        [SerializeField] private float slideUpDownTimeInSeconds = 1.0f;
        #pragma warning restore 0649

        private Coroutine virtualKeyboardCoroutine;
        private StringInputResult result;
        private string startingText;
        private float centerY = 0.0f;
        private float upperY = 0.0f;

        public string Text { get; private set; }

        public UnityTask<StringInputResult> Show(string title, string body, string startingText)
        {
            return UnityTask<StringInputResult>.Run(this.ShowInternal(title, body, startingText));
        }

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(this.cancelButton != null, "StringInputBox didn't define cancel button.", this);
            Debug.Assert(this.closeButton != null, "StringInputBox didn't define close button.", this);
            Debug.Assert(this.okButton != null, "StringInputBox didn't define an ok button.", this);
            Debug.Assert(this.inputField != null, "StringInputBox didn't define input field.", this);
            Debug.Assert(this.title != null, "StringInputBox didn't define title.", this);
            Debug.Assert(this.body != null, "StringInputBox didn't define body.", this);

            var rectTransform = this.transform as RectTransform;
            this.upperY = rectTransform.sizeDelta.y / 4.0f;

            this.cancelButton.onClick.AddListener(this.CancelButtonPressed);
            this.closeButton.onClick.AddListener(this.CancelButtonPressed);
            this.okButton.onClick.AddListener(this.OkButtonClicked);
        }

        protected override void OnShow()
        {
            base.OnShow();

            // Whenever we show it, make sure it's centered
            this.content.transform.localPosition = this.content.transform.localPosition.SetY(this.centerY);

            this.StopVirtualKeyboardCoroutine();
            this.StartVirtualKeyboardCoroutine();
        }

        protected override void OnHide()
        {
            base.OnHide();

            this.StopVirtualKeyboardCoroutine();
        }

        protected override void OnBackButtonPressed()
        {
            base.OnBackButtonPressed();

            this.CancelButtonPressed();
        }

        private IEnumerator VirtualKeyboardCoroutine()
        {
            while (true)
            {
                bool isTouchScreenVisible = false;

#if UNITY_ANDROID || UNITY_IPHONE || UNITY_WSA || UNITY_WSA_10_0
                isTouchScreenVisible = TouchScreenKeyboard.visible;
#endif

                float desiredY = isTouchScreenVisible ? this.upperY : this.centerY;
                float currentY = this.content.transform.localPosition.y;
                float distanceFromDesiredY = Mathf.Abs(desiredY - currentY);

                if (distanceFromDesiredY > 0.1f)
                {
                    float direction = desiredY < currentY ? -1.0f : 1.0f;
                    float speed = direction * (this.upperY - this.centerY) / this.slideUpDownTimeInSeconds;
                    float movement = speed * Time.deltaTime;

                    if (Mathf.Abs(movement) > distanceFromDesiredY)
                    {
                        this.content.transform.localPosition = this.content.transform.localPosition.SetY(desiredY);
                    }
                    else
                    {
                        this.content.transform.localPosition = this.content.transform.localPosition.AddToY(movement);
                    }
                }

                yield return null;
            }
        }

        private IEnumerator<StringInputResult> ShowInternal(string title, string body, string startingText = null)
        {
            // TODO [bgish]: If "in use", then wait till it becomes available

            this.startingText = string.IsNullOrWhiteSpace(startingText) ? string.Empty : startingText;
            this.inputField.text = this.startingText;
            this.title.text = title;
            this.body.text = body;

            this.Dialog.Show();

            // waiting for it to start showing
            while (this.Dialog.IsShowing == false)
            {
                yield return this.result;
            }

            // waiting for it to return to the hidden state
            while (this.Dialog.IsHidden == false)
            {
                yield return this.result;
            }
        }

        private void CancelButtonPressed()
        {
            this.Text = null;
            this.result = StringInputResult.Cancel;
            this.Dialog.Hide();
        }

        private void OkButtonClicked()
        {
            bool didTextChanged = this.inputField.text != this.startingText;

            if (didTextChanged)
            {
                this.Text = this.inputField.text;
                this.result = StringInputResult.Ok;
            }
            else
            {
                this.Text = null;
                this.result = StringInputResult.Cancel;
            }

            this.Dialog.Hide();
        }

        private void StartVirtualKeyboardCoroutine()
        {
            if (TouchScreenKeyboard.isSupported)
            {
                this.virtualKeyboardCoroutine = this.StartCoroutine(this.VirtualKeyboardCoroutine());
            }
        }

        private void StopVirtualKeyboardCoroutine()
        {
            if (this.virtualKeyboardCoroutine != null)
            {
                this.StopCoroutine(this.virtualKeyboardCoroutine);
                this.virtualKeyboardCoroutine = null;
            }
        }
    }
}
