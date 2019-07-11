//-----------------------------------------------------------------------
// <copyright file="TimespanText.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using Text = TMPro.TMP_Text;

    public enum TimespanTextFormat
    {
        ColonSeparatedDHMS = 100,
        ColonSeparatedHMS,
        ColonSeparatedMS,
        ColonSeparatedS,

        SingleLettersDHMS = 200,
        SingleLettersHMS,
        SingleLettersMS,
        SingleLettersS,
    }

    [RequireComponent(typeof(Text))]
    public class TimespanText : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private TimespanTextFormat daysLeftFormat;
        [SerializeField] private TimespanTextFormat hoursLeftFormat;
        [SerializeField] private TimespanTextFormat minutesLeftFormat;
        [SerializeField] private TimespanTextFormat secondsLeftFormat;

        // Hidden Serialized Fields
        [SerializeField, HideInInspector] private Text text;
        #pragma warning restore 0649

        private int seconds;

        public int Seconds
        {
            get
            {
                return this.seconds;
            }

            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                if (this.seconds != value)
                {
                    this.seconds = value;
                    this.UpdateText();
                }
            }
        }

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.text);
        }

        private void Awake()
        {
            this.OnValidate();
        }

        private void OnEnable()
        {
            this.UpdateText();
        }

        private TimespanTextFormat GetFormat()
        {
            if (this.seconds > 86400)
            {
                return this.daysLeftFormat;
            }
            else if (this.seconds > 3600)
            {
                return this.hoursLeftFormat;
            }
            else if (this.seconds > 60)
            {
                return this.minutesLeftFormat;
            }
            else
            {
                return this.secondsLeftFormat;
            }
        }

        private void UpdateText()
        {
            // update text can be called before Awake is called, so this is very necessary, but this will get called again OnEnable
            if (this.text == null)
            {
                return;
            }

            int days = this.seconds / 60 / 60 / 24;
            int hours = (this.seconds / 60 / 60) % 24;
            int minutes = (this.seconds / 60) % 60;
            int seconds = this.seconds % 60;

            switch (this.GetFormat())
            {
                case TimespanTextFormat.ColonSeparatedDHMS:
                    {
                        BetterStringBuilder.New()
                            .Append(days).Append(":")
                            .AppendTwoDigitNumber(hours).Append(":")
                            .AppendTwoDigitNumber(minutes).Append(":")
                            .AppendTwoDigitNumber(seconds)
                            .Set(this.text);

                        break;
                    }

                case TimespanTextFormat.ColonSeparatedHMS:
                    {
                        BetterStringBuilder.New()
                           .Append((days * 24) + hours).Append(":")
                           .AppendTwoDigitNumber(minutes).Append(":")
                           .AppendTwoDigitNumber(seconds)
                           .Set(this.text);

                        break;
                    }

                case TimespanTextFormat.ColonSeparatedMS:
                    {
                        BetterStringBuilder.New()
                          .Append((((days * 24) + hours) * 60) + minutes).Append(":")
                          .AppendTwoDigitNumber(seconds)
                          .Set(this.text);

                        break;
                    }

                case TimespanTextFormat.ColonSeparatedS:
                    {
                        BetterStringBuilder.New()
                         .Append(this.seconds)
                         .Set(this.text);

                        break;
                    }

                case TimespanTextFormat.SingleLettersDHMS:
                    {
                        BetterStringBuilder.New()
                            .Append(days).Append("d ")
                            .AppendTwoDigitNumber(hours).Append("h ")
                            .AppendTwoDigitNumber(minutes).Append("m ")
                            .AppendTwoDigitNumber(seconds).Append("s")
                            .Set(this.text);

                        break;
                    }

                case TimespanTextFormat.SingleLettersHMS:
                    {
                        BetterStringBuilder.New()
                           .Append((days * 24) + hours).Append("h ")
                           .AppendTwoDigitNumber(minutes).Append("m ")
                           .AppendTwoDigitNumber(seconds).Append("s")
                           .Set(this.text);

                        break;
                    }

                case TimespanTextFormat.SingleLettersMS:
                    {
                        BetterStringBuilder.New()
                          .Append((((days * 24) + hours) * 60) + minutes).Append("m ")
                          .AppendTwoDigitNumber(seconds).Append("s")
                          .Set(this.text);

                        break;
                    }

                case TimespanTextFormat.SingleLettersS:
                    {
                        BetterStringBuilder.New()
                         .Append(this.seconds).Append("s")
                         .Set(this.text);

                        break;
                    }

                default:
                    {
                        Debug.LogErrorFormat("Found Unknown Timespan Format {0}", this.GetFormat());
                        break;
                    }
            }
        }
    }
}
