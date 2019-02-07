//-----------------------------------------------------------------------
// <copyright file="CountDownTimerText.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;
    using Text = TMPro.TMP_Text;

    [RequireComponent(typeof(Text))]
    public class CountDownTimerText : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private string finishedText;

        // Hidden Serialized Fields
        [SerializeField, HideInInspector] private Text text;
        #pragma warning restore 0649

        private DateTime target;
        private float timer;

        public DateTime Target
        {
            get
            {
                return this.target;
            }

            set
            {
                this.target = value.ToUniversalTime();
                this.UpdateText();
            }
        }

        private void OnValidate()
        {
            this.AssertGetComponent<Text>(ref this.text);
        }

        private void Awake()
        {
            this.OnValidate();
        }

        private void OnEnable()
        {
            this.UpdateText();
        }

        private void Update()
        {
            this.timer += Time.unscaledDeltaTime;

            if (this.timer < 1.0f)
            {
                return;
            }

            this.timer = 0.0f;

            this.UpdateText();
        }

        private void UpdateText()
        {
            // update text can be called before Awake is called, so this is very necessary, but this will get called again OnEnable
            if (this.text == null)
            {
                return;
            }

            var utcNow = DateTime.UtcNow;

            // seeing if we're finished
            if (utcNow > this.Target)
            {
                this.text.text = string.IsNullOrEmpty(this.finishedText) == false ? this.finishedText : "00:00:00";
            }
            else
            {
                TimeSpan timeLeft = this.Target.Subtract(utcNow);
                this.text.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
            }
        }
    }
}
