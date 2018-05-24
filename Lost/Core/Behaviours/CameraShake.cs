//-----------------------------------------------------------------------
// <copyright file="CameraShake.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class CameraShake : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private float shakeTime = 0.25f;
        [SerializeField] private float shakeAmount = 0.2f;
        #pragma warning restore 0649

        private float currentShakeTime;
        private Vector3 originalPosition;

        public static void Shake()
        {
            foreach (var cameraShake in ObjectTracker.GetObjects<CameraShake>())
            {
                cameraShake.PrivateShake();
            }
        }

        private void PrivateShake()
        {
            this.currentShakeTime = this.shakeTime;
            this.originalPosition = this.transform.localPosition;
        }

        private void Update()
        {
            if (this.currentShakeTime > 0.0f)
            {
                this.transform.localPosition = this.originalPosition + (Random.insideUnitSphere * this.shakeAmount);
                this.currentShakeTime -= Time.deltaTime;
            }
        }

        private void OnEnable()
        {
            ObjectTracker.Register<CameraShake>(this);
        }

        private void OnDisable()
        {
            ObjectTracker.Deregister<CameraShake>(this);
        }
    }
}
