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

    #if USE_TEXTMESH_PRO
    using Text = TMPro.TMP_Text;
    #else
    using Text = UnityEngine.UI.Text;
    #endif

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
        [SerializeField] private UnityEvent onStartAnimation;
        [SerializeField] private UnityEvent onEndAnimation;
        [SerializeField] private AnimationCurve animationCurve = new AnimationCurve(new Keyframe { time = 0, value = 0 }, new Keyframe { time = 1, value = 1 });
        [SerializeField] private IntFormat format;
        #pragma warning restore 0649

        private Text text;
        private int intValue = int.MinValue;
        private int goalValue = int.MinValue;

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
                this.StopCoroutine("AnimateToGoalCoroutine");
                this.StartCoroutine("AnimateToGoalCoroutine");
            }
        }

        // NOTE [bgish]: has zero references bcause AnimateToGoal refers to this function by string name
        private IEnumerator AnimateToGoalCoroutine()
        {
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

        private void Awake()
        {
            this.text = this.GetComponent<Text>();
        }

        private void OnEnable()
        {
            this.UpdateText();
        }

        private void UpdateText()
        {
            if (this.text != null)
            {
                if (this.intValue == int.MinValue)
                {
                    this.text.text = this.unsetText;
                }
                else
                {
                    BetterStringBuilder.New().Append(this.intValue, this.format).Set(this.text);
                }
            }
        }
    }
}
