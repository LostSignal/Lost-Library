//-----------------------------------------------------------------------
// <copyright file="AnimateAlongSpline.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class AnimateAlongSpline : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));
        [SerializeField] private float animationTime = 1.0f;
        [SerializeField] private Spline spline;
        #pragma warning restore 0649

        private float currentTime;

        private void OnEnable()
        {
            this.currentTime = 0.0f;
        }

        private void Update()
        {
            this.currentTime += Time.deltaTime;

            if (this.currentTime < animationTime)
            {
                float percentage = this.animationCurve.Evaluate(this.currentTime / this.animationTime);
                float distance = this.spline.SplineLength * percentage;
                this.transform.position = this.spline.Evaluate(distance);
            }
            else
            {
                this.transform.position = this.spline.Evaluate(this.spline.SplineLength);
                this.enabled = false;
            }
        }
    }
}
