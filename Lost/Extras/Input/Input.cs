//-----------------------------------------------------------------------
// <copyright file="Input.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;
    
    public enum InputState
    {
        Hover,
        Pressed,
        Moved,
        Released
    }
    
    public enum InputType
    {
        Touch,
        Pen,
        Mouse
    }
    
    public enum InputButton
    {
        Left,
        Middle,
        Right,
        Eraser
    }
    
    [System.Serializable]
    public class Input
    {
        public Input(InputType inputType, InputButton inputButton, Vector2 position)
        {
            this.StartPosition = position;
            this.PreviousPosition = position;
            this.CurrentPosition = position;
            this.InputState = InputState.Pressed;
            this.InputType = inputType;
            this.InputButton = inputButton;
            this.FirstPressed = DateTime.Now;
        }

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
