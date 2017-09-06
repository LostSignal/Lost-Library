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
        private const float HoldTime = 2.0f;

        private DebugMenu debugMenu;
        private float threeFingerHoldTime = 0.0f;
        private float spaceHoldTime = 0.0f;

        private void Awake()
        {
            this.debugMenu = this.GetComponent<DebugMenu>();
        }

        private void Update()
        {
            this.CheckForFingers();
            this.CheckForSpaceBar();
        }

        private void CheckForFingers()
        {
            if (UnityEngine.Input.touchCount == 3)
            {
                this.threeFingerHoldTime += Time.unscaledDeltaTime;

                if (this.threeFingerHoldTime > HoldTime)
                {
                    this.threeFingerHoldTime = 0.0f;
                    this.debugMenu.ShowMenu();
                }
            }
            else
            {
                this.threeFingerHoldTime = 0.0f;
            }
        }

        private void CheckForSpaceBar()
        {
            if (UnityEngine.Input.GetKey(KeyCode.Space))
            {
                this.spaceHoldTime += Time.unscaledDeltaTime;

                if (this.spaceHoldTime > HoldTime)
                {
                    this.spaceHoldTime = 0.0f;
                    this.debugMenu.ShowMenu();
                }
            }
            else
            {
                this.spaceHoldTime = 0.0f;
            }
        }
    }
}
