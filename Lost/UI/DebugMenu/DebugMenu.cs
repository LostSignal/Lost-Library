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
        #pragma warning disable 0649
        [SerializeField] private DebugMenuItem debugMenuItemPrefab;
        [SerializeField] private RectTransform contentTransform;
        #pragma warning restore 0649
        
        public void AddItem(string name, UnityAction customAction)
        {
            var newItem = Pooler.Instantiate<DebugMenuItem>(this.debugMenuItemPrefab);
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
                    Pooler.Destroy(menuItem.gameObject);
                }
            }
        }
    }
}
