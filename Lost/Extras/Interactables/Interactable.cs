//-----------------------------------------------------------------------
// <copyright file="Interactable.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class Interactable : MonoBehaviour, InputHandler
    {
        public static readonly string LayerName = "Interactable";
        private static List<Transform> children = new List<Transform>();

        private Camera currentCamera;
        private Input currentInput;
        private Collider currentCollider;
        private bool isInteractable = true;

        public bool HasInput
        {
            get { return this.currentInput != null; }
        }

        public bool IsInteractable
        {
            get
            {
                return this.isInteractable;
            }

            set
            {
                this.isInteractable = value;

                if (this.isInteractable == false)
                {
                    this.ResetInputData();
                }
            }
        }

        protected virtual void Awake()
        {
            InteractablesManager.Initialize();

            int interactableLayer = LayerMask.NameToLayer(LayerName);

            this.GetComponentsInChildren<Transform>(true, children);

            bool hasInteractable = false;

            for (int i = 0; i < children.Count; i++)
            {
                hasInteractable |= children[i].gameObject.layer == interactableLayer;
            }

            if (hasInteractable == false)
            {
                Debug.LogErrorFormat(this, "Interactable \"{0}\" does not have an collider on the \"{1}\" layer and will not work!", this.name, LayerName);
            }

            children.Clear();
        }

        public void SetInputData(Input input, Collider collider, Camera camera)
        {
            if (input.InputState != InputState.Pressed || this.currentInput != null || this.isInteractable == false)
            {
                return;
            }

            this.currentCamera = camera;
            this.currentInput = input;
            this.currentCollider = collider;
            InputManager.Instance.AddHandler(this);

            this.OnInput(input, collider, camera);
        }

        protected abstract void OnInput(Input input, Collider collider, Camera camera);

        private void ResetInputData()
        {
            this.currentCamera = null;
            this.currentCollider = null;
            this.currentInput = null;
            InputManager.Instance.RemoveHandler(this);
        }

        void InputHandler.HandleInputs(List<Input> touches, Input mouse, Input pen)
        {
            this.OnInput(this.currentInput, this.currentCollider, this.currentCamera);

            if (this.currentInput != null && this.currentInput.InputState == InputState.Released)
            {
                this.ResetInputData();
            }
        }
    }
}
