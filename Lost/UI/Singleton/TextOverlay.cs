//-----------------------------------------------------------------------
// <copyright file="TextOverlay.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEngine.UI;
    
    public enum Corner
    {
        UpperLeft,
        UpperRight,
        LowerLeft,
        LowerRight
    }

    public class TextOverlay : SingletonDialogResource<TextOverlay>
    {
        #pragma warning disable 0649
        [SerializeField] private Text upperLeftText;
        [SerializeField] private Text upperRightText;
        [SerializeField] private Text lowerLeftText;
        [SerializeField] private Text lowerRightText;
        #pragma warning restore 0649

        protected override void Awake()
        {
            base.Awake();
            this.Show();
        }

        public void SetText(Corner corner, string text)
        {
            switch (corner)
            {
                case Corner.UpperLeft:
                    this.upperLeftText.text = text;
                    break;

                case Corner.UpperRight:
                    this.upperRightText.text = text;
                    break;

                case Corner.LowerLeft:
                    this.lowerLeftText.text = text;
                    break;

                case Corner.LowerRight:
                    this.lowerRightText.text = text;
                    break;

                default:
                    Debug.LogErrorFormat(this, "TextOverlay found unknown Corner type '{0}'", corner.ToString());
                    break;
            }
        }

        public void SetText(Corner corner, string text, Color color)
        {
            this.SetText(corner, text);

            switch (corner)
            {
                case Corner.UpperLeft:
                    this.upperLeftText.color = color;
                    break;

                case Corner.UpperRight:
                    this.upperRightText.color = color;
                    break;

                case Corner.LowerLeft:
                    this.lowerLeftText.color = color;
                    break;

                case Corner.LowerRight:
                    this.lowerRightText.color = color;
                    break;

                default:
                    Debug.LogErrorFormat(this, "TextOverlay found unknown Corner type '{0}'", corner.ToString());
                    break;
            }
        }
    }
}
