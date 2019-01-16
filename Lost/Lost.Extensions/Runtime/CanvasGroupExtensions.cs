//-----------------------------------------------------------------------
// <copyright file="CanvasGroupExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using UnityEngine;

    public static class CanvasGroupExtensions
    {
        public static IEnumerator EnableAndFadeIn(this CanvasGroup canvasGroup, float time)
        {
            canvasGroup.gameObject.SafeSetActive(true);
            canvasGroup.alpha = 0;
            yield return Fade(canvasGroup, canvasGroup.alpha, 1.0f, time);
        }

        public static IEnumerator FadeOutAndDisable(this CanvasGroup canvasGroup, float time)
        {
            yield return Fade(canvasGroup, canvasGroup.alpha, 0.0f, time);
            canvasGroup.gameObject.SafeSetActive(false);
        }

        public static IEnumerator FadeIn(this CanvasGroup canvasGroup, float time)
        {
            return Fade(canvasGroup, canvasGroup.alpha, 1.0f, time);
        }

        public static IEnumerator FadeOut(this CanvasGroup canvasGroup, float time)
        {
            return Fade(canvasGroup, canvasGroup.alpha, 0.0f, time);
        }

        private static IEnumerator Fade(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float time)
        {
            float currentTime = 0.0f;

            while (currentTime < time)
            {
                currentTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, currentTime / time);

                yield return null;
            }

            canvasGroup.alpha = endAlpha;
        }
    }
}
