//-----------------------------------------------------------------------
// <copyright file="KeyboardButton.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    
    public enum ButtonState
    {
        Idle,
        Pressed,
        Held,
        Released
    }
    
    public class KeyboardButton
    {        
        private DateTime pressedDateTime;
        private DateTime releasedDateTime;
        
        public ButtonState State
        {
            get; private set;
        }

        public bool IsDown
        {
            get { return this.State == ButtonState.Held || this.State == ButtonState.Pressed; }            
        }
        
        public bool IsUp
        {
            get { return this.State == ButtonState.Idle || this.State == ButtonState.Released; }
        }
        
        public bool IsPressed
        {
            get { return this.State == ButtonState.Pressed; }
        }
        
        public float PressedTime
        {
            get
            {
                if (this.State == ButtonState.Idle)
                {
                    return 0;
                }
                else if (this.State == ButtonState.Held || this.State == ButtonState.Pressed)
                {
                    return (float)DateTime.Now.Subtract(this.pressedDateTime).TotalSeconds;
                }
                else if (this.State == ButtonState.Released)
                {
                    return (float)this.releasedDateTime.Subtract(this.pressedDateTime).TotalSeconds;
                }
                else
                {
                    throw new NotImplementedException("Found unknown ButtonState!");
                }
            }
        }
        
        public void Update(bool isPressed)
        {
            if (this.State == ButtonState.Idle)
            {
                if (isPressed)
                {
                    this.State = ButtonState.Pressed;
                    this.pressedDateTime = DateTime.Now;
                }
            }
            else if (this.State == ButtonState.Pressed)
            {
                if (isPressed)
                {
                    this.State = ButtonState.Held;
                }
                else
                {
                    this.State = ButtonState.Released;
                    this.releasedDateTime = DateTime.Now;
                }
            }
            else if (this.State == ButtonState.Held)
            {
                if (isPressed == false)
                {
                    this.State = ButtonState.Released;
                    this.releasedDateTime = DateTime.Now;
                }
            }
            else if (this.State == ButtonState.Released)
            {
                if (isPressed)
                {
                    this.State = ButtonState.Pressed;
                    this.pressedDateTime = DateTime.Now;
                }
                else
                {
                    this.State = ButtonState.Idle;
                }
            }
        }
    }
}