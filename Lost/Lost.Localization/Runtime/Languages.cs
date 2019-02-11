//-----------------------------------------------------------------------
// <copyright file="Languages.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Localization
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEngine;

    public class Languages
    {
        private static ReadOnlyCollection<Language> languages;
        private static ReadOnlyCollection<string> languageNames;

        public static readonly Language English = new Language("English", "English", "en", SystemLanguage.English, ",", ".", "Yes", "No");
        public static readonly Language Vietnamese = new Language("Vietnamese", "Tiếng Việt", "vi", SystemLanguage.Vietnamese, ".", ",", "Có", "Không");

        public static ReadOnlyCollection<Language> AllLanguages
        {
            get
            {
                if (languages == null)
                {
                    languages = new ReadOnlyCollection<Language>(new List<Language>
                    {
                        English,
                        Vietnamese,
                    });
                }

                return languages;
            }
        }

        public static ReadOnlyCollection<string> AllIsoLanguageNames
        {
            get
            {
                if (languageNames == null)
                {
                    List<string> names = new List<string>();

                    for (int i = 0; i < AllLanguages.Count; i++)
                    {
                        names.Add(AllLanguages[i].IsoLanguageName);
                    }

                    languageNames = new ReadOnlyCollection<string>(names);
                }

                return languageNames;
            }
        }
    }
}
