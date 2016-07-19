//-----------------------------------------------------------------------
// <copyright file="RectTransformExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using UnityEngine;
    
    public static class RectTransformExtensions
    {
        public static void SetWidth(this RectTransform lhs, float width)
        {
            lhs.sizeDelta = lhs.sizeDelta.SetX(width);
        }
        
        public static void SetHeight(this RectTransform lhs, float height)
        {
            lhs.sizeDelta = lhs.sizeDelta.SetY(height);
        }

        // TODO rewrtie this (does not take into account rotations, try using built in GetWorldCorners
        public static Rect GetWorldRect(this RectTransform rectTransform)
        {
            Vector3 worldPosition = rectTransform.localToWorldMatrix.MultiplyPoint(rectTransform.rect.position);

            float worldWidth = rectTransform.rect.width * rectTransform.lossyScale.x;
            float worldHeight = rectTransform.rect.height * rectTransform.lossyScale.y;

            return new Rect(worldPosition, new Vector2(worldWidth, worldHeight));
        }

        public static IEnumerator Translate(this RectTransform rectTransform, Vector2 start, Vector2 end, float timeLengthInSeconds, float delayInSeconds, AnimationCurve animCurve)
        {
            float percentage = 0.0f;
            float animCurveValue = 0.0f;
            float time = 0.0f;

            do
            {
                rectTransform.anchoredPosition = Vector3.LerpUnclamped(start, end, animCurveValue);

                yield return null;

                percentage = Mathf.Max(0.0f, time - delayInSeconds) / timeLengthInSeconds;
                animCurveValue = animCurve == null ? percentage : animCurve.Evaluate(percentage);
                time += Time.deltaTime;
            }
            while (percentage < 1.0f);

            rectTransform.anchoredPosition = end;
        }
    }
}
