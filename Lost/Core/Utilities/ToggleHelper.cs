//-----------------------------------------------------------------------
// <copyright file="ToggleHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [RequireComponent(typeof(Toggle))]
    public class ToggleHelper : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private UnityEvent onToggleOn;
        [SerializeField] private UnityEvent onToggleOff;
        #pragma warning restore 0649

        private Toggle toggle;

        private void Awake()
        {
            this.toggle = this.GetComponent<Toggle>();
            this.toggle.onValueChanged.AddListener(this.ToggleChanged);
            this.ToggleChanged(this.toggle.isOn);
        }

        private void ToggleChanged(bool newValue)
        {
            if (newValue)
            {
                this.onToggleOn.InvokeIfNotNull();
            }
            else
            {
                this.onToggleOff.InvokeIfNotNull();
            }
        }
    }
}
