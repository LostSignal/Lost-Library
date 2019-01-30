//-----------------------------------------------------------------------
// <copyright file="ChangeLanguageToggle.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Localization
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class ChangeLanguageButton : MonoBehaviour
    {
        #pragma warning disable 0649
        [HideInInspector, SerializeField] private Button button;
        [SerializeField] private string isoLanguageName;
        #pragma warning restore 0649

        private void OnValidate()
        {
            this.Initialize();
        }

        private void Awake()
        {
            this.Initialize();
            this.button.onClick.AddListener(this.Clicked);
        }

        private void Clicked()
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

        private void Initialize()
        {
            if (this.button == null)
            {
                this.button = this.GetComponent<Button>();
            }
        }
    }
}
