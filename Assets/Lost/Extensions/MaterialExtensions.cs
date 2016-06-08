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
        public static IEnumerator FadeAlpha(this Material material, float startAlpha, float endAlpha, float timeLengthInSeconds, float delayInSeconds = 0.0f)
        {
            float percentage = 0.0f;
            float time = 0.0f;

            do
            {
                material.color = material.color.SetA(Mathf.Lerp(startAlpha, endAlpha, percentage));

                yield return null;

                percentage = Mathf.Max(0.0f, time - delayInSeconds) / timeLengthInSeconds;

                time += Time.deltaTime;
            }
            while (percentage < 1.0f);

            material.color = material.color.SetA(endAlpha);
        }
    }
}
