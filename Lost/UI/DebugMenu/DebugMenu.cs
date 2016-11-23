//--------------------------------------------------------------------s---
// <copyright file="DebugMenu.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEngine.Events;

    public class DebugMenu : SingletonDialogResource<DebugMenu>
    {
        private const float HoldTime = 2.0f;

        #pragma warning disable 0649
        [SerializeField] private DebugMenuItem debugMenuItemPrefab;
        [SerializeField] private RectTransform contentTransform;
        #pragma warning restore 0649

        private float threeFingerHoldTime = 0.0f;
        private float spaceHoldTime = 0.0f;

        public void AddItem(string name, UnityAction customAction)
        {
            var newItem = GameObject.Instantiate<DebugMenuItem>(this.debugMenuItemPrefab);
            newItem.transform.SetParent(this.contentTransform);
            newItem.transform.Reset();
            newItem.Initialize(name, customAction, this.Hide);
        }

        public void RemoveItem(string name)
        {
            foreach (var menuItem in this.contentTransform.GetComponentsInChildren<DebugMenuItem>())
            {
                if (menuItem.Name == name)
                {
                    GameObject.Destroy(menuItem.gameObject);
                }
            }
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
                    this.Show();
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
                    this.Show();
                }
            }
            else
            {
                this.spaceHoldTime = 0.0f;
            }
        }
    }
}
