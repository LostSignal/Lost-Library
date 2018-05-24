//-----------------------------------------------------------------------
// <copyright file="Input.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    public enum InputState : byte
    {
        Hover,
        Pressed,
        Moved,
        Released
    }

    public enum InputType : byte
    {
        Touch,
        Pen,
        Mouse
    }

    public enum InputButton : byte
    {
        None,
        Left,
        Middle,
        Right,
        Eraser,
    }

    [System.Serializable]
    public class Input
    {
        /// <summary>
        /// Returns the minimum squared pixel movement needed to consider an input movement intentional.
        ///
        /// Example:
        ///   bool didFingerMove =
        ///       (input.PreviousPosition - input.CurrentPosition).sqrMagnatude > Input.GetMinimumPixelMovementSquared();
        ///
        /// </summary>
        /// <returns>The minimum squared pixel movement.</returns>
        public static float GetMinimumPixelMovementSquared()
        {
            //// NOTE [bgish]: Originally I was going to do the below code that took into account dpi and fixed
            ////               delta time because input uses FixedUpdate and Unity defaults to 0.02.  But once
            ////               tested on device it seems that my pixel 2 xl does a really good job at returning
            ////               subpixel movement and knowing when a finger moves, so it turns out that not needed.
            ////
            //// float minDpi = 96.0f;
            //// float maxDpi = 538.0f;
            //// float dpi = Mathf.Clamp(Screen.dpi == 0.0f ? 160.0f : Screen.dpi, minDpi, maxDpi);
            //// float fixedTimeMultiplier = 1.0f;
            ////
            //// if (Time.fixedDeltaTime > 0.02f)
            //// {
            ////     fixedTimeMultiplier = Time.fixedDeltaTime / 0.02f;
            //// }
            ////
            //// return Mathf.Lerp(2.0f, 10.0f, (dpi - minDpi) / (maxDpi - minDpi)) * fixedTimeMultiplier;

            return 2.0f;
        }

        /// <summary>
        /// Returns a percetage to increase based on pinch.
        /// Ex: Returns 0.10f to increase by 10%, or returns -0.05f to decrease by 5%
        /// </summary>
        /// <param name="input1">The first input.</param>
        /// <param name="input2">The second input.</param>
        /// <returns>The percentage to scale based on the pinch/zoom.</returns>
        public static float CalculatePinchZoomFactor(Input input1, Input input2, out Vector2 center)
        {
            center = (input1.CurrentPosition + input2.CurrentPosition) / 2.0f;

            if (input1.InputState != InputState.Moved && input2.InputState != InputState.Moved)
            {
                return 0.0f;
            }

            float previousPixelLength = (input1.PreviousPosition - input2.PreviousPosition).sqrMagnitude;
            float currentPixelLength = (input1.CurrentPosition - input2.CurrentPosition).sqrMagnitude;

            return (previousPixelLength != 0.0f ? currentPixelLength / previousPixelLength : 1.0f) - 1.0f;
        }

        public void Reset(int id, int unityFingerId, InputType inputType, InputButton inputButton, Vector2 position)
        {
            this.Id = id;
            this.UnityFingerId = unityFingerId;
            this.StartPosition = position;
            this.PreviousPosition = position;
            this.CurrentPosition = position;
            this.InputState = InputState.Pressed;
            this.InputType = inputType;
            this.InputButton = inputButton;
            this.FirstPressed = DateTime.Now;
        }

        public int Id { get; private set; }

        public int UnityFingerId { get; private set; }

        public InputState InputState { get; private set; }

        public InputType InputType { get; private set; }

        public InputButton InputButton { get; private set; }

        public Vector2 StartPosition { get; private set; }

        public Vector2 PreviousPosition { get; private set; }

        public Vector2 CurrentPosition { get; private set; }

        /// <summary>
        /// Gets the time this input was first pressed in milliseconds since the app started.
        /// </summary>
        /// <value>The first pressed time.</value>
        public DateTime FirstPressed { get; private set; }

        /// <summary>
        /// Gets the number of milliseconds that have passed since this input was pressed.
        /// </summary>
        /// <value>The elapsed pressed time in milliseconds.</value>
        public double ElapsedPressedTimeInMillis
        {
            get { return DateTime.Now.Subtract(this.FirstPressed).TotalMilliseconds; }
        }

        public void Update(Vector2 position)
        {
            this.PreviousPosition = this.CurrentPosition;
            this.CurrentPosition = position;
            this.InputState = Lost.InputState.Moved;
        }

        public void UpdateHover(Vector2 position)
        {
            this.StartPosition = position;
            this.PreviousPosition = position;
            this.CurrentPosition = position;
            this.InputState = Lost.InputState.Hover;
        }

        public void Done()
        {
            this.PreviousPosition = this.CurrentPosition;
            this.InputState = Lost.InputState.Released;
        }

        public Vector3 GetCurrentPositionWorldSpace(Camera camera, float z = 0.0f)
        {
            return camera.ScreenToWorldPoint(new Vector3(this.CurrentPosition.x, this.CurrentPosition.y, z));
        }

        public Vector3 GetCurrentPositionLocalSpace(Transform transform, Camera camera, float z = 0.0f)
        {
            return transform.InverseTransformPoint(this.GetCurrentPositionWorldSpace(camera, z));
        }
    }
}
