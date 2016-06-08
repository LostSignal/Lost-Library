//-----------------------------------------------------------------------
// <copyright file="AppVersion.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class AppVersion : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private string baseString = "App Version: ";
        #pragma warning restore 0649

        private void Awake()
        {
            var text = this.GetComponent<TMPro.TextMeshProUGUI>();
            text.text = this.baseString + App.Version;
        }
    }
}
