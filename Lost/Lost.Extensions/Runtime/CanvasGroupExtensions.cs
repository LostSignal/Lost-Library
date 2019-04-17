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
        public static Coroutine EnableAndFadeIn(this CanvasGroup canvasGroup, float time)
        {
            canvasGroup.gameObject.SafeSetActive(true);
            canvasGroup.alpha = 0;

            return CoroutineRunner.Instance.StartCoroutine(FadeCoroutine(canvasGroup, canvasGroup.alpha, 1.0f, time));
        }

        public static Coroutine FadeOutAndDisable(this CanvasGroup canvasGroup, float time)
        {
            return CoroutineRunner.Instance.StartCoroutine(FadeCoroutine(canvasGroup, canvasGroup.alpha, 0.0f, time, true));
        }

        public static Coroutine FadeIn(this CanvasGroup canvasGroup, float time)
        {
            return CoroutineRunner.Instance.StartCoroutine(FadeCoroutine(canvasGroup, canvasGroup.alpha, 1.0f, time));
        }

        public static Coroutine FadeOut(this CanvasGroup canvasGroup, float time)
        {
            return CoroutineRunner.Instance.StartCoroutine(FadeCoroutine(canvasGroup, canvasGroup.alpha, 0.0f, time));
        }

        private static IEnumerator FadeCoroutine(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float time, bool disableCanvasGroupWhenDone = false)
        {
            float currentTime = 0.0f;

            while (currentTime < time)
            {
                currentTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, currentTime / time);

                yield return null;
            }

            canvasGroup.alpha = endAlpha;

            if (disableCanvasGroupWhenDone)
            {
                canvasGroup.gameObject.SafeSetActive(false);
            }
        }
    }
}
