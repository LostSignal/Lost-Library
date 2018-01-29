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
        private const int InputCacheSize = 20;

        private List<InputHandler> handlers = new List<InputHandler>();

        private List<Input> fingerInputs = new List<Input>(10);
        private Dictionary<int, Input> fingerIdToInputMap = new Dictionary<int, Input>();
        private HashSet<int> activeFingerIdsCache = new HashSet<int>();

        private Input mouseInput = null;
        private Input penInput = null;

        private bool useTouchInput;
        private bool useMouseInput;
        private bool usePenInput;

        private List<Input> inputCache = new List<Input>(InputCacheSize);
        private int inputIdCounter = 0;

        public void AddHandler(InputHandler handler)
        {
            if (handler != null && this.handlers.Contains(handler) == false)
            {
                this.handlers.Add(handler);
            }
        }

        public void RemoveHandler(InputHandler handler)
        {
            this.handlers.Remove(handler);
        }
        
        protected override void Awake()
        {
            // populating the input cache
            for (int i = 0; i < InputCacheSize; i++)
            {
                this.inputCache.Add(new Input());
            }

            this.useTouchInput = true;
            this.usePenInput = false;
            this.useMouseInput =
                Application.isEditor ||
                Application.platform == RuntimePlatform.OSXPlayer ||
                Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.LinuxPlayer;
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
            
            // sending inputs to all registered handlers
            for (int i = 0; i < this.handlers.Count; i++)
            {
                this.handlers[i].HandleInputs(this.fingerInputs, this.mouseInput, this.penInput);
            }
        }
        
        private void UpdateTouchInputs()
        {
            Debug.Assert(this.fingerInputs.Count == this.fingerIdToInputMap.Count, "Finger Inputs list and map don't match!");

            // remove all inputs that have been marked as released
            for (int i = this.fingerInputs.Count - 1; i >= 0; i--)
            {
                if (this.fingerInputs[i].InputState == InputState.Released)
                {
                    Input input = this.fingerInputs[i];
                    this.RecycleInput(input);
                    this.fingerInputs.RemoveAt(i);
                    this.fingerIdToInputMap.Remove(input.UnityFingerId);
                }
            }

            this.activeFingerIdsCache.Clear();

            // going through all the unity touch inputs and either creating/updating Lost.Inputs
            for (int i = 0; i < UnityEngine.Input.touchCount; i++)
            {
                Touch touch = UnityEngine.Input.GetTouch(i);
                this.activeFingerIdsCache.Add(touch.fingerId);

                Input input;
                if (this.fingerIdToInputMap.TryGetValue(touch.fingerId, out input))
                {
                    input.Update(touch.position);
                }
                else
                {
                    Input newInput = this.GetNewInput(touch.fingerId, InputType.Touch, InputButton.Left, touch.position);
                    this.fingerInputs.Add(newInput);
                    this.fingerIdToInputMap.Add(newInput.UnityFingerId, newInput);
                }
            }

            // testing if any of the Lost.Inputs no longer have their unity counterparts and calling Done() on them if that's the case
            for (int i = 0; i < this.fingerInputs.Count; i++)
            {
                Input input = this.fingerInputs[i];

                if (this.activeFingerIdsCache.Contains(input.UnityFingerId) == false)
                {
                    input.Done();
                }
            }
        }

        private void UpdateMouseInput()
        {
            // defaulting the mouse input to be in the hover state
            if (this.mouseInput == null || this.mouseInput.InputState == InputState.Released)
            {
                this.RecycleInput(this.mouseInput);
                this.mouseInput = this.GetNewInput(-1, InputType.Mouse, InputButton.None, UnityEngine.Input.mousePosition);
                this.mouseInput.UpdateHover(UnityEngine.Input.mousePosition);
            }
            
            if (this.mouseInput.InputState == InputState.Hover)
            {
                InputButton inputButton = InputButton.None;

                if (UnityEngine.Input.GetMouseButton(0))
                {
                    inputButton = InputButton.Left;
                }
                else if (UnityEngine.Input.GetMouseButton(1))
                {
                    inputButton = InputButton.Right;
                }
                else if (UnityEngine.Input.GetMouseButton(2))
                {
                    inputButton = InputButton.Middle;
                }

                if (inputButton != InputButton.None)
                {
                    this.RecycleInput(this.mouseInput);
                    this.mouseInput = this.GetNewInput(-1, InputType.Mouse, inputButton, UnityEngine.Input.mousePosition);
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
            // TODO implement (may need platform dependent code)
        }

        private void RecycleInput(Input input)
        {
            if (input != null)
            {
                this.inputCache.Add(input);
            }
        }

        private Input GetNewInput(int unityFingerId, InputType inputType, InputButton inputButton, Vector2 position)
        {
            Debug.Assert(this.inputCache.Count != 0, "InputManager's input cache has run out!  Figure out why we're leaking inputs.");

            int lastIndex = this.inputCache.Count - 1;
            Input lastInput = this.inputCache[lastIndex];
            this.inputCache.RemoveAt(lastIndex);

            lastInput.Reset(this.inputIdCounter++, unityFingerId, inputType, inputButton, position);

            return lastInput;
        }
    }
}
