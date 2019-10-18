//-----------------------------------------------------------------------
// <copyright file="LightEstimation.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_AR_FOUNDATION

////
//// Taken from Unity's AR Foundation Samples
////   https://github.com/Unity-Technologies/arfoundation-samples
////

namespace Lost.XR
{
    using UnityEngine;
    using UnityEngine.XR.ARFoundation;
    [RequireComponent(typeof(Light))]
    public class LightEstimation : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private ARCameraManager cameraManager;
        [SerializeField, HideInInspector] private Light lightToEstimate;
        #pragma warning restore 0649

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.lightToEstimate);
            this.AssertNotNull(this.cameraManager, nameof(this.cameraManager));
        }

        private void Awake()
        {
            this.OnValidate();
        }

        private void OnEnable()
        {
            if (this.cameraManager != null)
            {
                this.cameraManager.frameReceived += FrameChanged;
            }
        }

        private void OnDisable()
        {
            if (this.cameraManager != null)
            {
                this.cameraManager.frameReceived -= FrameChanged;
            }
        }

        private void FrameChanged(ARCameraFrameEventArgs args)
        {
            if (args.lightEstimation.averageBrightness.HasValue)
            {
                this.lightToEstimate.intensity = args.lightEstimation.averageBrightness.Value;
            }

            if (args.lightEstimation.averageColorTemperature.HasValue)
            {
                this.lightToEstimate.colorTemperature = args.lightEstimation.averageColorTemperature.Value;
            }

            if (args.lightEstimation.colorCorrection.HasValue)
            {
                this.lightToEstimate.color = args.lightEstimation.colorCorrection.Value;
            }
        }
    }
}

#endif
