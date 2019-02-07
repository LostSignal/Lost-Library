//-----------------------------------------------------------------------
// <copyright file="RectSpinner.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    [RequireComponent(typeof(RectTransform))]
    public class RectTransformSpinner : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private float degreesPerSecond = 180;
        [SerializeField] private bool shouldSnapDegrees = true;
        [SerializeField] private float snapDegrees = 45;

        // Hidden Serialized Fields
        [SerializeField, HideInInspector] private RectTransform rectTransform;
        #pragma warning restore 0649

        private float currentDegrees;

        private void OnValidate()
        {
            this.AssertGetComponent<RectTransform>(ref this.rectTransform);
        }

        private void Awake()
        {
            this.OnValidate();
        }

        private void Update()
        {
            this.currentDegrees += Time.deltaTime * this.degreesPerSecond;

            float degrees = this.shouldSnapDegrees ? (((int)(this.currentDegrees / this.snapDegrees)) * this.snapDegrees) : this.currentDegrees;

            this.rectTransform.localRotation = Quaternion.Euler(0, 0, degrees);
        }
    }
}
