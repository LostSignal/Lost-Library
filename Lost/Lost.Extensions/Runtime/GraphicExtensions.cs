//-----------------------------------------------------------------------
// <copyright file="GraphicExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public static class GraphicExtensions
    {
        public static Coroutine FadeAlpha(this Graphic image, float startAlpha, float endAlpha, float timeLengthInSeconds, float delayInSeconds = 0.0f)
        {
            return CoroutineRunner.Instance.StartCoroutine(FadeAlphaCoroutine());

            IEnumerator FadeAlphaCoroutine()
            {
                yield return WaitForUtil.Seconds(delayInSeconds);

                Color color = image.color;
                float time = 0.0f;

                while (time < timeLengthInSeconds)
                {
                    color = color.SetA(Mathf.Lerp(startAlpha, endAlpha, time / timeLengthInSeconds));
                    image.color = color;

                    yield return null;

                    time += Time.deltaTime;
                }

                color = color.SetA(endAlpha);
                image.color = color;
            }
        }
    }
}
