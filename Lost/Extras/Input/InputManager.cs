//-----------------------------------------------------------------------
// <copyright file="InputManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class InputManager : SingletonGameObject<InputManager>
    {
        private List<InputHandler> handlers = new List<InputHandler>();
        private Dictionary<int, Input> fingerInputs = new Dictionary<int, Input>();
        private List<int> fingerRemoveList = new List<int>(5);
        private List<Input> touchInputs = new List<Input>(10);

        private Input mouseInput = null;
        private Input penInput = null;

        private bool useTouchInput;
        private bool useMouseInput;
        private bool usePenInput;

        public static IEnumerable<Input> EnumerateInputs(List<Input> touch, Input mouse, Input pen)
        {
            for (int i = 0; i < touch.Count; i++)
            {
                yield return touch[i];
            }

            if (mouse != null)
            {
                yield return mouse;
            }

            if (pen != null)
            {
                yield return pen;
            }
        }
        
        public void AddHandler(InputHandler handler)
        {
            this.handlers.AddIfNotNullAndUnique(handler);
        }
        
        public void RemoveHandler(InputHandler handler)
        {
            this.handlers.Remove(handler);
        }

        protected override void Awake()
        {
            base.Awake();

            UnityEngine.Input.simulateMouseWithTouches = false;
            this.useTouchInput = Platform.IsTouchSupported;
            this.useMouseInput = Platform.IsMousePresent;
            this.usePenInput = Platform.IsPenPresent;
        }

        private void FixedUpdate()
        {
            if (this.useTouchInput)
            {
                this.UpdateTouchInputs();
            }

            if (this.useMouseInput)
            {
                this.UpdateMouseInput();
            }

            if (this.usePenInput)
            {
                this.UpdatePenInput();
            }

            this.touchInputs.Clear();
            this.touchInputs.AddRange(this.fingerInputs.Values);

            // sending inputs to all registered handlers
            for (int i = 0; i < this.handlers.Count; i++)
            {
                this.handlers[i].HandleInputs(this.touchInputs, this.mouseInput, this.penInput);
            }
        }
        
        private void UpdateTouchInputs()
        {
            // remove all inputs that have been marked as released
            this.fingerRemoveList.Clear();

            foreach (int touchId in this.fingerInputs.Keys)
            {
                if (this.fingerInputs[touchId].InputState == InputState.Released)
                {
                    this.fingerRemoveList.Add(touchId);
                }
            }

            foreach (int id in this.fingerRemoveList)
            {
                this.fingerInputs.Remove(id);
            }

            // going through all the touch inputs and either creating/updating Lost.Inputs
            foreach (Touch touch in UnityEngine.Input.touches)
            {
                Input input;
                if (this.fingerInputs.TryGetValue(touch.fingerId, out input))
                {
                    input.Update(touch.position);
                }
                else
                {
                    this.fingerInputs.Add(touch.fingerId, new Input(InputType.Touch, InputButton.Left, touch.position));
                }
            }

            // testing if any of the Lost.Inputs are no longer around and we should call Done() on them
            foreach (int touchId in this.fingerInputs.Keys)
            {
                bool foundTouchId = false;

                foreach (Touch touch in UnityEngine.Input.touches)
                {
                    if (touch.fingerId == touchId)
                    {
                        foundTouchId = true;
                        break;
                    }
                }

                if (foundTouchId == false)
                {
                    this.fingerInputs[touchId].Done();
                }
            }
        }

        private void UpdateMouseInput()
        {
            // defaulting the mouse input to be in the hover state, and making sure we 
            if (this.mouseInput == null || this.mouseInput.InputState == InputState.Released)
            {
                this.mouseInput = new Input(InputType.Mouse, InputButton.Left, UnityEngine.Input.mousePosition);
                this.mouseInput.UpdateHover(UnityEngine.Input.mousePosition);
            }
            
            if (this.mouseInput.InputState == InputState.Hover)
            {
                if (UnityEngine.Input.GetMouseButton(0))
                {
                    // left mouse
                    this.mouseInput = new Input(InputType.Mouse, InputButton.Left, UnityEngine.Input.mousePosition);
                }
                else if (UnityEngine.Input.GetMouseButton(1))
                {
                    // right mouse
                    this.mouseInput = new Input(InputType.Mouse, InputButton.Right, UnityEngine.Input.mousePosition);
                }
                else if (UnityEngine.Input.GetMouseButton(2))
                {
                    // middle mouse
                    this.mouseInput = new Input(InputType.Mouse, InputButton.Middle, UnityEngine.Input.mousePosition);
                }
                else
                {
                    this.mouseInput.UpdateHover(UnityEngine.Input.mousePosition);
                }
            }
            else
            {
                if (this.mouseInput.InputButton == InputButton.Left)
                {
                    if (UnityEngine.Input.GetMouseButton(0))
                    {
                        this.mouseInput.Update(UnityEngine.Input.mousePosition);
                    }
                    else
                    {
                        this.mouseInput.Done();
                    }
                }
                else if (this.mouseInput.InputButton == InputButton.Right)
                {
                    if (UnityEngine.Input.GetMouseButton(1))
                    {
                        this.mouseInput.Update(UnityEngine.Input.mousePosition);
                    }
                    else
                    {
                        this.mouseInput.Done();
                    }
                }
                else if (this.mouseInput.InputButton == InputButton.Middle)
                {
                    if (UnityEngine.Input.GetMouseButton(2))
                    {
                        this.mouseInput.Update(UnityEngine.Input.mousePosition);
                    }
                    else
                    {
                        this.mouseInput.Done();
                    }
                }
                else
                {
                    Debug.LogError("UpdateMouseInput found an invalid InputButton type!!!", this);
                }
            }
        }

        private void UpdatePenInput()
        {
            //// TODO implement (may need platform dependent code)
        }
    }
}
