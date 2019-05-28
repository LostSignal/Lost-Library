//-----------------------------------------------------------------------
// <copyright file="MaterialExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using UnityEngine;

    public static class MaterialExtensions
    {
        public static Coroutine FadeAlpha(this Material material, float startAlpha, float endAlpha, float timeLengthInSeconds, float delayInSeconds = 0.0f)
        {
            return CoroutineRunner.Instance.StartCoroutine(FadeAlphaCoroutine());

            IEnumerator FadeAlphaCoroutine()
            {
                yield return WaitForUtil.Seconds(delayInSeconds);

                Color currentColor = material.color;
                float time = 0.0f;

                while (time < timeLengthInSeconds)
                {
                    if (material)
                    {
                        currentColor = currentColor.SetA(Mathf.Lerp(startAlpha, endAlpha, time / timeLengthInSeconds));
                        material.color = currentColor;
                    }

                    yield return null;
                    time += Time.deltaTime;
                }

                if (material)
                {
                    currentColor = currentColor.SetA(endAlpha);
                    material.color = currentColor;
                }
            }
        }

        public static Coroutine FadeAlpha(this Material material, string colorPropertyName, float startAlpha, float endAlpha, float timeLengthInSeconds, float delayInSeconds = 0.0f)
        {
            return CoroutineRunner.Instance.StartCoroutine(FadeAlphaCoroutine());

            IEnumerator FadeAlphaCoroutine()
            {
                yield return WaitForUtil.Seconds(delayInSeconds);

                Color currentColor = material.GetColor(colorPropertyName);
                float time = 0.0f;

                while (time < timeLengthInSeconds)
                {
                    if (material)
                    {
                        currentColor = currentColor.SetA(Mathf.Lerp(startAlpha, endAlpha, time / timeLengthInSeconds));
                        material.SetColor(colorPropertyName, currentColor);
                    }

                    yield return null;
                    time += Time.deltaTime;
                }

                if (material)
                {
                    currentColor = currentColor.SetA(endAlpha);
                    material.SetColor(colorPropertyName, currentColor);
                }
            }
        }
    }
}
