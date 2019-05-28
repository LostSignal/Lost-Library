//--------------------------------------------------------------------s---
// <copyright file="DebugMenuListener.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    [RequireComponent(typeof(DebugMenu))]
    public class DebugMenuListener : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField, HideInInspector] private DebugMenu debugMenu;
        #pragma warning restore 0649

        private const float HoldTime = 2.0f;

        private float fingerHoldTime = 0.0f;
        private float keyHoldTime = 0.0f;

        private void OnValidate()
        {
            this.AssertGetComponent<DebugMenu>(ref this.debugMenu);
        }

        private void Awake()
        {
            this.OnValidate();
        }

        private void Update()
        {
            this.CheckTouch();
            this.CheckKeyboard();
        }

        private void CheckTouch()
        {
            if (UnityEngine.Input.touchCount == this.debugMenu.Settings.FingerDownCount)
            {
                this.fingerHoldTime += Time.unscaledDeltaTime;

                if (this.fingerHoldTime > this.debugMenu.Settings.FingerDownTime)
                {
                    this.fingerHoldTime = 0.0f;
                    this.debugMenu.ShowMenu();
                }
            }
            else
            {
                this.fingerHoldTime = 0.0f;
            }
        }

        private void CheckKeyboard()
        {
            if (UnityEngine.Input.GetKey(this.debugMenu.Settings.Key))
            {
                this.keyHoldTime += Time.unscaledDeltaTime;

                if (this.keyHoldTime > this.debugMenu.Settings.KeyHoldTime)
                {
                    this.keyHoldTime = 0.0f;
                    this.debugMenu.ShowMenu();
                }
            }
            else
            {
                this.keyHoldTime = 0.0f;
            }
        }
    }
}
