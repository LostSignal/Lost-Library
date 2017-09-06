//-----------------------------------------------------------------------
// <copyright file="DebugMenuItem.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class DebugMenuItem : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI text;
        #pragma warning restore 0649

        public string Name
        {
            get { return this.text.text; }
        }

        public void Initialize(string name, params UnityAction[] unityActions)
        {
            this.button.onClick.RemoveAllListeners();

            this.text.text = name;

            if (unityActions == null)
            {
                return;
            }

            for (int i = 0; i < unityActions.Length; i++)
            {
                this.button.onClick.AddListener(unityActions[i]);
            }
        }
    }
}
