//-----------------------------------------------------------------------
// <copyright file="LocalizationMenuItems.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEditor;

    public static class LocalizationMenuItems
    {
        // EFIGS
        private const string English = "Lost/Localization/English"; // Canada, UK, Australia?
        private const string French = "Lost/Localization/French";
        private const string Italian = "Lost/Localization/Italian";
        private const string German = "Lost/Localization/German";
        private const string Spanish = "Lost/Localization/Spanish";  // Spain, Mexico?

        // CJK
        private const string ChineseSimplifed = "Lost/Localization/Chinese (Simplified)";
        private const string ChineseTraditional = "Lost/Localization/Chinese (Traditional)";
        private const string Japanese = "Lost/Localization/Japanese";
        private const string Korean = "Lost/Localization/Korean";

        // Other
        private const string Russian = "Lost/Localization/Russian";
        private const string Vietnamese = "Lost/Localization/Vietnamese";
        private const string Portuguese = "Lost/Localization/Portuguese";

        //// Future Languages
        //// Arabic
        //// Hindi
        //// Dutch
        //// Ukranian
        //// Kazakh
        //// Filipino
        //// Persian
        //// Czech
        //// Thai

        // Language MenuItems
        [MenuItem(English, false)]            public static void SetEnglish()            => SetLanguage(English);
        [MenuItem(French, false)]             public static void SetFrench()             => SetLanguage(French);
        [MenuItem(Italian, false)]            public static void SetItalian()            => SetLanguage(Italian);
        [MenuItem(German, false)]             public static void SetGerman()             => SetLanguage(German);
        [MenuItem(Spanish, false)]            public static void SetSpanish()            => SetLanguage(Spanish);
        [MenuItem(ChineseSimplifed, false)]   public static void SetChineseSimplifed()   => SetLanguage(ChineseSimplifed);
        [MenuItem(ChineseTraditional, false)] public static void SetChineseTraditional() => SetLanguage(ChineseTraditional);
        [MenuItem(Japanese, false)]           public static void SetJapanese()           => SetLanguage(Japanese);
        [MenuItem(Korean, false)]             public static void SetKorean()             => SetLanguage(Korean);
        [MenuItem(Russian, false)]            public static void SetRussian()            => SetLanguage(Russian);
        [MenuItem(Vietnamese, false)]         public static void SetVietnamese()         => SetLanguage(Vietnamese);
        [MenuItem(Portuguese, false)]         public static void SetPortuguese()         => SetLanguage(Portuguese);

        // Language MenuItem Validators
        [MenuItem(English, true)]            public static bool ValidateEnglish()            => ValidateLanguage(English);
        [MenuItem(French, true)]             public static bool ValidateFrench()             => ValidateLanguage(French);
        [MenuItem(Italian, true)]            public static bool ValidateItalian()            => ValidateLanguage(Italian);
        [MenuItem(German, true)]             public static bool ValidateGerman()             => ValidateLanguage(German);
        [MenuItem(Spanish, true)]            public static bool ValidateSpanish()            => ValidateLanguage(Spanish);
        [MenuItem(ChineseSimplifed, true)]   public static bool ValidateChineseSimplifed()   => ValidateLanguage(ChineseSimplifed);
        [MenuItem(ChineseTraditional, true)] public static bool ValidateChineseTraditional() => ValidateLanguage(ChineseTraditional);
        [MenuItem(Japanese, true)]           public static bool ValidateJapanese()           => ValidateLanguage(Japanese);
        [MenuItem(Korean, true)]             public static bool ValidateKorean()             => ValidateLanguage(Korean);
        [MenuItem(Russian, true)]            public static bool ValidateRussian()            => ValidateLanguage(Russian);
        [MenuItem(Vietnamese, true)]         public static bool ValidateVietnamese()         => ValidateLanguage(Vietnamese);
        [MenuItem(Portuguese, true)]         public static bool ValidatePortuguese()         => ValidateLanguage(Portuguese);

        private static bool ValidateLanguage(string menuItem)
        {
            bool isLanguageSelected = menuItem == English;
            Menu.SetChecked(menuItem, isLanguageSelected);

            bool isLanguageSupported = menuItem != French;
            return isLanguageSupported;
        }

        private static void SetLanguage(string menuItem)
        {
            foreach (var language in GetAllLanguages())
            {
                Menu.SetChecked(English, menuItem == language);
            }
        }

        private static IEnumerable<string> GetAllLanguages()
        {
            yield return English;
            yield return French;
            yield return Italian;
            yield return German;
            yield return Spanish;
            yield return ChineseSimplifed;
            yield return ChineseTraditional;
            yield return Japanese;
            yield return Korean;
            yield return Russian;
            yield return Vietnamese;
            yield return Portuguese;
        }
    }
}
