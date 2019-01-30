//-----------------------------------------------------------------------
// <copyright file="Localization.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Localization
{
    using System.Collections.Generic;

    public static class Localization
    {
        private const string CurrentLanguageKey = "CurrentLanguage";

        public delegate void LanguageChangedDelegate();

        public static LanguageChangedDelegate LanguagedChanged;

        private static Language currentLanguage = null;

        public static Language CurrentLanguage
        {
            get
            {
                SetCurrentLanguage();
                return currentLanguage;
            }

            set
            {
                if (value != null && currentLanguage != value)
                {
                    currentLanguage = value;
                    SaveCurrentLangauge();
                    UpdateI2Localization();
                    LanguagedChanged?.Invoke();
                }
            }
        }

        // TODO [bgish]: Get this from the AppConfig Runtime properties
        public static Language DefaultLanguage
        {
            get { return Languages.English; }
        }

        // TODO [bgish]: Get this from the AppConfig Runtime properties
        public static IEnumerable<Language> GetSupportedLanguages()
        {
            yield return Languages.English;
            yield return Languages.Vietnamese;
        }

        public static string GetTranslation(string localizationKey)
        {
            return I2.Loc.LocalizationManager.GetTranslation(localizationKey);
        }

        public static string GetThousandsSeperator()
        {
            return CurrentLanguage.ThousandsSeparator;
        }

        public static string GetDecimalPointSeperator()
        {
            return CurrentLanguage.DecimalSeparator;
        }

        private static void SetCurrentLanguage()
        {
            if (currentLanguage != null)
            {
                return;
            }

            string currentLanguageName = LostPlayerPrefs.GetString(CurrentLanguageKey, null);

            if (currentLanguageName == null)
            {
                // This is the first time running the app, so the language has never been
                // set, so determining the language based on the device's SystemLanguage.
                foreach (var language in GetSupportedLanguages())
                {
                    if (language.SystemLanguage == UnityEngine.Application.systemLanguage)
                    {
                        currentLanguage = language;
                        SaveCurrentLangauge();
                        UpdateI2Localization();
                        return;
                    }
                }
            }
            else
            {
                // Looking up the Language by the name we last saved in LostPlayerPrefs
                foreach (var language in GetSupportedLanguages())
                {
                    if (language.IsoLanguageName == currentLanguageName)
                    {
                        currentLanguage = language;
                        UpdateI2Localization();
                        return;
                    }
                }
            }

            // If we got here, then we failed to find the right language, so returning the default
            currentLanguage = DefaultLanguage;
            SaveCurrentLangauge();
            UpdateI2Localization();
        }

        private static void SaveCurrentLangauge()
        {
            LostPlayerPrefs.SetString(CurrentLanguageKey, CurrentLanguage.IsoLanguageName);
            LostPlayerPrefs.Save();
        }

        // TODO [bgish]: When Lost Localization is far enough along, remove this
        private static void UpdateI2Localization()
        {
            I2.Loc.LocalizationManager.CurrentLanguage = Localization.CurrentLanguage.IsoLanguageName;
        }
    }
}
