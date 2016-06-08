//-----------------------------------------------------------------------
// <copyright file="TextMenuUGUIExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using TMPro;
    using UnityEngine;

    public static class TextMenuUGUIExtensions
    {
        public static IEnumerator FadeTextAlpha(this TextMeshProUGUI text, float startAlpha, float endAlpha, float timeLengthInSeconds, float delayInSeconds = 0.0f)
        {
            float percentage = 0.0f;
            float time = 0.0f;

            do
            {
                text.alpha = Mathf.Lerp(startAlpha, endAlpha, percentage);

                yield return null;

                percentage = Mathf.Max(0.0f, time - delayInSeconds) / timeLengthInSeconds;

                time += Time.deltaTime;
            }
            while (percentage < 1.0f);

            text.alpha = endAlpha;
        }
    }
}
