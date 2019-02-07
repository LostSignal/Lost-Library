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

        // Hidden Serialized Fields
        [SerializeField, HideInInspector] private Toggle toggle;
        #pragma warning restore 0649

        private void OnValidate()
        {
            this.AssertGetComponent<Toggle>(ref this.toggle);
        }

        private void Awake()
        {
            this.OnValidate();

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
