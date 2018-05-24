//-----------------------------------------------------------------------
// <copyright file="ScreenFader.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    public class ScreenFader : MonoBehaviour
    {
        private Image image;

        public static void FadeScreenOut(float seconds = 1.0f)
        {
            foreach (ScreenFader fader in ObjectTracker.GetObjects<ScreenFader>())
            {
                fader.StartCoroutine(fader.FadeScreenOutCoroutine(seconds));
                break;
            }
        }

        public static void FadeScreenIn(float seconds = 1.0f)
        {
            foreach (ScreenFader fader in ObjectTracker.GetObjects<ScreenFader>())
            {
                fader.StartCoroutine(fader.FadeScreenInCoroutine(seconds));
                break;
            }
        }

        private void Awake()
        {
            this.image = this.GetComponent<Image>();
        }

        private void OnEnable()
        {
            ObjectTracker.Register<ScreenFader>(this);
        }

        private void OnDisable()
        {
            ObjectTracker.Deregister<ScreenFader>(this);
        }

        private IEnumerator FadeScreenOutCoroutine(float seconds)
        {
            float speed = 1.0f / seconds;
            float alpha = 1.0f;  // starts at 1 and goes to 0

            while (alpha > 0.001f)
            {
                alpha = Mathf.Max(alpha - (Time.deltaTime * speed), 0.0f);
                this.image.color = this.image.color.SetA(1.0f - alpha);
                yield return null;
            }
        }

        private IEnumerator FadeScreenInCoroutine(float seconds)
        {
            float speed = 1.0f / seconds;
            float alpha = 1.0f;  // starts at 1 and goes to 0

            while (alpha > 0.001f)
            {
                alpha = Mathf.Max(alpha - (Time.deltaTime * speed), 0.0f);
                this.image.color = this.image.color.SetA(alpha);
                yield return null;
            }
        }
    }
}
