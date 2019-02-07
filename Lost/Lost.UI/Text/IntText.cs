//-----------------------------------------------------------------------
// <copyright file="IntText.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    using Text = TMPro.TMP_Text;

    public enum TextUpdateType
    {
        Instant,
        Animate,
        None,
    }

    [RequireComponent(typeof(Text))]
    public class IntText : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private string unsetText = "?";
        [SerializeField] private string prefixValue = "";
        [SerializeField] private string postfixValue = "";
        [SerializeField] private UnityEvent onStartAnimation;
        [SerializeField] private UnityEvent onEndAnimation;
        [SerializeField] private AnimationCurve animationCurve = new AnimationCurve(new Keyframe { time = 0, value = 0 }, new Keyframe { time = 1, value = 1 });
        [SerializeField] private IntFormat format;

        // Hidden Serialized Fields
        [SerializeField, HideInInspector] private Text text;
        #pragma warning restore 0649

        private Coroutine animateToGoalCoroutine;
        private int intValue = int.MinValue;
        private int goalValue = int.MinValue;

        public Text Text
        {
            get
            {
                this.UpdateTextField();
                return this.text;
            }
        }

        public bool HasValueBeenSet
        {
            get { return this.intValue != int.MinValue; }
        }

        public int CurrentValue
        {
            get { return this.intValue; }
        }

        public int GoalValue
        {
            get { return this.goalValue; }
        }

        public void UpdateValue(int newValue, TextUpdateType updateType)
        {
            if (this.goalValue == newValue)
            {
                return;
            }

            this.goalValue = newValue;

            if (updateType == TextUpdateType.Instant)
            {
                if (this.intValue != newValue)
                {
                    this.intValue = newValue;
                    this.goalValue = newValue;

                    this.UpdateText();
                }
            }
            else if (updateType == TextUpdateType.Animate)
            {
                this.AnimateToGoal();
            }
            else if (updateType == TextUpdateType.None)
            {
                // do nothing
            }
            else
            {
                Debug.LogErrorFormat("IntText.UpdateValue found unknown TextUpdateType {0}", updateType.ToString());
            }
        }

        public void AnimateToGoal()
        {
            if (this.intValue != this.goalValue)
            {
                if (this.animateToGoalCoroutine != null)
                {
                    CoroutineRunner.Instance.StopCoroutine(this.animateToGoalCoroutine);
                    this.animateToGoalCoroutine = null;
                }

                this.animateToGoalCoroutine = CoroutineRunner.Instance.StartCoroutine(this.AnimateToGoalCoroutine());
            }
        }

        // NOTE [bgish]: has zero references bcause AnimateToGoal refers to this function by string name
        private IEnumerator AnimateToGoalCoroutine()
        {
            this.UpdateTextField();

            if (this.HasValueBeenSet == false)
            {
                this.intValue = 0;
                this.text.text = "0";
            }

            this.onStartAnimation.InvokeIfNotNull();

            float startValue = this.intValue;
            float endValue = this.goalValue;
            float difference = endValue - startValue;
            float animationTime = this.animationCurve.keys.Last().time;
            float currentTime = 0;

            while (currentTime < animationTime)
            {
                float newValue = startValue + (difference * this.animationCurve.Evaluate(currentTime));

                this.intValue = (int)newValue;
                this.UpdateText();

                yield return null;

                currentTime += Time.deltaTime;
            }

            this.intValue = this.goalValue;
            this.UpdateText();

            this.onEndAnimation.InvokeIfNotNull();
        }

        private void OnValidate()
        {
            this.UpdateTextField();
        }

        private void Awake()
        {
            Localization.Localization.LanguagedChanged += this.UpdateText;
        }

        private void OnEnable()
        {
            this.UpdateText();
        }

        private void OnDestroy()
        {
            Localization.Localization.LanguagedChanged -= this.UpdateText;
        }

        private void UpdateTextField()
        {
            this.AssertGetComponent<Text>(ref this.text);
        }

        private void UpdateText()
        {
            this.UpdateTextField();

            if (this.text != null)
            {
                if (this.intValue == int.MinValue)
                {
                    this.text.text = this.unsetText;
                }
                else
                {
                    BetterStringBuilder.New()
                        .Append(this.prefixValue)
                        .Append(this.intValue, this.format)
                        .Append(this.postfixValue)
                        .Set(this.text);
                }
            }
        }
    }
}
