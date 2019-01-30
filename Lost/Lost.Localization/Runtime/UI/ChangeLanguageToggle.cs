//-----------------------------------------------------------------------
// <copyright file="ChangeLanguageToggle.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Localization
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Toggle))]
    public class ChangeLanguageToggle : MonoBehaviour
    {
        #pragma warning disable 0649
        [HideInInspector, SerializeField] private Toggle toggle;
        [SerializeField] private string isoLanguageName;
        #pragma warning restore 0649

        private void OnValidate()
        {
            this.Initialize();
        }

        private void Awake()
        {
            this.Initialize();
            this.toggle.onValueChanged.AddListener(this.ValueChanged);
        }

        private void ValueChanged(bool newValue)
        {
            if (newValue)
            {
                foreach (var language in Localization.GetSupportedLanguages())
                {
                    if (language.IsoLanguageName == this.isoLanguageName)
                    {
                        Localization.CurrentLanguage = language;
                        return;
                    }
                }

                Debug.LogErrorFormat(this, "ChangeLanguage.ChangeLanguageTo couldn't find supported language {0}!", this.isoLanguageName);
            }
        }

        private void Initialize()
        {
            if (this.toggle == null)
            {
                this.toggle = this.GetComponent<Toggle>();
            }
        }
    }
}
