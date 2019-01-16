//-----------------------------------------------------------------------
// <copyright file="ARLightEstimationSetter.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

////
//// Taken from Unity's githug depot arfoundation-samples
//// https://github.com/Unity-Technologies/arfoundation-samples/blob/4f8085283794056bde5dc8810cbc7adbde650537/Assets/Scripts/LightEstimation.cs
////

#if USING_UNITY_AR_FOUNDATION

namespace Lost
{
    using UnityEngine;
    using UnityEngine.XR.ARFoundation;

    /// <summary>
    /// A component that can be used to access the most
    /// recently received light estimation information
    /// for the physical environment as observed by an
    /// AR device.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class ARLightEstimationSetter : MonoBehaviour
    {
        Light m_Light;

        /// <summary>
        /// The estimated brightness of the physical environment, if available.
        /// </summary>
        public float? brightness { get; private set; }

        /// <summary>
        /// The estimated color temperature of the physical environment, if available.
        /// </summary>
        public float? colorTemperature { get; private set; }

        /// <summary>
        /// The estimated color correction value of the physical environment, if available.
        /// </summary>
        public Color? colorCorrection { get; private set; }

        void Awake()
        {
            m_Light = GetComponent<Light>();
        }

        void OnEnable()
        {
            ARSubsystemManager.cameraFrameReceived += FrameChanged;
        }

        void OnDisable()
        {
            ARSubsystemManager.cameraFrameReceived -= FrameChanged;
        }

        void FrameChanged(ARCameraFrameEventArgs args)
        {
            if (args.lightEstimation.averageBrightness.HasValue)
            {
                brightness = args.lightEstimation.averageBrightness.Value;
                m_Light.intensity = brightness.Value;
            }

            if (args.lightEstimation.averageColorTemperature.HasValue)
            {
                colorTemperature = args.lightEstimation.averageColorTemperature.Value;
                m_Light.colorTemperature = colorTemperature.Value;
            }

            if (args.lightEstimation.colorCorrection.HasValue)
            {
                colorCorrection = args.lightEstimation.colorCorrection.Value;
                m_Light.color = colorCorrection.Value;
            }
        }
    }
}

#endif
